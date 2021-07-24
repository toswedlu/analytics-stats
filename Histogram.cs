using System;
using System.Collections.Generic;

namespace AnalyticsStats
{
    public class Histogram
    {
        public long Min { get; private set; } = 0;
        public long Max { get; private set; } = 0;
        public double Avg { get; private set; } = 0;
        public long Quartile25Count { get; private set; } = 0;
        public long Quartile50Count { get; private set; } = 0;
        public long Quartile75Count { get; private set; } = 0;
        public long Quartile100Count { get; private set; } = 0;

        public Histogram(List<long> values)
        {
            if (values.Count == 0) return;
            CalculateStats(values);
        }

        private void CalculateStats(List<long> values)
        {
            List<long> sortedValues = new List<long>(values);
            sortedValues.Sort();
            Min = sortedValues[0];
            Max = sortedValues[sortedValues.Count - 1];
            Avg = CalculateAvg(sortedValues);
            CalculateQuartileCounts(sortedValues);
        }

        private void CalculateQuartileCounts(List<long> values)
        {
            List<double> quartileValues = new List<double>() { 0, 0, 0, 0 };
            double median, pos;
            CalculateMedian(0, values.Count - 1, values, out median, out pos);
            quartileValues[1] = median;
            int lowerBound = (int)Math.Floor(pos + 1);
            CalculateMedian(0, (int)Math.Ceiling(pos - 1), values, out median, out pos);
            quartileValues[0] = median;
            CalculateMedian(lowerBound, values.Count - 1, values, out median, out pos);
            quartileValues[2] = median;
            quartileValues[3] = values[values.Count - 1];

            List<int> quartileCounts = new List<int>() { 0, 0, 0, 0 };
            int currQuartile = 0;
            for (int i = 0; i < values.Count; ++i)
            {
                if (values[i] <= quartileValues[currQuartile])
                {
                    quartileCounts[currQuartile]++;
                }
                else
                {
                    currQuartile++;
                    i--;
                }
            }

            Quartile25Count = quartileCounts[0];
            Quartile50Count = quartileCounts[1];
            Quartile75Count = quartileCounts[2];
            Quartile100Count = quartileCounts[3];
        }

        private static double CalculateAvg(List<long> values)
        {
            long sum = 0;
            foreach (long value in values) sum += value;
            return sum / (double)values.Count;
        }

        private static void CalculateMedian(int start, int end, List<long> values, out double median, out double pos)
        {
            int count = end - start + 1;
            pos = count / 2.0 - 0.5 + start;
            if ((count % 2) == 0)
            {
                int index = (int)pos;
                if (count >= 2) median = (values[index] + values[index + 1]) / 2.0;
            }
            median = values[(int)pos];
        }
    }
}
