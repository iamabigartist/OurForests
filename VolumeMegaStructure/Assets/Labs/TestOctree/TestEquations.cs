using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using static VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Sequential256.Octree.OctreeDebugUtil;
using static VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Sequential256.Octree.OctreeEnumerateUtil;
using static VolumeMegaStructure.Util.DebugUtils;
using Random = Unity.Mathematics.Random;
namespace Labs.TestOctree
{
	public class TestEquations : MonoBehaviour
	{
		public bool draw_label;
		public float ratio;
		public List<(int3 anchor, int size_level)> region_array;
		[ContextMenu("Gen_Region_Nested_3")]
		public void Gen_Region_Nested_3()
		{
			int3 a_chunk = int3.zero;
			get_region_child(a_chunk, 8, 3, out var a_3);
			get_region_child(a_3, 7, 2, out var a_3_2);
			get_region_child(a_3_2, 6, 4, out var a_3_2_4);
			Log_Json(new
			{
				chunk = RegionString(a_chunk, 8),
				r_3 = RegionString(a_3, 7),
				r_3_2 = RegionString(a_3_2, 6),
				r_3_2_4 = RegionString(a_3_2_4, 5)
			});
			region_array = new()
			{
				(a_chunk, 8),
				(a_3, 7),
				(a_3_2, 6),
				(a_3_2_4, 5)
			};
		}

		void TraverseNode(ref Random rand, int3 parent_anchor, int parent_size_level, int cur_octant)
		{
			var cur_size_level = parent_size_level - 1;
			get_region_child(parent_anchor, parent_size_level, cur_octant, out var cur_anchor);
			if (parent_size_level != 1)
			{
				for (int i = 0; i < 8; i++)
				{
					TraverseNode(ref rand, cur_anchor, cur_size_level, i);
				}
			}
			if (rand.NextFloat(1) < ratio)
			{
				region_array.Add((cur_anchor, cur_size_level));
			}

		}

		[ContextMenu("Gen_Region_Random")]
		public void Gen_Region_Random()
		{
			region_array = new();
			var rand = Random.CreateFromIndex(100);
			for (int i = 0; i < 8; i++)
			{
				TraverseNode(ref rand, int3.zero, 6, i);
			}
			// region_array.Add((int3.zero, 8));
		}

	}

	[CustomEditor(typeof(TestEquations))]
	public class TestEquationEditor : Editor
	{
		public bool draw_label;
		public float ratio;

		void OnSceneGUI()
		{
			var t = target as TestEquations;
			if (t.region_array == null) { return; }
			draw_label = t.draw_label;
			ratio = t.ratio;
			foreach ((int3 anchor, int size_level) in t.region_array)
			{
				RegionCube(anchor, size_level, out var anchor_v, out var center, out var size);
				Handles.DrawWireCube(center, size);
				get_region_parent(anchor, size_level, out var parent_anchor);
				if (draw_label)
				{
					Handles.Label(anchor_v,
						$"{RegionString(anchor, size_level)}\n" +
						$"{RegionString(parent_anchor, size_level + 1)}", GUI.skin.box);
				}
			}
		}
	}
}