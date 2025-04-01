namespace BlackLagoon.ViewModels
{
    public class RadialBarChartDto
    {
        public decimal TotalCount { get; set; }
        public decimal CountInCurrentMotnth { get; set; }
        public bool HasRatioIncreased { get; set; }
        public int[] Series { get; set; }
    }
}
