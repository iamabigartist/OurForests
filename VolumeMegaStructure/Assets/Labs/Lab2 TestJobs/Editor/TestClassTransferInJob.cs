using System.Linq;
using PrototypeUtils;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
namespace Labs.Lab2_TestJobs.Editor
{
	/// <summary>
	///     <para> Conclusion: Job structs can not contain any reference types.</para>
	///     <para>
	///         如果想要使用job之间的传值，一是直接使用一个数组，而是数组本身的count可以作为一个单独信息传出去，要么就是
	///         <see cref="VolumeMegaStructure.Util.JobSystem.NativeCounter" />.
	///     </para>
	/// </summary>
	public class TestClassTransferInJob : EditorWindow
	{
		[MenuItem("Labs/Labs.Lab2_TestJobs.Editor/TestClassTransferInJob")]
		static void ShowWindow()
		{
			var window = GetWindow<TestClassTransferInJob>();
			window.titleContent = new("TestClassTransferInJob");
			window.Show();
		}

		public class StatisticsInfo
		{
			public double between_ratio;
			public int average_value;
		}

		public struct BeforeReduceJob : IJobParallelFor
		{
			[WriteOnly] public NativeArray<int> list;
			public void Execute(int i)
			{
				var rand = Random.CreateFromIndex((uint)i);
				list[i] = rand.NextInt(1000);
			}
		}

		public struct ReduceJob : IJob
		{
			[ReadOnly] public NativeArray<int> list;
			[WriteOnly] public StatisticsInfo transfer_info;
			public void Execute()
			{
				int sum = list.Sum();
				int between_count = 0;
				transfer_info = new();
				transfer_info.average_value = sum / list.Length;
				for (int i = 0; i < list.Length; i++)
				{
					if (transfer_info.average_value - 100 <= list[i] &&
						list[i] <= transfer_info.average_value + 100)
					{
						between_count++;
					}
				}
				transfer_info.between_ratio = (double)between_count / list.Length;
			}
		}

		public struct AfterReduceJob : IJobParallelFor
		{
			[ReadOnly] public StatisticsInfo transfer_info;
			[WriteOnly] public NativeArray<int> result_list;
			public void Execute(int i)
			{
				var rand = Random.CreateFromIndex((uint)i);
				// if (rand.NextDouble(0, 1) < transfer_info.between_ratio)
				// {
				// 	result_list[i] = rand.NextInt(
				// 		transfer_info.average_value - 100,
				// 		transfer_info.average_value + 100);
				// }
				// else
				// {
				// 	result_list[i] = rand.NextInt(1000);
				// }
				result_list[i] = transfer_info.average_value;
			}
		}

		string result_string;
		void OnEnable()
		{
			var info = new StatisticsInfo();
			var original_array = new NativeArray<int>(1000, Allocator.TempJob);
			var result_array = new NativeArray<int>(1000, Allocator.TempJob);
			var before_job = new BeforeReduceJob()
			{
				list = original_array
			};
			var do_job = new ReduceJob()
			{
				list = original_array,
				transfer_info = info
			};
			var after_job = new AfterReduceJob()
			{
				result_list = result_array,
				transfer_info = info
			};
			var before_handle = before_job.Schedule(1000, 1);
			var do_handle = do_job.Schedule(before_handle);
			var after_handle = after_job.Schedule(1000, 1, do_handle);
			after_handle.Complete();
			result_string = after_job.result_list.ToArray()[..100].ToMString(",");
			original_array.Dispose();
			result_array.Dispose();
		}

		void OnGUI()
		{
			EditorGUILayout.LabelField(result_string);
		}
	}
}