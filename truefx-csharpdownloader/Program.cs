using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace truefx_csharpdownloader
{
    class Program
    {

        static bool show_help = false;
        static string truefx_username = null;
        static string truefx_password = null;
        static string destination_folder = null;
        static int year;
        static string symbol = null;

        static void Main(string[] args)
        {

            var p = new OptionSet() {
                { "u|username=", "username", v => truefx_username=v },
                { "p|password=", "password", v => truefx_password=v },
                { "f|folder=", "folder", v => destination_folder=v },
                { "y|year=", "year", v => year=int.Parse(v) },
                { "s|symbol=", "symbol", v => symbol=v },
                { "h|help",  "show this message and exit", v => show_help = v != null },
            };

            List<string> extra = p.Parse(args);

            if (show_help)
            {
                ShowHelp(p);
                return;
            }

            Manager truefxManager = new Manager();

            if (truefxManager.login_to_true_fx(truefx_username, truefx_password)) {
                Console.WriteLine("Successfully logged to TrueFx");
                truefxManager.download_and_merge_to_one_file(year, symbol, destination_folder + '\\');
            }
            else {
                Console.WriteLine("Can't login to TrueFx");
            }


        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: python get_data_for_year_in_csv.py -u <truefxUsername> -p <truefxPassword> -f <folder> -y <year> -s <symbol>");
        }


    }
}
