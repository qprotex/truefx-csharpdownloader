using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace truefx_csharpdownloader
{
    class Manager
    {

        UrlProvider url_provider = new UrlProvider();
        HttpClient httpClient;
        HttpClientHandler handler;


        public void download_and_merge_to_one_file(int year, string symbol, string directory)
        {

            download_for_year(year, symbol, directory);
            unzip_for_year(year, symbol, directory);

            string output_filename = symbol.ToLower() + "-" + year + ".csv";
            List<string> files = get_filenames_to_merge(year, symbol, directory);

            using (var output = File.Create(output_filename))
            {
                foreach (var file in files)
                {
                    using (var input = File.OpenRead(file))
                    {
                        input.CopyTo(output);
                    }
                }
            }

            List<string> deleteFiles = get_filenames_to_delete(year, symbol, directory);
            foreach (string file in deleteFiles)
            {
                File.Delete(file);
            }
            
        }

        public List<string> get_filenames_to_merge(int year, string symbol, string directory) {
            return Directory.EnumerateFiles(directory, symbol.ToUpper() + '-' + year + "-*.csv", SearchOption.AllDirectories).ToList<string>();

        }

        public List<string> get_filenames_to_delete(int year, string symbol, string directory)
        {
            return Directory.EnumerateFiles(directory, symbol.ToUpper() + '-' + year + "-*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".csv") || s.EndsWith(".zip")).ToList<string>();
        }

        public void unzip_for_year(int year, string symbol, string directory) {

            for (int month = 1; month <= 12; month++)
                unzip_for_month(year, month, symbol, directory);
        }

        public void unzip_for_month(int year, int month, string symbol, string directory) {

            string filename = get_downloaded_filename(month, symbol, year);

            string filename_with_directory = directory + filename;

            try
            {
                ZipFile.ExtractToDirectory(filename_with_directory, directory);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error uncompressing file {0} - {1}", filename_with_directory, ex.Message);
            }
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

            HttpResponseMessage response = null;
            handler = new HttpClientHandler() {
                CookieContainer = new CookieContainer()
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
