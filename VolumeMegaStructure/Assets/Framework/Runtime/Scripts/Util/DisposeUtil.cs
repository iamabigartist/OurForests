using System;
namespace VolumeMegaStructure.Util
{
	public static class DisposeUtil
	{
		public static void DisposeAll(params IDisposable[] Disposables)
		{
			foreach (IDisposable disposable in Disposables)
			{
				disposable.Dispose();
			}
		}

		public static void DisposeAll<T>(params T[] Disposables)
			where T : IDisposable
		{
			foreach (T disposable in Disposables)
			{
				disposable.Dispose();
			}
		}
	}
}