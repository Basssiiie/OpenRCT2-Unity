using System.Collections.Generic;
using System.Linq;
using Graphics;
using Lib;
using UnityEngine;

#nullable enable

namespace Sprites
{
    /// <summary>
    /// Controller which moves and updates all the peeps in the park.
    /// </summary>
    public class PeepController : SpriteController<Peep>
    {
        readonly Dictionary<uint, Graphic> _flipbookCache = new Dictionary<uint, Graphic>();


        /// <summary>
        /// Find the associated peep id for the specified gameobject.
        /// </summary>
        public ushort FindPeepIdForGameObject(GameObject peepObject)
        {
            KeyValuePair<ushort, SpriteObject> entry = _spriteObjects.FirstOrDefault(p => p.Value.gameObject == peepObject);

            int bufferIndex = entry.Value.bufferIndex;
            return _spriteBuffer[bufferIndex].Id;
        }


        /// <summary>
        /// Find the associated peep struct for the specified id, or
        /// null if the gameobject is not a peep.
        /// </summary>
        public Peep? GetPeepById(ushort peepId)
        {
            if (!_spriteObjects.TryGetValue(peepId, out SpriteObject peepObject))
                return null;

            int bufferIndex = peepObject.bufferIndex;
            return _spriteBuffer[bufferIndex];
        }


        /// <summary>
        /// Gets all the peep sprites from the dll hook.
        /// </summary>
        protected override int FillSpriteBuffer(Peep[] buffer)
            => OpenRCT2.GetAllPeeps(buffer);


        /// <summary>
        /// Sets the name and colors of the peep sprite.
        /// </summary>
        protected override SpriteObject AddSprite(int index, in Peep sprite)
        {
            SpriteObject spriteObject = base.AddSprite(index, sprite);
            GameObject obj = spriteObject.gameObject;

            ushort id = sprite.Id;
            obj.name = $"Peep sprite {id}";

            return spriteObject;
        }


        /// <inheritdoc/>
        protected override SpriteObject UpdateSprite(int index, in Peep sprite)
        {
            var obj = base.UpdateSprite(index, sprite);

            SetPeepBillboard(obj.gameObject, sprite);
            return obj;
        }


        void SetPeepBillboard(GameObject peepObj, in Peep sprite)
        {
            uint imageId = sprite.imageId;

            if (!_flipbookCache.TryGetValue(imageId, out Graphic flipbook))
            {
                Graphic[] rotations = new Graphic[]
                {
                    GraphicsFactory.ForImageIndex(imageId),
                    GraphicsFactory.ForImageIndex(imageId + 1),
                    GraphicsFactory.ForImageIndex(imageId + 2),
                    GraphicsFactory.ForImageIndex(imageId + 3)
                };

                flipbook = FlipbookFactory.CreateFlipbookGraphic(rotations);
                _flipbookCache.Add(imageId, flipbook);
            }

            var renderer = peepObj.GetComponent<MeshRenderer>();

            Material material = renderer.material;
            material.mainTexture = flipbook.GetTexture(TextureWrapMode.Repeat);
            material.SetInt("_StartIndex", sprite.direction >> 3);
            material.SetVector("_SpriteSizeOffset", new Vector4((flipbook.Width / 4), flipbook.Height, flipbook.OffsetX, flipbook.OffsetY));
        }
    }
}
