using System;
using UnityEngine;
namespace VolumeMegaStructure.Util
{
	public static class TextureUtil
	{
		public static Texture2D CPU_DXT1ToDXT5(
			this Texture2D src_texture)
		{
			if (src_texture.format == TextureFormat.DXT5)
			{
				return src_texture;
			}
			if (src_texture.format != TextureFormat.DXT1)
			{
				throw new ArgumentException("Src is not DXT1 texture format");
			}
			var target_texture = new Texture2D(src_texture.width, src_texture.height,
				TextureFormat.RGBAFloat, src_texture.mipmapCount,
				src_texture.filterMode != FilterMode.Point);
			target_texture.SetPixels(src_texture.GetPixels());
			target_texture.Apply();
			target_texture.Compress(true);
			return target_texture;
		}

		public static Texture2DArray GenTexture2DArray(this Texture2D[] array, bool linear)
		{
			var info_texture = array[0];
			var texture_2d_array = new Texture2DArray(info_texture.width, info_texture.height, array.Length, info_texture.format, info_texture.mipmapCount, linear);
			for (int i = 0; i < array.Length; i++)
			{
				Graphics.CopyTexture(array[i],
					0, 0, texture_2d_array, i, 0);
			}
			return texture_2d_array;
		}
	}
}