using System;
using System.Collections.Generic;
using OpenRCT2.Bindings.Entities;

#nullable enable

namespace OpenRCT2.Generators.Sprites
{
    public static class PeepAnimationsFactory
    {
        static readonly Dictionary<int, PeepAnimation> _cache = new();


        /// <summary>
        /// Get a sprite sheet for the specified peep animation.
        /// </summary>
        public static PeepAnimation GetOrCreate(byte group, byte type, byte tshirt, byte trousers, byte accessory)
        {
            var key = HashCode.Combine(group, type, tshirt, trousers, accessory);

            if (_cache.TryGetValue(key, out PeepAnimation animation))
            {
                return animation;
            }

            animation = Create(group, type, tshirt, trousers, accessory);
            _cache.Add(key, animation);

            return animation;
        }

        /// <summary>
        /// Create a sprite sheet for the specified peep animation.
        /// </summary>
        public static PeepAnimation Create(byte group, byte type, byte tshirt, byte trousers, byte accessory)
        {
            var data = EntityRegistry.GetPeepAnimationData(group, type);
            var rotations = data.rotations;
            var framesLength = data.length;
            var sprites = new SpriteTexture[rotations * framesLength];

            for (var rot = 0; rot < rotations; rot++)
            {
                for (var frame = 0; frame < framesLength; frame++)
                {
                    var imageId = (uint)(data.baseImageId + (frame * rotations) + rot);
                    var index = (rot * framesLength) + frame;
                    var sprite = SpriteFactory.Create(imageId, tshirt, trousers);

                    if (data.accessoryImageOffset != 0)
                    {
                        var accessorySprite = SpriteFactory.Create(imageId + data.accessoryImageOffset, accessory);
                        sprite = sprite.Overlay(accessorySprite);
                    }

                    sprites[index] = sprite;
                }
            }

            var array = TextureFactory.CreatePalettedArray(sprites);
            return new PeepAnimation(array, data);
        }
    }
}
