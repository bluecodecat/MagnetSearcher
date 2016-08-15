using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MagnetSearcher
{
    class TabItemObservable:INotifyPropertyChanged
    {
        private string keyword;

        public string Keyword
        {
            get
            {
                return keyword;
            }
            set
            {
                if (value != keyword)
                {
                    keyword=value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<MagnetLinkItem> magnetList;

        public ObservableCollection<MagnetLinkItem> MagnetList
        {
            get { return magnetList; }
            set
            {
                if (value == magnetList)
                {
                    magnetList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int pageCount;

        public int PageCount
        {
            get { return pageCount; }
            set
            {
                if (value == pageCount)
                {
                    pageCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
