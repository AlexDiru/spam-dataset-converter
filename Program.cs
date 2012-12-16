using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace spam_email_convertor
{
    class Program
    {
        /// <summary>
        /// Tests if a character is a boundary character
        /// </summary>
        static bool IsBoundaryCharacter(char c)
        {
            //Bounded by NON-ALPHANUMERIC characters
            if (Char.IsLetterOrDigit(c))
                return false;

            return true;
        }

        /// <summary>
        /// Counts the frequency of a word in a string
        /// </summary>
        static int CountFrequency(String email, String word)
        {
            int freq = 0;

            for (int i = 0; i < (email.Length - word.Length + 1); i++)
            {
                //If word is equal
                if (email.Substring(i, word.Length).ToLower() == word.ToLower())
                    //If string boundaries
                    if (i == 0)
                    {
                        if (IsBoundaryCharacter(email[i + word.Length]))
                            freq++;
                    }
                    else if (i == (email.Length - word.Length))
                    {
                        if (IsBoundaryCharacter(email[i - 1]))
                            freq++;
                    }
                    else if (IsBoundaryCharacter(email[i - 1]) && IsBoundaryCharacter(email[i + word.Length]))
                        freq++;
            }

            return freq;
        }

        /// <summary>
        /// Joins an array of strings together
        /// </summary>
        static String Join(String[] list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var d in list)
                sb.Append(d);

            return sb.ToString();
        }

        /// <summary>
        /// Tests if a character is a capital letter
        /// </summary>
        static bool IsCapital(char c)
        {
            return c >= 'A' && c <= 'Z';
        }

        /// <summary>
        /// Gets average capital runs, longest capital run, and count of capital runs
        /// </summary>
        static Tuple<double, int, int> GetCapitalRunLength(String email)
        {
            List<int> allRuns = new List<int>();
            int currentRun = 0;

            for (int i = 0; i < email.Length; i++)
            {
                if (IsCapital(email[i]))
                    currentRun++;
                else if (currentRun > 0)
                {
                    allRuns.Add(currentRun);
                    currentRun = 0;
                }
            }

            return new Tuple<double, int, int>(allRuns.Average(), allRuns.Max(), allRuns.Count());
        }

        /// <summary>
        /// Counts the number of words in a string
        /// </summary>
        static int CountWords(String email)
        {
            int words = 0;
            bool inWord = false;
            for (int i = 0; i < email.Length; i++)
            {
                if (Char.IsLetterOrDigit(email[i]))
                {
                    inWord = true;
                    continue;
                }
                else if (inWord)
                {
                    words++;
                    inWord = false;
                }
            }

            return words;
        }

        static int CountChar(String email, char c)
        {
            int count = 0;
            for (int i = 0; i < email.Length; i++)
                if (email[i] == c)
                    count++;
            return count;
        }

        static String ConvertEmail(String fileLocation, bool isSpam)
        {
            String email;
            String dataItemSeparator = "\t";

            try
            {
                email = Join(System.IO.File.ReadAllLines(fileLocation));
            }
            catch (System.IO.FileNotFoundException)
            {
                return "File " + fileLocation + " does not exist";
            }

            String[] words = { "make", "address", "all", "3d", "our", "over", "remove",
                                 "internet", "order", "mail", "receive", "will", "people",
                                 "report", "addresses", "free", "business", "email", "you",
                                 "credit", "your", "font", "000", "money", "hp", "hpl", "george",
                                 "650", "lab", "labs", "telnet", "857", "data", "415", "85",
                                 "technology", "1999", "parts", "pm", "direct", "cs", "meeting",
                                 "original", "project", "re", "edu", "table", "conference" };

            String dataEntry = "";
            String header = "57" + dataItemSeparator + "1" + dataItemSeparator + "1" + dataItemSeparator + "2";
            int wordCount = CountWords(email);

            foreach (var word in words)
            {
                dataEntry += (100f * (float)CountFrequency(email, word)/(float)wordCount) + dataItemSeparator;
            }

            //Char freq
            char[] chars = { ';', '(', '[', '!', '$', '#' };

            foreach (var c in chars)
            {
                dataEntry += (100f * (float)CountChar(email, c)/(float)email.Length) + dataItemSeparator;
            }

            //Capital runs
            var tuple = GetCapitalRunLength(email);
            dataEntry += tuple.Item1 + dataItemSeparator;
            dataEntry += tuple.Item2 + dataItemSeparator;
            dataEntry += tuple.Item3 + dataItemSeparator;

            //Is spam
            if (isSpam)
                dataEntry += "1";
            else
                dataEntry += "0";

            return header + "\n" + dataEntry + "\n" + dataEntry + "\n" + dataEntry;
        }

        static void Main(string[] args)
        {
            //First argument should be the location of the file containing the email
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: <program> <input> <output> <spam>\n<program>: The file path of this program\n<input>: The file path of the spam email\n<output>: The file path where the one entry data set should be created\n<spam>: (Y/N) whether the email is spam");
                return;
            }

            bool spam;
            try
            {
                spam = args[2][0].ToString().ToLower().Equals("y");
            }
            catch
            {
                Console.WriteLine("<spam> argument is incorrect");
                return;
            }

            System.IO.File.WriteAllLines(args[1], (ConvertEmail(args[0], spam)).Split("\n".ToCharArray()));
        }
    }
}
