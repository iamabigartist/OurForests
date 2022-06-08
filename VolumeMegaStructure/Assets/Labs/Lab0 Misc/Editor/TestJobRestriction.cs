using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
namespace Labs.Lab0_Misc.Editor
{
	/// <summary>
	///     结论：job中可以使用普通结构并且通过burst编译，可以使用普通结构作为只读成员，可以使用自定义结构来包装 native container!
	/// </summary>
	struct MyUtils
	{
		public NativeArray<int> table1;
		public int config2;
	}

	[BurstCompile]
	struct TestJob : IJobParallelFor
	{
		[ReadOnly] public MyUtils utils;
		[WriteOnly] public NativeArray<int> table2;
		public void Execute(int index)
		{
			table2[index] = utils.table1[index] + utils.config2;
		}
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
			var m_job = new TestJob()
			{
				utils = new()
				{
					table1 = new(new int[6] { 1, 2, 234, 4, 2, 4 }, Allocator.Persistent),
					config2 = 10
				},
				table2 = new(new int[6], Allocator.Persistent)
			};
			m_job.Schedule(m_job.utils.table1.Length, 1).Complete();
			Debug.Log(m_job.table2[0]);
			m_job.utils.table1.Dispose();
			m_job.table2.Dispose();
		}

		void OnGUI() {}
	}
}