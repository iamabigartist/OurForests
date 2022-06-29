#ifndef VOXEL_MESH_INFO
#define VOXEL_MESH_INFO
#define FACE_SIZE 6
#define FACE_SIZE_D 1/6.0
#define QUAD_VERTEX_SIZE 4
#define QUAD_VERTEX_SIZE_D 1/4.0

struct voxel_color_texture
{
    float3 base_color;
    float smoothness;
    float metallic;
};

StructuredBuffer<float2> vertex_uvs;//4 vertex in a quad
StructuredBuffer<float3> face_normals;//6 face of a cube
StructuredBuffer<float4> face_tangents;//6 face of a cube
StructuredBuffer<voxel_color_texture> quad_color_textures;//all color texture for this shader of all kinds of quads

void GetColorTexture_float(int texture_id, out float3 base_color, out float smoothness, out float metallic)
{
    voxel_color_texture cur_texture = quad_color_textures[texture_id];
    base_color = cur_texture.base_color;
    smoothness = cur_texture.smoothness;
    metallic = cur_texture.metallic;
}

void GetVertexUV_float(int vertex_index, out float2 uv)
{
    uv = vertex_uvs[vertex_index];
}

void GetFaceNormal_float(int face_index, out float3 normal)
{
    normal = face_normals[face_index];
}

void GetFaceTangent_float(int face_index, out float4 tangent)
{
    tangent = face_tangents[face_index];
}

void Decompose(uint source, float remain_size_product_d, uint remain_size_product, out uint result, out uint remain_source)
{
    result = source * remain_size_product_d;
    remain_source = source - result * remain_size_product;
}

void i_face_i_uv_i_texture_Decompose_float(uint i, out uint i_texture, out uint i_face, out uint i_uv)
{
    Decompose(i,FACE_SIZE_D * QUAD_VERTEX_SIZE_D,FACE_SIZE * QUAD_VERTEX_SIZE, i_texture, i);
    Decompose(i,QUAD_VERTEX_SIZE_D,QUAD_VERTEX_SIZE, i_face, i);
    i_uv = i;
}

void i_face_i_texture_Decompose_float(uint i, out uint i_texture, out uint i_face)
{
    Decompose(i,FACE_SIZE_D,FACE_SIZE, i_texture, i);
    i_face = i;
}

void position_Decompose_float(uint composed_position, uint size_x, float size_x_d, uint size_y, float size_y_d, out float3 position)
{
    float x, y, z;
    Decompose(composed_position, size_y_d * size_x_d, size_y * size_x, z, composed_position);
    Decompose(composed_position, size_x_d, size_x, y, composed_position);
    x = composed_position;
    position = float3(x, y, z);
}

#endif
