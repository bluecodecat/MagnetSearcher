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

    class BtCherryProxy : ISearchEngineProxy
    {
        private Dictionary<string, int> pageCnt = new Dictionary<string, int>();
        public async Task<List<MagnetLinkItem>> GetAllMagnetLinks(string keyword)
        {
            List<MagnetLinkItem> list = new List<MagnetLinkItem>(100);
            list.AddRange(await GetMagnetLinks(keyword, 1));
            for (int i = 2; i < await GetResultPages(keyword); i++)
            {
                list.AddRange(await GetMagnetLinks(keyword,i));
            }
            return list;
        }

        public async Task<string> GetUrltoHtml(string url)
        {
            string result = "";
            try
            {
                HttpClient webclient = new HttpClient();
                result = await webclient.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return result;

        }
        public async Task<List<MagnetLinkItem>> GetMagnetLinks(string keyword, int page)
        {
            string text = HttpUtility.UrlEncode(keyword);
            string urltoHtml = await GetUrltoHtml(string.Concat(new object[]
            {"http://www.btcherry.org/search?keyword=",text,"&p=",page}));

            string patternItem = "<div class=\"r\">(?:.|\n)*?</div>";
            Regex itemRegex = new Regex(patternItem, RegexOptions.IgnoreCase);
            string patternInner = "<h5 class=\"h\">(.*)</h5>(?:.|\n)*?<span (?:.|\n)*?<span(?:.|\n)*?class=\"prop_val\">(.*?)</span>(?:.|\n)*?class=\"prop_val\">(.*?)</span>(?:.|\n)*?class=\"prop_val\">(.*?)</span>(?:.|\n)*?href=\"(.*)?&xl=(.*)&dn=(.*)\">";
            Regex innerRegex = new Regex(patternInner, RegexOptions.IgnoreCase);
            string patternPages = @"totalPages: (\d+)";
            Regex pageRegex = new Regex(patternPages);

            string pageStr = pageRegex.Match(urltoHtml).Groups[1].Value;
            int ptmp = 0;
            Int32.TryParse(pageStr, out ptmp);
            pageCnt[keyword] = ptmp;

            MatchCollection mc = itemRegex.Matches(urltoHtml);

            List<MagnetLinkItem> list = new List<MagnetLinkItem>(20);
            foreach (var m in mc)
            {
                var match = innerRegex.Match(m.ToString());
                var group = match.Groups;
                MagnetLinkItem mag = new MagnetLinkItem();
                if (group.Count == 8)
                {
                    mag.SizeStr = group[3].Value;
                    mag.Name = group[7].Value;
                    long ltmp = 0;
                    long.TryParse(group[6].Value, out ltmp);
                    if (ltmp == 0)
                    {
                        ltmp = MagnetUtility.StringSize2ByteSize(mag.SizeStr);
                    }
                    mag.SizeByte = ltmp;
                   
                    mag.MagnetLink = group[5].Value;
                    mag.CreatedTime = group[2].Value;
                    int tmp = 0;
                    Int32.TryParse(group[4].Value, out tmp);
                    mag.FileCountP = tmp;
                }
                list.Add(mag);
            }

            return list;
        }

        public async Task<int> GetResultPages(string keyword)
        {
            if (pageCnt.ContainsKey(keyword))
            {
                return pageCnt[keyword];
            }

            string text = HttpUtility.UrlEncode(keyword);
            string urltoHtml = await GetUrltoHtml(string.Concat(new object[]
            {"http://www.btcherry.org/search?keyword=",text,"&p=",1}));
            string patternPages = @"totalPages: (\d +)";
            Regex pageRegex = new Regex(patternPages);
            string pageStr = pageRegex.Match(urltoHtml).Value;
            int ptmp = 0;
            Int32.TryParse(pageStr, out ptmp);
            pageCnt[keyword] = ptmp;
            return ptmp;
        }
    }
}
