using UnityEngine;
namespace VolumeMegaStructure.Generate.Mesh
{
    public class VoxelSourceMesh
    {
        void A()
        {
            var m = new UnityEngine.Mesh();
            var gb= m.GetVertexBuffer( 0 );
            var aa = Resources.Load<ComputeShader>("");
            // aa.SetVectorArray();
        }
    }
}
