using System;
using System.Reflection;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace DSFI
{
    public class PortraitIconRenderer : MonoBehaviour
    {
        public void RenderPortraitIcon(Pawn pawn, RenderTexture rt)
        {
            Camera portraitCamera = PortraitIconCameraManager.Camera;
            portraitCamera.targetTexture = rt;
            Vector3 position = portraitCamera.transform.position;

            float orthographicSize = portraitCamera.orthographicSize;
            portraitCamera.transform.position += new Vector3(0f, 0f, 0.3f);
            portraitCamera.orthographicSize = 1f;

            this.pawn = pawn;
            portraitCamera.Render();

            this.pawn = null;
            portraitCamera.transform.position = position;
            portraitCamera.orthographicSize = orthographicSize;
            portraitCamera.targetTexture = null;
        }

        public void OnPostRender()
        {
            try
            {
                methodRenderPawnInternal.Invoke(pawn.Drawer.renderer, new object[] {
                    Vector3.zero,
                    0f,
                    false,
                    Rot4.South,
                    Rot4.South,
                    RotDrawMode.Fresh,
                    true,
                    false,
                    false
                });
            }
            catch (Exception e)
            {
#if DEBUG
                Log.Error(e.ToString());
#endif
            }
        }
        
        private Pawn pawn;

        private static MethodInfo methodRenderPawnInternal = AccessTools.Method(typeof(PawnRenderer), "RenderPawnInternal", parameters: new Type[] { typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(bool), typeof(bool), typeof(bool) });


    }
}
