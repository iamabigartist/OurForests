using System;
using Unity.Collections;
using Unity.Mathematics;
namespace VolumeMegaStructure.Util
{
	public struct NativeArray3D<T> : IDisposable
		where T : struct
	{
		public int3 size;
		public NativeArray<T> array_1d;
		public NativeArray3D(int3 size, NativeArray<T> array_1d)
		{
			this.size = size;
			this.array_1d = array_1d;
		}
		public void Dispose() { array_1d.Dispose(); }


		public T this[int x, int y, int z] => array_1d[x + y * size.x + z * size.y * size.x];
		public T this[int3 i3] => array_1d[i3.x + i3.y * size.x + i3.z * size.y * size.x];

		public int3 GetPosition(int i)
		{
			int z = i / (size.x * size.y);
			i -= z * size.x * size.y;
			int y = i / size.x;
			i -= y * size.x;
			int x = i;

			return new(x, y, z);
		}

		public bool IsEdge(int x, int y, int z)
		{
			return
				x == 0 || x == size.x - 1 ||
				y == 0 || y == size.y - 1 ||
				z == 0 || z == size.z - 1;
		}

		public bool OutOfRange(int x, int y, int z)
		{
			return
				x < 0 || x > size.x - 1 ||
				y < 0 || y > size.y - 1 ||
				z < 0 || z > size.z - 1;
		}

	}
}