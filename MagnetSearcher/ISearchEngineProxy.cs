using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;

namespace MagnetSearcher
{
    interface ISearchEngineProxy
    {

        Task<List<MagnetLinkItem>> GetAllMagnetLinks(string keyword);
        Task<List<MagnetLinkItem>> GetMagnetLinks(string keyword, int page);

        Task<int> GetResultPages(string keyword);



    }

}
