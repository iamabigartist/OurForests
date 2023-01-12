using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
namespace VolumeMegaStructure.Util.JobUtils.Jobs
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	public struct NativeArrayToHashSetForJob<T> : IJobParallelFor where T : unmanaged, IEquatable<T>
	{
		[ReadOnly] NativeArray<T> array;
		[WriteOnly] NativeHashSet<T>.ParallelWriter set;
		public void Execute(int i)
		{
			set.Add(array[i]);
		}

		public static JobHandle ScheduleParallel(NativeArray<T> array, out NativeHashSet<T> set, JobHandle deps = default)
		{
			var count = array.Length;
			set = new(array.Length, Allocator.TempJob);
			var job = new NativeArrayToHashSetForJob<T>()
			{
				array = array,
				set = set.AsParallelWriter()
			};
			return job.Schedule(count, count / JobsUtility.JobWorkerMaximumCount, deps);
		}

	}
}