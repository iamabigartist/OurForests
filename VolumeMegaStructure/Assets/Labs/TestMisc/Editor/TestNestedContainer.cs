using PrototypePackages.JobUtils.Template;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using static PrototypePackages.JobUtils.Template.IPlan;
using static Unity.Collections.LowLevel.Unsafe.UnsafeUtility;
using static VolumeMegaStructure.Util.Collection.MemUtil;
namespace Labs.TestMisc.Editor
{
	public unsafe struct TestNestedContainerJob : IJob, IPlan
	{
		public UnsafePtrList<NativeArray<int>> arrays;
		public void Execute()
		{
			for (int i = 0; i < 3; i++)
			{
				arrays.Add(New(new NativeArray<int>(10, Allocator.TempJob), Allocator.TempJob));
			}
			(*arrays[0])[0] = 1;
			(*arrays[1])[0] = 2;
			(*arrays[2])[0] = 3;
		}
		public TestNestedContainerJob(out UnsafePtrList<NativeArray<int>> arrays)
		{
			arrays = new(3, Allocator.TempJob);

			this.arrays = arrays;
		}
	}



	public unsafe class TestNestedContainer : EditorWindow
	{
		[MenuItem("Labs/Labs.Lab0_Misc.Editor/TestNestedContainer")]
		static void ShowWindow()
		{
			var window = GetWindow<TestNestedContainer>();
			window.titleContent = new("TestNestedContainer");
			window.Show();
		}

		void OnEnable()
		{
			Plan<TestNestedContainerJob>(new(out var arrays)).Complete();
			Debug.Log((*arrays[0])[0]);
			Debug.Log((*arrays[1])[0]);
			Debug.Log((*arrays[2])[0]);
			for (int i = 0; i < 3; i++)
			{
				arrays[i]->Dispose();
				Free(arrays[i], Allocator.TempJob);
			}
			arrays.Dispose();
		}

		void OnGUI() {}
	}
}