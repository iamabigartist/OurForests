using System;
using System.Collections.Generic;
namespace VolumeMegaStructure.Util
{
	public static class DisposeUtil
	{
		public static void DisposeAll(params IDisposable[] Disposables)
		{
			foreach (IDisposable disposable in Disposables)
			{
				disposable?.Dispose();
			}
		}

		public static void DisposeArray<T>(IEnumerable<T> Disposables)
			where T : IDisposable
		{
			foreach (T disposable in Disposables)
			{
				disposable?.Dispose();
			}
		}
	}
}