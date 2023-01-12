using Unity.Jobs.LowLevel.Unsafe;
namespace VolumeMegaStructure.Util.JobUtils
{
	public static class Utils
	{
		public static int GetBatchSize_MaxThreadCount(int count)
		{
			return count / JobsUtility.MaxJobThreadCount;
		}
	}
}