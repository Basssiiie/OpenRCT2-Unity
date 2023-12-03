using System.Collections.Generic;
using System.Linq;
using OpenRCT2.Bindings.Entities;
using OpenRCT2.Generators;
using OpenRCT2.Generators.Sprites;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Behaviours.Controllers
{
    /// <summary>
    /// Controller which moves and updates all the peeps in the park.
    /// </summary>
    public class PeepController : SpriteController<Peep>
    {
        readonly Dictionary<uint, SpriteTexture> _flipbookCache = new Dictionary<uint, SpriteTexture>();


        /// <summary>
        /// Find the associated peep id for the specified gameobject.
        /// </summary>
        public ushort FindPeepIdForGameObject(GameObject peepObject)
        {
            KeyValuePair<ushort, SpriteObject> entry = _spriteObjects.FirstOrDefault(p => p.Value.gameObject == peepObject);

            int bufferIndex = entry.Value.bufferIndex;
            return _spriteBuffer[bufferIndex].id;
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
            => EntityRegistry.GetAllPeeps(buffer);


        /// <summary>
        /// Returns an id of the peep, currently based on the sprite index.
        /// </summary>
        protected override ushort GetId(in Peep peep)
            => peep.id;


        /// <summary>
        /// Returns the peep's position in Unity coordinates.
        /// </summary>
        protected override Vector3 GetPosition(in Peep peep)
            => World.CoordsToVector3(peep.x, peep.z, peep.y);


        /// <summary>
        /// Sets the name and colors of the peep sprite.
        /// </summary>
        protected override SpriteObject AddSprite(int index, in Peep sprite)
        {
            SpriteObject spriteObject = base.AddSprite(index, sprite);
            GameObject obj = spriteObject.gameObject;

            ushort id = sprite.id;
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

            if (!_flipbookCache.TryGetValue(imageId, out SpriteTexture flipbook))
            {
                SpriteTexture[] rotations = new SpriteTexture[]
                {
                    SpriteFactory.ForImageIndex(imageId),
                    SpriteFactory.ForImageIndex(imageId + 1),
                    SpriteFactory.ForImageIndex(imageId + 2),
                    SpriteFactory.ForImageIndex(imageId + 3)
                };

                flipbook = FlipbookFactory.CreateFlipbookGraphic(rotations);
                _flipbookCache.Add(imageId, flipbook);
            }

            var renderer = peepObj.GetComponent<MeshRenderer>();

            Material material = renderer.material;
            material.mainTexture = flipbook.GetTexture(TextureWrapMode.Repeat);
            material.SetInt("_StartIndex", sprite.direction);
            material.SetVector("_SpriteSizeOffset", new Vector4((flipbook.Width / 4), flipbook.Height, flipbook.OffsetX, flipbook.OffsetY));
        }
    }
}
