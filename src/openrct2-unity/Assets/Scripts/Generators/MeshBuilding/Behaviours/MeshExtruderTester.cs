using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.MeshBuilding.Behaviours
{
    /// <summary>
    /// Tester behaviour for the mesh extruder.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshExtruderTester : MonoBehaviour
    {
        [SerializeField] Mesh? _mesh;
        [SerializeField] float _offset;
        [SerializeField, Range(0.1f, 2f)] float _multiplier = 1;
        [SerializeField] Transform? _start;
        [SerializeField] Transform? _end;

        MeshFilter? _filter;
        MeshExtruder? _extruder;


        void OnValidate()
        {
            _filter = GetComponent<MeshFilter>();

            if (_mesh != null && _start != null && _end != null)
            {
                _extruder = new MeshExtruder(_mesh);
                _extruder.AddSegment(_start.position, _start.rotation, _end.position, _end.rotation, _offset, _multiplier, 0);

                _filter.sharedMesh = _extruder.ToMesh();
            }
        }


        void OnDrawGizmos()
        {
            const int axis = 2;

            if (_mesh == null || _start == null || _end == null)
                return;

            Bounds bounds = _mesh.bounds;
            Gizmos.color = new Color(1, 1, 1, 0.7f);

            Vector3 size = bounds.size + Vector3.one * 0.15f;
            size[axis] = 0;

            Gizmos.matrix = _start.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, size);

            Gizmos.matrix = _end.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, size);
        }
    }
}
