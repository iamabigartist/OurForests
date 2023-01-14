using Unity.Jobs.LowLevel.Unsafe;
namespace VolumeMegaStructure.Util.JobSystem
{
	public static class ScheduleUtils
	{
		public static int GetBatchSize_MaxThreadCount(int count)
		{
			return count / JobsUtility.MaxJobThreadCount;
		}
	}
}