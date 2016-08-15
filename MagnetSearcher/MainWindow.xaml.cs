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
        private DataGrid lastEditedDataGrid = null;
        public MainWindow()
        {
            InitializeComponent();
            tabMain.ItemsSource = dataSrcs;
        }

        private ObservableConcurrentDictionary<string, ObservableCollection<MagnetLinkItem>> dataSrcs =
            new ObservableConcurrentDictionary<string, ObservableCollection<MagnetLinkItem>>();

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            btnSearch.IsEnabled = false;
            stopLoading = false;
            BtCherryProxy proxy = new BtCherryProxy();
            var keyword = txtKeyWord.Text;
            ObservableCollection<MagnetLinkItem> dataSrc;
            if (dataSrcs.ContainsKey(keyword))
            {
                dataSrc = dataSrcs[keyword];
                dataSrc.Clear();
            }
            else
            {
                dataSrc = new ObservableCollection<MagnetLinkItem>();
                dataSrcs[keyword] = dataSrc;
            }




            SetStatusText("Loading Page 1");

            var list = await proxy.GetMagnetLinks(keyword, 1);

            int no = dataSrc.Count + 1;
            foreach (var magnetLinkItem in list)
            {
                magnetLinkItem.No = no++;
                if (!preciseMatch || (preciseMatch && magnetLinkItem.Name.ToUpper().Contains(keyword.ToUpper())))
                {
                    dataSrc.Add(magnetLinkItem);
                }
            }
            tabMain.SelectedValuePath = "Value";
            tabMain.SelectedValue = dataSrcs[keyword];
            var totalPages = await proxy.GetResultPages(keyword);
            for (int i = 2; i <= totalPages && !stopLoading; i++)
            {
                SetStatusText("Loading Page " + i + " of " + totalPages);
                list = await proxy.GetMagnetLinks(keyword, i);
                foreach (var magnetLinkItem in list)
                {
                    magnetLinkItem.No = no++;
                    if (!preciseMatch || (preciseMatch && magnetLinkItem.Name.ToUpper().Contains(keyword.ToUpper())))
                    {
                        dataSrc.Add(magnetLinkItem);
                    }
                }
            }
            SetStatusText("Ready");
            btnSearch.IsEnabled = true;

        }

        private void btnClear_OnClick(object sender, RoutedEventArgs e)
        {
            if (btnSearch.IsEnabled == true)
            {
                dataSrcs.Clear();
            }
            else
            {
                SetStatusText("Cannot do this");
            }
        }

        private void BtnCopyAll_OnClick(object sender, RoutedEventArgs e)
        {
            int cnt = 0;
            if (tabMain.SelectedContent != null)
            {
                var dataSrc = ((KeyValuePair<string, ObservableCollection<MagnetLinkItem>>)tabMain.SelectedContent).Value;

                StringBuilder data = new StringBuilder(1024);
                foreach (var item in dataSrc)
                {
                    cnt++;
                    data.Append(item.MagnetLink).Append("\r\n");
                }
                Clipboard.SetText(data.ToString());
            }
            SetStatusText(cnt + " links copied to clipboard");
        }

        private void dgList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lastEditedDataGrid = sender as DataGrid;

        }

        private void dgList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                if (lastEditedDataGrid != null)
                {
                    CopySelectedLink(lastEditedDataGrid);
                }
            }

        }

        private void SetStatusText(string text)
        {
            txtStatus.Text = text;
        }

        private void CopySelectedLink(DataGrid dgList)
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
            if (lastEditedDataGrid != null)
            {

                CopySelectedLink(lastEditedDataGrid);
            }
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

        private void tabbtnClose_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).DataContext != null)
            {
                var t = ((KeyValuePair<string, ObservableCollection<MagnetLinkItem>>)(sender as Button).DataContext).Key;
                dataSrcs.Remove(t);
            }
        }
    }
}
