using UnityEngine;
using VolumeMegaStructure.DataDefinition.DataUnit;
using static VolumeMegaStructure.Manage.MainManager;
namespace VolumeMegaStructure.Config
{
	[CreateAssetMenu(fileName = "Block1", menuName = "", order = 0)]
	public class BlockConfig : ScriptableObject
	{
		public Texture2D texture_right;
		public Texture2D texture_left;
		public Texture2D texture_up;
		public Texture2D texture_down;
		public Texture2D texture_forward;
		public Texture2D texture_back;

		public VoxelBlockTexturePack GetTexturePack()
		{
			var get_texture_id = id_manager.voxel_texture_id;
			int right_id = get_texture_id[texture_right];
			int left_id = get_texture_id[texture_left];
			int up_id = get_texture_id[texture_up];
			int down_id = get_texture_id[texture_down];
			int forward_id = get_texture_id[texture_forward];
			int back_id = get_texture_id[texture_back];
			return new(right_id, left_id, up_id, down_id, forward_id, back_id);
		}
	}
}