using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.MeshBuilding
{
    /// <summary>
    /// Class that can build extruded meshes out of one of more warped segments.
    /// </summary>
    public class MeshExtruder
    {
        readonly Vector3[] _vertices;
        readonly Vector3[] _normals;
        readonly Vector2[] _uvs;
        readonly int[] _triangles;
        readonly Bounds _bounds;

        readonly MeshBuilder _builder = new MeshBuilder();
        readonly int _axis;


        /// <summary>
        /// Creates a mesh extruder based on the given base mesh.
        /// </summary>
        public MeshExtruder(Mesh mesh)
        {
            _axis = 2; // Z axis

            _vertices = mesh.vertices;
            _normals = mesh.normals;
            _uvs = mesh.uv;
            _triangles = mesh.triangles;
            _bounds = mesh.bounds;
        }


        /// <summary>
        /// Creates a mesh extruder based on the given base mesh, on the given axis. (X = 0, Y = 1, Z = 2)
        /// </summary>
        public MeshExtruder(Mesh mesh, int axis)
        {
            _axis = axis;

            _vertices = mesh.vertices;
            _normals = mesh.normals;
            _uvs = mesh.uv;
            _triangles = mesh.triangles;
            _bounds = mesh.bounds;
        }


        /// <summary>
        /// Adds an extruded segment to the final mesh. Returns the length of the segment.
        /// </summary>
        public float AddSegment(Vector3 positionStart, Quaternion directionStart, Vector3 positionEnd, Quaternion directionEnd, float offset, float multiplier, int submesh = 0)
        {
            float length = Vector3.Distance(positionStart, positionEnd);
            float meshExtent = _bounds.extents[_axis] * multiplier;
            float segmentLength = meshExtent * 2;

            Matrix4x4 matrixStart = Matrix4x4.TRS(positionStart, directionStart, Vector3.one);
            Matrix4x4 matrixEnd = Matrix4x4.TRS(positionEnd, directionEnd, Vector3.one);

            // Shift back bounds to 0, and apply the start as negative position.
            float startMesh = _bounds.center[_axis] - meshExtent + Maths.Modulo(offset, segmentLength);

            for (int t = 0; t < _triangles.Length; t += 3)
            {
                // Get index of vertices/normals for this triangle.
                int idx1 = _triangles[t];
                int idx2 = _triangles[t + 1];
                int idx3 = _triangles[t + 2];

                Vector3 pos1 = _vertices[idx1];
                Vector3 pos2 = _vertices[idx2];
                Vector3 pos3 = _vertices[idx3];

                float unit1 = (pos1[_axis] - startMesh) * multiplier;
                float unit2 = (pos2[_axis] - startMesh) * multiplier;
                float unit3 = (pos3[_axis] - startMesh) * multiplier;

                float triangleStart = Mathf.Min(unit1, unit2, unit3);

                int step = 0;
                int stepCount = Mathf.CeilToInt((length - triangleStart) / segmentLength);

                // If the triangle end is before the start, skip a step.
                float triangleEnd = Mathf.Max(unit1, unit2, unit3);
                if (triangleEnd < 0)
                    step++;

                // Draw as many triangles as necessary between the start step and total step count.
                for (; step < stepCount; step++)
                {
                    float stepPosition = step * segmentLength;

                    // Clamp triangles to the limit if they're allowed to render.
                    pos1[_axis] = Mathf.Clamp(unit1 + stepPosition, 0, length);
                    pos2[_axis] = Mathf.Clamp(unit2 + stepPosition, 0, length);
                    pos3[_axis] = Mathf.Clamp(unit3 + stepPosition, 0, length);

                    // Create vertices and apply matrix.
                    Vertex vertex1 = LerpVertex(pos1, _normals[idx1], _uvs[idx1]);
                    Vertex vertex2 = LerpVertex(pos2, _normals[idx2], _uvs[idx2]);
                    Vertex vertex3 = LerpVertex(pos3, _normals[idx3], _uvs[idx3]);

                    _builder.AddTriangle(vertex1, vertex2, vertex3, submesh);
                }
            }
            return length;


            Vertex LerpVertex(Vector3 position, in Vector3 normal, in Vector2 uvs)
            {
                float time = position[_axis] / length;
                Matrix4x4 warpedMatrix = LerpMatrix(matrixStart, matrixEnd, time);
                position[_axis] = 0;

                return new Vertex
                (
                    warpedMatrix.MultiplyPoint3x4(position),
                    warpedMatrix.MultiplyVector(normal),
                    uvs
                );
            }
        }


        /// <summary>
        /// Adds an extruded segment to the final mesh. Returns the length of the segment.
        /// </summary>
        public float AddSegment(TransformPoint start, TransformPoint end, float offset, float multiplier, int submesh = 0)
            => AddSegment(start.Position, start.Rotation, end.Position, end.Rotation, offset, multiplier, submesh);



        static Matrix4x4 LerpMatrix(in Matrix4x4 left, in Matrix4x4 right, float time)
        {
            const int size = 4 * 4;
            var result = new Matrix4x4();

            for (int i = 0; i < size; i++)
            {
                result[i] = Mathf.Lerp(left[i], right[i], time);
            }

            return result;
        }



        /// <summary>
        /// Exports all extrusions as a mesh.
        /// </summary>
        public Mesh ToMesh()
            => _builder.ToMesh();


        /// <summary>
        /// Clears all extruded mesh data, but keeps the original base mesh.
        /// </summary>
        public void Clear()
            => _builder.Clear();
    }
}
