using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
namespace BurstTest
{

	public struct TestStruct1
	{
		int a;
		public TestStruct1(int a)
		{
			this.a = a;
		}
	}

	[BurstCompile]
	public struct TestJob2 : IJobParallelFor
	{
		[ReadOnly] public NativeArray<(TestStruct1 struct1, int b)> array1;
		[WriteOnly] public NativeArray<TestStruct1> array2;
		public void Execute(int i)
		{
			array2[i] = array1[i].struct1;
		}
	}

	public class TestTupleArray : EditorWindow
	{
		[MenuItem("BurstTest/TestTupleArray")]
		static void ShowWindow()
		{
			var window = GetWindow<TestTupleArray>();
			window.titleContent = new("TestTupleArray");
			window.Show();
		}

		void OnEnable()
		{
			var array1 = new NativeArray<(TestStruct1 struct1, int b)>(new[]
			{
				(new(1), 1),
				(new(2), 1),
				(new TestStruct1(1), 2)
			}, Allocator.Persistent);
			var array2 = new NativeArray<TestStruct1>(3, Allocator.Persistent);
			var job = new TestJob2()
			{
				array1 = array1,
				array2 = array2
			};
			job.Schedule(3, 1).Complete();
			array1.Dispose();
			array2.Dispose();
		}

		void OnGUI() {}
	}
}