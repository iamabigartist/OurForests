---
id: g33ly6zmgrorjvg0193njk7
title: identity
desc: ''
updated: 1646881974944
created: 1646880750992
---

## 需求

1. block实体配置，按照block类型划分
   1. voxel
      1. texture id, 旋转信息
      2. 生成组（voxel即使再同一个chunk，但是可以在不同组内生成）
         1. 透明组（不同透明组之间的边界需要生成，因为能够看到）
         2. 细分volume，远看是正常volume，近看被细分
   2. 普通mesh
      1. mesh id, 旋转矩阵
      2. 对应材质
2. 实体voxel  
每个block对应一个id，可以实时生成也可以固定，比如用名字。id 存储在volume matrix中。
