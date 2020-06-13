# 替换掉VFS

在这个示例向我们展示了如何不使用“TinaX.VFS”包而使Framework的其他的其他部分依旧能正常运行。



Unity版本：2019.4.0f1



在项目中，我们使用Unity自带的`UnityEngine.Resources`类来为TinaX.UIKit提供资产管理服务。



UIKit在打开UI时需要加载UI Page文件（本质上是Prefab)， 但UIKit并没有依赖任何资产管理相关的Package. 

其内部的实现原理是，TinaX内部约定了一套“内置服务接口”，UIKit直接向TinaX.Core获取该接口的某个实现来进行资源加载。而我们编写的资产管理服务要做的就是，实现这个接口，并向XCore注册它。



在本示例中请重点关注`Assets/CustomAssetsManager`处的代码：

- `AssetsService.cs` : 该处代码实现了TinaX约定的内置服务接口`TinaX.Services.IAssetService`, 这是UIKit能够加载UI文件的关键。
- `AssetsProvider.cs` : 该文件定义了“TinaX服务提供者”，我们在服务提供者里向`XCore`注册了内置服务接口和我们编写的具体逻辑。需要注意的是，该服务提供者的类通过`XServiceProviderOrder`这个`Attribute`定义了启动次序。通常服务提供者的默认次序为100，此处定义为60，它将在其他服务之前被启动，以避免其他服务在启动时需要加载资源，但资产管理服务还未准备好的情况。
- `IAssets.cs` 该文件定义的接口应该是我们所编写的资产管理服务的对外服务接口，类似于VFS的`IVFS` 、 UIKit的`IUIKit`这种接口。