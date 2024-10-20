using System.Collections.Generic;
using OpenRCT2.Bindings;
using OpenRCT2.Bindings.Graphics;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.Extensions;
using OpenRCT2.Generators.MeshBuilding;
using OpenRCT2.Generators.Sprites;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map
{
    public class PathGenerator : ITileElementGenerator
    {
        const float _surfaceExtents = World.TileCoordsXYMultiplier / 2f;
        const float _surfaceHeight = 0.01f;
        const float _railingHeight = 0.4f;
        const float _railingDistance = _surfaceExtents * 0.8f;

        readonly Material _pathMaterial;
        readonly string _pathTextureName;
        readonly Mesh _mesh;


        public PathGenerator(Material pathMaterial, string pathTextureName)
        {
            _pathMaterial = pathMaterial;
            _pathTextureName = pathTextureName;
            _mesh = GeneratePathMesh();
        }

        /// <inheritdoc/>
        public IEnumerator<LoadStatus> Run(Map map, Transform transform)
        {
            return map.ForEach("Creating paths...", (Tile tile, int index, in TileElementInfo element, in PathInfo path) =>
            {
                CreateElement(transform, tile.x, tile.y, element, path);
            });
        }

        public void CreateElement(Transform transform, int x, int y, in TileElementInfo element, in PathInfo path)
        {
            uint surfaceIndex = path.surfaceIndex;
            SpriteTexture graphic = SpriteFactory.GetOrCreate(surfaceIndex);

            if (graphic.pixelCount == 0)
            {
                return;
            }

            var pathObject = new GameObject
            {
                name = $"Path (index: {surfaceIndex})",
                isStatic = true
            };
            Transform pathTF = pathObject.transform;
            pathTF.parent = transform;
            pathTF.SetLocalPositionAndRotation(World.TileCoordsToUnity(x, y, element.baseHeight), Quaternion.Euler(0, 180, 0));

            MeshFilter filter = pathObject.AddComponent<MeshFilter>();
            filter.sharedMesh = _mesh;

            MeshRenderer renderer = pathObject.AddComponent<MeshRenderer>();
            renderer.staticShadowCaster = true;
            renderer.material = _pathMaterial;

            Texture2D texture = TextureFactory.CreateFullColour(graphic);
            Material material = renderer.material;
            material.SetTexture(_pathTextureName, texture);

            SpriteData data = GraphicsDataFactory.GetTextureData(surfaceIndex);
            material.SetVector("ImageOffset", new Vector2(data.offsetX, data.offsetY));
        }


        /// <summary>
        /// Generates a new mesh for the specified path element.
        /// </summary>
        Mesh GeneratePathMesh()
        {
            var pathMeshBuilder = new MeshBuilder();

            var a = new Vertex(_surfaceExtents, _surfaceHeight, _surfaceExtents, Vector3.up, Vector2.one);
            var b = new Vertex(_surfaceExtents, _surfaceHeight, -_surfaceExtents, Vector3.up, Vector2.right);
            var c = new Vertex(-_surfaceExtents, _surfaceHeight, -_surfaceExtents, Vector3.up, Vector2.zero);
            var d = new Vertex(-_surfaceExtents, _surfaceHeight, _surfaceExtents, Vector3.up, Vector2.up);

            pathMeshBuilder.AddQuad(a, b, c, d);
            return pathMeshBuilder.ToMesh();
        }
    }
}
