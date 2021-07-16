using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DSFI
{
    public static class PortraitIconMaterialCache
    {
        public static Material MatFrom(Pawn pawn, Color color)
        {
#if DEBUG
            Log.Message("try to pawn " + pawn.Name + " portrait icon cached.");
#endif

            if (!UnityData.IsInMainThread)
            {
                Log.Error("Tried to get a material from a different thread.");
                return null;
            }

            var reqKey = new KeyValuePair<Pawn, Color>(pawn, color);
            if (!_matDict.TryGetValue(reqKey, out Material mat))
            {
                if (!_texDict.TryGetValue(pawn, out Texture tex))
                {
                    tex = new RenderTexture(128, 128, 0)
                    {
                        filterMode = FilterMode.Bilinear,
                    };

                    Find.PawnCacheRenderer.RenderPawn(pawn, (RenderTexture)tex, new Vector3(0f, 0f, 0.2f), 1f, 0f, Rot4.South, 
                        renderHead: true, renderBody: false, renderHeadgear: true, renderClothes: false, portrait: true);

                    _texDict.Add(pawn, tex);
                }
                
                mat = new Material(ShaderDatabase.TransparentPostLight);
                mat.name = pawn.Name + "_" + "PortraitIcon";
                mat.mainTexture = tex;
                mat.color = color;

                _matDict.Add(reqKey, mat);
            }

#if DEBUG
            Log.Message("pawn " + pawn.Name + " portrait icon cached.");
#endif
            return mat;
        }

        private static Dictionary<KeyValuePair<Pawn, Color>, Material> _matDict = new Dictionary<KeyValuePair<Pawn, Color>, Material>();
        private static Dictionary<Pawn, Texture> _texDict = new Dictionary<Pawn, Texture>();
    }
}
