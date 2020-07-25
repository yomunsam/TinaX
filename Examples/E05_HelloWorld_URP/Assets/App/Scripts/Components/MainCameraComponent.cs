using System.Collections;
using System.Collections.Generic;
using TinaX;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Nekonya.Example
{
    [RequireComponent(typeof(Camera))]
    public class MainCameraComponent : MonoBehaviour
    {
        public static MainCameraComponent Instance { get; private set; }

        private Camera m_Camera;
        private UniversalAdditionalCameraData m_UACD;

        private void Awake()
        {
            this.gameObject.DontDestroy();
            MainCameraComponent.Instance = this;
            m_Camera = this.GetComponent<Camera>();
            m_UACD = m_Camera.GetUniversalAdditionalCameraData();
        }

        public void AddCamera(Camera camera)
        {
            m_UACD.cameraStack.Add(camera);
        }

    }

}
