---
id: jw9yf35b87zifrzj2wz4y9t
title: data definition
desc: ''
updated: 1649834426547
created: 1649682065893
---

## Config

### Voxel Texture

一张抽象的texture在一个固定的管线和shader中，由多个子效果贴图组成，必要的是颜色贴图。

```yaml
VoxelTexture:
 file_name: unique_string
 id: auto_gen int
 Albedo: Texture2D
 Normal: Texture2D
 ...
```

### Block Render Config

```yaml
BlockRenderConfig:
 RenderingMethod: Voxel|MarchingCube|Mesh
 material_variants:
  color_variants: object
  ...
```

```yaml
VoxelRenderConfig:
 FaceTextures: VoxelTexture[6]
```

## Table

### BlockTable

```yaml
Block_table: Block
```

#### Block

一个block仅代表着一种独立的方块。因此采取一个表格来表示。

```yaml
Block:
 name: unique string
 id: auto_gen int
```

### Voxel Render Buffer Groups

```yaml
VoxelRenderBuffersGroups:
 VerticesBuffers: Buffers
 Textures: Buffers
```

#### PerVertex Buffers

```yaml
PerVertexBuffers: #用来获取Vertex Shader的输出
 UV: float2[4]
 Normal: float3[6]
 Tangent: float4[6]
```

#### Textures

```yaml
Textures:
 Albedo: Texture2D[texture_numbers]
```

## Interface

### Matrix Scan Output - Compute Shader MeshGen Input

```yaml
Quad:
 #这里是quad中心点或者offset position
 #（取决于compute shader 如何生成）
 #为了省bus依旧是压缩的。
 Position: float
 Face_index_Texture_identity: float
```

#### Face_index_Texture_identity

通过Check是否生成得到的quad中

- x/y/z和正负方向的check代表了它的face_index。
- 通过反向旋转目前的面方向，得到贴图所在的面方向。
- 通过访问要生成面片的block config，获取texture_id。

##### 2022/4/11 关于texture_id获取要注意

需要通过当前quad所属于cube的旋转信息以及quad本身的朝向信息，获取其原方向信息，才能再带入到block_texture_config中，拿到正确朝向的texture_id.

#### Position

quad自己的位置index。

### Block Config Output

输入一个反向旋转后的面方向，输出texture_id。

### MeshGen Output - Vertex Shader Input

```yaml
MeshBuffers:
 VertexBuffer: Vertex[QuadCount*4]
 IndexBuffer: int[QuadCount*6]
```

#### Vertex Buffer

```yaml
Vertex: #作为vertex shader的输入
 Position: float #三维压缩
 Face_uv_indices_Texture_identity: float
```

##### Vertex Position

通过旋转，面朝向和面片点信息来从Structure Buffer中获取顶点的position，压缩得到float。

##### 2022/4/11 对于之前规定的更改以及决定

1. 由于认为贴图数量会不足8k，因此决定将贴图id和face_uv_indices放在一起。
2. 另外由于SV_VertexId不确定是否可用，还是选择自己生成uv_index。

#### Index Buffer

如果不进行cull，基础的访问索引就是012231加上offset的重复。
