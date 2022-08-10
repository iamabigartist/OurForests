---
id: egf47cbjygn6fjc3bmuipf4
title: 2022_8_8_在情况稳定之后开始做后面的各种系统
desc: ''
updated: 1659966495165
created: 1659923923247
---

## JobScheduler

由于目前很多job都是无法一次串联完成，而是需要使用complete之后的信息来进行后续schedule，因此：

1. 要么使用isComplete不断检测，complete之后再进行下次schedule，但是这样的坏处在于两个schedule之间可能有空挡，导致worker反复开关。
2. 要么每帧只执行某个整体任务的其中一个complete阶段，并且很多相同任务一起schedule。
3. 或者两者结合起来，是很多任务同时进行，但是每个任务自己检测自己的完成情况，根据完成情况，通过携程来继续下一步。