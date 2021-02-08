using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using NLog;

namespace TextComparer
{
    /// <summary>
    /// Class for comparison of two texts.
    /// Uses DiffPlex library.
    /// </summary>
    public class Comparator
    {
        // Logs varible
        public static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Main method for comparison, returns list of Line objects which keeps all information about difference
        /// </summary>
        /// <param name="textFirst">First text</param>
        /// <param name="textSecond">Second text</param>
        /// <returns></returns>
        public static List<Line> Compare(string textFirst, string textSecond)
        {
            // List of differences objects
            List<Line> list = new List<Line>();

            logger.Debug($"Geting the inline textual diffs");

            //Getting differencies model object that keeps information about each string difference
            var builder = InlineDiffBuilder.Diff(textFirst, textSecond, true);

            logger.Debug($"Creating DiffResault");

            // Getting DiffResault object that keeps arrays of strings.
            var differ = new Differ().CreateLineDiffs(textFirst, textSecond, true);

            var strings = differ.PiecesOld;

            // Poured the array into the list to get to the sorting methods, but in the end it wasn't necessary
            List<string> rows = new List<string>();

            logger.Debug("Filling list by strings");
            if (strings.Length > 0)
            {
                foreach (var item in strings)
                    rows.Add(item);
            }
            else
            {
                logger.Debug("Returned list = null, strings array is empty");
                return null;
            }

            // Catching differencies. For some reason ChangeType.Modified value doesn't works, so i had to improvise
            foreach (var line in builder.Lines)
            {
                if (string.IsNullOrWhiteSpace(line.Text))
                {
                    continue;
                }

                switch (line.Type)
                {
                    // Deleted string
                    case ChangeType.Deleted:
                        logger.Debug("Text deleted");
                        // Here i add new Line object to list.
                        list.Add(new Line() { text = line.Text, lineNumber = rows.IndexOf(line.Text) + 1, lineAction = Line.LineAction.Deleted });
                        break;
                    // My monster is here. This part detects if the difference is added string or modified.
                    case ChangeType.Inserted:
                        bool flag = false;
                        foreach (var item2 in list)
                        {  
                            if (item2.lineNumber == line.Position && item2.lineAction == Line.LineAction.Deleted)
                            {
                                logger.Debug("Text modified");
                                list.Remove(item2);
                                list.Add(new Line() { text = line.Text, lineNumber = line.Position, lineAction = Line.LineAction.Modified });
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                            logger.Debug("Text Added");
                            list.Add(new Line() { text = line.Text, lineNumber = line.Position, lineAction = Line.LineAction.Added });
                        break;
                }
            }
            return list;
        }

        /// <summary>
        /// Saving output file method.
        /// </summary>
        /// <param name="output">What to save to output</param>
        public static void SaveOutput(string output)
        {
            logger.Debug("Reading answer from user");
            string answer = Console.ReadLine();

            while (answer.Length > 1 || (answer != "y" && answer != "n"))
            {
                logger.Debug("Incorrect answer by user");
                Console.WriteLine("enter 'y' or 'n'");
                answer = Console.ReadLine();                
            }

            if (answer == "y")
            {
                try
                {
                    logger.Debug("Answer - Yes, saving output file to MyDocuments folder");
                    File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Output.txt", output);
                    Console.WriteLine("File saved\nHope see you again");
                    Thread.Sleep(2000);
                }
                catch (Exception exp)
                {
                    logger.Error(exp, "Error when saving output file");
                    Console.WriteLine(exp.Message);
                }
            }
            else if (answer == "n")
            {
                logger.Debug("Answer - No");
                Console.WriteLine("Hope see you again");

                Thread.Sleep(2000);
            }
        }
    }
}
