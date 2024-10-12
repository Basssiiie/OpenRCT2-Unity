#nullable enable

namespace OpenRCT2.Generators
{
    public class LoadStatus
    {
        public readonly string text;
        public readonly int progress;
        public readonly int maximum;

        public LoadStatus(string message, int progress, int maximum)
        {
            this.text = message;
            this.progress = progress;
            this.maximum = maximum;
        }
    }
}
