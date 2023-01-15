using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using static Unity.Mathematics.math;
namespace VolumeMegaStructure.Util.JobSystem
{
	public static class ScheduleUtils
	{
		public static JobHandle Combine(this JobHandle[] jhs)
		{
			var native_array = new NativeArray<JobHandle>(jhs, Allocator.Temp);
			var result = JobHandle.CombineDependencies(native_array);
			native_array.Dispose();
			return result;
		}

		public static int GetBatchSize_WorkerThreadCount(int total, int exp = 2)
		{
			float pow_count = pow(JobsUtility.JobWorkerMaximumCount, exp);
			return (int)ceil(total / pow_count);
		}

		public static int GetBatchSize_MaxThreadCount(int total, int exp = 2)
		{
			float pow_count = pow(JobsUtility.MaxJobThreadCount, exp);
			return (int)ceil(total / pow_count);
		}
	}
}