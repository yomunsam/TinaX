# TinaX
简洁、愉快、“开箱即用”的Unity独立游戏开发工具 | Unity-based Game Client Framework

[![LICENSE](Doc/996icu_license.svg)](https://github.com/996icu/996.ICU/blob/master/LICENSE)
<a href="https://996.icu"><img src="https://img.shields.io/badge/link-996.icu-red.svg" alt="996.icu"></a>
[![LICENSE](https://camo.githubusercontent.com/890acbdcb87868b382af9a4b1fac507b9659d9bf/68747470733a2f2f696d672e736869656c64732e696f2f62616467652f6c6963656e73652d4d49542d626c75652e737667)](https://github.com/yomunsam/TinaX/blob/master/LICENSE)
![GitHub stars](https://img.shields.io/github/stars/yomunsam/Tinax?style=flat-square)
<!-- [![LICENSE](Doc/AGPL3_license.svg)](https://github.com/yomunsam/TinaX/blob/master/LICENSE) -->

> 为美好的游戏献上Framework!

TinaX Framework 是基于Unity引擎的简单、完整的、开箱即用的开发框架。TinaX的目标是制作一个可以让一个团队、公司“从小用到大”的框架。

- 支持除了WebGL之外的所有Unity目标平台
- 使用C#/Lua开发游戏并支持代码热更新
- 按需引入项目的各个模块、并可自由替换
- 异步优先的设计理念
- 面向接口和控制反转的弱耦合设计


## 环境要求

**Unity**
- 当前TinaX开发版本：`Unity 2019.4.0f1`
- 推荐用于生产环境的版本：`Unity 2019.4.x LTS`
- 理论最低兼容版本：`Unity 2019.4.x`

**C# 版本**
- Unity Api Compatibility Level: `.NET 4.X`/`.NET Standard 2.0` 
- C# `7.2`

<br>

## 社群交流

- 您可在Github发起Issues和Pull requests.
- QQ群组：560649770  -[点击加入](https://jq.qq.com/?_wv=1027&k=g4TmslMA)
- Telegram群组: [https://t.me/tinax_framework](https://t.me/tinax_framework)

## 了解TinaX

**Packages**: TinaX Framework使用Unity包（Packages）的形式来组织和管理各个功能的模块。通过安装不同的Packages，即可向项目中添加不同的功能模块，无论您的项目处于哪个阶段您都可以轻易的接入TinaX.

**示例代码**： 您可在本仓库中查看[示例代码工程]https://github.com/yomunsam/TinaX/tree/TinaX6.6/Examples)

关于各个Packages的描述如下：

### [TinaX.Core](https://github.com/yomunsam/TinaX.Core)

`TinaX.Core`是TinaX Framework的基础包，它负责启动、管理所有的Service，提供通用的基础功能，提供事件系统、依赖注入等。
- TinaX框架的核心Package
- 控制反转容器（IoC）
- 事件广播系统
- 时间驱动系统
- 常用方法扩展和工具

访问： 
- 仓库地址：[https://github.com/yomunsam/TinaX.Core](https://github.com/yomunsam/TinaX.Core)
- 大陆镜像仓库: [https://gitee.com/nekonyas/TinaX.Core](https://gitee.com/nekonyas/TinaX.Core)
- 包名: `io.nekonya.tinax.core`
- [访问文档](https://tinax.corala.space/#/cmn-hans/core/README)

<br>

### [TinaX.VFS](https://github.com/yomunsam/TinaX.Core)

虚拟文件系统（VFS）是TinaX的资源管理服务，它在运行时中模拟了Unity工程中"Assets/xxx"的目录结构，实现了资源的加载、依赖管理、版本管理与更新、内存gc等。
- 根据Unity Asset Path加载资产
- 无感知的AssetBundle管理
- AssetBundle打包
- 资产热更新、“边下边玩”

访问： 
- 仓库地址：[https://github.com/yomunsam/TinaX.VFS](https://github.com/yomunsam/TinaX.VFS)
- 大陆镜像仓库: [https://gitee.com/nekonyas/TinaX.VFS](https://gitee.com/nekonyas/TinaX.VFS)
- 包名：`io.nekonya.tinax.vfs`

<br>

### TinaX.UIKit

UIKit为TinaX提供了基于UGUI的UI管理服务，如打开、关闭、隐藏UI、全屏UI互相避让、UI启动参数等。同时提供对组件的扩展、可扩展的UI动画等相关内容。
- 基于“页面”概念的UI管理
- UI动画框架
- UGUI功能扩展

访问：
- 仓库地址: [https://github.com/yomunsam/TinaX.UIKit](https://github.com/yomunsam/TinaX.UIKit)
- 大陆镜像地址：[https://gitee.com/nekonyas/TinaX.UIKit](https://gitee.com/nekonyas/TinaX.UIKit)
- 包名：`io.nekonya.tinax.uikit`

<br>

### TinaX.I18N

基于`key/value`形式的国际化支持服务，让你的应用对全世界不同语言和地区的用户更加友好。
- 使用Json （或.asset文件）定义的key/value配表
- 实时的区域切换
- 针对UGUI的扩展

访问

- 仓库地址: [https://github.com/yomunsam/TinaX.I18N](https://github.com/yomunsam/TinaX.I18N)
- 大陆镜像地址：[https://gitee.com/nekonyas/TinaX.I18N](https://gitee.com/nekonyas/TinaX.I18N)
- 包名：`io.nekonya.tinax.i18n`

<br>

### TinaX.I18N

基于`key/value`形式的国际化支持服务，让你的应用对全世界不同语言和地区的用户更加友好。

- 仓库地址: [https://github.com/yomunsam/TinaX.I18N](https://github.com/yomunsam/TinaX.I18N)
- 大陆镜像地址：[https://gitee.com/nekonyas/TinaX.I18N](https://gitee.com/nekonyas/TinaX.I18N)
- 包名：`io.nekonya.tinax.i18n`

<br>

### TinaX.Lua

为TinaX提供Lua语言的开发运行环境。该服务基于`Tencent/xLua`开发。

- 仓库地址: [https://github.com/yomunsam/TinaX.Lua](https://github.com/yomunsam/TinaX.Lua)
- 大陆镜像地址：[https://gitee.com/nekonyas/TinaX.Lua](https://gitee.com/nekonyas/TinaX.Lua)
- 该部分内容需直接导入到工程`Assets`目录中，不以包形式提供

<br>

### TinaX.ILRuntime

为TinaX提供可热更新的C#运行环境。该服务基于`ILRuntime`开发。

- 仓库地址：[https://github.com/yomunsam/TinaX.ILRuntime](https://github.com/yomunsam/TinaX.ILRuntime)
- 大陆镜像地址：[https://gitee.com/nekonyas/TinaX.ILRuntime](https://gitee.com/nekonyas/TinaX.ILRuntime)
- 包名：`io.nekonya.tinax.ilruntime`

> 大陆镜像地址推荐仅供应急情况使用, 它可能会比Github仓库的版本略有延迟。并且镜像仓库会关闭fork, issues和pr功能，请使用Github进行相关操作。

<br>

------

## 优秀的Unity项目安利

- **[QFramework](https://github.com/liangxiegame/QFramework)** : 一套渐进式的快速开发框架
- **[xasset](https://github.com/xasset/xasset)** : 一个简易轻量的Unity资源管理框架
- **[CatLib](https://github.com/CatLib/Core)** : 轻量级依赖注入框架