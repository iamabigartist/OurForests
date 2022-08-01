using PrototypeUtils;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using VolumeMegaStructure.DataDefinition.DataUnit;
namespace Labs.Lab0_Misc.Editor
{
	/// <summary>
	///     结论：job中可以使用普通结构并且通过burst编译，可以使用普通结构作为只读成员，可以使用自定义结构来包装 native container,甚至可以不用标记乱序访问来访问结构中的native container!
	///     结论2：对于静态数据只能只能访问只读字段
	///     结论3：在Burst中加载大部分ManagedType都是不支持的，包括委托函数。
	///     结论4：在BurstCompile的Job中对于Execute方法进行BurstDiscard修饰会导致输出数组内容都为0.
	/// </summary>
	public class MyConfig
	{
		// public static readonly NativeArray<int> array1 = new(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, Allocator.Persistent);

		class ArrayShared1Key {}
		// public static readonly SharedStatic<NativeArray<int>> array_shared_1 = SharedStatic<NativeArray<int>>.GetOrCreate<MyConfig, ArrayShared1Key>();
		public delegate bool VolumeSelector(VolumeUnit unit);
		public static VolumeSelector OpacitySelector(int opacity_group)
		{
			return delegate(VolumeUnit unit)
			{
				return unit.block_id == opacity_group;
			};
		}

		public delegate MyUtils GetConfigViaFunction();


		static MyUtils utils = new()
		{
			table1 = new(new[] { 1, 2, 234, 4, 2, 4 }, Allocator.Persistent),
			config2 = 10
		};
		public static GetConfigViaFunction GetConfigViaFunctionStorage()
		{
			return delegate
			{
				return utils;
			};
		}
	}

	public struct MyUtils
	{
		public NativeArray<int> table1;
		public int config2;
	}

	[BurstCompile]
	struct TestJob : IJobParallelFor
	{
		public TestJob(MyUtils utils, NativeArray<int> table2)
		{
			this.utils = utils;
			this.table2 = table2;
		}


		[ReadOnly] public MyUtils utils;
		[WriteOnly] public NativeArray<int> table2;
		// public FunctionPointer<GetConfigViaFunction> config;
		// public FunctionPointer<VolumeSelector> selector;
		public void Execute(int index)
		{
			int bonus = 0;
			if (index > 0)
			{
				bonus = utils.table1[index - 1];
			}

			table2[index] = utils.table1[index] + utils.config2 + bonus /* + array1[index % array1.Length]*/;
		}
	}

	[BurstCompile]
	public struct TestJob2 : IJobParallelFor
	{
		[WriteOnly] public NativeHashMap<int, int>.ParallelWriter write_to_list;
		public void Execute(int index) {}
	}

	public class TestJobRestriction : EditorWindow
	{
		[MenuItem("Labs/Labs.Lab0_Misc.Editor/TestJobRestriction")]
		static void ShowWindow()
		{
			var window = GetWindow<TestJobRestriction>();
			window.titleContent = new("TestJobRestriction");
			window.Show();
		}

		void OnEnable()
		{
			// Debug.Log(array1.ToMString(","));
			var m_job = new TestJob(new()
				{
					table1 = new(new int[6] { 1, 2, 234, 4, 2, 4 }, Allocator.Persistent),
					config2 = 10
				},
				new(new int[6], Allocator.Persistent));
			/*var m_job = new TestJob()
			{
				utils = new()
				{
					table1 = new(new int[6] { 1, 2, 234, 4, 2, 4 }, Allocator.Persistent),
					config2 = 10
				},
				table2 = new(new int[6], Allocator.Persistent)
			};*/

			m_job.Schedule(m_job.utils.table1.Length, 1).Complete();
			Debug.Log(m_job.table2.ToArray().ToMString(","));
			m_job.utils.table1.Dispose();
			m_job.table2.Dispose();

			NativeList<int> write_to_list = new();

			// var test_job2 = new TestJob2()
			// {
			// 	write_to_list = write_to_list.AsParallelWriter()
			// };
			// test_job2.Schedule(10, 1).Complete();
			Debug.Log(write_to_list.ToArray());
			write_to_list.Dispose();
		}

		void OnGUI() {}
	}
}