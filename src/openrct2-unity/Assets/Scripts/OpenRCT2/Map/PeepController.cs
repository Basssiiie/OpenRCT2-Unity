using System.Linq;
using UnityEngine;

namespace Lib
{
    /// <summary>
    /// Controller which moves and updates all the peeps in the park.
    /// </summary>
    public class PeepController : SpriteController<Peep>
    {

        /// <summary>
        /// Find the associated peep id for the specified gameobject, or
        /// null if the gameobject is not a peep.
        /// </summary>
        public ushort FindPeepIdForGameObject(GameObject peepObject)
        {
            var entry = spriteObjects.FirstOrDefault(p => p.Value.gameObject == peepObject);

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
            PeepType type = sprite.type;
            obj.name = $"{type} {id}";

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

            tshirtRenderer.material.color = DecodeColour(peep.tshirtColour);
            trousersRenderer.material.color = DecodeColour(peep.trousersColour);
        }


        Color DecodeColour(byte colour)
        {
            var colourRGB = new Color32(0, 0, 0, 1);
            switch (colour)
            {
                case 0:
                    colourRGB = new Color32(0, 0, 0, 1);
                    break;
                case 1:
                    colourRGB = new Color32(128, 128, 128, 1);
                    break;
                case 2:
                    colourRGB = new Color32(255, 255, 255, 1);
                    break;
                case 3:
                    colourRGB = new Color32(85, 26, 139, 1);
                    break;
                case 4:
                    colourRGB = new Color32(171, 130, 255, 1);
                    break;
                case 5:
                    colourRGB = new Color32(160, 32, 240, 1);
                    break;
                case 6:
                    colourRGB = new Color32(0, 0, 139, 1);
                    break;
                case 7:
                    colourRGB = new Color32(102, 102, 255, 1);
                    break;
                case 8:
                    colourRGB = new Color32(135, 206, 235, 1);
                    break;
                case 9:
                    colourRGB = new Color32(0, 128, 128, 1);
                    break;
                case 10:
                    colourRGB = new Color32(127, 255, 212, 1);
                    break;
                case 11:
                    colourRGB = new Color32(124, 205, 124, 1);
                    break;
                case 12:
                    colourRGB = new Color32(0, 100, 0, 1);
                    break;
                case 13:
                    colourRGB = new Color32(110, 139, 61, 1);
                    break;
                case 14:
                    colourRGB = new Color32(0, 255, 0, 1);
                    break;
                case 15:
                    colourRGB = new Color32(192, 255, 62, 1);
                    break;
                case 16:
                    colourRGB = new Color32(85, 107, 47, 1);
                    break;
                case 17:
                    colourRGB = new Color32(255, 255, 0, 1);
                    break;
                case 18:
                    colourRGB = new Color32(139, 139, 0, 1);
                    break;
                case 19:
                    colourRGB = new Color32(255, 165, 0, 1);
                    break;
                case 20:
                    colourRGB = new Color32(255, 127, 0, 1);
                    break;
                case 21:
                    colourRGB = new Color32(244, 164, 96, 1);
                    break;
                case 22:
                    colourRGB = new Color32(165, 42, 42, 1);
                    break;
                case 23:
                    colourRGB = new Color32(205, 170, 125, 1);
                    break;
                case 24:
                    colourRGB = new Color32(61, 51, 37, 1);
                    break;
                case 25:
                    colourRGB = new Color32(255, 160, 122, 1);
                    break;
                case 26:
                    colourRGB = new Color32(205, 55, 0, 1);
                    break;
                case 27:
                    colourRGB = new Color32(200, 0, 0, 1);
                    break;
                case 28:
                    colourRGB = new Color32(255, 0, 0, 1);
                    break;
                case 29:
                    colourRGB = new Color32(205, 16, 118, 1);
                    break;
                case 30:
                    colourRGB = new Color32(255, 105, 180, 1);
                    break;
                case 31:
                    colourRGB = new Color32(255, 174, 185, 1);
                    break;
                default:
                    break;
            }
            return colourRGB;
        }
    }
}
