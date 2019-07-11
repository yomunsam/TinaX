using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace TinaXEditor
{
    /// <summary>
    /// 编译流程控制
    /// </summary>
    public class DevOpsBuild
    {
        public void BuildPlayer(BuildTarget target, string path, BuildOptions opt)
        {
            List<string> scenes = new List<string>();
            for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                scenes.Add(SceneManager.GetSceneByBuildIndex(i).path);
            }
            var buildOption = new BuildPlayerOptions();
            buildOption.scenes = scenes.ToArray();
            buildOption.target = target;
            buildOption.locationPathName = path;
            buildOption.options = opt;
            BuildPipeline.BuildPlayer(buildOption);
        }
    }
}

