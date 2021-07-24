using AnalyticsStats.FileInfo;
using System.Collections.Generic;

namespace AnalyticsStats
{
    public class PartitionStats
    {
        public string UUID { get; private set; }
        public bool IsArchived { get; private set; }
        public int NumPartialSegments { get; private set; }
        public int NumSealedSegments { get; private set; }
        public Histogram PartialSegmentHistogram { get; private set; }
        public Histogram SealedSegmentHistogram { get; private set; }

        public PartitionStats(PartitionInfo partition)
        {
            UUID = partition.UUID;
            IsArchived = partition.IsArchived;
            List<long> partialSizes, sealedSizes;
            EnumerateSizes(partition, out partialSizes, out sealedSizes);
            NumPartialSegments = partialSizes.Count;
            NumSealedSegments = sealedSizes.Count;
            PartialSegmentHistogram = new Histogram(partialSizes);
            SealedSegmentHistogram = new Histogram(sealedSizes);
        }

        private void EnumerateSizes(PartitionInfo partition, out List<long> partialSizes, out List<long> sealedSizes)
        {
            partialSizes = new List<long>();
            sealedSizes = new List<long>();
            if (partition.SegmentFiles != null)
            {
                foreach (var segment in partition.SegmentFiles)
                {
                    if (segment.IsPartialSegment) partialSizes.Add(segment.Size);
                    else sealedSizes.Add(segment.Size);
                }
            }
        }
    }
}
