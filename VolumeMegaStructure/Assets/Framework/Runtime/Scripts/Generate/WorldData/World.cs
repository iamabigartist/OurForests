using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.Rendering;
namespace VolumeMegaStructure.Generate.WorldData
{
	public class World
	{
		public readonly int3 chunk_size;
		Dictionary<int3, Chunk> chunk_dict;

		public Chunk this[int3 position]
		{
			get => chunk_dict[position];
		}

		public void RenderAllChunk()
		{
			//is it ok to use DrawMesh for every chunk?
			//How to use command buffer
			var command_buffer = new CommandBuffer();
		}
	}
}