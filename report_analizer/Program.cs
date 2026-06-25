using System.IO;
using System;
using System.Xml.Schema;
using System.Formats.Asn1;



namespace Analyzer
{
    class ReportAnalyzer
    {
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

        static string filepath = "reports.txt";
        const int MAX_LINES = 100;

        static void Main()
        {
            string[] units = new string[MAX_LINES];
            Reports[] reports = new Reports[MAX_LINES];
            int[] priorities = new int[MAX_LINES];
            double[] scores = new double[MAX_LINES];
            Statuses[] statuses = new Statuses[MAX_LINES];

            string[]? fileAsArray = LoadFile(filepath);
            if (fileAsArray is null)
            {
                Console.WriteLine($"Error: File '{filepath}' not found.");
            }
            else if (fileAsArray.Length == 0)
            {
                Console.WriteLine("Error: File is empty");
            }
            else
            {
                Console.WriteLine($"File loaded: {fileAsArray.Length} lines found");

                int countValidLines = ProcessReports(
                    fileAsArray,
                    units,
                    reports,
                    priorities,
                    scores,
                    statuses);


                //DisplayLines(
                //    countValidLines,
                //    units,
                //    reports,
                //    priorities,
                //    scores,
                //    statuses);
            }
        }


        static string[]? LoadFile(string path)
        {
            bool existed = File.Exists(path);
            if (!existed) { return null;}

            else
            {
                string[] fileAsArray = File.ReadAllLines(path);
                return fileAsArray;
            }
        }
   static int ProcessReports(
            string[] fileAsArray,
            string[] units,
            Reports[] reports,
            int[] priorities,
            double[] scores,
            Statuses[] statuses)
        {
            int currIndex = 0;
            foreach (string line in fileAsArray)
            {
                 bool isValid = ProcessLine(
                    line,
                    out string unit,
                    out Reports report,
                    out int priority,
                    out double score,
                    out Statuses status);

                if (isValid)
                {
                    units[currIndex] = unit;
                    reports[currIndex] = report;
                    priorities[currIndex] = priority;
                    scores[currIndex] = score;
                    statuses[currIndex] = status;

                    currIndex++;
                }
            }
            int countValidLines = currIndex;
            int countInvalidLines = fileAsArray.Length - countValidLines;
            Console.WriteLine("Processing complete.");
            Console.WriteLine($"Valid lines: {countValidLines}");
            Console.WriteLine($"Invalid lines: {countInvalidLines}");

            return countValidLines;

        }

        static bool ProcessLine(
            string line,
            out string unit,
            out Reports report,
            out int priority,
            out double score,
            out Statuses status)

        {
            unit = "";
            report = default;
            priority = 0;
            score = 0.0;
            status = default;


            string[] fields = line.Split(",");

            if (fields.Length != 5) { return false; }

            unit = fields[0].Trim();
            if (! HasAny(unit)){ return false; }

            if (! Enum.TryParse<Reports>(fields[1].Trim(), true, out report)){ return false; }

            if (! int.TryParse(fields[2].Trim(), out priority)&&
                priority < 1 || priority > 5) { return false; }

            if (! double.TryParse(fields[3].Trim(), out score) ||
                (score > 100.0 || score <0.0)) { return false; }

            if (! Enum.TryParse<Statuses>(fields[4].Trim(), true, out status)) { return false; }

            else { return true; }


        }

        static bool HasAny(string unit)
        {
            return unit.Length != 0;
        }

        static void DisplayLines(
            int len,
            string[] units,
            Reports[] reports,
            int[] priorities,
            double[] scores,
            Statuses[] statuses)
        {

            int currIndex = 0;
            for (int i = 0; i < len; i++)
            {
                Console.WriteLine($"{units[currIndex]}, {reports[currIndex]}, {priorities[currIndex]}, {scores[currIndex]}, {statuses[currIndex]}");
                currIndex++;
            }
        }

        static double CalculateAverage(double[] scores, int len)
        {
            double total = 0;
            foreach (double score in scores)
            {
                total += score;
            }
            double average = total / len;
            return average;
            
        }

        static double FindMaxScore(double[] scores, int len)
        {
            double maxi = scores[0];
            foreach (double score in scores)
            {
                if (maxi < score)
                {
                    maxi = score;
                }
            }
            return maxi;
        }

        static int CountByStatus(Statuses[] statuses, int len, Statuses selectedStatus)
        {
            int counter = 0;
            foreach (Statuses currStatus in statuses)
            {
                if (currStatus == selectedStatus)
                {
                    counter++;
                }
            }
            return counter;
        }

    }
}

