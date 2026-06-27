using System.IO;
using System;


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

                DisplayBasicStatistics(scores, countValidLines);

                DisplayStatusCounts(statuses, countValidLines);

                DisplayTypeCount(reports, countValidLines);

                DisplayHighestPriorityApproved(
                    units,
                    reports,
                    priorities,
                    scores,
                    statuses,
                    countValidLines);

                DisplayAverageByPriority(priorities, scores, countValidLines);
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
                    if (currIndex >= MAX_LINES)
                    {
                        Console.WriteLine("warning: Maximum number of reports reached.");
                        break;
                    }
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

            //Check 'line'. Whether has only 5 fields.
            if (fields.Length != 5) { return false; }

            // Check 'unit'. Whether has any value.
            unit = fields[0].Trim();
            if (!HasAny(unit)) { return false; }

            // Check 'report'. Whether the report is one of the 'Reports'.
            if (!Enum.TryParse<Reports>(fields[1].Trim(), true, out report)) { return false; }

            // Check 'priority'
            // 1. whether is a digit.
            if (!int.TryParse(fields[2].Trim(), out priority)) { return false; }
            // 2. Between 1 and 5.
            if (priority < 1 || priority > 5) { return false; }

            // Check 'score'
            // 1. whether is a double
            if (!double.TryParse(fields[3].Trim(), out score)) { return false; }
            // 2. Between 0.0 and 100.0.
            if (score > 100.0 || score < 0.0) { return false; }

            // Check 'status'. Whether the report is one of the 'Statuses'.
            if (!Enum.TryParse<Statuses>(fields[4].Trim(), true, out status)) { return false; }

            return true;
        }


        static bool HasAny(string unit)
        {
            return unit.Length != 0;
        }

        static double CalculateAverage(double[] scores, int len)
        {
            double total = 0;
            for (int i = 0; i < len; i++)
            {
                double currScore = scores[i];
                total += currScore;
            }
            double average = total / len;
            return Math.Round(average, 2);

        }

        static double FindMaxScore(double[] scores, int len)
        {
            double maxi = scores[0];
            for (int i = 1; i < len; i++)
            {
                double currScore = scores[i];
                if (maxi < currScore)
                {
                    maxi = currScore;
                }
            }
            return maxi;
        }
        
        static double FindMinScore(double[] scores, int len)
        {
            double mini = scores[0];
            for (int i = 1; i < len; i++)
            {
                double currScore = scores[i];
                if (mini > currScore)
                {
                    mini = currScore;
                }
            }
            return mini;
        }
        static void DisplayBasicStatistics(double[] scores, int validReports)
        {
            int total = validReports;
            double average = CalculateAverage(scores, validReports);
            double highScore = FindMaxScore(scores, validReports);
            double lowScore = FindMinScore(scores, validReports);

            Console.WriteLine("\n=== Report Statistics ===\n");
            Console.WriteLine($"Total Reports: {total}");
            Console.WriteLine($"Average Score: {average}");
            Console.WriteLine($"Highest Score: {highScore}");
            Console.WriteLine($"Lowest Score: {lowScore}");
        }

        static int CountByStatus(Statuses[] statuses, int len, Statuses selectedStatus)
        {
            int counter = 0;
            for (int i = 0; i < len; i++)
            {
                Statuses currStatus = statuses[i];
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
            for (int i = 0; i < len; i++)
            {
                Reports currReport = reports[i];
                if (currReport == selectedReport)
                {
                    counter++;
                }
            }
            return counter;
        }

        static void DisplayStatusCounts(Statuses[] statuses, int len)
        {
            int countPending = CountByStatus(statuses, len, Statuses.Pending);
            int countApproved = CountByStatus(statuses, len, Statuses.Approved);
            int countRejected = CountByStatus(statuses, len, Statuses.Rejected);
            Console.WriteLine("\n=== Report By Status ===\n");
            Console.WriteLine($"Pending: {countPending}");
            Console.WriteLine($"Approved: {countApproved}");
            Console.WriteLine($"Rejected: {countRejected}\n");
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
        static void DisplayHighestPriorityApproved(
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
            double[] totals = new double[6];
            int[] counts = new int[6];
            int currrentPriority;

            for (int i = 0; i < len; i++)
            {
                currrentPriority = priorities[i];
                totals[currrentPriority] += scores[i];
                counts[currrentPriority]++;
            }

            Console.WriteLine("\n=== Average Score By Priority ===\n");
            for (int i = 1; i <= 5; i++)
            {            
                if (counts[i] != 0)
                {
                    double average = totals[i] / counts[i];
                    double roundedAverage = Math.Round(average, 2);
                    Console.WriteLine($"Priority {i}: {roundedAverage}");
                }
                else
                {
                    Console.WriteLine($"Priority {i}: no report");
                }
            }
        }
 
    }

}

