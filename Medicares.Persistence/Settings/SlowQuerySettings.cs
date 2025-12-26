namespace Medicares.Persistence.Settings
{
    public class SlowQuerySettings
    {
        public bool Enabled { get; set; } = true;
        public int ThresholdMilliseconds { get; set; } = 1000;
    }
}
