using System;
using System.Collections.Generic;
using OpenRCT2.Utilities;
using UnityEngine;

#nullable enable

namespace OpenRCT2.Generators.MeshBuilding
{
    /// <summary>
    /// Extents the MeshBuilder base class with some helpful extra methods.
    /// </summary>
    public static class MeshBuilderExtensions
    {
        #region Polygons

        /// <summary>
        /// Adds a convex polygon to the mesh under the specified submesh. Vertices are
        /// expected to be in clock-wise order.
        ///
        /// Currently used triangulation implementation is 'fan triangulation'.
        /// </summary>
        public static void AddConvexPolygon(this MeshBuilder builder, int submesh, params Vertex[] vertices)
        {
            int length = vertices.Length;
            if (length < 3)
            {
                throw new ArgumentException("A convex polygon should have at least 3 vertices.", nameof(vertices));
            }

            ref Vertex fanRoot = ref vertices[0];

            for (int v = 2; v < length; v++)
            {
                builder.AddTriangle(fanRoot, vertices[v - 1], vertices[v], submesh);
            }
        }


        /// <summary>
        /// Adds a convex polygon to the mesh under submesh 0. Vertices are expected to
        /// be in clock-wise order.
        /// 
        /// Currently used triangulation implementation is 'fan triangulation'.
        /// </summary>
        public static void AddConvexPolygon(this MeshBuilder builder, params Vertex[] vertices)
            => builder.AddConvexPolygon(submesh: 0, vertices);


        /// <summary>
        /// Adds a convex polygon to the mesh under the specified submesh. Vertices are
        /// expected to be in clock-wise order.
        ///
        /// Currently used triangulation implementation is 'fan triangulation'.
        /// </summary>
        public static void AddConvexPolygon(this MeshBuilder builder, int submesh, IEnumerable<Vertex> vertices)
        {
            IEnumerator<Vertex> enumerator = vertices.GetEnumerator();

            // First take out 3 vertices if possible.
            if (enumerator.MoveNext())
            {
                Vertex fanRoot = enumerator.Current;
                if (enumerator.MoveNext())
                {
                    Vertex vert1 = enumerator.Current;
                    if (enumerator.MoveNext())
                    {
                        Vertex vert2 = enumerator.Current;

                        // Add triangle and keep taking out move vertices.
                        while (true)
                        {
                            builder.AddTriangle(fanRoot, vert1, vert2, submesh);

                            if (!enumerator.MoveNext())
                            {
                                return;
                            }

                            vert1 = enumerator.Current;
                            builder.AddTriangle(fanRoot, vert2, vert1, submesh);

                            if (!enumerator.MoveNext())
                            {
                                return;
                            }

                            vert2 = enumerator.Current;
                        }
                    }
                }
            }

            throw new ArgumentException("A convex polygon should have at least 3 vertices.", nameof(vertices));
        }


        /// <summary>
        /// Adds a convex polygon to the mesh under submesh 0. Vertices are expected to
        /// be in clock-wise order.
        /// 
        /// Currently used triangulation implementation is 'fan triangulation'.
        /// </summary>
        public static void AddConvexPolygon(this MeshBuilder builder, IEnumerable<Vertex> vertices)
            => builder.AddConvexPolygon(submesh: 0, vertices);


        #endregion


        #region Subdivision

        /// <summary>
        /// Add a simple subdivided triangle to the builder. Vertex order is clock-wise.
        /// </summary>
        public static void AddSubdividedTriangle(this MeshBuilder builder, Vertex a, Vertex b, Vertex c, int levels, int submesh = 0)
        {
            var midAB = new Vertex(Maths.GetCenter(a.position, b.position), Maths.GetCenter(a.normal, b.normal), Maths.GetCenter(a.uv, b.uv));
            var midBC = new Vertex(Maths.GetCenter(b.position, c.position), Maths.GetCenter(b.normal, c.normal), Maths.GetCenter(b.uv, c.uv));
            var midCA = new Vertex(Maths.GetCenter(c.position, a.position), Maths.GetCenter(c.normal, a.normal), Maths.GetCenter(c.uv, a.uv));

            if (levels <= 1)
            {
                builder.AddTriangle(a, midAB, midCA, submesh);
                builder.AddTriangle(b, midBC, midAB, submesh);
                builder.AddTriangle(c, midCA, midBC, submesh);
                builder.AddTriangle(midAB, midBC, midCA, submesh);
            }
            else
            {
                levels--;
                builder.AddSubdividedTriangle(a, midAB, midCA, levels, submesh);
                builder.AddSubdividedTriangle(b, midBC, midAB, levels, submesh);
                builder.AddSubdividedTriangle(c, midCA, midBC, levels, submesh);
                builder.AddSubdividedTriangle(midAB, midBC, midCA, levels, submesh);
            }
        }


