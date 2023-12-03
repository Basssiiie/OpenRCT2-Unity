using OpenRCT2.Bindings;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.Sprites;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map.Retro
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
        public void CreateElement(in MapData map, int x, int y, int index, in TileElementInfo element)
        {
            Assert.IsNotNull(_prefab, nameof(_prefab));
            Assert.IsNotNull(_textureField, nameof(_textureField));

            Vector3 position = World.TileCoordsToUnity(x, y, element.baseHeight);
            Quaternion rotation = Quaternion.Euler(0, 90 * element.rotation + 90, 0);

            GameObject obj = Object.Instantiate(_prefab, position, rotation, map.transform);

            // Apply the wall sprite
            WallInfo wall = Park.GetWallElementAt(x, y, index);
            SpriteTexture sprite = SpriteFactory.ForImageIndex(wall.imageIndex);

            MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
            renderer.material.SetTexture(_textureField, sprite.GetTexture(TextureWrapMode.Repeat));

            // Set the visual scale of the model.
            const int diagonalPixelHeight = 15;

            obj.transform.localScale = new Vector3(1, (sprite.Height - diagonalPixelHeight) / 10f, 1);
        }
    }
}
