using UnityEngine;

namespace MeshBuilding
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshExtruderTest : MonoBehaviour
    {
        [SerializeField] Mesh mesh;
        [SerializeField] float start;
        [SerializeField, Min(0)] float length = 1;
        [SerializeField, Range(0.1f, 2f)] float multiplier = 1;


        MeshFilter filter;
        MeshExtruder extruder;


        void OnValidate()
        {
            filter = GetComponent<MeshFilter>();

            if (mesh != null)
            {
                extruder = new MeshExtruder(mesh);
                extruder.AddExtrusion(Matrix4x4.identity, start, length, multiplier, Vector3.forward, Vector3.forward);

                filter.sharedMesh = extruder.ToMesh();
            }
        }


        void OnDrawGizmosSelected()
        {
            const int axis = 2;

            Bounds bounds = mesh.bounds;
            Gizmos.color = new Color(1, 1, 1, 0.7f);
            Gizmos.matrix = transform.localToWorldMatrix;

            Vector3 start = bounds.center;
            Vector3 end = start;
            Vector3 size = bounds.size + (Vector3.one * 0.15f);

            start[axis] = 0;
            end[axis] = length;
            size[axis] = 0;

            Gizmos.DrawCube(start, size);
            Gizmos.DrawCube(end, size);
        }
    }



    public class MeshExtruder
    {
        readonly Vector3[] vertices;
        readonly Vector3[] normals;
        readonly Vector2[] uvs;
        readonly int[] triangles;

        readonly Bounds bounds;
        readonly MeshBuilder builder = new MeshBuilder();


        /// <summary>
        /// Creates a mesh extruder based on the given base mesh.
        /// </summary>
        public MeshExtruder(Mesh mesh)
        {
            vertices = mesh.vertices;
            normals = mesh.normals;
            uvs = mesh.uv;
            triangles = mesh.triangles;
            bounds = mesh.bounds;
        }


        /// <summary>
        /// Adds an extruded element to the mesh.
        /// </summary>
        public void AddExtrusion(Matrix4x4 positioningMatrix, float start, float length, float multiplier, Vector3 startNormal, Vector3 endNormal, int submesh = 0)
        {
            const int axis = 2; // = Z

            float meshExtent = bounds.extents[axis] * multiplier;
            float segmentLength = (meshExtent * 2);

            // Shift back bounds to 0, and apply the start as negative position.
            float startMesh = ((bounds.center[axis] - meshExtent) + Maths.Modulo(start, segmentLength));

            for (int t = 0; t < triangles.Length; t += 3)
            {
                // Get index of vertices/normals for this triangle.
                int idx1 = triangles[t];
                int idx2 = triangles[t + 1];
                int idx3 = triangles[t + 2];

                Vector3 pos1 = vertices[idx1];
                Vector3 pos2 = vertices[idx2];
                Vector3 pos3 = vertices[idx3];

                float unit1 = (pos1[axis] - startMesh) * multiplier;
                float unit2 = (pos2[axis] - startMesh) * multiplier;
                float unit3 = (pos3[axis] - startMesh) * multiplier;

                int step = 0;
                int stepCount = Mathf.FloorToInt(length / segmentLength);

                // If the triangle end is before the start, ignore it.
                float triangleEnd = Mathf.Max(unit1, unit2, unit3);
                if (triangleEnd < 0)
                    step++;

                // If triangle start ends up past the end boundry, don't add it.
                float triangleStart = Mathf.Min(unit1, unit2, unit3);
                if ((length % segmentLength) < triangleStart)
                    stepCount--;

                // Draw as many triangles as necessary between the start step and total step count.
                for (; step <= stepCount; step++)
                {
                    float offset = (step * segmentLength);

                    // Clamp triangles to the limit if they're allowed to render.
                    pos1[axis] = Mathf.Clamp(unit1 + offset, 0, length);
                    pos2[axis] = Mathf.Clamp(unit2 + offset, 0, length);
                    pos3[axis] = Mathf.Clamp(unit3 + offset, 0, length);

                    // Create vertices and apply matrix.
                    Vertex vertex1 = new Vertex(pos1, normals[idx1], uvs[idx1]);
                    Vertex vertex2 = new Vertex(pos2, normals[idx2], uvs[idx2]);
                    Vertex vertex3 = new Vertex(pos3, normals[idx3], uvs[idx3]);

                    vertex1.ApplyMatrix(positioningMatrix);
                    vertex2.ApplyMatrix(positioningMatrix);
                    vertex3.ApplyMatrix(positioningMatrix);

                    builder.AddTriangle(vertex1, vertex2, vertex3, submesh);
                }
            }
        }


        /// <summary>
        /// Exports all extrusions as a mesh.
        /// </summary>
        public Mesh ToMesh()
            => builder.ToMesh();


        /// <summary>
        /// Clears all extruded mesh data, but keeps the original base mesh.
        /// </summary>
        public void Clear()
            => builder.Clear();
    }
}
