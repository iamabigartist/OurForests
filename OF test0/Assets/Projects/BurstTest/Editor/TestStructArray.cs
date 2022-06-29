using System.Linq;
using PrototypeUtils;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
namespace BurstTest.Editor
{
	/// <summary>
	///     结论:对于Struct内部的NativeArray,无法通过<see cref="NativeDisableParallelForRestrictionAttribute" /> 或者
	///     <see cref="NativeDisableContainerSafetyRestrictionAttribute" />
	///     来实现多索引访问，禁用安全系统。而只能通过对于Job的BurstCompile选项中设置DisableSafetyChecks来实现
	/// </summary>
	public struct MyTestStruct
	{
		public NativeArray<int> array;

		public int this[int i]
		{
			get => array[i];
			set => array[i] = value;
		}
	}

	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	public struct TestJob : IJobParallelFor
	{


		[NativeDisableContainerSafetyRestriction] [ReadOnly] public MyTestStruct my_test_struct_1;
		[WriteOnly] public MyTestStruct my_test_struct_2;
		public void Execute(int i)
		{
			int a = i != 0 ? my_test_struct_1[i - 1] : my_test_struct_1[i];
			my_test_struct_2[i] = my_test_struct_1[i - 1] * my_test_struct_1[i];
		}
	}

	public class TestStructArray : EditorWindow
	{
		[MenuItem("BurstTest/TestStructArray")]
		static void ShowWindow()
		{
			var window = GetWindow<TestStructArray>();
			window.titleContent = new("TestStructArray");
			window.Show();
		}

		void OnEnable()
		{
			int count = 1000000;
			var array1 = new NativeArray<int>(Enumerable.Range(0, count).ToArray(), Allocator.Persistent);
			var array2 = new NativeArray<int>(count, Allocator.Persistent);
			var struct1 = new MyTestStruct()
			{
				array = array1
			};
			var struct2 = new MyTestStruct()
			{
				array = array2
			};
			var test_job = new TestJob()
			{
				my_test_struct_1 = struct1,
				my_test_struct_2 = struct2
			};
			test_job.Schedule(count, 1024).Complete();

			Debug.Log(array2.ToArray()[..1000].ToMString(","));


			array1.Dispose();
			array2.Dispose();

		}

		void OnGUI() {}
	}
}