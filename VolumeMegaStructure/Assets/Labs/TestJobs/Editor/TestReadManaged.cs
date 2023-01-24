using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using VolumeMegaStructure.Util;
namespace Labs.TestJobs.Editor
{

	public struct ReadWriteJob : IJobParallelFor
	{
		// InvalidOperationException: ReadWriteJob.managed_array is not a value type. Job structs may not contain any reference types.
		// public int[] managed_array;
		[ReadOnly]
		public NativeArray<int> managed_array;
		public NativeArray<int> unmanaged_array;
		public void Execute(int i)
		{
			unmanaged_array[i] = managed_array[i];
		}
	}
	public class TestReadManaged : EditorWindow
	{
		[MenuItem("Labs/Labs.Lab2_TestJobs.Editor/TestReadManaged")]
		static void ShowWindow()
		{
			var window = GetWindow<TestReadManaged>();
			window.titleContent = new("TestReadManaged");
			window.Show();
		}

		int length;
		int[] managed_read_array;
		int[] managed_write_array;

		void OnEnable()
		{
			length = 10000;
			managed_read_array = (..length).ToArray();
			managed_write_array = new int[length];
			var job = new ReadWriteJob
			{
				managed_array = new(managed_read_array, Allocator.TempJob),
				unmanaged_array = new(length, Allocator.TempJob)
			};
			var handle = job.Schedule(length, 64);
			handle.Complete();
			managed_write_array = job.unmanaged_array.ToArray();
			job.unmanaged_array.Dispose();
		}


		void OnGUI()
		{
			EditorGUILayout.LabelField($"{string.Join(",", managed_write_array)}");
		}
	}
}