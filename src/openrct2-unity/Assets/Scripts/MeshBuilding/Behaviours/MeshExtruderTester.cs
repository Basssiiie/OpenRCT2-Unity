using UnityEngine;

namespace MeshBuilding
{
    /// <summary>
    /// Tester behaviour for the mesh extruder.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshExtruderTester : MonoBehaviour
    {
        [SerializeField] Mesh mesh;
        [SerializeField] float offset;
        [SerializeField, Range(0.1f, 2f)] float multiplier = 1;
        [SerializeField] Transform start;
        [SerializeField] Transform end;

        MeshFilter filter;
        MeshExtruder extruder;


        void OnValidate()
        {
            filter = GetComponent<MeshFilter>();

            if (mesh != null && start != null && end != null)
            {
                extruder = new MeshExtruder(mesh);
                extruder.AddSegment(start.position, start.rotation, end.position, end.rotation, offset, multiplier, 0);

                filter.sharedMesh = extruder.ToMesh();
            }
        }


        void OnDrawGizmos()
        {
            const int axis = 2;

            if (mesh == null || start == null || end == null)
                return;

            Bounds bounds = mesh.bounds;
            Gizmos.color = new Color(1, 1, 1, 0.7f);

            Vector3 size = bounds.size + (Vector3.one * 0.15f);
            size[axis] = 0;

            Gizmos.matrix = start.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, size);

            Gizmos.matrix = end.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, size);
        }
    }
}
