#nullable enable

namespace OpenRCT2.Generators.Map
{
    public readonly struct MapLoadStatus
    {
        public readonly string text;
        public readonly int progress;
        public readonly int maximum;

        public MapLoadStatus(string message, int progress, int maximum)
        {
            this.text = message;
            this.progress = progress;
            this.maximum = maximum;
        }
    }
}
