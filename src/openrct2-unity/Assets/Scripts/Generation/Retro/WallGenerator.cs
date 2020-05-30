using Graphics;
using Lib;
using UnityEngine;

namespace Generation.Retro
{
    /// <summary>
    /// A generator for wall elements.
    /// </summary>
    public class WallGenerator : IElementGenerator
    {
        [SerializeField] GameObject prefab;
        [SerializeField] string textureField = "Wall";


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
            Vector3 position = Map.TileCoordsToUnity(x, tile.baseHeight, y);
            Quaternion rotation = Quaternion.Euler(0, (90 * tile.Rotation + 90), 0);           

            GameObject obj = GameObject.Instantiate(prefab, position, rotation, map.transform);

            // Apply the wall sprite
            uint imageIndex = OpenRCT2.GetWallImageIndex(tile, 0);
            Texture2D texture = GraphicsFactory.ForImageIndex(imageIndex).ToTexture2D(TextureWrapMode.Repeat);

            MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
            renderer.material.SetTexture(textureField, texture);

            // Set the visual scale of the model.
            obj.transform.localScale = new Vector3(1, (tile.clearanceHeight - tile.baseHeight), 1);
        }
    }
}
