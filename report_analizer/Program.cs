using System.IO;
using System;


namespace Analyzer
{
    class ReportAnalyzer
    {
        static string filepath = "reports.txt";
        enum Reports
        {
            Collect,
            Analyze,
            Recon,
            Intel
        }
        enum Statuses
        {
            Pending,
            Approved,
            Rejected
        }
        const int MAX_LINES = 0;

        static void Main()
        {
            string[] units = new string[MAX_LINES];
            string[] reports = new string[MAX_LINES];
            int[] priorities = new int[MAX_LINES];
            int[] scores = new int[MAX_LINES];
            string[] statuses = new string[MAX_LINES];


            if (!IsExists(filepath))
            {
                Console.WriteLine($"Error: File '{filepath}' not found.");
            }
            else
            {
                string[]? fileAsArray = LoadFile(filepath);
                if (fileAsArray.Length == 0)
                {
                    Console.WriteLine("Error: File is empty");
                }
                else
                {
                    Console.WriteLine($"File loaded: {fileAsArray.Length} lines found");
                }
            }







        }
        
        static bool IsExists(string path)
        {
            bool existed = File.Exists(path);
            return existed;
        }

        static string[] LoadFile(string path)
        {
            string[] fileAsArray = File.ReadAllLines(filepath);
            return fileAsArray;
        }

        static int ProcessReports(
            string[] fileAsArray, 
            ref string[] units,
            ref string[] reports,
            ref int[] priorities,
            ref int[] scores,
            ref string[] statuses
            )
        {

        }

        static string ProcessLine(string line)
        {

        }
    }
}
