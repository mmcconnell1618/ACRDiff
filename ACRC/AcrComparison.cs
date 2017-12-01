using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ACRDiff
{
    class AcrComparison
    {
        // Properties
        public List<int> NameIndexes { get; set; }
        public int AmountIndex { get; set; }
        public string File1 { get; set; }
        public string File1Title { get; set; }
        public string File2 { get; set; }
        public string File2Title { get; set; }
        public string OutputFile { get; set; }
        public string OutputTitle { get; set; }

        // Constructor
        public AcrComparison(Dictionary<string, string> args)
        {
            // Parse the args
            File1 = Parse(args, "file1");
            File1Title = Parse(args, "title1");
            File2 = Parse(args, "file2");
            File2Title = Parse(args, "title2");
            OutputFile = Parse(args, "output");
            OutputTitle = Parse(args, "outputtitle");

            // Set Titles to defaults if not specified
            if (File1Title.Length < 1) File1Title = Path.GetFileNameWithoutExtension(File1);
            if (File2Title.Length < 1) File2Title = Path.GetFileNameWithoutExtension(File2);
            if (OutputTitle.Length < 1) OutputTitle = Path.GetFileNameWithoutExtension(OutputFile);

            ParseNameIndexes(args);
            ParseAmountIndex(args);
        }

        #region argument parsing
        private void ParseAmountIndex(Dictionary<string,string> args)
        {
            if (args.ContainsKey("amountindex"))
            {
                this.AmountIndex = 5;

                    int parsed = -1;
                    if (int.TryParse(args["amountindex"], out parsed))
                    {
                        if (parsed >= 0)
                        {
                            this.AmountIndex = parsed;
                        }
                    }
            }
            else
            {
                this.AmountIndex = 5;
            }
        }
        private void ParseNameIndexes(Dictionary<string, string> args)
        {
            if (args.ContainsKey("nameindexes"))
            {
                this.NameIndexes = new List<int>();
                var indexes = args["nameindexes"].Split(',');
                foreach (var index in indexes)
                {
                    int parsed = -1;
                    if (int.TryParse(index, out parsed))
                    {
                        if (parsed >= 0)
                        {
                            this.NameIndexes.Add(parsed);
                            parsed = -1;
                        }
                    }
                }
            }
            else
            {
                this.NameIndexes = new List<int>() { 0, 1, 2, 3, 4 };
            }
        }
        public Boolean ArgsValid()
        {
            if (File1.Length < 1) return false;
            if (File2.Length < 1) return false;
            if (OutputFile.Length < 1) return false;

            return true;
        }
        private string Parse(Dictionary<string, string> args, string keyName)
        {
            string result = "";

            if (args.ContainsKey(keyName))
            {
                result = args[keyName];
                result = result.Trim();
            }

            return result;
        }

        #endregion

        public void Compare()
        {
            Console.WriteLine();
            Console.WriteLine("#");
            Console.WriteLine("# Starting Comparison ");
            Console.WriteLine("#");
            Console.WriteLine();

            if (!File.Exists(File1))
            {
                Console.WriteLine(" ERROR: Missing file1: " + File1);
            }
            if (!File.Exists(File2))
            {
                Console.WriteLine(" ERROR: Missing file2: " + File2);
            }

            Dictionary<string, AzureLineItem> MergedLines = new Dictionary<string, AzureLineItem>();

            // Read file 1
            foreach(string line1 in File.ReadAllLines(File1))
            {
                ParseLine(line1, MergedLines, false);
            }

            // Read file 2
            foreach (string line2 in File.ReadAllLines(File2))
            {
                ParseLine(line2, MergedLines, true);
            }

            /*foreach(var kvp in MergedLines)
            {
                Console.WriteLine(kvp.Value.Output());
            }*/

            StringBuilder sb = new StringBuilder();
            sb.Append("Level1,Level2,Level3,Level4,Level5," + File1Title + "," + File2Title + ",Change " + OutputFile + System.Environment.NewLine);
            foreach(var kvp in MergedLines)
            {
                sb.Append(kvp.Value.Output());
            }

            if (File.Exists(OutputFile))
            {
                Console.WriteLine();
                Console.WriteLine(" WARNING: Output file exists. Overwrite? (Y/N)");
                var choice = Console.ReadKey();
                if (choice.Key == ConsoleKey.Y)
                {
                    File.WriteAllText(OutputFile, sb.ToString());
                }
                else
                {
                    Console.WriteLine("Cancelled output!");
                }
            }
            else
            {
                File.WriteAllText(OutputFile, sb.ToString());
            }
        }

        private void ParseLine(string input, Dictionary<string, AzureLineItem> merged, bool isSecondFile)
        {
            try
            {
                string[] parts = input.Split(',');
                string key = string.Empty;
                foreach(int keyIndex in this.NameIndexes)
                {
                    key += parts[keyIndex] + "|";
                }
                key = key.TrimEnd('|');

                Decimal amount = -1;
                string inputAmount = parts[AmountIndex].Replace('$',' ').Trim();
                if (Decimal.TryParse(inputAmount,out amount))
                {
                    // Check for exists
                    if (merged.ContainsKey(key))
                    {
                        if (isSecondFile)
                        {
                            merged[key].Amount2 = amount;
                        }
                        else
                        {
                            merged[key].Amount1 = amount;
                        }
                    }
                    else
                    {
                        if (isSecondFile)
                        {
                            merged.Add(key, new AzureLineItem(key, -1, amount));
                        }
                        else
                        {
                            merged.Add(key, new AzureLineItem(key, amount, -1));
                        }
                    }
                }
                else
                {
                    amount = -1;
                    return;
                }                
            }
            catch(Exception ex)
            {
                Console.WriteLine("Parse Exception: " + ex.Message);
            }
        }
    }
}
