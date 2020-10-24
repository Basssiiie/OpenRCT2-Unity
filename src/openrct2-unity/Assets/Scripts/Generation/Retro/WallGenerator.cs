using Graphics;
using Lib;
using UnityEngine;

namespace Generation.Retro
{
    /// <summary>
    /// A generator for wall elements.
    /// </summary>
    [CreateAssetMenu(menuName = (MenuPath + "Retro/" + nameof(WallGenerator)))]
    public class WallGenerator : TileElementGenerator
    {
        [SerializeField] GameObject prefab;
        [SerializeField] string textureField = "Wall";


        /// <inheritdoc/>
        public override void CreateElement(int x, int y, in TileElement tile)
        {
            Vector3 position = Map.TileCoordsToUnity(x, tile.baseHeight, y);
            Quaternion rotation = Quaternion.Euler(0, (90 * tile.Rotation + 90), 0);           

            GameObject obj = GameObject.Instantiate(prefab, position, rotation, map.transform);

            // Apply the wall sprite
            uint imageIndex = OpenRCT2.GetWallImageIndex(tile, 0);
            Texture2D texture = GraphicsFactory.ForImageIndex(imageIndex).ToTexture2D(TextureWrapMode.Repeat);
            if (texture == null)
            {
                Debug.LogError($"Missing wall sprite image: {imageIndex & 0x7FFFF}");
                return;
            }

            MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
            renderer.material.SetTexture(textureField, texture);

            // Set the visual scale of the model.
            const int diagonalPixelHeight = 15;

            obj.transform.localScale = new Vector3(1, (texture.height - diagonalPixelHeight) / 10f, 1);
        }
    }
}
