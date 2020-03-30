using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ludiq;
using Ludiq.Bolt;

public class MsgProxy : MonoBehaviour
{
    public BoltComponent BoltComponent;

    void OnOpenUIMessage(params object[] args)
    {
        if (BoltComponent != null)
            BoltComponent.Invoke("OnOpenUIMessage", args);
    }
    void OnShowUIAnimationFinish()
    {
        if (BoltComponent != null)
            BoltComponent.Invoke("OnShowUIAnimationFinish");
    }

    void OnCloseUIMessage(params object[] args)
    {
        if (BoltComponent != null)
            BoltComponent.Invoke("OnCloseUIMessage",args);
    }
}
