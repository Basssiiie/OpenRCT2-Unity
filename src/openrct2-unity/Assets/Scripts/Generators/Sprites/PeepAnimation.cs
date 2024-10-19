using OpenRCT2.Bindings.Entities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Sprites
{
    /// <summary>
    /// A gemerated peep animation sprite sheet with accompanying data.
    /// </summary>
    public class PeepAnimation
    {
        public readonly Texture2DArray frames;
        public readonly PeepAnimationData data;

        public PeepAnimation(Texture2DArray frames, in PeepAnimationData data)
        {
            this.frames = frames;
            this.data = data;
        }
    }
}
