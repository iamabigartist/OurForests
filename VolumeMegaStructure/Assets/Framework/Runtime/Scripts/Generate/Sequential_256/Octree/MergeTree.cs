using Unity.Collections;
using Unity.Jobs;
using VolumeMegaStructure.Util.JobSystem;
namespace VolumeMegaStructure.Generate.Sequential_256.Octree
{
	public struct VolumeUnit
	{
		public ushort id;
		public byte size_log2;
	}
	public struct MergeTree : IJob
	{
		NativeHashMap<byte4, VolumeUnit> chunk;
		public void Execute()
		{
			
		}
	}
}