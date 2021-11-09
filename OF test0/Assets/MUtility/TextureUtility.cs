using System;
using UnityEngine;
namespace MUtility
{
    public static class TextureUtility
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
                throw new ArgumentException( "Src is not DXT1 texture format" );
            }
            var target_texture = new Texture2D( src_texture.width, src_texture.height,
                TextureFormat.RGBAFloat, src_texture.mipmapCount,
                src_texture.filterMode != FilterMode.Point );
            target_texture.SetPixels( src_texture.GetPixels() );
            target_texture.Apply();
            target_texture.Compress( true );
            return target_texture;
        }
    }
}
