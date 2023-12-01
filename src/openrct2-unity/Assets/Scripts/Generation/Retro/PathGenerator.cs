using System.Collections.Generic;
using Graphics;
using Lib;
using MeshBuilding;
using UnityEngine;
using Utilities;

#nullable enable

namespace Generation.Retro
{
    [CreateAssetMenu(menuName = (MenuPath + "Retro/" + nameof(PathGenerator)))]
    public class PathGenerator : TileElementGenerator
    {
        readonly static Dictionary<int, Mesh> _pathMeshCache = new Dictionary<int, Mesh>();


        const float SurfaceExtents = (Map.TileCoordsXYMultiplier / 2f);
        const float SurfaceHeight = (0.01f);
        const float RailingHeight = (0.4f);
        const float RailingDistance = (SurfaceExtents * 0.8f);


        [SerializeField, Required] Material _pathMaterial = null!;
        [SerializeField, Required] string _pathTextureName = null!;


        Mesh? _mesh;


        protected override void Startup(Map map)
        {
            _mesh = GeneratePathMesh();
        }


        /// <inheritdoc/>
        public override void CreateElement(Map map, int x, int y, int index, in TileElementInfo tile)
        {
            PathInfo path = OpenRCT2.GetPathElementAt(x, y, index);

            uint surfaceIndex = path.surfaceIndex;
            GameObject pathObject = new GameObject
            {
                name = $"Path (index: {surfaceIndex})",
                isStatic = true
            };
            Transform pathTF = pathObject.transform;
            pathTF.parent = map.transform;
            pathTF.localPosition = Map.TileCoordsToUnity(x, y, tile.baseHeight);
            pathTF.localRotation = Quaternion.Euler(0, 180, 0);

            MeshFilter filter = pathObject.AddComponent<MeshFilter>();
            filter.sharedMesh = _mesh;

            MeshRenderer renderer = pathObject.AddComponent<MeshRenderer>();
            renderer.material = _pathMaterial;

            Graphic graphic = GraphicsFactory.ForImageIndex(surfaceIndex);
            Material material = renderer.material;
            material.SetTexture(_pathTextureName, graphic.GetTexture());

            SpriteData data = OpenRCT2.GetTextureData(surfaceIndex);
            material.SetVector("ImageOffset", new Vector2(data.offsetX, data.offsetY));
        }


        /// <summary>
        /// Generates a new mesh for the specified path element.
        /// </summary>
        Mesh GeneratePathMesh()
        {
            var pathMeshBuilder = new MeshBuilder();

            Vertex a = new Vertex(SurfaceExtents, SurfaceHeight, SurfaceExtents, Vector3.up, Vector2.one);
            Vertex b = new Vertex(SurfaceExtents, SurfaceHeight, -SurfaceExtents, Vector3.up, Vector2.right);
            Vertex c = new Vertex(-SurfaceExtents, SurfaceHeight, -SurfaceExtents, Vector3.up, Vector2.zero);
            Vertex d = new Vertex(-SurfaceExtents, SurfaceHeight, SurfaceExtents, Vector3.up, Vector2.up);

            pathMeshBuilder.AddQuad(a, b, c, d);
            return pathMeshBuilder.ToMesh();
        }
    }
}
