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
                Console.WriteLine($"File loaded: {fileAsArray.Length} lines found\n");

                int countValidLines = ProcessReports(
                    fileAsArray,
                    units,
                    reports,
                    priorities,
                    scores,
                    statuses);

                Console.WriteLine($"Stored {countValidLines} valid records for analysis.");

                DisplayHighestPriorityApprovode(
                    units,
                    reports,
                    priorities,
                    scores,
                    statuses,
                    countValidLines);


            }
        }


        static string[]? LoadFile(string path)
        {
            bool existed = File.Exists(path);
            if (!existed) { return null; }

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
            Console.WriteLine("Processing complete.\n");
            Console.WriteLine($"Valid lines: {countValidLines}\n");
            Console.WriteLine($"Invalid lines: {countInvalidLines}\n");

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
            if (!HasAny(unit)) { return false; }

            if (!Enum.TryParse<Reports>(fields[1].Trim(), true, out report)) { return false; }

            if (!int.TryParse(fields[2].Trim(), out priority) &&
                priority < 1 || priority > 5) { return false; }

            if (!double.TryParse(fields[3].Trim(), out score) ||
                (score > 100.0 || score < 0.0)) { return false; }

            if (!Enum.TryParse<Statuses>(fields[4].Trim(), true, out status)) { return false; }

            else { return true; }


        }

        static bool HasAny(string unit)
        {
            return unit.Length != 0;
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

        static int CountByType(Reports[] reports, int len, Reports selectedReport)
        {
            int counter = 0;
            foreach (Reports currReport in reports)
            {
                if (currReport == selectedReport)
                {
                    counter++;
                }
            }
            return counter;
        }

        //static void DisplayBasicStatistics(double[] scores, int len)
        //    {

        //    }

        static void DidplayStatusCounts(Statuses[] statuses, int len)
        {
            int countPending = CountByStatus(statuses, len, Statuses.Pending);
            int countApproved = CountByStatus(statuses, len, Statuses.Approved);
            int coiuntRejected = CountByStatus(statuses, len, Statuses.Rejected);
            Console.WriteLine($"Pending: {countPending}\n");
            Console.WriteLine($"Approved: {countApproved}\n");
            Console.WriteLine($"Rejected: {coiuntRejected}\n");
        }

        static void DisplayTypeCount(Reports[] reports, int len)
        {
            int countCollect = CountByType(reports, len, Reports.Collect);
            int countAnalyze = CountByType(reports, len, Reports.Analyze);
            int countRecon = CountByType(reports, len, Reports.Recon);
            int countIntel = CountByType(reports, len, Reports.Intel);
            Console.WriteLine("\n=== Reports By Type ===\n");
            Console.WriteLine($"Collect: {countCollect}");
            Console.WriteLine($"Analyze: {countAnalyze}");
            Console.WriteLine($"Recon: {countRecon}");
            Console.WriteLine($"Intel: {countIntel}\n");
        }
        static void DisplayHighestPriorityApprovode(
            string[] units,
            Reports[] reports,
            int[] priorities,
            double[] scores,
            Statuses[] statuses,
            int len)
        {
            int maxPriority = GetMaxPriority(priorities, statuses, len);
            for (int i =  0; i < len; i++)
            {
                if (priorities[i] == maxPriority &&
                    statuses[i] == Statuses.Approved)
                {
                    Console.WriteLine("\n=== Highest Priority Approved Report ===\n");
                    Console.WriteLine($"Unit: {units[i]}");
                    Console.WriteLine($"Type: {reports[i]}");
                    Console.WriteLine($"Priority: {maxPriority}");
                    Console.WriteLine($"Score: {scores[i]}\n");
                    break;
                }
            }
        }

        static int GetMaxPriority(int[] priorities, Statuses[] statuses, int len)
        {
            int maxPriority = 1;
            for (int i = 0; i < len; i++)
            {
                if (statuses[i] == Statuses.Approved)
                {
                    if (maxPriority < priorities[i])
                    {
                        maxPriority = priorities[i];
                    }
                }
            }
            return maxPriority;
        }

        static void DisplayAverageByPriority(int[] priorities, double[] scores, int len)
        {
            double[] totals = new double[5];
            int[] counts = new int[5];
            int currrPriority;

            for (int i = 0; i < len; i++)
            {
                currrPriority = priorities[i];
                totals[currrPriority] = scores[i];
                counts[currrPriority]++;
            }

            Console.WriteLine("\n=== Average Score By Priority ===\n");
            for (int i = 0; i <= 5; i++)
            {            
                if (totals[i] != 0)
                {
                    double average = totals[0] / counts[i];
                    Console.WriteLine($"Priority {i}: {average}");
                }
                else
                {
                    Console.WriteLine($"Priority {i}: no report");
                }
            }
        }

    }
}