        /// <summary>
        /// Add a simple subdivided quad to the builder. Vertex order is clock-wise.
        /// </summary>
        public static void AddSimpleSubdividedQuad(this MeshBuilder builder, Vertex a, Vertex b, Vertex c, Vertex d, int submesh = 0)
        {
            float[] matrix = new float[] { 1 / 16f, 3 / 16f, 9 / 16f, 3 / 16f };
            Vertex[] innerVerts = new Vertex[4];

            for (int p = 0; p < 4; p++)
            {
                innerVerts[p] = new Vertex(
                    CalculateMatrix(matrix, p, a.position, b.position, c.position, d.position),
                    Vector3.up,
                    CalculateMatrix(matrix, p, a.uv, b.uv, c.uv, d.uv));
            }

            builder.AddQuad(innerVerts[3], innerVerts[2], innerVerts[1], innerVerts[0], submesh);
            builder.AddQuad(d, a, innerVerts[2], innerVerts[3], submesh);
            builder.AddQuad(a, b, innerVerts[1], innerVerts[2], submesh);
            builder.AddQuad(b, c, innerVerts[0], innerVerts[1], submesh);
            builder.AddQuad(c, d, innerVerts[3], innerVerts[0], submesh);
        }


        /// <summary>
        /// Calculates the matrix for the specified <see cref="Vector3"/>.
        /// </summary>
		static Vector3 CalculateMatrix(float[] matrix, int offset, params Vector3[] vectors)
        {
            Vector3 result = Vector3.zero;
            int max = vectors.Length;

            for (int i = 0; i < max; i++)
            {
                result += matrix[(i + offset) % 4] * vectors[i];
            }

            return result;
        }


        /// <summary>
        /// Calculates the matrix for the specified <see cref="Vector2"/>.
        /// </summary>
        static Vector2 CalculateMatrix(float[] matrix, int offset, params Vector2[] vectors)
        {
            Vector2 result = Vector2.zero;
            int max = vectors.Length;

            for (int i = 0; i < max; i++)
            {
                result += matrix[(i + offset) % 4] * vectors[i];
            }

            return result;
        }

        #endregion


        #region Slicing

        /// <summary>
        /// Add a triangle that is potentially sliced by one or more planes.
        /// </summary>
        public static void AddSlicedTriangle(this MeshBuilder builder, in Vertex a, in Vertex b, in Vertex c, int submesh = 0, params Plane[] slicePlanes)
        {
            var output = new List<Vertex>(capacity: 8)
            {
                a, b, c
            };

            for (int s = 0; s < slicePlanes.Length; s++)
            {
                Vertex[] input = output.ToArray();
                output.Clear();

                ref Plane plane = ref slicePlanes[s];
                int amount = input.Length;

                for (int v = 0; v < amount; v++)
                {
                    ref Vertex current = ref input[v];
                    ref Vertex previous = ref input[(v + amount - 1) % amount];

                    SliceEdge(output, previous, current, plane);
                }
            }

            if (output.Count >= 3)
            {
                builder.AddConvexPolygon(submesh, output);
            }
        }


        /// <summary>
        /// Slices off a part of the triangle if it overlaps the plane inspired
        /// by the Sutherland-Hodgman algorithm.
        /// </summary>
        static bool SliceEdge(List<Vertex> buffer, in Vertex start, in Vertex end, in Plane plane)
        {
            float negEpsilon = -Mathf.Epsilon;

            Vector3 pos1 = start.position;
            Vector3 pos2 = end.position;

            float dist1 = plane.GetDistanceToPoint(pos1);
            float dist2 = plane.GetDistanceToPoint(pos2);

            bool inside1 = dist1 >= negEpsilon;
            bool inside2 = dist2 >= negEpsilon;

            float length = dist1 - dist2;
            float time = 1 + dist2 / length;

            if (inside1 != inside2)
            {
                Vertex intersection = new Vertex(
                    Vector3.Lerp(start.position, end.position, time),
                    Vector3.Lerp(start.normal, end.normal, time)
                );
                buffer.Add(intersection);
            }

            if (inside2)
            {
                buffer.Add(end);
            }

            return inside1 && inside2;
        }

        #endregion
    }
}
