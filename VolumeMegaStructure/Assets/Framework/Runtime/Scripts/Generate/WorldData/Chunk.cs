using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
namespace VolumeMegaStructure.Generate.WorldData
{
	public class Chunk
	{
		public readonly World world;
		public int3 world_position;

		public NativeArray<ushort> blocks;
		public JobHandle blocks_gen_jh;

		public Mesh mesh;
		public JobHandle mesh_gen_jh;

		public Material material;
		public float4x4 matrix;

		//Use gameobject or manage manually?
		public Chunk(World world, int3 world_position)
		{
			this.world = world;
			this.world_position = world_position;
		}
	}
}