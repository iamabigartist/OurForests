using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
namespace Labs.Lab0_Misc.Editor
{
	public struct CreateNativeContainerJob : IJob
	{

		public int length;
		public UnsafeStream.Writer writer;
		public void Execute()
		{
			writer.BeginForEachIndex(0);
			writer.Write(new NativeArray<int>(length, Allocator.Temp));
			writer.EndForEachIndex();
		}

		public CreateNativeContainerJob(int length, out UnsafeStream stream)
		{
			stream = new(1, Allocator.TempJob);
			this.length = length;
			writer = stream.AsWriter();
		}

		public static void Test()
		{
			new CreateNativeContainerJob(100, out var stream).Schedule().Complete();
			var reader = stream.AsReader();
			reader.BeginForEachIndex(0);
			var array = reader.Read<NativeArray<int>>();
			reader.EndForEachIndex();
			Debug.Log(array.Length);
		}
	}

	public struct RefNativeArrayJob : IJob
	{
		NativeReference<NativeArray<int>> a;
		UnsafeReference<NativeArray<int>> array_ref;
		public void Execute()
		{
			var array = a.Value;
			Debug.Log("len: " + a.Value);
			array[0] = 1;
		}
		public RefNativeArrayJob(int length, out NativeArray<int> array)
		{
			array = new(length, Allocator.TempJob);
			array_ref = new(array);
			a = new(array, Allocator.TempJob);
		}

		public static void Test()
		{
			new RefNativeArrayJob(10, out var array).Schedule().Complete();
			Debug.Log("0: " + array[0]);

		}

	}

	public class TestNoBreakJob : EditorWindow
	{
		[MenuItem("Labs/Labs.Lab0_Misc.Editor/TestNoBreakJob")]
		static void ShowWindow()
		{
			var window = GetWindow<TestNoBreakJob>();
			window.titleContent = new("TestNoBreakJob");
			window.Show();
		}

		void OnEnable()
		{
			RefNativeArrayJob.Test();
		}

		void OnGUI() {}
	}
}