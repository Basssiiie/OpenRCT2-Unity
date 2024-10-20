using OpenRCT2.Bindings.Graphics;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.Map.Data;
using OpenRCT2.Generators.Sprites;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map.Providers
{
    /// <summary>
    /// A provider that creates small scenery objects with original sprite textures.
    /// </summary>
    public class SmallScenerySpriteObjectProvider : IObjectProvider<SmallSceneryInfo>
    {
        static readonly int _paletteKey = Shader.PropertyToID("_Palette");
        static readonly int _spriteKey = Shader.PropertyToID("_Sprite");
        static readonly int _spritesKey = Shader.PropertyToID("_Sprites");
        static readonly int _frameRateKey = Shader.PropertyToID("_FrameRate");
        static readonly int _lengthKey = Shader.PropertyToID("_Length");

        readonly GameObject _prefab;
        readonly Shader? _animationShader;
        readonly ObjectScaleMode _scaleMode;

        public SmallScenerySpriteObjectProvider(GameObject prefab, Shader? animationShader, ObjectScaleMode scaleMode)
        {
            _prefab = prefab;
            _animationShader = animationShader;
            _scaleMode = scaleMode;
        }

        /// </inheritdoc>
        public GameObject CreateObject(int x, int y, int index, in TileElementInfo element, in SmallSceneryInfo data)
        {
            GameObject obj = GameObject.Instantiate(_prefab);

            // Apply the sprites
            bool spriteApplied = false;
            if (data.animated && _animationShader != null)
            {
                // Animate if possible
                spriteApplied = TryApplyAnimation(obj, _scaleMode, x, y, index, data);
            }

            if (!spriteApplied)
            {
                ApplySprite(obj, _scaleMode, data);
            }

            return obj;
        }

        /// <summary>
        /// Gets the sprite of the tile element and applies it to the gameobject.
        /// </summary>
        static void ApplySprite(GameObject obj, ObjectScaleMode scaleMode, in SmallSceneryInfo scenery)
        {
            MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
            Material[] materials = renderer.materials;
            int materialCount = materials.Length;

            if (materialCount > 4)
            {
                Debug.LogWarning($"Material count for mesh of is greater than 4: {materialCount}");
                materialCount = 4;
            }

            uint maskedImageIndex = scenery.imageIndex & 0x7FFFF;
            Texture2D palette = PaletteFactory.GetPalette();
            SpriteTexture? graphicForScaling = null; // TODO: refactor this file

            // Get all rotations that fit within the material count.
            for (uint i = 0; i < materialCount; i++)
            {
                SpriteTexture graphic = SpriteFactory.GetOrCreate(scenery.imageIndex + i, scenery.colour1, scenery.colour2, scenery.colour3);

                if (graphic.pixelCount == 0)
                {
                    Debug.LogError($"Missing small scenery sprite image: {maskedImageIndex}");
                    break;
                }
                else if (graphicForScaling == null)
                {
                    graphicForScaling = graphic;
                }

                var material = materials[i];
                material.SetTexture(_paletteKey, palette);
                material.SetTexture(_spriteKey, TextureFactory.CreatePaletted(graphic)); // todo: avoid duplicate textures here
            }

            ApplyScaleMode(obj, scaleMode, graphicForScaling!);
            obj.name = $"SmallScenery (ID: {scenery.identifier}, idx: {maskedImageIndex})";
        }

        /// <summary>
        /// Applies a sprite animation to the specified gameobject.
        /// </summary>
        bool TryApplyAnimation(GameObject obj, ObjectScaleMode scaleMode, int x, int y, int index, in SmallSceneryInfo scenery)
        {
            int animationDelay = scenery.animationFrameDelay;
            int animationFrameCount = scenery.animationFrameCount;
            uint[] imageIndices = GraphicsDataFactory.GetSmallSceneryAnimationIndices(x, y, index, animationFrameCount);

            obj.name = $"SmallScenery (ID: {scenery.identifier}, frames: {animationFrameCount}, delay: {animationDelay})";

            SpriteTexture[] graphics = SpriteFactory.ForAnimationIndices(imageIndices);
            if (graphics.Length == 0)
            {
                Debug.LogError("Small scenery: animation load failed with 0 frames");
                return false;
            }

            Texture2DArray flipbook = TextureFactory.CreatePalettedArray(graphics);
            // TODO: cache flipbooks for multiple instances.

            MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
            Material material = renderer.material;

            material.shader = _animationShader;
            material.SetTexture(_paletteKey, PaletteFactory.GetPalette());
            material.SetTexture(_spritesKey, flipbook);

            // Fps => delay;  40 => 0;  20 => 1;  10 => 2;  5 => 3.
            int framerate = 40 / Mathf.FloorToInt(Mathf.Pow(2, animationDelay));
            material.SetInt(_frameRateKey, framerate);
            material.SetInt(_lengthKey, animationFrameCount);

            ApplyScaleMode(obj, scaleMode, graphics[0]);
            return true;
        }

        static void ApplyScaleMode(GameObject obj, ObjectScaleMode scaleMode, SpriteTexture graphic)
        {
            switch (scaleMode)
            {
                case ObjectScaleMode.ObjectHeight:
                    // Relative to difference between base and clearance height.
                    break;

                case ObjectScaleMode.SpriteSize:
                    // Set the visual scale of the model.
                    float width = graphic.width * World.PixelPerUnitMultiplier;
                    obj.transform.localScale = new Vector3(width, graphic.height * World.PixelPerUnitMultiplier, width);
                    break;
            }
        }
    }
}
