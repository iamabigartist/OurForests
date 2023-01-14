using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
namespace VolumeMegaStructure.Util.JobUtils.Jobs
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	public struct NativeQueueToHashSetJob<T> : IJob where T : unmanaged, IEquatable<T>
	{
		[ReadOnly] NativeQueue<T> queue;
		[WriteOnly] NativeHashSet<T> set;
		public void Execute()
		{
			while (queue.TryDequeue(out var item))
			{
				set.Add(item);
			}
		}

		public static JobHandle Schedule(NativeQueue<T> queue, out NativeHashSet<T> set, JobHandle deps = default)
		{
			var count = queue.Count;
			set = new(queue.Count, Allocator.TempJob);
			var job = new NativeQueueToHashSetJob<T>()
			{
				queue = queue,
				set = set
			};
			return job.Schedule(deps);
		}

	}
}