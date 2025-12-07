using OpenRCT2.Bindings;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.Extensions;
using OpenRCT2.Generators.Sprites;
using OpenRCT2.Utilities;
using System.Collections.Generic;
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

            return map.ForEach("Creating walls...", (in Element<WallInfo> element) =>
            {
                CreateElement(map, transform, element);
            });
        }

        void CreateElement(Map map, Transform transform, in Element<WallInfo> element)
        {
            Tile tile = element.tile;
            Vector3 position = World.TileCoordsToUnity(tile.x, tile.y, element.info.baseHeight);
            Quaternion rotation = Quaternion.Euler(0, 90 * element.info.rotation + 90, 0);

            GameObject obj = Object.Instantiate(_prefab, position, rotation, transform)!;
            obj.isStatic = true;

            // Apply the wall sprite
            ref WallInfo wall = ref element.GetData();
            SpriteTexture sprite = SpriteFactory.GetOrCreate(wall.imageIndex, wall.colour1, wall.colour2, wall.colour3);

            MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
            renderer.material.SetTexture(_textureField, TextureFactory.CreateFullColour(sprite, TextureWrapMode.Repeat));

            // Set the visual scale of the model.
            const int diagonalPixelHeight = 15;

            obj.transform.localScale = new Vector3(1, (sprite.height - diagonalPixelHeight) / 10f, 1);
        }
    }
}
