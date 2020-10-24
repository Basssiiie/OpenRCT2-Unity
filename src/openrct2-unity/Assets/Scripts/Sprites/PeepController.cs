using System.Collections.Generic;
using System.Linq;
using Graphics;
using Lib;
using UnityEngine;

namespace Sprites
{
    /// <summary>
    /// Controller which moves and updates all the peeps in the park.
    /// </summary>
    public class PeepController : SpriteController<Peep>
    {

        /// <summary>
        /// Find the associated peep id for the specified gameobject.
        /// </summary>
        public ushort FindPeepIdForGameObject(GameObject peepObject)
        {
            KeyValuePair<ushort, SpriteObject> entry = spriteObjects.FirstOrDefault(p => p.Value.gameObject == peepObject);

            int bufferIndex = entry.Value.bufferIndex;
            return spriteBuffer[bufferIndex].Id;
        }


        /// <summary>
        /// Find the associated peep struct for the specified id, or
        /// null if the gameobject is not a peep.
        /// </summary>
        public Peep? GetPeepById(ushort peepId)
        {
            if (!spriteObjects.TryGetValue(peepId, out SpriteObject peepObject))
                return null;

            int bufferIndex = peepObject.bufferIndex;
            return spriteBuffer[bufferIndex];
        }


        /// <summary>
        /// Gets all the peep sprites from the dll hook.
        /// </summary>
        protected override int FillSpriteBuffer(Peep[] buffer)
            => OpenRCT2.GetAllPeeps(buffer);


        /// <summary>
        /// Sets the name and colors of the peep sprite.
        /// </summary>
        protected override SpriteObject AddSprite(int index, ref Peep sprite)
        {
            SpriteObject spriteObject = base.AddSprite(index, ref sprite);
            GameObject obj = spriteObject.gameObject;

            ushort id = sprite.Id;
            obj.name = $"Peep sprite {id}";
            //PeepType type = sprite.type;
            //obj.name = $"{type} {id}";

            UpdateColours(obj, ref sprite);
            return spriteObject;
        }


        /// <summary>
        /// Rotates the sprite to look towards where its walking.
        /// </summary>
        protected override void MoveSprite(SpriteObject spriteObject)
        {
            base.MoveSprite(spriteObject);

            // Update rotation
            Vector3 forward = new Vector3(
                spriteObject.towards.x - spriteObject.from.x,
                0,
                spriteObject.towards.z - spriteObject.from.z
            );

            if (forward != Vector3.zero)
            {
                spriteObject
                    .gameObject
                    .transform
                    .rotation = Quaternion.LookRotation(forward);
            }
        }


        void UpdateColours(GameObject peepObj, ref Peep peep)
        {
            GameObject tshirt = peepObj.transform.GetChild(0).gameObject;
            GameObject trousers = peepObj.transform.GetChild(1).gameObject;

            var tshirtRenderer = tshirt.GetComponent<Renderer>();
            var trousersRenderer = trousers.GetComponent<Renderer>();

            tshirtRenderer.material.color = GraphicsFactory.PaletteToColor(peep.tshirtColour);
            trousersRenderer.material.color = GraphicsFactory.PaletteToColor(peep.trousersColour);
        }
    }
}
