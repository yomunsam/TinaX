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
