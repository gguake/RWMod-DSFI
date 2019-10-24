using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace DSFI.JobGivers
{
    public class IdleJobGiver_ThrowingStone : IdleJobGiver<IdleJobGiverDef>
    {
        public override float GetWeight(Pawn pawn, Trait traitIndustriousness)
        {
            if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                return 0f;
            }

            float multiplier = 1f;
            if (pawn.story.traits.HasTrait(TraitDefOf.ShootingAccuracy))
            {
                multiplier *= 1.5f;
            }

            if (pawn.skills.GetSkill(SkillDefOf.Shooting).passion == Passion.Major)
            {
                multiplier *= 1.5f;
            }

            if (pawn.skills.GetSkill(SkillDefOf.Shooting).passion == Passion.Minor)
            {
                multiplier *= 1.2f;
            }

            return base.GetWeight(pawn, traitIndustriousness) * multiplier;
        }
        
        public override Job TryGiveJob(Pawn pawn)
        {
            if (!JoyUtility.EnjoyableOutsideNow(pawn.Map))
            {
                return null;
            }

            IntVec3 position = IntVec3.Invalid;
            
            RCellFinder.TryFindRandomSpotJustOutsideColony(pawn.Position, pawn.Map, pawn, out position, (IntVec3 x) => pawn.CanReserveAndReach(x, PathEndMode.Touch, Danger.None));
            if (!position.IsValid)
            {
                return null;
            }

            IntVec3 standPosition = IntVec3.Invalid;
            (from x in GenRadial.RadialCellsAround(position, 4.0f, true)
             where x.DistanceToSquared(position) > 9 &&
                   GenSight.LineOfSight(position, x, pawn.Map)
             select x).TryRandomElement(out standPosition);
            if (standPosition == null)
            {
                return null;
            }
            

            if (position.IsValid)
            {
                return new Job(IdleJobDefOf.IdleJob_ThrowingStone, position, standPosition)
                {
                    locomotionUrgency = LocomotionUrgency.Walk
                };
            }

            return null;
        }
    }
}
