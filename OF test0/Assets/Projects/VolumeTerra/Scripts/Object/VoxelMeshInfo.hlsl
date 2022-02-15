#ifndef VOXEL_MESH_INFO
#define VOXEL_MESH_INFO
StructuredBuffer<float> volume;

void MyFunctionA_float(int i,out float v)
{
    v=volume[i];
}
#endif 