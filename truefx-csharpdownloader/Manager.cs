using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace truefx_csharpdownloader
{
    class Manager
    {

        //bool login_response_cookies = false;
        CookieContainer login_response_cookies;
        UrlProvider url_provider = new UrlProvider();
        HttpClient httpClient;
        HttpClientHandler handler;


        public void download_and_merge_to_one_file(int year, string symbol, string directory)
        {

            download_for_year(year, symbol, directory);
            //unzip_for_year(year, symbol, directory)

            //output_filename = symbol.lower() + '-' + str(year) + '.csv'
            //files = self.get_filenames_to_merge(str(year), symbol, directory)

            //with open(directory + output_filename, 'w') as outfile:
            //    for fname in files:
            //        with open(fname) as infile:
            //            for line in infile:
            //                outfile.write(line)
            //            infile.close()
            //    outfile.close()

            //for fname in self.get_filenames_to_delete(year, symbol, directory):
            //    os.unlink(fname)
        }

        public void download_for_year(int year, string symbol, string destination_directory)
        {

            for (int month = 1; month <= 12; month++)
            {
                download_for_month(year, month, symbol, destination_directory);
            }
        }

        public string download_for_month(int year, int month, string symbol, string destination_directory)
        {

            string filename = get_downloaded_filename(month, symbol, year);

            string filename_with_directory = destination_directory + filename;

            if (File.Exists(destination_directory + filename))
            {
                new Exception("File '" + filename_with_directory + "' already exists");
            }

            httpClient.DefaultRequestHeaders.Referrer=new Uri(url_provider.get_download_referrer_url(year, month));

            string url_to_download = url_provider.get_download_url(year, month, symbol);
            Console.WriteLine("Downloading '{0}' to '{1}'", url_to_download, filename_with_directory);

            HttpResponseMessage response = httpClient.GetAsync(url_to_download).Result;

            using (HttpContent content = response.Content)
            {
                byte[] data = content.ReadAsByteArrayAsync().Result;
                File.WriteAllBytes(filename_with_directory, data);
            }

            return filename_with_directory;
        }

        public string get_downloaded_filename(int month, string symbol, int year)
        {
            return symbol.ToLower() + "-" + year + "-" + month.ToString().PadLeft(2, '0') + ".zip";
        }


        public bool login_to_true_fx(string username, string password)
        {

            string proxyUri = "127.0.0.1:8888";

            //NetworkCredential proxyCreds = new NetworkCredential(
            //    proxyServerSettings.UserName,
            //    proxyServerSettings.Password
            //);

            WebProxy proxy = new WebProxy(proxyUri, false)
            {
                UseDefaultCredentials = false,
                Credentials = null,
            };

            HttpResponseMessage response = null;
            handler = new HttpClientHandler() {
                CookieContainer = new CookieContainer(),
                Proxy = proxy,
            };

            httpClient = new HttpClient(handler) {
                BaseAddress = new Uri(UrlProvider.base_url_https)
            };

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("USERNAME", username),
                new KeyValuePair<string, string>("PASSWORD", password),
            });
            response = httpClient.PostAsync(url_provider.get_login_url(), content).Result;
 
            if (check_login_from_response(response))
                return true;
            else
                return false;
        }

        public bool check_login_from_response(HttpResponseMessage response)
        {
            string the_page = response.Content.ReadAsStringAsync().Result;

            if (!the_page.Contains("login-form"))
                return true;
            else
                return false;
        }

    }
}
