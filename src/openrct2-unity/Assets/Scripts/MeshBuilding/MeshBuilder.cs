using System;
using System.Collections.Generic;
using UnityEngine;

namespace MeshBuilding
{
    /// <summary>
    /// Small class to easily create meshes through C#.
    /// </summary>
    public partial class MeshBuilder
	{
        /// <summary>
        /// Returns all vertices currently added to the builder.
        /// </summary>
        public IReadOnlyCollection<Vertex> Vertices
            => vertices.Keys;


        /// <summary>
        /// Returns all vertices currently added to the builder.
        /// </summary>
        public Bounds Bounds
            => CalculateBounds();


        // Dictionary of vertices and their indexes.
        readonly Dictionary<Vertex, int> vertices;

        // List of triangles per submesh.
        readonly List<List<int>> triangles;


        #region Constructors

        /// <summary>
        /// Creates a regular mesh builder.
        /// </summary>
        public MeshBuilder()
        {
            vertices = new Dictionary<Vertex, int>(64);
            triangles = new List<List<int>>(1);
        }


        /// <summary>
        /// Creates a mesh builder from an existing mesh.
        /// </summary>
        public MeshBuilder(Mesh mesh)
        {
            // Load vertices
            Vector3[] meshVerts = mesh.vertices;
            Vector3[] meshNormals = mesh.normals;
            Vector2[] meshUvs = mesh.uv;

            int vertexCount = mesh.vertexCount;
            vertices = new Dictionary<Vertex, int>(vertexCount);

            int index = 0;
            for (int v = 0; v < vertexCount; v++)
            {
                Vertex vertex = new Vertex(
                    pos: meshVerts[v],
                    nor: meshNormals[v],
                    uvs: meshUvs[v]
                );

                if (vertices.ContainsKey(vertex))
                    continue;

                vertices.Add(vertex, index);
                index++;
            }

            // Load triangles
            int submeshCount = mesh.subMeshCount;
            triangles = new List<List<int>>(submeshCount);

            for (int s = 0; s < submeshCount; s++)
            {
                List<int> buffer = new List<int>(0);
                mesh.GetTriangles(buffer, s);

                triangles[s] = buffer;
            }
        }

        #endregion


        /// <summary>
        /// Add a vertex to the mesh. Returns the index of this vertex.
        /// </summary>
        int AddVertex(Vertex a)
		{
			if (vertices.ContainsKey(a))
			{
				return vertices[a];
			}
			else
			{
				int idx = vertices.Count;
				vertices[a] = idx;
				return idx;
			}
		}


		/// <summary>
		/// Add a triangle with 3 vertices. Order: clock-wise.
		/// </summary>
		public void AddTriangle(Vertex a, Vertex b, Vertex c, int submesh = 0)
		{
            // Useless triangle, if 2 vertices are in the same location.
            if (a.position == b.position || b.position == c.position || a.position == c.position)
                return;

			while (submesh >= triangles.Count)
				triangles.Add(new List<int>(16));

			triangles[submesh].AddRange(new int[]
            {
				AddVertex(a),
				AddVertex(b),
				AddVertex(c)
			});
		}


		/// <summary>
		/// Add two triangles in the shape of a quad. Order: top left -> top 
		/// right -> bottom right -> bottom left (clock-wise).
		/// </summary>
		public void AddQuad(Vertex a, Vertex b, Vertex c, Vertex d, int submesh = 0)
		{
			AddTriangle(a, b, c, submesh);
			AddTriangle(c, d, a, submesh);
		}


		/// <summary>
		/// Clear all vertice and triangle data in the MeshBuilder.
		/// </summary>
		public void Clear()
		{
			vertices.Clear();
			triangles.Clear();
		}


		/// <summary>
		/// Build the vertices and triangles into a Mesh.
		/// </summary>
		public Mesh ToMesh()
		{
            Mesh mesh = new Mesh();

			int count = vertices.Count;
			if (count == 0)
				return null;

			Vector3[] verts = new Vector3[count];
			Vector3[] norms = new Vector3[count];
			Vector2[] uvs = new Vector2[count];
			int idx;
			foreach (KeyValuePair<Vertex, int> vert in vertices)
			{
				idx = vert.Value;

				verts[idx] = vert.Key.position;
				norms[idx] = vert.Key.normal;
				uvs[idx] = vert.Key.uv;
			}

			mesh.vertices = verts;
			mesh.normals = norms;
			mesh.uv = uvs;

			// Add all submeshes
			int subMeshCount = triangles.Count;
			mesh.subMeshCount = subMeshCount;

			for (int i = 0; i < subMeshCount; i++)
				mesh.SetTriangles(triangles[i], i);

			return mesh;
		}


        /// <summary>
        /// Returns the bounds of all vertices.
        /// </summary>
        Bounds CalculateBounds()
        {
            float min_x = float.MaxValue, min_y = float.MaxValue, min_z = float.MaxValue,
                max_x = float.MinValue, max_y = float.MinValue, max_z = float.MinValue;

            foreach (Vertex vertex in vertices.Keys)
            {
                Vector3 pos = vertex.position;

                if (min_x > pos.x) min_x = pos.x;
                if (min_y > pos.y) min_y = pos.y;
                if (min_z > pos.z) min_x = pos.z;

                if (max_x < pos.x) max_x = pos.x;
                if (max_y < pos.y) max_y = pos.y;
                if (max_z < pos.z) max_x = pos.z;
            }

            Vector3 min = new Vector3(min_x, min_y, min_z);
            Vector3 max = new Vector3(max_x, max_y, max_z);

            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }
    }
}

