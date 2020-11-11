using System.Collections.Generic;
using Graphics;
using Lib;
using MeshBuilding;
using UnityEngine;

namespace Generation.Retro
{
    [CreateAssetMenu(menuName = (MenuPath + "Retro/" + nameof(PathGenerator)))]
    public class PathGenerator : TileElementGenerator
    {
        readonly static Dictionary<int, Mesh> pathMeshCache = new Dictionary<int, Mesh>();


        const float surfaceExtents = (Map.TileCoordsXYMultiplier / 2f);
        const float surfaceHeight = (0.01f);
        const float railingHeight = (0.4f);
        const float railingDistance = (surfaceExtents * 0.8f);


        [SerializeField] Material pathMaterial;
        [SerializeField] string pathTextureName;

        MeshBuilder pathMeshBuilder;


        /// <inheritdoc/>
        protected override void Start()
        {
            pathMeshBuilder = new MeshBuilder();
        }


        /// <inheritdoc/>
        protected override void Finish()
        {
            pathMeshBuilder = null;
        }


        /// <inheritdoc/>
        public override void CreateElement(int x, int y, in TileElement tile)
        {
            PathElement path = tile.AsPath();
            uint imageIndex = OpenRCT2.GetPathSurfaceImageIndex(tile);
            int cacheKey = GetCacheKey(path);

            if (!pathMeshCache.TryGetValue(cacheKey, out Mesh mesh))
            {
                mesh = GeneratePathMesh(x, y, path);
                pathMeshCache.Add(cacheKey, mesh);
            }

            GameObject pathObject = new GameObject
            {
                name = $"Path (index: {imageIndex})",
                isStatic = true
            };
            Transform pathTF = pathObject.transform;
            pathTF.parent = map.transform;
            pathTF.localPosition = Map.TileCoordsToUnity(x, tile.baseHeight, y);
            pathTF.localRotation = Quaternion.Euler(0, 180, 0);

            MeshFilter filter = pathObject.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;

            MeshRenderer renderer = pathObject.AddComponent<MeshRenderer>();
            renderer.material = pathMaterial;

            Graphic graphic = GraphicsFactory.ForImageIndex(imageIndex);
            if (graphic == null)
            {
                Debug.LogError($"Missing path sprite image: {imageIndex}");
                return;
            }

            Material material = renderer.material;
            material.SetTexture(pathTextureName, graphic.GetTexture());

            SpriteData data = OpenRCT2.GetTextureData(imageIndex);
            material.SetVector("ImageOffset", new Vector2(data.offsetX, data.offsetY));
        }



        /// <summary>
        /// Generates a new mesh for the specified path element.
        /// </summary>
        Mesh GeneratePathMesh(int x, int y, in PathElement path)
        {
            pathMeshBuilder.Clear();

            Vertex a = new Vertex(surfaceExtents, surfaceHeight, surfaceExtents, Vector3.up, Vector2.one);
            Vertex b = new Vertex(surfaceExtents, surfaceHeight, -surfaceExtents, Vector3.up, Vector2.right);
            Vertex c = new Vertex(-surfaceExtents, surfaceHeight, -surfaceExtents, Vector3.up, Vector2.zero);
            Vertex d = new Vertex(-surfaceExtents, surfaceHeight, surfaceExtents, Vector3.up, Vector2.up);

            pathMeshBuilder.AddQuad(a, b, c, d);
            return pathMeshBuilder.ToMesh();
        }


        /// <summary>
        /// Returns the cache key for this path element.
        /// </summary>
        int GetCacheKey(in PathElement path)
        {
            // 0b1_0000_0000 for sloped pieces.
            // 0b0_####_#### for whatever the edges and corners are.
            if (path.IsSloped)
                return (1 << 8); 

            return (path.EdgesAndCorners);
        }
    }
}
