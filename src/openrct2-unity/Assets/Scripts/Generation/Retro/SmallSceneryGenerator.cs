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
        [SerializeField] GameObject _defaultPrefab;
        [SerializeField] ObjectScaleMode _defaultScaleMode;
        [SerializeField] ObjectEntry[] _prefabOverrides;


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
            string identifier = entry.Identifier.Trim();
            ObjectEntry objectEntry = FindObjectEntry(identifier);

            GameObject prefab;
            ObjectScaleMode scaleMode;
            if (objectEntry != null)
            {
                prefab = objectEntry.prefab;
                scaleMode = objectEntry.scaleMode;
            }
            else
            {
                prefab = _defaultPrefab;
                scaleMode = _defaultScaleMode;
            }

            GameObject obj = InstantiateElement(prefab, pos_x, pos_y, pos_z, (90 * tile.Rotation + 90));

            if ((entry.Flags & SmallSceneryFlags.Animated) != 0
                && TryApplyAnimation(obj, tile, entry))
            {
                return;
            }

            uint imageId = ApplySprite(obj, scaleMode, tile);
            obj.name = $"SmallScenery (ID: {identifier}, idx: {imageId})";
        }


        /// <summary>
        /// Find the correct prefab for the specified entry name.
        /// </summary>
        ObjectEntry FindObjectEntry(string entryName)
        {
            for (int i = 0; i < _prefabOverrides.Length; i++)
            {
                ObjectEntry prefabOverride = _prefabOverrides[i];

                if (prefabOverride.IsMatch(entryName))
                {
                    return prefabOverride;
                }
            }

            return null;
        }


        /// <summary>
        /// Instantiates the prefab in the place of a tile element.
        /// </summary>
        GameObject InstantiateElement(GameObject prefab, float x, float y, float z, float rotation)
        {
            Vector3 position = Map.TileCoordsToUnity(x, y, z);
            Quaternion quatRot = Quaternion.Euler(0, rotation, 0);

            return GameObject.Instantiate(prefab, position, quatRot, _map.transform);
        }


        /// <summary>
        /// Gets the sprite of the tile element and applies it to the gameobject.
        /// </summary>
        static uint ApplySprite(GameObject obj, ObjectScaleMode scaleMode, in TileElement tile)
        {
            MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
            Material[] materials = renderer.materials;
            int materialCount = materials.Length;

            if (materialCount > 4)
            {
                Debug.LogWarning($"Material count for mesh of is greater than 4: {materialCount}");
                materialCount = 4;
            }

            uint imageIndex = OpenRCT2.GetSmallSceneryImageIndex(tile, 0);
            uint unmaskedImageIndex = (imageIndex & 0x7FFFF);
            Graphic graphicForScaling = null; // TODO: refactor this file

            // Get all rotations that fit within the material count.
            for (uint i = 0; i < materialCount; i++)
            {
                Graphic graphic = GraphicsFactory.ForImageIndex(imageIndex + i);

                if (graphic == null)
                {
                    Debug.LogError($"Missing small scenery sprite image: {unmaskedImageIndex}");
                    return unmaskedImageIndex;
                }
                else if (graphicForScaling == null)
                {
                    graphicForScaling = graphic;
                }

                materials[i].mainTexture = graphic.GetTexture();
            }

            ApplyScaleMode(obj, scaleMode, graphicForScaling);
            return unmaskedImageIndex;
        }


        /// <summary>
        /// Applies a sprite animation to the specified gameobject.
        /// </summary>
        static bool TryApplyAnimation(GameObject obj, in TileElement tile, in SmallSceneryEntry entry)
        {
            int numberOfSupposedFrames = entry.AnimationFrameCount;
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
                Debug.LogWarning($"Applying animation failed for {entry.Identifier.Trim()}.", obj);
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


        static void ApplyScaleMode(GameObject obj, ObjectScaleMode scaleMode, Graphic graphic)
        {
            switch (scaleMode)
            {
                case ObjectScaleMode.ObjectHeight:

                    break;

                case ObjectScaleMode.SpriteSize:
                    // Set the visual scale of the model.
                    float width = (graphic.Width * Map.PixelPerUnitMultiplier);
                    obj.transform.localScale = new Vector3(width, graphic.Height * Map.PixelPerUnitMultiplier, width);
                    break;
            }
        }
    }
}
