using Unity.Collections;
using static Unity.Collections.LowLevel.Unsafe.UnsafeUtility;
namespace VolumeMegaStructure.Util.Collection
{
	public static unsafe class MemUtil
	{
		public static T* New<T>(T value, Allocator allocator)
			where T : unmanaged
		{
			var size = SizeOf<T>();
			var p = Malloc(size, AlignOf<T>(), Allocator.TempJob);
			MemCpy(p, &value, size);
			return (T*)p;
		}

		public static void Dispose<T>(T* p, Allocator allocator)
			where T : unmanaged
		{
			Free(p, allocator);
		}
	}
}