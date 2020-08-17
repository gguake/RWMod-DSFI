using UnityEngine;
using Verse;
using RimWorld;

namespace DSFI
{
    [StaticConstructorOnStartup]
    public class RTMoteBubble : MoteDualAttached
    {
        public void SetupMoteBubble(Pawn pawn)
        {
            motePawn = pawn;
            iconMat = PortraitIconMaterialCache.MatFrom(pawn, Color.white);
        }

        public override void Draw()
        {
            base.Draw();
            if (motePawn != null && iconMat != null)
            {
                Vector3 drawPos = DrawPos;
                drawPos.y += 0.01f;
                float alpha = Alpha;
                if (alpha <= 0f)
                {
                    return;
                }

                Color instanceColor = base.instanceColor;
                instanceColor.a *= alpha;
                Material material = iconMat;
                if (instanceColor != material.color)
                {
                    material = PortraitIconMaterialCache.MatFrom(motePawn, instanceColor);
                }
                
                Vector3 s = new Vector3(def.graphicData.drawSize.x, 1f, def.graphicData.drawSize.y);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(drawPos, Quaternion.identity, s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);
            }
        }

        public Material iconMat;
        public Pawn motePawn;
    }
}
