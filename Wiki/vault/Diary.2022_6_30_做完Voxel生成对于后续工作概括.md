---
id: tseui9stc9wuj2jf9rgytj4
title: 2022_6_30_做完Voxel生成对于后续工作概括
desc: ''
updated: 1656592094272
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
   2. 使用MonoChunkRenderer将ChunkMeshManager的内容实际绑定到GameObject的渲染流程中。
4. ColorTexture 需要一个方便的编辑器。
