using Unity.Jobs;
namespace VolumeMegaStructure.Util.JobSystem
{
	public interface IForPlan
	{
		int length { get; }
		int batch { get; }

		public static JobHandle Plan<T>(T job, JobHandle deps = default)
			where T : struct, IForPlan, IJobFor
		{
			return job.ScheduleParallel(job.length, job.batch, deps);
		}
	}
}