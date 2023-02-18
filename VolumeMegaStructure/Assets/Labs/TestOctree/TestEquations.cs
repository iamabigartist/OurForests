using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Sequential256.Octree;
using static VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Sequential256.Octree.OctreeDebugUtil;
using static VolumeMegaStructure.Util.DebugUtils;
namespace Labs.TestOctree
{
	public class TestEquations : MonoBehaviour
	{
		public bool draw_label;
		public float ratio;
		public List<OctRegion> region_array;
		[ContextMenu("Gen_Region_Nested_3")]
		public void Gen_Region_Nested_3()
		{
			var a_chunk = new OctRegion(int3.zero, 8);
			var a_3 = a_chunk.child(3);
			var a_3_2 = a_3.child(2);
			var a_3_2_4 = a_3_2.child(4);
			Log_Json(new
			{
				chunk = RegionString(a_chunk),
				r_3 = RegionString(a_3),
				r_3_2 = RegionString(a_3_2),
				r_3_2_4 = RegionString(a_3_2_4)
			});
			region_array = new()
			{
				a_chunk,
				a_3,
				a_3_2,
				a_3_2_4
			};
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
			foreach (var cur_region in t.region_array)
			{
				RegionCube(cur_region, out var anchor_v, out var center, out var size);
				Handles.DrawWireCube(center, size);
				var parent_region = cur_region.parent();
				if (draw_label)
				{
					Handles.Label(anchor_v,
						$"{RegionString(cur_region)}\n" +
						$"{RegionString(parent_region)}", GUI.skin.box);
				}
			}
		}
	}
}