using Newtonsoft.Json;
using PrototypePackages.PrototypeUtils;
using UnityEditor;
using UnityEngine;
using static Unity.Mathematics.math;
namespace Labs.Lab0_Misc.Editor
{
	/// <summary>
	///     结论：改变shader参数，并不会影响此shader之前的还未结束的dispatch。假设？每个dispatch选取当前所有的设置打包变为一个GPU状态。
	/// </summary>
	public class TestMultipleDispatch : EditorWindow
	{
		[MenuItem("Labs/Labs.Lab0_Misc.Editor/TestMultipleDispatch")]
		static void ShowWindow()
		{
			var window = GetWindow<TestMultipleDispatch>();
			window.titleContent = new("TestMultipleDispatch");
			window.Show();
		}

		ComputeShader cs;
		ComputeBuffer buffer;
		int dispatch_count;
		int array_len;
		int[] array;

		void OnEnable()
		{
			dispatch_count = (int)pow(2, 3);
			array_len = (int)pow(2, 18);
			array = new int[array_len];

			cs = Resources.Load<ComputeShader>("AssignConstantValue");
			buffer = new(array_len, sizeof(int));
			int dispatch_stride = array_len / dispatch_count;
			cs.SetInt("len", dispatch_stride);
			cs.SetBuffer(0, "buffer", buffer);
			for (int i = 0; i < dispatch_count; i++)
			{
				int cur_start = i * dispatch_stride;
				cs.SetInt("start", cur_start);
				cs.SetInt("value", i);
				cs.Dispatch(0, dispatch_stride, 1, 1);
			}
			buffer.GetData(array);
			Debug.Log(array[..1000].JoinString(","));
			buffer.Release();

			int i_array = 0;
			for (int i_dispatch = 0; i_dispatch < dispatch_count; i_dispatch++)
			{
				int cur_correct_value = i_dispatch;
				for (int i_stride = 0; i_stride < dispatch_stride; i_stride++)
				{
					var cur_value = array[i_array++];
					if (cur_value != cur_correct_value)
					{
						Debug.Log(JsonConvert.SerializeObject(new
						{
							i_dispatch,
							i_stride,
							i_array,
							cur_value
						}, Formatting.Indented));
						break;
					}
				}
			}
		}
	}
}