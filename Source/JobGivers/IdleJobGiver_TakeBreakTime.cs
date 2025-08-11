using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI.JobGivers
{
    public class IdleJobGiver_TakeBreakTime : IdleJobGiver<IdleJobGiverDef>
    {
        public override float GetWeight(Pawn pawn, Trait traitIndustriousness)
        {
            if (pawn.story.traits.HasTrait(MoreTraitDefOf.Neurotic))
            {
                return base.GetWeight(pawn, traitIndustriousness) * 0.1f;
            }

            return base.GetWeight(pawn, traitIndustriousness);
        }

        public override Job TryGiveJob(Pawn pawn)
        {
            Need_Rest need = pawn.needs.rest;
            if (need == null || need.CurLevelPercentage > 0.7f)
            {
                return null;
            }

            var sittable = IdleJobUtility.FindNearSittableForIdle(pawn);
            if (sittable.IsValid)
            {
                return new Job(IdleJobDefOf.IdleJob_TakeBreakTime, sittable)
                {
                    locomotionUrgency = modSettings.wanderMovePolicy
                };
            }

            return null;
        }
    }
}
