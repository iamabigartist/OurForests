using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
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

		public static int GetBatchSize_MaxThreadCount(int count)
		{
			return count / JobsUtility.MaxJobThreadCount;
		}
	}
}