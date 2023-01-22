using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
namespace VolumeMegaStructure.Util.JobSystem.Jobs
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	public struct NativeArrayToParallelHashSetForJob<T> : IJobFor where T : unmanaged, IEquatable<T>
	{
		[ReadOnly] NativeArray<T> array;
		[WriteOnly] NativeParallelHashSet<T>.ParallelWriter set;
		
		public void Execute(int i)
		{
			set.Add(array[i]);
		}

		public static JobHandle ScheduleParallel(NativeArray<T> array, out NativeParallelHashSet<T> set, JobHandle deps = default)
		{
			var count = array.Length;
			set = new(array.Length, Allocator.TempJob);
			var job = new NativeArrayToParallelHashSetForJob<T>()
			{
				array = array,
				set = set.AsParallelWriter()
			};
			return job.ScheduleParallel(count, count / JobsUtility.JobWorkerMaximumCount, deps);
		}

		public static JobHandle ScheduleParallel(NativeArray<T>[] arrays, out NativeParallelHashSet<T>[] sets, JobHandle deps = default)
		{
			sets = new NativeParallelHashSet<T>[arrays.Length];
			var jhs = new NativeArray<JobHandle>(arrays.Length, Allocator.Temp);
			for (int i = 0; i < arrays.Length; i++)
			{
				var cur_array = arrays[i];
				var count = cur_array.Length;
				jhs[i] = ScheduleParallel(cur_array, out sets[i], deps);
			}
			var total_deps = JobHandle.CombineDependencies(jhs);
			jhs.Dispose();
			return total_deps;
		}

	}
}