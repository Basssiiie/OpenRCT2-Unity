using OpenRCT2.Bindings;
using OpenRCT2.Bindings.Graphics;
using OpenRCT2.Bindings.TileElements;
using OpenRCT2.Generators.MeshBuilding;
using OpenRCT2.Generators.Sprites;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.Map.Retro
{
    public class PathGenerator : ITileElementGenerator
    {
        //readonly static Dictionary<int, Mesh> _pathMeshCache = new Dictionary<int, Mesh>();

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


        public void CreateElement(in MapData map, int x, int y, int index, in TileElementInfo element)
        {
            PathInfo path = Park.GetPathElementAt(x, y, index);

            uint surfaceIndex = path.surfaceIndex;
            var pathObject = new GameObject
            {
                name = $"Path (index: {surfaceIndex})",
                isStatic = true
            };
            Transform pathTF = pathObject.transform;
            pathTF.parent = map.transform;
            pathTF.SetLocalPositionAndRotation(World.TileCoordsToUnity(x, y, element.baseHeight), Quaternion.Euler(0, 180, 0));

            MeshFilter filter = pathObject.AddComponent<MeshFilter>();
            filter.sharedMesh = _mesh;

            MeshRenderer renderer = pathObject.AddComponent<MeshRenderer>();
            renderer.material = _pathMaterial;

            SpriteTexture graphic = SpriteFactory.ForImageIndex(surfaceIndex);
            Material material = renderer.material;
            material.SetTexture(_pathTextureName, graphic.GetTexture());

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
