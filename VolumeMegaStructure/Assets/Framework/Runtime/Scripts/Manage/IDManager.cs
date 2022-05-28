using System.Collections.Generic;
using UnityEngine;
using VolumeMegaStructure.Util;
namespace VolumeMegaStructure.Manage
{
	public class IDManager
	{
		public BiDictionary<Texture2D, int> voxel_texture_id = new();
	}
}