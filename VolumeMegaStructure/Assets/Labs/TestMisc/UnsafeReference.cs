using Unity.Collections.LowLevel.Unsafe;
namespace Labs.TestMisc
{
	public unsafe struct UnsafeReference<T> where T : unmanaged
	{
		[NativeDisableUnsafePtrRestriction]
		void* Data;
		public T value => *(T*)Data;
		public UnsafeReference(T value)
		{
			Data = default;
			// *(T*)Data = value;
		}

	}
}