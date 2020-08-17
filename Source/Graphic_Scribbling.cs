using UnityEngine;
using Verse;

namespace DSFI
{
    public class Graphic_Scribbling : Graphic_Random
    {
        public override void Init(GraphicRequest req)
        {
            req.shader = ShaderDatabase.WorldOverlayAdditive;
            base.Init(req);
        }
    }
}
