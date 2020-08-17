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
            float multiplier = 1f;
            if (pawn.story.traits.HasTrait(TraitDefOf.ShootingAccuracy))
            {
                multiplier *= 1.5f;
            }
            
            return base.GetWeight(pawn, traitIndustriousness) * multiplier;
        }
        
        public override Job TryGiveJob(Pawn pawn)
        {
            if (!JoyUtility.EnjoyableOutsideNow(pawn.Map))
            {
                return null;
            }

            IntVec3 position = AIUtility.FindRandomSpotOutsideColony(pawn, def.searchDistance, canReach: false, canReserve: false);
            if (!position.IsValid)
            {
                return null;
            }

            IntVec3 standPosition = IntVec3.Invalid;
            if (!AIUtility.FindAroundSpotFromTarget(pawn, position, 4.0f, 3.0f, canSee: true, canReach: true, canReserve: true).TryRandomElement(out standPosition))
            {
                return null;
            }
            
            return new Job(IdleJobDefOf.IdleJob_ThrowingStone, position, standPosition)
            {
                locomotionUrgency = modSettings.wanderMovePolicy
            };
        }
    }
}
