using System.Collections.Generic;
using Unity.Jobs;
namespace VolumeMegaStructure.Util.JobSystem
{
	//比较难写，因为需要自己整理那个schedule的关系，如果一个job schedule 的时候，他的dependency还没有schedule，那么他就无法schedule了
	//然而这个可以用开发者自己来保证，因为开发者至少可以自己控制他的依赖关系，通过list.Add(job)来控制schedule的先后
	public class Scheduler
	{
		public delegate JobHandle ScheduleDelegate(JobHandle dependency);
		Dictionary<string, ScheduleDelegate> planned_schedules = new();
		// public JobHandle ScheduleAll(string name, JobHandle dependency)
		// {
		// 	if (planned_schedules.ContainsKey(name))
		// 	{
		// 		return planned_schedules[name](dependency);
		// 	}
		// }
	}
}