using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using documentvaultapi.Helper;

namespace documentvaultapi.Helper
{
    public static class CommonHelper
    {
        public static int calculateAvailableQuantity(short currentPosition, short end, short start)
        {
            return (currentPosition == 0) ? (end - start) + 1 : (end - currentPosition) + 1;
        }

        public static List<(int start, int end)> SplitRange(int start, int end, List<int> exclusions)
        {
            List<(int start, int end)> ranges = new List<(int start, int end)>();

            // Sort the exclusions to handle them in order
            exclusions.Sort();

            int currentStart = start;

            foreach (int exclusion in exclusions)
            {
                if (exclusion >= currentStart && exclusion <= end)
                {
                    if (exclusion > currentStart)
                    {
                        ranges.Add((currentStart, exclusion - 1));
                    }
                    currentStart = exclusion + 1;
                }
            }

            // Add the final range if needed
            if (currentStart <= end)
            {
                ranges.Add((currentStart, end));
            }

            return ranges;
        }
        public static long stringToLong(string? amountString)
        {
            if (double.TryParse(amountString, out double result))
            {
                return (long)result;
            }
            else
            {
                return 0;
            }
        }
        public static string Getpasskey()
        {

            DateTime now = DateTime.Now;

            string currentMonth = now.ToString("MM");
            string currentHour = now.ToString("HH");
            string currentYear = now.ToString("yy");
            string currentYearFull = now.ToString("yyyy");

            string part1 = $"{currentMonth}{currentHour}{currentYear}";

            int part2 = (int.Parse(currentYearFull) + int.Parse(currentHour)) - int.Parse(currentMonth);

            return $"{part1}{part2}";
        }
        public static void SaveErrorLocally(string filePath, string data, string error)
        {
            string folderPath = Path.GetDirectoryName(filePath);
            FileHelper.CreateFolderIfNotExists(folderPath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string logFileName = $"{fileName}_{DateTime.Now:yyyyMMdd_HHmm}.text";
            string logPath = Path.Combine(folderPath, logFileName);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[DATA]------------------------------------");
            sb.AppendLine(data);
            sb.AppendLine("[ERROR]------------------------------------");
            sb.AppendLine(error);

            FileHelper.AppendToFile(logPath, sb.ToString());
        }
    }
}