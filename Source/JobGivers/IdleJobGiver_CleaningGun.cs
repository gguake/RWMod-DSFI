using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace DSFI.JobGivers
{
    public class IdleJobGiver_CleaningGun : IdleJobGiver<IdleJobGiverDef>
    {
        public override float GetWeight(Pawn pawn, Trait traitIndustriousness)
        {
            if (pawn.story.WorkTagIsDisabled(WorkTags.Violent))
            {
                return 0f;
            }

            if (pawn.story.WorkTagIsDisabled(WorkTags.Crafting))
            {
                return 0f;
            }

            if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                return 0f;
            }

            return base.GetWeight(pawn, traitIndustriousness);
        }

        public override Job TryGiveJob(Pawn pawn)
        {
            Thing weapon = null;
            if (pawn.equipment != null && pawn.equipment.Primary != null && pawn.equipment.Primary.def.IsRangedWeapon)
            {
                weapon = pawn.equipment.Primary;
            }

            if (weapon == null)
            {
                return null;
            }
            
            if (weapon.HitPoints >= weapon.MaxHitPoints || weapon.HitPoints / (float)weapon.MaxHitPoints < 0.6f)
            {
                return null;
            }

            return new Job(IdleJobDefOf.IdleJob_CleaningGun, weapon)
            {
                locomotionUrgency = LocomotionUrgency.Walk
            };
        }
    }
}
