using PrototypePackages.JobUtils.Template;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine.UIElements;
using VolumeMegaStructure.Util;
using static PrototypePackages.JobUtils.Template.IPlan;
using static VolumeMegaStructure.Util.DebugUtils;
namespace Labs.TestJobs.Editor
{
	// public struct DateTimeTimer
	// {
	// 	DateTime start;
	// 	DateTime end;
	// 	public long ticks => end.Ticks - start.Ticks;
	// 	public void Start() => start = DateTime.Now;
	// 	public void End() => end = DateTime.Now;
	// }

	[BurstCompile(OptimizeFor = OptimizeFor.Performance, DisableSafetyChecks = true)]
	public struct TestStreamSpeed : IJob, IPlan
	{
		int count;
		NativeStream.Writer stream;
		public void Execute()
		{
			stream.BeginForEachIndex(0);
			for (int i = 0; i < count; i++)
			{
				stream.Write(new int2(i, i));
			}
			stream.EndForEachIndex();
		}

		public unsafe TestStreamSpeed(int count, out NativeStream stream)
		{
			stream = new(1, Allocator.TempJob);
			this.count = count;
			this.stream = stream.AsWriter();
		}
	}

	[BurstCompile(OptimizeFor = OptimizeFor.Performance, DisableSafetyChecks = true)]
	public struct TestQueueSpeed : IJob, IPlan
	{
		int count;
		NativeQueue<int2> queue;
		public void Execute()
		{
			for (int i = 0; i < count; i++)
			{
				queue.Enqueue(new(i, i));
			}
		}
		public TestQueueSpeed(int count, out NativeQueue<int2> queue)
		{
			queue = new(Allocator.TempJob);
			this.count = count;
			this.queue = queue;
		}
	}


	public class TestBufferSpeed : EditorWindow
	{
		[MenuItem("Labs/Labs.TestJobs.Editor/TestBufferSpeeds")]
		static void ShowWindow()
		{
			var window = GetWindow<TestBufferSpeed>();
			window.titleContent = new("TestBufferSpeed");
			window.Show();
		}

		int count;

		void RunOnce()
		{
			var w = new ProfileStopWatch();

			w.Start("buffer");
			Plan<TestStreamSpeed>(new(count, out var stream)).Complete();
			w.Stop();

			w.Start("queue");
			Plan<TestQueueSpeed>(new(count, out var queue)).Complete();
			w.Stop();

			stream.Dispose();
			queue.Dispose();

			Log(w.PrintAllRecords());
		}

		void CreateGUI()
		{
			var int_field = new IntegerField("Pow Len");
			int_field.RegisterValueChangedCallback(e =>
			{
				count = 1 << e.newValue;
			});
			var button = new Button(RunOnce) { text = "RunOnce" };
			rootVisualElement.Add(int_field);
			rootVisualElement.Add(button);
		}


	}
}