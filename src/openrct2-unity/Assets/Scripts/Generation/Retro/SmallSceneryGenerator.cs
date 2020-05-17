using OpenRCT;
using UnityEngine;

namespace Generation.Retro
{
    /// <summary>
    /// A generator for small scenery tile elements.
    /// </summary>
    public class SmallSceneryGenerator : IElementGenerator
    {
        [Header("Meshes")]
        [SerializeField] GameObject crossShape;

        Map map;


        /// <inheritdoc/>
        public void StartGenerator(Map map)
        {
            this.map = map;
        }


        /// <inheritdoc/>
        public void FinishGenerator()
        {
            map = null;
        }


        /// <inheritdoc/>
        public void CreateElement(int x, int y, ref TileElement tile)
        {
            float pos_x = x;
            float pos_y = tile.baseHeight;
            float pos_z = y;

            SmallSceneryElement smallScenery = tile.AsSmallScenery();

            SmallSceneryEntry entry = OpenRCT2.GetSmallSceneryEntry(smallScenery.EntryIndex);
            SmallSceneryFlags flags = entry.Flags;

            // If not a full tile, move small scenery to the correct quadrant.
            if ((flags & SmallSceneryFlags.FullTile) == 0)
            {
                const float distanceToQuadrant = (Map.TileCoordsToVector3Multiplier / 4);
                byte quadrant = smallScenery.Quadrant;

                switch (quadrant)
                {
                    case 0: pos_x -= distanceToQuadrant; pos_z -= distanceToQuadrant; break;
                    case 1: pos_x -= distanceToQuadrant; pos_z += distanceToQuadrant; break;
                    case 2: pos_x += distanceToQuadrant; pos_z += distanceToQuadrant; break;
                    case 3: pos_x += distanceToQuadrant; pos_z -= distanceToQuadrant; break;
                }
            }

            // Instantiate the element.
            GameObject obj = InstantiateElement(crossShape, pos_x, pos_y, pos_z, (90 * tile.Rotation + 90));
            ApplySprite(obj, ref tile);
        }


        /// <summary>
        /// Instantiates the prefab in the place of a tile element.
        /// </summary>
        GameObject InstantiateElement(GameObject prefab, float x, float y, float z, float rotation)
        {
            Vector3 position = Map.TileCoordsToUnity(x, y, z);
            Quaternion quatRot = Quaternion.Euler(0, rotation, 0);

            return GameObject.Instantiate(prefab, position, quatRot, map.transform);
        }


        /// <summary>
        /// Gets the sprite of the tile element and applies it to the gameobject.
        /// </summary>
        static void ApplySprite(GameObject obj, ref TileElement tile)
        {
            uint imageIndex = OpenRCT2.GetSmallSceneryImageIndex(tile, 0);
            Texture2D texture = GraphicsFactory.ForImageIndex(imageIndex).ToTexture2D(TextureWrapMode.Clamp);

            MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
            renderer.material.SetTexture("_BaseMap", texture);

            // Set the visual scale of the model.
            float width = (texture.width * Map.PixelPerUnitMultiplier);
            obj.transform.localScale = new Vector3(width, texture.height * Map.PixelPerUnitMultiplier, width);
        }
    }
}
