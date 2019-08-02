## 2019.8.3

1. 针对UnityEngine.GameObject类的C#扩展方法调整：
    1. `DestroySelf` 方法增加延迟参数的重载。
    2. `GetComponentOrAdd` 方法增加传入`System.Type`参数的重载。
    3. `RemoveComponentIfExists` 方法增加传入`string`类型参数和`System.Type`类型参数的重载。
    4. `RemoveComponentsIfExists` 方法增加传入`System.Type`类型参数的重载。
    5. `FindOrCreateGo`方法 改为`FindOrCreateGameObject`，（原方法目前仍可用），并为`FindOrCreateGameObject`方法增加传入`params System.Type[]`类型的Components参数。
    6. 新增 `CreateGameObject` 方法，与 `FindOrCreateGameObject`用法类似。
    7. 咱家了一堆设置常用的`Transform`属性的扩展方法。
    8. `HasComponent` 方法增加传入`string`类型参数和`System.Type`类型参数的重载。
2. UIKit 新增常用UI组件：XEmpty4Raycast


## 2019.8.1

1. [Remove][xUISafeArea] 暂时移除了xUISafeArea 关于畸形屏设备自适应的功能（目前组件还在），因为新版Unity有了关于这方面的更新，所以原来的实现方法可能就不需要了，以后会重构。
2. [Remove][JSON.NET for Unity] 移除了Json.NET for Unity依赖，因为该库在UWP上的适配比较麻烦（需要有专门针对平台的特殊操作）。框架内部将只使用`UnityEngine.JsonUtility` 类进行Json操作。
3. [Update]更新了框架内部依赖`xNode`到1.7版本。
4. [Upgrade] 工程升级到Unity 2019.2.0f1.

## 2019.7.28
1. [UIKit] C# 接口增加：
    - UIKit_UIRoot_RectTrans
    - UIKit_UICamera_GameObject
    - UIKit_UICamera
    - [详见文档](https://tinax.corala.space/#/api/system/uikit/uikit?id=c%e5%b1%9e%e6%80%a7)
2. [UIKit] 功能增加：[获取某个UI元素相对UIKit UIRoot（全屏）的坐标](https://tinax.corala.space/#/api/system/uikit/uikit?id=%e5%9d%90%e6%a0%87%e8%bd%ac%e6%8d%a2%ef%bc%9a%e8%8e%b7%e5%8f%96%e6%9f%90%e4%b8%aaui%e5%85%83%e7%b4%a0%e7%9b%b8%e5%af%b9uikit-uiroot%ef%bc%88%e5%85%a8%e5%b1%8f%ef%bc%89%e7%9a%84%e5%9d%90%e6%a0%87)