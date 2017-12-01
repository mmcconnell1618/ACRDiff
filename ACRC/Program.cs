using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ACRDiff
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("#####");
            Console.WriteLine("##### Azure Consumed Revenue Diff");
            Console.WriteLine("#####");
            Console.WriteLine();

            Dictionary<string, string> parsedArgs = ParseArguments(args);
            var acrc = new AcrComparison(parsedArgs);


            if (acrc.ArgsValid() == true)
            {
                acrc.Compare();
            }
            else
            {
                ShowHelp();
            }

        }

        private static Dictionary<string, string> ParseArguments(string[] args)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (args.Length == 3)
            {
                // Short cut parsing
                result.Add("file1", args[0]);
                result.Add("file2", args[1]);
                result.Add("output", args[2]);
            }
            else
            {
                string keyName = "";
                string keyValue = "";
                bool parsingName = true;

                for (int i = 0; i < args.Length; i++)
                {
                    if (parsingName)
                    {
                        keyName = args[i].TrimStart('-').Trim().ToLowerInvariant();
                        parsingName = false;
                    } else
                    {
                        keyValue = args[i];
                        result.Add(keyName, keyValue);
                        keyName = "";
                        keyValue = "";
                        parsingName = true;
                    }
                }
                
            }

            return result;
        }

        private static void ShowHelp()
        {
            Console.WriteLine("This program takes two comma separated value (CSV) files from the Azure Health");
            Console.WriteLine("Report and shows changes between the two files.");
            Console.WriteLine();
            Console.WriteLine("    Example 1:");
            Console.WriteLine("    ACRdiff.exe file1.csv file2.csv output.csv");
            Console.WriteLine();
            Console.WriteLine("    Example 2:");
            Console.WriteLine("    ACRdiff.exe -file1 Sept.csv -file2 Oct.csv -output Report.csv ");
            Console.WriteLine("                -Title1 \"September 2017\" -title2 \"October 2017\"");
            Console.WriteLine("                -outputtitle \"Changes Sep to Oct 2017\"");
            Console.WriteLine();
            Console.WriteLine("  Required Parameters:");
            Console.WriteLine("    -file1            Path to the first file  (c:\\myfile1.csv)");
            Console.WriteLine("    -file2            Path to the second file (c:\\myfile2.csv)");
            Console.WriteLine("    -output           Path for output file    (c:\\putput.csv)");
            Console.WriteLine("");
            Console.WriteLine("  Optional Parameters:");
            Console.WriteLine("    -title1           Title for file 1");
            Console.WriteLine("    -title2           Title for file 2");
            Console.WriteLine("    -outputitle       Title for output file");
            Console.WriteLine("    -amountindex      Zero based index of the column for amount");
            Console.WriteLine("    -nameindexes      Comma separated list of column indexes for");
            Console.WriteLine("                      fields to make up a unique name (0,1,2,3)");
            Console.WriteLine();
        }
    }
}
