using OpenRCT2.Bindings;
using OpenRCT2.Bindings.Graphics;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.Map.Retro.Data;
using OpenRCT2.Generators.Sprites;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map.Retro
{
    /// <summary>
    /// A generator for small scenery tile elements.
    /// </summary>
    public class SmallSceneryGenerator : ITileElementGenerator
    {
        readonly Shader? _animationShader;
        readonly GameObject _defaultPrefab;
        readonly ObjectScaleMode _defaultScaleMode;
        readonly ObjectEntry[] _prefabOverrides;


        public SmallSceneryGenerator(Shader? animationShader, GameObject defaultPrefab, ObjectScaleMode defaultScaleMode, ObjectEntry[] prefabOverrides)
        {
            _animationShader = animationShader;
            _defaultPrefab = defaultPrefab;
            _defaultScaleMode = defaultScaleMode;
            _prefabOverrides = prefabOverrides;
        }


        /// <inheritdoc/>
        public void CreateElement(in MapData map, int x, int y, int index, in TileElementInfo element)
        {
            float pos_x = x;
            float pos_y = y;
            float height = element.baseHeight;

            SmallSceneryInfo scenery = Park.GetSmallSceneryElementAt(x, y, index);

            // If not a full tile, move small scenery to the correct quadrant.
            if (!scenery.fullTile)
            {
                const float distanceToQuadrant = World.TileCoordsXYMultiplier / 4;
                byte quadrant = scenery.quadrant;

                switch (quadrant)
                {
                    case 0: pos_x -= distanceToQuadrant; pos_y -= distanceToQuadrant; break;
                    case 1: pos_x -= distanceToQuadrant; pos_y += distanceToQuadrant; break;
                    case 2: pos_x += distanceToQuadrant; pos_y += distanceToQuadrant; break;
                    case 3: pos_x += distanceToQuadrant; pos_y -= distanceToQuadrant; break;
                }
            }

            // Instantiate the element.
            string identifier = scenery.identifier.Trim();
            ObjectEntry? objectEntry = FindObjectEntry(identifier);

            GameObject prefab;
            ObjectScaleMode scaleMode;
            if (objectEntry.HasValue && objectEntry.Value.prefab != null)
            {
                prefab = objectEntry.Value.prefab;
                scaleMode = objectEntry.Value.scaleMode;
            }
            else
            {
                prefab = _defaultPrefab;
                scaleMode = _defaultScaleMode;
            }

            Vector3 position = World.TileCoordsToUnity(pos_x, pos_y, height);
            Quaternion quatRot = Quaternion.Euler(0, 90 * element.rotation + 90, 0);

            GameObject obj = Object.Instantiate(prefab, position, quatRot, map.transform);

            // Apply the sprites
            bool spriteApplied = false;
            if (scenery.animated)
            {
                // Animate if possible
                spriteApplied = TryApplyAnimation(obj, scaleMode, x, y, index, scenery, identifier);
            }

            if (!spriteApplied)
            {
                ApplySprite(obj, scaleMode, scenery.imageIndex, identifier);
            }
        }


        /// <summary>
        /// Find the correct prefab for the specified entry name.
        /// </summary>
        ObjectEntry? FindObjectEntry(string entryName)
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
        /// Gets the sprite of the tile element and applies it to the gameobject.
        /// </summary>
        static void ApplySprite(GameObject obj, ObjectScaleMode scaleMode, uint imageIndex, string identifier)
        {
            MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
            Material[] materials = renderer.materials;
            int materialCount = materials.Length;

            if (materialCount > 4)
            {
                Debug.LogWarning($"Material count for mesh of is greater than 4: {materialCount}");
                materialCount = 4;
            }

            uint maskedImageIndex = imageIndex & 0x7FFFF;
            SpriteTexture? graphicForScaling = null; // TODO: refactor this file

            // Get all rotations that fit within the material count.
            for (uint i = 0; i < materialCount; i++)
            {
                SpriteTexture graphic = SpriteFactory.ForImageIndex(imageIndex + i);

                if (graphic.PixelCount == 0)
                {
                    Debug.LogError($"Missing small scenery sprite image: {maskedImageIndex}");
                    break;
                }
                else if (graphicForScaling == null)
                {
                    graphicForScaling = graphic;
                }

                materials[i].mainTexture = graphic.GetTexture();
            }

            ApplyScaleMode(obj, scaleMode, graphicForScaling!);
            obj.name = $"SmallScenery (ID: {identifier}, idx: {maskedImageIndex})";
        }


        /// <summary>
        /// Applies a sprite animation to the specified gameobject.
        /// </summary>
        bool TryApplyAnimation(GameObject obj, ObjectScaleMode scaleMode, int x, int y, int index, in SmallSceneryInfo scenery, string identifier)
        {
            int animationDelay = scenery.animationFrameDelay;
            int animationFrameCount = scenery.animationFrameCount;
            uint[] imageIndices = GraphicsDataFactory.GetSmallSceneryAnimationIndices(x, y, index, animationFrameCount);

            obj.name = $"SmallScenery (ID: {identifier}, frames: {animationFrameCount}, delay: {animationDelay})";

            SpriteTexture[] graphics = SpriteFactory.ForAnimationIndices(imageIndices);
            SpriteTexture flipbook = FlipbookFactory.CreateFlipbookGraphic(graphics);
            // TODO: cache flipbooks for multiple instances.

            MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
            Material material = renderer.material;

            material.shader = _animationShader;
            material.mainTexture = flipbook.GetTexture(TextureWrapMode.Repeat);

            // Fps => delay;  40 => 0;  20 => 1;  10 => 2;  5 => 3.
            int framerate = 40 / Mathf.FloorToInt(Mathf.Pow(2, animationDelay));
            material.SetInt("_FrameRate", framerate);
            material.SetInt("_FrameCount", animationFrameCount);

            ApplyScaleMode(obj, scaleMode, graphics[0]);
            return true;
        }


        static void ApplyScaleMode(GameObject obj, ObjectScaleMode scaleMode, SpriteTexture graphic)
        {
            switch (scaleMode)
            {
                case ObjectScaleMode.ObjectHeight:

                    break;

                case ObjectScaleMode.SpriteSize:
                    // Set the visual scale of the model.
                    float width = graphic.Width * World.PixelPerUnitMultiplier;
                    obj.transform.localScale = new Vector3(width, graphic.Height * World.PixelPerUnitMultiplier, width);
                    break;
            }
        }
    }
}
