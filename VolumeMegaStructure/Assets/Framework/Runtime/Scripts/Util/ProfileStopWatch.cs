using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
namespace VolumeMegaStructure.Util
{
	public class ProfileRecord
	{
		public DateTime start_time;
		public long ticks;

		public static ProfileRecord CreateRecord()
		{
			ProfileRecord record = new ProfileRecord
			{
				start_time = DateTime.Now
			};
			return record;
		}
	}
	public class ProfileStopWatch
	{
		public Dictionary<string, ProfileRecord> record_dict;
		string cur_record_name;
		Stopwatch stopwatch;
		public ProfileStopWatch()
		{
			record_dict = new();
			stopwatch = new();
		}
		public void Start(string record_name)
		{
			cur_record_name = record_name;
			stopwatch.Restart();
			record_dict[cur_record_name] = ProfileRecord.CreateRecord();
		}
		public void Stop()
		{
			stopwatch.Stop();
			record_dict[cur_record_name].ticks = stopwatch.ElapsedTicks;
		}

		public string PrintAllRecords()
		{
			var records_object = record_dict.Select(pair =>
			{
				return new
				{
					name = pair.Key,
					start_time = pair.Value.start_time.Ticks.Get_ms(),
					ms = pair.Value.ticks.Get_ms()
				};
			});
			string str = JsonConvert.SerializeObject(records_object, Formatting.Indented);
			return str;
		}

		public void Clear()
		{
			cur_record_name = default;
			record_dict.Clear();
			stopwatch.Reset();
		}
	}
}