using UnityEngine;
namespace PrototypeZ.Tests
{
    public class ShaderData<T>
    {
        int id;
        T arg;
    }
    public class VoxelGenerator : MonoBehaviour
    {
        ComputeShader cs;
        (
            ShaderData<Vector3Int> volume_number_size,
            ShaderData<Vector3Int> cube_number_size,
            ShaderData<float> threshold,
            ShaderData<ComputeBuffer> volume_matrix,
            ShaderData<float> grid_size,
            ShaderData<ComputeBuffer> quads,
            ShaderData<ComputeBuffer> quad_centers
            ) cs_data;

    }
}
