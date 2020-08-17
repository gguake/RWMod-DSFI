using Verse;
using RimWorld;

namespace DSFI
{
    public class CompGraffitiDecay : ThingComp
    {
        public override void CompTickRare()
        {
            parent.TakeDamage(new DamageInfo(DamageDefOf.Rotting, 0.4f));
        }
    }
}
