using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GIS_Downloader
{
    class Crawler
    {
        private string url = "https://www.google.com.vn/search?tbm=isch&tbo=u&source=univ&sa=X";
        private string Error = String.Empty;
        private HtmlDocument dom = new HtmlDocument();
        private List<string> img = new List<string>();
        private Crawler()
        {
        }

        public static async Task<Crawler> Load(string key, string size)
        {
            Crawler crl = new Crawler();
            var _url = crl.url+"&q=" + key.Replace(" ", "+") + size;
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.AcceptLanguage] = "en-US";
                    client.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36";
                    string html = await client.DownloadStringTaskAsync(_url);
                    // feed the HTML to HTML Agility Pack
                    crl.dom.LoadHtml(html);
                    crl.parseImg();
                    // now do the parsing
                }
            }
            catch (Exception e)
            {
                crl.Error = e.Message;
            }
            return crl;
        }
        public List<string> GetImages()
        {
            //img.Add(dom.DocumentNode.SelectNodes("//a").First().Attributes["value"].Value);
            return img;
        }
        private void parseImg()
        {
            
            var htmlNodes = dom.DocumentNode.SelectNodes("//div[@class='rg_meta notranslate']");
            for(int i = 0; i<htmlNodes.Count; i++)
            {
                img.Add(UnescapeCodes(htmlNodes[i].InnerText));
            }
        }
        public static string UnescapeCodes(string src)
        {
            var rx = new Regex("\\\\u([0-9A-Fa-f]{4})");
            var res = new StringBuilder();
            var pos = 0;
            foreach (Match m in rx.Matches(src))
            {
                res.Append(src.Substring(pos, m.Index - pos));
                pos = m.Index + m.Length;
                res.Append((char)Convert.ToInt32(m.Groups[1].ToString(), 16));
            }
            res.Append(src.Substring(pos));
            return res.ToString();
        }
        public string GetHtml()
        {
            return this.dom.DocumentNode.OuterHtml;
        }
    }
}
