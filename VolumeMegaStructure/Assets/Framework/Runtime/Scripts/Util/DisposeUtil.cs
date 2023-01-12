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
	}
}