using PrototypeUtils;
using Unity.Mathematics;
using UnityEngine;
using static VolumeMegaStructure.Generate.ProceduralMesh.Voxel.GenVoxelSourceTables;
using static VolumeMegaStructure.Util.VoxelProcessUtility;
namespace Labs.Lab8_TestVerticesRotation
{
    /// <summary>
    ///     <para>
    ///         1. Show how to use <see cref="VolumeMegaStructure.Generate.ProceduralMesh.Voxel.GenVoxelSourceTables" /> and
    ///         relative part in <see cref="VolumeMegaStructure.Util.VoxelProcessUtility" />
    ///     </para>
    ///     <para>
    ///         2. Test the rotation correctness of
    ///         <see cref="VolumeMegaStructure.Generate.ProceduralMesh.Voxel.GenVoxelSourceTables" />
    ///     </para>
    /// </summary>
    public class TestVRotation : MonoBehaviour
	{
		static readonly int2[] uv_list =
		{
			(0, 0).i2(),
			(1, 0).i2(),
			(1, 1).i2(),
			(0, 1).i2()
		};

        /// <summary>
        ///     Construct a quad with 4 vertices, each with a uv in the const list.
        /// </summary>
        static Mesh InitQuad(int i_up, int i_forward, int i_face)
		{
			if (!ValidLookRotation(i_up, i_forward))
			{
				Debug.LogError("Invalid rotation");

				return null;
			}

			i_rotation_i_face_i_vertex_Compose(i_up, i_forward, i_face, 0, out var i0);
			var mesh = new Mesh
			{
				vertices = FixedUVVertexTable[i0..(i0 + 4)].ToVectorArray(),
				uv = uv_list.ToVectorArray(),
				triangles = new[] { 0, 1, 2, 2, 3, 0 }
			};
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();

			return mesh;
		}

		Transform transform_mesh;
		Transform table_mesh;
		MeshFilter table_mesh_filter;
		MeshFilter transform_mesh_filter;

		public int i_up = 2;
		public int i_forward = 4;
		public int i_face = 0;

		void Start()
		{
			transform_mesh = transform.Find("TransformQuad");
			transform_mesh_filter = transform_mesh.Find("Quad").GetComponent<MeshFilter>();
			table_mesh = transform.Find("TableQuad");
			table_mesh_filter = table_mesh.GetComponent<MeshFilter>();
			table_mesh_filter.mesh = InitQuad(2, 4, 0);
		}

		[ContextMenu("Regenerate")]
		void Regenerate()
		{
			table_mesh_filter.mesh = InitQuad(i_up, i_forward, i_face);
			transform_mesh_filter.mesh = InitQuad(2, 4, i_face);
			transform_mesh.rotation = IndexLookRotation(i_up, i_forward);
		}
	}
}