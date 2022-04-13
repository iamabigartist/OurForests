---
id: voc1ymm6l57jx2ow5mtifot
title: process
desc: ''
updated: 1649682350599
created: 1649682198351
---


## 生成voxel的生成辅助数据

之前已经实现，详见 [GenVoxelSourceTable.cs](../../VolumeMegaStructure/Assets/Framework/Runtime/Scripts/Generate/ProceduralMesh/Voxel/GenVoxelSourceTables.cs)

## 生成mesh

CPU 对于volume matrix进行检测，列出需要生成的面列表，面本身数据是静态的。留下面片位置与在面列表种索引的映射字典，将面片数据传给compute shader.
ComputeShader 根据面片数据生成面片对应的四个点对应的顶点数据。

## 渲染mesh

使用 [上述数据结构](#现在由已经实现的技术预测的到渲染的数据结构-2022410-118) 编写独立的vertex shader, 这个vertex shader通过固定的输出数据结构可以对接到不同的fragment shader上实现渲染模式的改变。

## 修改mesh

在CPU确定需要修改的面片列表，然后确认相应的数据库操作。包括删除，覆盖 vertex buffer，裁剪index buffer长度等操作。
将这些操作传给compute buffer 进行实际执行，以实现不需要重新传输整个数组就能够修改mesh的操作。
