using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PrototypePackages.JobUtils.Template;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine.UIElements;
using VolumeMegaStructure.Util;
using static PrototypePackages.JobUtils.Template.IPlanFor;
using static PrototypePackages.PrototypeUtils.AsyncUtil;
namespace Labs.TestPerformance.Editor
{
	public class TestPtr : EditorWindow
	{
		[MenuItem("Labs/Labs.Lab13_TestPerformance.Editor/TestPtr")]
		static void ShowWindow()
		{
			var window = GetWindow<TestPtr>();
			window.titleContent = new("TestPtr");
			window.Show();
		}

		struct Container<T> where T : struct
		{
			public NativeArray<T> array;
			public Container(int len, Allocator allocator)
			{
				array = new(len, allocator);
			}
			public void Dispose()
			{
				array.Dispose();
			}
			public void Dispose(JobHandle jh)
			{
				array.Dispose(jh);
			}
		}

		[BurstCompile(OptimizeFor = OptimizeFor.Performance, DisableSafetyChecks = true)]
		struct TestAddArray : IJobFor, IPlanFor
		{
			public int length => array.Length;
			public int batch => 1;

			int loop_count;
			NativeArray<int4> array;
			public void Execute(int i)
			{
				for (int j = 0; j < loop_count; j++)
				{
					var a = array[i];
					a++;
					array[i] = a;
				}
			}
			public TestAddArray(int len, out NativeArray<int4> array, int loop_count)
			{
				array = new(len, Allocator.TempJob);
				this.array = array;
				this.loop_count = loop_count;
			}
		}

		[BurstCompile(OptimizeFor = OptimizeFor.Performance, DisableSafetyChecks = true)]
		struct TestAddContainer : IJobFor, IPlanFor
		{
			public int length => container.array.Length;
			public int batch => 1;

			int loop_count;
			Container<int4> container;
			public void Execute(int i)
			{
				for (int j = 0; j < loop_count; j++)
				{
					var a = container.array[i];
					a++;
					container.array[i] = a;
				}
			}
			public TestAddContainer(int len, out Container<int4> container, int loop_count)
			{
				container = new(len, Allocator.TempJob);
				this.container = container;
				this.loop_count = loop_count;
			}
		}

		async Task<string> Test0()
		{
			int len = 1 << 16;
			int loop_count = 1 << 10;
			var w = new ProfileStopWatch();

			w.Start("Add Array");
			var jh_0 = Plan<TestAddArray>(new(len, out var array, loop_count));
			JobHandle.ScheduleBatchedJobs();
			await WaitUntil(() => jh_0.IsCompleted, 1);
			w.Stop();

			w.Start("Add Container");
			var jh_1 = Plan<TestAddContainer>(new(len, out var container, loop_count));
			JobHandle.ScheduleBatchedJobs();
			await WaitUntil(() => jh_1.IsCompleted, 1);
			w.Stop();

			array.Dispose(jh_0);
			container.Dispose(jh_1);

			return w.PrintAllRecords();
		}

		CancellationTokenSource handle;
		ConcurrentQueue<Func<Task<string>>> task_queue;
		List<string> result_list;
		ListView list_view;

		async void PlanTask(CancellationToken token)
		{
			while (true)
			{
				if (task_queue.TryDequeue(out var task))
				{
					string result = await task();
					lock (result_list)
					{
						result_list.Add(result);
						list_view.Rebuild();
					}
				}
				try { await Task.Delay(1, token); }
				catch (TaskCanceledException) { return; }
			}
		}

		void OnEnable()
		{
			task_queue = new();
			result_list = new();
			handle = new();
			PlanTask(handle.Token);
		}

		void OnDisable()
		{
			handle.Cancel();
			handle.Dispose();
		}

		void CreateGUI()
		{
			var button = new Button(() => { task_queue.Enqueue(Test0); }) { text = "Test0" };
			list_view = new(result_list, 200f, () => new Label(),
				(ve, i) => ((Label)ve).text = result_list[i]);
			rootVisualElement.Add(button);
			rootVisualElement.Add(list_view);
		}
	}
}