using System;
using System.Collections.Generic;
using System.IO;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using DiffPlex.Model;

namespace TextComparer
{
    class Program
    {        
        static string output = "";
        static string textFist = "";
        static string textSecond = "";
        static void Main(string[] args)
        {
            try
            {                
                Comparator.logger.Debug("Trying to read text files");
                Console.WriteLine("Specify the path to the first file");

                // Reading first file from user's path
                textFist = File.ReadAllText(Console.ReadLine());

                Console.WriteLine("Specify the path to the second file");

                // Reading second file from user's path
                textSecond = File.ReadAllText(Console.ReadLine());
            }
            catch (Exception exp)
            {
                // Exception to log
                Comparator.logger.Error(exp, "Error when trying to read text files");
                Console.WriteLine(exp.Message);
            }

            // Getting list of differencies between text files 
            var list = Comparator.Compare(textFist, textSecond);

            //Filling output string varible
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    output += $"For line {item.lineNumber} {item.lineAction}  <{item.text}> \n";
                }
            }
            

            Console.WriteLine("\n" + output);

            Console.WriteLine("Do you want to save output file in Docs (y/n)?");

            //Saving output file in MyDocuments
            Comparator.SaveOutput(output);

        }
    }
}
