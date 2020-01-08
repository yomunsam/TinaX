# TinaX Framework
“开箱即用”的Unity独立游戏开发工具 | Unity-based Game Client Framework

[![LICENSE](https://img.shields.io/badge/license-NPL%20(The%20996%20Prohibited%20License)-blue.svg)](https://github.com/996icu/996.ICU/blob/master/LICENSE)
<a href="https://996.icu"><img src="https://img.shields.io/badge/link-996.icu-red.svg" alt="996.icu"></a>
[![LICENSE](https://camo.githubusercontent.com/3867ce531c10be1c59fae9642d8feca417d39b58/68747470733a2f2f696d672e736869656c64732e696f2f6769746875622f6c6963656e73652f636f6f6b6965592f596561726e696e672e737667)](https://github.com/yomunsam/TinaX/blob/master/LICENSE)

> 为美好的引擎献上Framework ！

TinaX Framework 开源版

------

## 概述

TinaX Framework 是一个基于Unity引擎的游戏开发框架，旨在为中小型游戏提供一个稳定、相对完善的开箱即用的解决方案。

完整文档请访问：[tinax.corala.space](https://tinax.corala.space)

### 预览版本

当前`master`分支版本为`6.4.x`，这个版本已经被用于我们的多个生产项目，基本稳定。

但是由于剥离了一些我们团队内部的工具，*如基于内部项目管理系统（其实就是禅道）的补丁打包工具、关联到内部服务器的热更新推送、机型性能查询等服务* ，6.4.x的部分功能显得过于孱弱。

于是，我们开始对Framework的部分内容进行重构，重构部分暂时不定期会更新到[预览分支](https://github.com/yomunsam/TinaX/tree/tinax_6.5)

预览分支将在相对稳定之后并入master分支，期间的新功能也会放在预览分支中，master分支不出意外的话，仅进行bug修复相关的更新。

截至2020年1月，预览分支已基本趋于稳定，建议尝试。（但要注意的是，新版的热更新**补丁**的打包工具我们从框架里移除了，开发者可以在业务逻辑中根据VFS的结构自行编写符合各自项目需求的热更新方案，补丁覆盖在对应位置之后VFS自己会去识别加载的。目前我们也正在探索尝试更加易用高效且大众化的资源解决方案。）

## 环境要求

### Unity 版本 

TinaX 最低兼容 `Unity 2018.3` 以上版本，由于TinaX基于新的Unity prefab机制，固不兼容更低版本。

当前TinaX Framework开发基于 : `Unity 2019.2.0b7`

当前推荐用于业务的Unity版本： `Unity 2019.1.x` / `Unity 2018.4.x LTS`

### C# 版本 

Unity Api Compatibility Level : `.NET 4.x` / `.NET Standard 2.0`


## TinaX 功能概述

TinaX 主要实现了以下功能：

### Lua 语言支持 

出于普遍的热更新需求，TinaX原生提供了基于 [Tencent/xlua](https://github.com/Tencent/xLua) 的Lua语言运行环境，并为主要功能提供了Lua层面的API支持。

如果不需要Lua环境的话，也可以在项目中将Lua相关功能完全关闭，不会影响包体体积。

### 虚拟文件系统 

虚拟文件系统（VFS: Virtual File System)是TinaX 中实现的基于用来管理资源的功能模块。VFS为业务逻辑开发提供了可虚拟寻址的、统一的资源加载API，并在系统内部处理AssetBundle的依赖加载与释放。

VFS支持热更新。

### UIKit 用户界面系统 

基于uGUI提供的UIKit工具，提供了统一的、基于页面的UI管理框架。

### 其他 

- I18N 国际化系统
- 跨语言的事件消息广播
- 场景管理工具
- 简单音频管理
- wwise扩展接口
- 全局时间任务调度
- ……


## 说明

> TinaX Framework 的开发时间跨度有点长，其中可能包含一些作者刚接触Unity不久的萌新时期的不明所以的奇怪代码（虽然它跑起来并没有什么问题），作者也会不断的去找到这些奇怪的代码并优化它。如果您发现有什么匪夷所思的地方，欢迎提PR和Issues，谢谢。

## 开源引用

> TinaX 引用和参考了以下项目：

**CatLib.Core**


<a href="https://github.com/CatLib/Core" target="_blank"><img src="https://camo.githubusercontent.com/d402b21f4ebb6532d5d20d94fbfbb3a5c26914fa/687474703a2f2f6361746c69622e696f2f696d67732f6c6f676f2d7478742e706e67" width = "150" /></a>

**[UniRx](https://github.com/neuecc/UniRx)**

**[SharpZipLib](https://github.com/icsharpcode/SharpZipLib)**

------

> TinaX 开发时引用了以下项目，但可在使用TinaX时移除这些项目。

**xLua**

<a href="https://github.com/Tencent/xLua" target="_blank"><img src="https://github.com/Tencent/xLua/raw/master/Assets/XLua/Doc/xLua.png" width = "150" /></a>

**Odin Inspector** (付费)

<a href="https://odininspector.com/" target="_blank"><img src="https://odininspector.com/files/misc/logo.png" width = "150" /></a>


## 优秀的Unity项目安利

- [QFramework](https://github.com/liangxiegame/QFramework) : 一套渐进式的快速开发框架
- [xasset](https://github.com/xasset/xasset) : 一个简易轻量的Unity资源管理框架
- [CatLib](https://github.com/CatLib/Core) : 轻量级依赖注入框架