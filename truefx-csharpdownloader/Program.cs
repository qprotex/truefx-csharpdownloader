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

            try
            {
                p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("bundling: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `truefx_csharpdownloader --help' for more information.");
                return;
            }

            if (truefx_username == null || truefx_password == null || destination_folder == null || year == 0 || symbol == null) {
                print_usage(p);
                Environment.Exit(2);
             }

            if (show_help)
            {
                print_usage(p);
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

        static void print_usage(OptionSet p)
        {
            //Console.WriteLine("Usage: python get_data_for_year_in_csv.py -u <truefxUsername> -p <truefxPassword> -f <folder> -y <year> -s <symbol>");
            Console.WriteLine("Usage: truefx_csharpdownloader [OPTIONS]+");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }


    }
}
