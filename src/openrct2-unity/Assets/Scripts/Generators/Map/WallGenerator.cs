using System.Collections.Generic;
using OpenRCT2.Bindings;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.Extensions;
using OpenRCT2.Generators.Sprites;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map
{
    /// <summary>
    /// A generator for wall elements.
    /// </summary>
    public class WallGenerator : ITileElementGenerator
    {
        readonly GameObject? _prefab;
        readonly string? _textureField;


        public WallGenerator(GameObject? prefab, string? textureField)
        {
            _prefab = prefab;
            _textureField = textureField;
        }

        /// <inheritdoc/>
        public IEnumerator<LoadStatus> Run(Map map, Transform transform)
        {
            Assert.IsNotNull(_prefab, nameof(_prefab));
            Assert.IsNotNull(_textureField, nameof(_textureField));

            return map.ForEach("Creating walls...", (Tile tile, int index, in TileElementInfo element, in WallInfo wall) =>
            {
                CreateElement(transform, tile.x, tile.y, element, wall);
            });
        }

        void CreateElement(Transform transform, int x, int y, in TileElementInfo element, in WallInfo wall)
        {
            Vector3 position = World.TileCoordsToUnity(x, y, element.baseHeight);
            Quaternion rotation = Quaternion.Euler(0, 90 * element.rotation + 90, 0);

            GameObject obj = Object.Instantiate(_prefab, position, rotation, transform)!;
            obj.isStatic = true;

            // Apply the wall sprite
            SpriteTexture sprite = SpriteFactory.GetOrCreate(wall.imageIndex, wall.colour1, wall.colour2, wall.colour3);

            MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
            renderer.material.SetTexture(_textureField, TextureFactory.CreateFullColour(sprite, TextureWrapMode.Repeat));

            // Set the visual scale of the model.
            const int diagonalPixelHeight = 15;

            obj.transform.localScale = new Vector3(1, (sprite.height - diagonalPixelHeight) / 10f, 1);
        }
    }
}
