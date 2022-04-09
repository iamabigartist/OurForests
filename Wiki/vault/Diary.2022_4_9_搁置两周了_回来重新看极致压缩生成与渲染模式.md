---
id: xphpr0kujqgi326brj70gqs
title: 2022_4_9_搁置两周了_回来重新看极致压缩生成与渲染模式
desc: ''
updated: 1649480681665
created: 1649434825217
---
12点了才刚刚把之前的思路和已经到达的地步理清了一些。

1. 之前由于默认不能在shader graph里面的vertex shader中设置uv输出，因此想着加一个自定义的插值器。但是uv channel在fragment和vertex下的输出真的一致吗，会不会是一个插值了一个没插值呢？这需要实验证明。
2. uv既然能够作为shader graph的输入，那么必然证明它是mesh下面的一个数组！那么说明在使用uv channel的时候，使用的就是未插值的per vertex uv。而在离开vertex shader 之前，uv也不可能被插值。
3. 使用structure buffer 当作mesh 传入信息会非常灵活，因为结构自定，可以使用custom node 传入graph。但是怎么获取每顶点信息是一个问题，shader graph 不提供吗？ VertexId 到底是不是？
4. 使用VBO 传入的问题在于shader graph没办法读取其他格式的比如int16的position，可能需要强制的转换（我指的是把一小块内存的byte从一种数据看成另一种数据）。
5. 在模型生成这块，如果模型本身数据量足够小的话，那么可能没有必要把赋值的步骤转到GPU去做了。而是完全由Job system去生成模型。
6. 压缩数据问题，包括面方向，旋转方向，面片点索引，和贴图id。
7. 是否使用贴图还是纯色，这绝对是一个非常关键的问题，如果没有贴图，那么就省大麻烦了。而且渲染会更快速。但是效果绝对没法和贴图去比。
