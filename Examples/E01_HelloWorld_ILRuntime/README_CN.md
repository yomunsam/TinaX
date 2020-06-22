# TinaX Example - Hello World (C#  via ILRuntime)

- Unity : 2019.4.0f1


这是一个使用C#代码进行热更新的案例。

相较于纯C#实现的案例，本案例中，大部分业务逻辑代码被移动到了热更工程（HotfixProject）中，

## 上手

热更新工程在开发阶段，IDE需要依赖一些外部的DLL文件，（比如Unity的DLL文件 、 TinaX的DLL文件等）

在研究本项目时，请先使用Unity打开一遍Unity工程，这时候Unity会自动生成出一些Packages中代码的DLL文件，存放路径是：`Library/ScriptAssemblies`

通常大部分需要的代码都可以在这个路径找到，

此外，访问Unity的API，需要在热更新工程中引用Unity的相关DLL，这个需要到Unity编辑器的安装目录中去找，在本项目中，我们给放在了`HotfixProject/UnityDlls`这个目录下，方便找。

`CatLib.Core.dll`是`TinaX.Core`直接引用的一个DLL,它位于TinaX.Core的packages目录下，我们可以在这个位置找到它：`Library/PackageCache/io.nekonya.tinax.core@版本号/Plugins/CatLib`，本项目中为了方便找，也已经给放在了`HotfixProject/UnityDlls`这个目录。（偷懒的话，CatLib也可以用Nuget安装）

> 注意： 热更新工程引用的这些DLL，仅用于开发，不需要复制到工程里。

<br>

## 运行

首先，编译热更新工程，得到DLL,并将其复制到项目中，

在本案例中，复制路径为：`Assets/App/Assemblies/Nekonya.Main.dll.bytes` 和 `Assets/App/Assemblies/Nekonya.Main.pdb.bytes` , 一些说明如下：

1. 在别人的ILRuntime案例中，dll文件大多复制到了`StreamingAssets`目录下，以便加载。在本案例中，我们将dll文件交给了TinaX的资产管理系统`TinaX.VFS` 去管理加载和更新（就和资源文件一样管理了），中间节省了很多步骤。
2. 和dll文件同名的pdb后缀的文件，是调试符号，调试用的，加载调试符号会影响性能，但是方便Debug，是否加载调试符号的默认规则如下：
    - 在编辑器模式下始终加载
    - 在真机包中，如果TinaX被设置为开发模式（在Project Setting窗口里设置，不是指Unity的Debug模式），则会加载调试符号，否则不加载。
    - 如果不喜欢以上默认规则，我们可以在框架**初始化阶段**在TinaX.ILRuntime的服务接口`IXILRuntime`中设置属性`LoadSymbol = true/false`.
3. 案例中的dll文件和pdb文件都被另外加上了`.bytes`后缀，这样Unity就不会把它当成一个dll文件，而是当成普通的资源文件。以免引擎影响我们
4. 如果使用`Visual Studio`等IDE编译热更新工程的话，每次编译出新的DLL文件之后会自动复制到我们的项目里。设置在Visual Studio的“生成后事件命令行”配置项。

<br>

## 然后运行即可

更多的使用方法请看[ILRuntime的官方文档](https://ourpalm.github.io/ILRuntime/public/v1/guide/index.html)