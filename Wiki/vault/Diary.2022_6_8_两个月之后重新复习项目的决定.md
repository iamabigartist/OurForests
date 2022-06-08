---
id: 9cjtxj3ueh45c0b7il5u4l4
title: 2022_6_8_两个月之后重新复习项目的决定
desc: ''
updated: 1654702056160
created: 1654701599788
---

适用纯色体素来正确描述巨构，旋转等物体信息应该sparse的放到对象中去（即使是树木也可以高效）。

NativeArray也可以用struct去封装，job中的readonly特性可以放在非native的结构上，使得两个job可以对于只读结构同时运行！

因此应该适用结构对于nativearray进行封装来达到3D数组访问目的，而不是indexer.

并且global配置，也可以适用struct封装所有native array的指针然后copy出一个新配置给每个job.
