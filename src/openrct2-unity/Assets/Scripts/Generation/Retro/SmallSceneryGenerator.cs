using Graphics;
using Lib;
using UnityEngine;

namespace Generation.Retro
{
    /// <summary>
    /// A generator for small scenery tile elements.
    /// </summary>
    [CreateAssetMenu(menuName = (MenuPath + "Retro/" + nameof(SmallSceneryGenerator)))]
    public class SmallSceneryGenerator : TileElementGenerator
    {
        [SerializeField] GameObject crossShape;


        /// <inheritdoc/>
        public override void CreateElement(int x, int y, in TileElement tile)
        {
            float pos_x = x;
            float pos_y = tile.baseHeight;
            float pos_z = y;

            SmallSceneryElement smallScenery = tile.AsSmallScenery();
            SmallSceneryEntry entry = OpenRCT2.GetSmallSceneryEntry(smallScenery.EntryIndex);

            // If not a full tile, move small scenery to the correct quadrant.
            if ((entry.Flags & SmallSceneryFlags.FullTile) == 0)
            {
                const float distanceToQuadrant = (Map.TileCoordsXYMultiplier / 4);
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

            if ((entry.Flags & SmallSceneryFlags.Animated) != 0
                && TryApplyAnimation(obj, tile, entry))
            {
                return;
            }

            ApplySprite(obj, tile);
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
        static void ApplySprite(GameObject obj, in TileElement tile)
        {
            uint imageIndex = OpenRCT2.GetSmallSceneryImageIndex(tile, 0);
            Graphic graphic = GraphicsFactory.ForImageIndex(imageIndex);

            if (graphic == null)
            {
                Debug.LogError($"Missing small scenery sprite image: {imageIndex & 0x7FFFF}");
                return;
            }

            MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
            renderer.material.SetTexture("_BaseMap", graphic.GetTexture());

            // Set the visual scale of the model.
            float width = (graphic.Width * Map.PixelPerUnitMultiplier);
            obj.transform.localScale = new Vector3(width, graphic.Height * Map.PixelPerUnitMultiplier, width);
        }


        /// <summary>
        /// Applies a sprite animation to the specified gameobject.
        /// </summary>
        static bool TryApplyAnimation(GameObject obj, in TileElement tile, in SmallSceneryEntry entry)
        {
            int numberOfSupposedFrames = entry.NumberOfFrames;
            if (numberOfSupposedFrames == 0)
            {
                // Some entries do not use this property, they use a default of 0xF (15) instead.
                numberOfSupposedFrames = 0xF;
            }

            int animationDelay = entry.AnimationDelay;

            uint[] imageIndices = new uint[numberOfSupposedFrames];
            int actualFrameCount = OpenRCT2.GetSmallSceneryAnimationIndices(tile, 0, imageIndices, numberOfSupposedFrames);

            obj.name = $"frame count, supposed: {numberOfSupposedFrames}, actual: {actualFrameCount}, delay: {animationDelay & 0xFF}";

            if (actualFrameCount == 0)
            {
                Debug.LogWarning($"Applying animation failed.", obj);
                return false;
            }
            /*
            Graphic[] graphics = GraphicsFactory.ForAnimationIndices(imageIndices);
            Texture2D[] frames = new Texture2D[actualFrameCount];

            for (int i = 0; i < actualFrameCount; i++)
            {
                frames[i] = graphics[i].ToTexture2D();
            }

            AnimatedMaterial material = obj.AddComponent<AnimatedMaterial>();

            material.animationDelay = 0.2f;
            material.frames = frames;
            material.StartAnimating();
            */
            return true;
        }
    }
}
