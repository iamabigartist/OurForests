#ifndef VOXEL_MESH_INFO
#define VOXEL_MESH_INFO
StructuredBuffer<float2> vertex_uvs;//4 vertex in a quad
StructuredBuffer<float3> face_normals;//6 face of a cube
StructuredBuffer<float4> face_tangents;//6 face of a cube
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
#endif
