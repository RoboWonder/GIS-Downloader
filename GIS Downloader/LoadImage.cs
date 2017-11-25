using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GIS_Downloader
{
    public static class LoadImage
    {
        public static async Task<Image> Get(string url)
        {
            try
            {
                using (var client = new WebClient())
                {
                    Stream stream = await OpenReadTaskAsync(client, new Uri(url));
                    
                    return Image.FromStream(stream);
                }
                
            }
            catch
            {
                return Image.FromFile("error.png");
            }
            
        }
        public static Task<Stream> OpenReadTaskAsync(this WebClient client, Uri uri)
        {
            var tcs = new TaskCompletionSource<Stream>();

            OpenReadCompletedEventHandler openReadEventHandler = null;
            openReadEventHandler = (sender, args) =>
            {
                try
                {
                    tcs.SetResult(args.Result);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            };
            client.Headers[HttpRequestHeader.AcceptLanguage] = "en-US";
            client.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36";

            client.OpenReadCompleted += openReadEventHandler;
            client.OpenReadAsync(uri);

            return tcs.Task;
        }
    }
}
