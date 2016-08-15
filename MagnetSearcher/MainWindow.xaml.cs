using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MagnetSearcher
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool stopLoading = false;

        private bool preciseMatch = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private ObservableCollection<MagnetLinkItem> dataSrc = new ObservableCollection<MagnetLinkItem>();

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            stopLoading = false;
            BtCherryProxy proxy = new BtCherryProxy();
            this.dgList.ItemsSource = dataSrc;
            SetStatusText("Loading Page 1");
            var list = await proxy.GetMagnetLinks(txtKeyWord.Text, 1);

            int no = dataSrc.Count + 1;
            foreach (var magnetLinkItem in list)
            {
                magnetLinkItem.No = no++;
                if (!preciseMatch || (preciseMatch && magnetLinkItem.Name.ToUpper().Contains(txtKeyWord.Text.ToUpper())))
                {
                    dataSrc.Add(magnetLinkItem);
                }
            }
            var totalPages = await proxy.GetResultPages(txtKeyWord.Text);
            for (int i = 2; i <= totalPages && !stopLoading; i++)
            {
                SetStatusText("Loading Page " + i + " of " + totalPages);
                list = await proxy.GetMagnetLinks(txtKeyWord.Text, i);
                foreach (var magnetLinkItem in list)
                {
                    magnetLinkItem.No = no++;
                    if (!preciseMatch || (preciseMatch && magnetLinkItem.Name.ToUpper().Contains(txtKeyWord.Text.ToUpper())))
                    {
                        dataSrc.Add(magnetLinkItem);
                    }
                }
            }
            SetStatusText("Ready");
        }

        private void btnClear_OnClick(object sender, RoutedEventArgs e)
        {
            dataSrc.Clear();
        }

        private void BtnCopyAll_OnClick(object sender, RoutedEventArgs e)
        {
            int cnt = 0;
            StringBuilder data = new StringBuilder(1024);
            foreach (var item in dataSrc)
            {
                cnt++;
                data.Append(item.MagnetLink).Append("\r\n");
            }
            Clipboard.SetText(data.ToString());
            SetStatusText(cnt + " links copied to clipboard");
        }

        private void dgList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dgList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                CopySelectedLink();
            }

        }

        private void SetStatusText(string text)
        {
            txtStatus.Text = text;
        }

        private void CopySelectedLink()
        {
            int cnt = 0;
            var items = dgList.SelectedItems;
            StringBuilder data = new StringBuilder(1024);
            foreach (var item in items)
            {
                cnt++;
                data.Append((item as MagnetLinkItem).MagnetLink).Append("\r\n");
            }
            Clipboard.SetText(data.ToString());
            SetStatusText(cnt + " links copied to clipboard");
        }
        private void BtnCopySelect_OnClick(object sender, RoutedEventArgs e)
        {
            CopySelectedLink();
        }

        private void BtnStopLoading_OnClick(object sender, RoutedEventArgs e)
        {
            stopLoading = true;
            SetStatusText("stop signal sent");
        }

        private void chkPrecise_Click(object sender, RoutedEventArgs e)
        {
            this.preciseMatch = (sender as CheckBox).IsChecked.Value;
        }
    }
}
