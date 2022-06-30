---
id: tseui9stc9wuj2jf9rgytj4
title: 2022_6_30_做完Voxel生成对于后续工作概括
desc: ''
updated: 1656596387136
created: 1656590694157
---

哎呀VoxelGen终于搞完了，就这个玩意本身搞了整整一年了。

后面的东西会更加简单，重要的是如何设计数据结构和使用方式，是对于实际使用所需要的功能的实现。

目前离可以展开实际游戏开发还是有一段距离。

主要是几个部分：

1. 对于世界内所有体素的管理，？？？Manager，负责处理对于世界体素的增删查改。
   1. 这个管理器内部要包含所有的chunks。
   2. 目标世界坐标和chunk的世界坐标可以得出chunk的局部坐标。
   3. 把volume修改的通知发给其他manager。
2. 对于所有chunk初始化渲染的整体安排管理器，对于初始化不同阶段的unity job和compute shader job进行统一安排执行。
   1. 在安排的任务计算量已经足够大的时候，进行之后的初始化（或者这一步计算比较少，那就常态化），并同时batchScheduledJobs()，最后wait for complete.
3. voxel chunk这个数据结构应该与渲染分离开来。
   1. 单独地使用ChunkMeshManager进行chunk与voxel mesh的耦合。
      1. ChunkMeshManager还是一个单例，负责把所有的对于所有chunk的volume的修改apply到对应的voxelMesh上面。
      2. 这个改Mesh的流程是一个技术核心，使用CPU check需要更改的voxel，重新查看voxel周围对应的新的quad情况，然后对于quad_unit_array进行分析，决定新的quad应该放到哪里，最终将信息传给compute shader来执行写入GPU内存。
      3. 三种修改volume的API，修改单个，修改提供一个稀疏矩阵，修改提供一个稠密矩阵。
   2. 使用MonoChunkRenderer将ChunkMeshManager的内容实际绑定到GameObject的渲染流程中。
4. ColorTexture 需要一个方便的编辑器。
5. 对于volume需要一整套的编辑和保存系统。
   1. 首先在所有功能之上，使用鼠标进行raycast获取所选volume信息的功能。
   2. 框选并查看选中的volume是什么的功能。
   3. 手动add,remove voxel的能力。
   4. 保存一个区域内的volume成稀疏和稠密矩阵的能力。
   5. 选取场景中的一些volume然后保存成预设的能力。
   6. 使用sparse表示chunk，使用dense表示chunk内数据。
6. 是否要使用ushort代volume unit呢，世界中的复杂数据在这一部分上是否可以全部转化成ushort保存？
7. volume生成
   1. 需要job来处理这部分，不然会非常慢
   2. 但是又需要灵活性很强，所以很有可能也使用管理器管理生成各个部分。
   3. 在实际生成一个世界这个过程中，
      1. 不可能把所有chunk的volume数据保存下来，因为首先世界接近无限大，其次chunk本身占空间大。
      2. 那么对于world中的每个chunk，就需要有能力只根据world的生成设置来得到这个chunk将要进行的全部生成过程，只可能会保存一些小型但是难以多次计算的生成特性。
      3. chunk的生成过程在每次触发要加载chunk的时候进行。
   4. 将noise使用一个固定高度范围，广度范围变为一个指定block的data matrix。这是地形生成的基础操作单元。
   5. 如何使用job去处理texture.GetPixelData<>()，以及和外部引擎相连（调用dll?）。
   6. 一些基于已有volume_matrix的地形修改器，比如赋雪，赋草，洞穴，侵蚀，花草树木等。

至少要做出volume prefab，编辑，chunkmesh，world功能，才有可能较为轻松的使用timeline来展示demo给工作室。
