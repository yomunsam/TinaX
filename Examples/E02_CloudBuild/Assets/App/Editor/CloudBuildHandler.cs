using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TinaXEditor.VFSKit;
using TinaXEditor.VFSKit.Pipeline;
using System.IO;
using TinaX.VFSKit;
using TinaX;

namespace Nekonya.Example
{
    public static class CloudBuildHandler
    {
        /// <summary>
        /// Call this method before build by "Unity Cloud Build" or "Jenkins"
        /// 从Unity Cloud Build或Jenkins中调用这个方法即可
        /// </summary>
        public static void BuildAndroid()
        {
            CloudBuildHandler.Handler(XRuntimePlatform.Android);
        }

        public static void BuildWindowsAMD64()
        {
            CloudBuildHandler.Handler(XRuntimePlatform.Windows);
        }

        public static void Handler(XRuntimePlatform platform)
        {
            IVFSBuilder builder = new VFSBuilder() //VFS Builder : used to build assetbundle.
                .UseProfile("Default")                          //TinaX global profile.
                .SetConfig(VFSManagerEditor.VFSConfig);         //Get TinaX.VFS config file. or you can use "AssetDatabase.LoadAssetAtPath<VFSConfigModel>("Assets/Resources/XConfig/TinaX/VFSConfig.asset")"

            #region if you need pipeline 
            // if you not need, ignore it;
            var pipeline = new BuilderPipeline();
            pipeline.AddLast(new AssetsBuildPipelineDemo());

            builder.UsePipeline(pipeline);
            #endregion

            builder.EnableTipsGUI = true;
            builder.CopyToStreamingAssetsFolder = true;
            builder.ForceRebuild = false;

            builder.Build(platform, AssetCompressType.LZ4);

        }
    }

    /// <summary>
    /// VFS Build Pipeline 
    /// Used to implement some advanced customization features, it is not required. 
    /// If customization is not required, ignore it!
    /// VFS 资源构建管线 用于在构建Assetbundle时实现一些高级功能，它并不是必须的。
    /// 如果你不需要进行一些细节的自定义，可以直接忽略这个功能。
    /// 
    /// About Attribute :
    /// This attribute can be used to set priority for Pipeline (default is 100) when building resources using the editor GUI. For self-written build code (that is, this case), it is invalid.
    /// 在使用编辑器GUI进行资源构建时，可使用该attribute对Pipeline设置优先级（默认为100）。对于自行编写的构建代码（也就是本案例），它是无效的。
    /// </summary>
    [TinaX.Priority(100)] 
    public class AssetsBuildPipelineDemo : IBuildHandler
    {
        public bool BeforeAssetBundleFileSavedByGroup(ref VFSEditorGroup group, string assetBundleFileName, ref FileStream fileStream)
        {
            //if you don't want to break pipeline, just return 'true'
            //如果不打算中断这个pipeline，直接返回true就完事了
            return true;
        }

        public bool BeforeSetAssetBundleSign(ref string assetbundleName, ref AssetsStatusQueryResult assetQueryResult)
        {
            //as above 
            //同上
            return true;
        }
    }

}

