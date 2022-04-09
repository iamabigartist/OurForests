---
id: hb7707td21smrl7oiof5ptu
title: 2022_4_9_要实验的三种方式
desc: ''
updated: 1649517872476
created: 1649481682996
---

## 想要测试的三种思路

1. 用DrawMeshInstance来渲染，可以做每个方块自己的渲染信息，包括哪个面朝向自己，以及哪个面在外面，对于每个方块的设置比较灵活。底层原理就是把这些所有mesh batch成一个draw call。
2. 使用每个方块作为一个Entity来渲染（就是类似与gameobject加上mesh renderer）,需要学习ECS 的hybrid renderer. 但是实际上最后的目的还是收集所有需要的信息然后形成batch，触发draw call。比起1更好用的在于，renderer自己会处理模型的先后信息，视锥等等。
3. 还是之前说的，用生成完整mesh来渲染。这种情况下GPU有自己的一系列剔除操作，包括深度和背面剔除，是可以自动进行的。

## DrawMeshInstance的延续

### 如何进行cube mesh的静态背面剔除

首先如果要自行剔除背面，那么只要把响应面的法线和实现做点乘然后剔除点乘值小于等于0的即可！点乘本身代表一个面与自己视线前方的垂直平面的投影有多少，如果点乘等于0 那么就说明面片在平面上的长度为0 就看不到。如果小于0则说明，面片在平面上的投影是自己的背面，因此也不需要渲染。本质上说明，大量立方体的6个面的法向量是固定的！

### 关于graphics buffer, native array 和 mesh的使用

graphic buffer 使用和compute buffer类似。 自己手动创建的graphic buffer 可以调用命令然后当作 native array 使用，效率搞。 但是目前mesh自己的vertex buffer 没有变成 native array 的方法。

### 之前写的很多很宝贵的实验都忘记了

之前其实试验过可以使用uv作为vertex buffer作为shader graph的输入来渲染三角形的！！这直接解决了昨天重新复习时候遇到的打到问题。

## 目前已经解决的四个大的难题：要记住

1. 使用uv 作为vertex buffer的param特性是可以传入到shader graph 里面然后作为顶点渲染的。
2. mesh的vertex buffer 可以临时声明作为structure buffer传入compute shader 进行修改操作，但是要记住用完之后要及时Release()，不然下次mesh返回的buffer还是原来旧的buffer。这个操作加上声明mesh的graphic buffer性质，可以直接在CPU上声明GPU内存，而不需要进行数据的传输。
3. 关于如何进行各种所需要的culling。
   1. 首先volume matrix的一个很大的优化就在于它的物体很规整，在很多条件下都可以被人为剔除掉。
   2. 之前想过用submesh 进行剔除，但是submesh还是只能指定一个固定范围而不能指定多个区域。
   3. 因此现在想到使用visit_index来进行特定顶点的访问来支持custom culling这个机制，也就是说不访问哪个vertex，就相当于不渲染，cull 掉了哪个vertex.
4. 有关于如何用raycast对于matrix进行culling：
   1. 环境是compute shader（或者job system 也行但是需要CPU到GPU的bus够大）
   2. 每个线程自己分配一条光线
   3. 这个线程对于这个方向上的所有volume从近处到远处进行遍历，直到找到需要渲染的面片。
   4. 找到所需要面片位置之后，通过字典将面片作为key转化为对应visit_index
   5. 这步最重要，我们把visit_index组成一个新的mesh索引数组，而不是重新构建一个mesh，来进行cull的渲染。
   6. 这样子一个过程，能够让ray cast culling 这个过程直接和原始的mesh 渲染兼容，不用重新修改mesh，因为只需要改变索引数组即可！！

## 关于DrawInstance

也许使用DrawInstance在每一个chunk上面不太合适，有各种各样性能和性质的问题。但是不要忘记最后是要draw 多个chunk的！！这个时候就必须要选择使用作为对象来存储和渲染chunks，使用gameobject 或者entity最后还是要合批！
但是这里的问题是每个mesh 都是不一样的，只是使用的材质是一样的，而非像draw instance 那样只有一些性质不同。
所以最后大概率是选择Entity或者gameobject来进行多chunk渲染。

## 关于数据压缩的精准度丢失问题

这个问题要分两种情况来看，

- 首先有一部分是非常繁杂而单体数据范围小，这类数据往往是用来生成更复杂信息的整数指示，他们需要保证精准度来生成正确的复杂信息。但是呢他们的数据范围很小，因此根本不会遇到float的精准度不够的问题。
- 但是另外一部分是比如color（color其实也可以压缩，我去，但是因为格式太多了，所以会很麻烦，color自带压缩其实）和position，这类数据的数据范围很大，因此会遇到浮点数不精准的问题。
  - 但是首先他们本身就是末端显示数据，因此数据误差在没有被多重计算放大的情况下其实是无所谓的。
  - 而且另一方面，就是位置信息本身，如果不能够表示大数据量的位置，那么对于渲染的限制也太大了。只有在位置特别大的情况下，unity才会显示报错，或者渲染错误。
  - 经过测试，1^6以内的数据float能够完全精准的解压缩，因此用来做100*100*100的chunk是没有问题的。
  - 总之虽然通常不需要考虑精准问题，但是这类数据压缩后的值还是不能够超过官方文档所给的精准范围。
