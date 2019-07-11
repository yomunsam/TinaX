using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TinaX.UIKit;


namespace TinaXEditor.UIKit
{
    [InitializeOnLoad]
    public class UIKitEditHelper
    {
        static UIKitEditHelper()
        {
            UnityEditor.Experimental.SceneManagement.PrefabStage.prefabStageOpened += OnSceneOpened;
            
        }


        private static void OnSceneOpened(UnityEditor.Experimental.SceneManagement.PrefabStage prefabStage)
        {
            //Debug.Log("emm");
            if(prefabStage.prefabContentsRoot.GetComponent<UIEntity>() != null)
            {
                
            }
        }
    }
}

