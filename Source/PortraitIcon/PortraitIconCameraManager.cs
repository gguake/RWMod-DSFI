using UnityEngine;
using Verse;

namespace DSFI
{
    public static class PortraitIconCameraManager
    {
        public static Camera Camera { get; private set; }
        public static PortraitIconRenderer Renderer { get; private set; }

        static PortraitIconCameraManager()
        {
            Camera = CreateCamera();
            Renderer = Camera.GetComponent<PortraitIconRenderer>();
        }

        private static Camera CreateCamera()
        {
            GameObject gameObject = new GameObject("DSFI_PortraitIconCamera", typeof(Camera));
            gameObject.SetActive(false);
            gameObject.AddComponent<PortraitIconRenderer>();

            Object.DontDestroyOnLoad(gameObject);
            Camera component = gameObject.GetComponent<Camera>();
            component.transform.position = new Vector3(0f, 15f, 0f);
            component.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            component.orthographic = true;
            component.cullingMask = 0;
            component.orthographicSize = 1f;
            component.clearFlags = CameraClearFlags.Color;
            component.backgroundColor = new Color(0f, 0f, 0f, 0f);
            component.useOcclusionCulling = false;
            component.renderingPath = RenderingPath.Forward;
            Camera camera = Current.Camera;
            component.nearClipPlane = camera.nearClipPlane;
            component.farClipPlane = camera.farClipPlane;
            return component;
        }
    }
}
