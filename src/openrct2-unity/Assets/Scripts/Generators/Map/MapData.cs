using UnityEngine;

namespace OpenRCT2.Generators.Map
{
    /// <summary>
    /// Data about the map of the park.
    /// </summary>
    public readonly struct MapData
    {
        public readonly int width;
        public readonly int height;
        public readonly Transform transform;


        public MapData(int width, int height, Transform transform)
        {
            this.width = width;
            this.height = height;
            this.transform = transform;
        }
    }
}
