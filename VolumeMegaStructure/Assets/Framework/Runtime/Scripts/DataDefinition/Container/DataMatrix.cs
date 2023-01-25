using System;
using Unity.Collections;
using Unity.Mathematics;
using VolumeMegaStructure.Util;
namespace VolumeMegaStructure.DataDefinition.Container
{
	/// <summary>
	///     A 3D matrix which represents particles align to uniform grid in a rectangular.
	/// </summary>
	/// <typeparam name="TData">It should be A Serializable Type.</typeparam>
	/// <remarks>The core data type of <see cref="VolumeMegaStructure" /></remarks>
	[Serializable]
	public struct DataMatrix<TData> : IDisposable
		where TData : struct
	{

	#region Data

		public int3 size;
		public NativeArray<TData> data;

	#endregion

	#region Property

		public int Count => size.x * size.y * size.z;

		public int3 CenterPoint => size / 2;

	#endregion

	#region Factory

		public DataMatrix(int3 size, NativeArray<TData> data)
		{
			this.size = size;
			this.data = data;
		}

		public DataMatrix(int3 size, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
		{
			this.size = size;
			data = new(this.size.volume(), allocator, options);
		}

		public void Dispose() { data.Dispose(); }

	#endregion

	#region Indexer

		public TData this[int x, int y, int z]
		{
			get => data[x + y * size.x + z * size.y * size.x];
			set => data[x + y * size.x + z * size.y * size.x] = value;
		}

		public TData this[int3 idx]
		{
			get => data[idx.x + idx.y * size.x + idx.z * size.y * size.x];
			set => data[idx.x + idx.y * size.x + idx.z * size.y * size.x] = value;
		}

		public TData this[int i]
		{
			get => data[i];
			set => data[i] = value;
		}

	#endregion

	#region Util

		public int IndexByPosition(int x, int y, int z)
		{
			return x + y * size.x + z * size.y * size.x;
		}

		public int3 PositionByIndex(int i)
		{
			int z = i / (size.x * size.y);
			i -= z * size.x * size.y;
			int y = i / size.x;
			i -= y * size.x;
			int x = i;

			return new(x, y, z);
		}

		public void PositionByIndex(int i, out int x, out int y, out int z)
		{
			z = i / (size.x * size.y);
			i -= z * size.x * size.y;
			y = i / size.x;
			i -= y * size.x;
			x = i;
		}

		public bool IsPositiveEdge(int x, int y, int z)
		{
			return
				x == size.x - 1 ||
				y == size.y - 1 ||
				z == size.z - 1;
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

	#endregion

	}
}