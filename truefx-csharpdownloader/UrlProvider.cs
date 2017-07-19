using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace truefx_csharpdownloader
{
    class UrlProvider
    {

        public const string base_url_https = "https://www.truefx.com/";
        public const string base_url_http = "http://www.truefx.com/";

        string[] months = new string[12] {
            "January",
           "February",
           "March",
           "April",
           "May",
           "June",
           "July",
           "August",
           "September",
           "October",
           "November",
           "December"
            };


        public string get_login_url()
        {
            //return base_url_https + "?page=loginz";
            return "?page=loginz";
        }

        public string get_download_referrer_url(int year, int month)
        {
            return base_url_http
                + "?page=download&description="
                + months[month-1].ToLower() + year
                + "&dir=" + year + "/"
                + months[month-1].ToUpper() + "-" + year;
         }

        public string get_download_url(int year, int month, string symbol)
        {
            return base_url_http
               + "dev/data/"
               + year + "/"
               + months[month-1].ToUpper() + "-" + year + "/" + symbol.ToUpper() + "-"
               + year + "-" + month.ToString().PadLeft(2, '0') + ".zip";
        }

    }
}
