using TinaX.VFSKit;
using UnityEditor;
using UnityEngine;

namespace TinaXEditor.VFSKit
{
    public class VFSPackerWindow : EditorWindow
    {


        [MenuItem("TinaX/发布流程/资源打包",false,1)]
        public static void OpenWindow()
        {
            GetWindow<VFSPackerWindow>();
        }


        private XVFSPacker mPacker;
        private TinaX.Const.PlatformConst.E_Platform mCurEdit_Pack_platform;
        private bool mCurEdit_ClearPack = false;
        private bool mCurEdit_CopyToStreamingAssets = false;
        private bool mCurEdit_StrictMode = false;
        private VFSPackPlan.CompressType mCurEdit_ComppressType = VFSPackPlan.CompressType.LZ4;

        private Vector2 mScroll1;

        

        VFSPackerWindow()
        {
            this.titleContent = new GUIContent("TinaX 资源打包管理");
            this.minSize = new Vector2(600, 400);
        }


        private void OnGUI()
        {
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(610),GUILayout.MinWidth(600));

            GUILayout.BeginVertical();
            DrawPlanAddPad();
            GUILayout.Space(10);
            DrawPlanList();

            GUILayout.EndVertical();

            DrawConsolePad();

            GUILayout.EndHorizontal();



        }

        private void OnEnable()
        {
            if(mPacker == null)
            {
                mPacker = new XVFSPacker();
            }

            
        }


        private void OnDestroy()
        {
            mPacker = null;
        }

        private void DrawPlanAddPad()
        {
            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(400));
            EditorGUILayout.LabelField("资源打包计划", EditorStyles.largeLabel);
            GUILayout.Space(10);

            //打包平台
            mCurEdit_Pack_platform = (TinaX.Const.PlatformConst.E_Platform)EditorGUILayout.EnumPopup("打包平台：",mCurEdit_Pack_platform);

            //清理
            mCurEdit_ClearPack = EditorGUILayout.Toggle("清空输出目录",mCurEdit_ClearPack);

            //复制到StreamingAssets
            mCurEdit_CopyToStreamingAssets = EditorGUILayout.Toggle("复制到StreamingAssets", mCurEdit_CopyToStreamingAssets);

            //严格模式
            mCurEdit_StrictMode = EditorGUILayout.Toggle("严格模式", mCurEdit_StrictMode);

            //压缩模式
            mCurEdit_ComppressType = (VFSPackPlan.CompressType)EditorGUILayout.EnumPopup("压缩模式:", mCurEdit_ComppressType);

            GUILayout.Space(10);
            if (GUILayout.Button("添加打包计划"))
            {
                mPacker?.AddPackPlan(new VFSPackPlan()
                {
                    Platform = TinaX.Platform.GetBuildTarget(mCurEdit_Pack_platform),
                    ClearOutputFolders = mCurEdit_ClearPack,
                    CopyToStreamingAssets = mCurEdit_CopyToStreamingAssets,
                    OutputPath = $"{TinaX.Setup.Framework_AssetSystem_Pack_Path}/{mCurEdit_Pack_platform.ToString().ToLower()}/",
                    StrictMode = mCurEdit_StrictMode,
                    AssetCompressType = mCurEdit_ComppressType,
                    XPlatform = mCurEdit_Pack_platform,
                });


            }

            EditorGUILayout.EndVertical();
        }

        private void DrawPlanList()
        {
            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(400));
            GUILayout.Label("打包队列",EditorStyles.largeLabel);
            if (mPacker.PackPlanList.Count > 0)
            {
                mScroll1 = EditorGUILayout.BeginScrollView(mScroll1);

                foreach(var item in mPacker.PackPlanList)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label($"打包计划:{item.XPlatform.ToString()}");
                    if (GUILayout.Button("Remove",GUILayout.MaxWidth(60)))
                    {
                        mPacker.PackPlanList.Remove(item);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.EndVertical();
        }


        private void DrawConsolePad()
        {
            //全局控制
            GUILayout.BeginVertical(GUILayout.MaxWidth(200));
            GUILayout.Label("打包行动", EditorStyles.largeLabel);

            //是否遍历检查工程中的所有资源
            mPacker.CheackAllFileInProject = EditorGUILayout.Toggle("遍历检查工程中的所有资源", mPacker.CheackAllFileInProject);
            EditorGUILayout.HelpBox("防止工程中有不在VFS管理范围内的资源被其他途径设置了AssetBundle打包标记，而影响包体大小，可以勾选完全遍历检查工程中的所有资源。", MessageType.Info, true);

            mPacker.RemoveABSignAfterAllPlanFinish = EditorGUILayout.Toggle("任务结束后，移除所有ab标记", mPacker.RemoveABSignAfterAllPlanFinish);

            
            if(mPacker.PackPlanList.Count > 0)
            {
                if (GUILayout.Button("开始打包队列"))
                {
                    var vfs_config = TinaX.Config.GetTinaXConfig<TinaX.VFSKit.VFSConfigModel>(TinaX.Conf.ConfigPath.vfs);
                    if(vfs_config == null)
                    {
                        Debug.LogError("没有有效的VFS配置");
                        return;
                    }
                    var final_config = vfs_config.GetPerfect();
                    mPacker.StartPacker(final_config);
                }
            }

            GUILayout.EndVertical();
        }

    }
}

