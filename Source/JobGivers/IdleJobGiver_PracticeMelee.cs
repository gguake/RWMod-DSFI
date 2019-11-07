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
    public class IdleJobGiver_PracticeMelee : IdleJobGiver<IdleJobGiverDef>
    {
        public override float GetWeight(Pawn pawn, Trait traitIndustriousness)
        {
            if (pawn.story.WorkTagIsDisabled(WorkTags.Violent))
            {
                return 0f;
            }

            if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                return 0f;
            }

            float multiplier = 1f;
            if (pawn.story.traits.HasTrait(TraitDefOf.Brawler))
            {
                multiplier *= 2.0f;
            }

            if (pawn.story.traits.HasTrait(TraitDefOf.Bloodlust))
            {
                multiplier *= 1.2f;
            }

            if (pawn.skills.GetSkill(SkillDefOf.Melee).passion == Passion.Major)
            {
                multiplier *= 1.5f;
            }

            if (pawn.skills.GetSkill(SkillDefOf.Melee).passion == Passion.Minor)
            {
                multiplier *= 1.2f;
            }

            return base.GetWeight(pawn, traitIndustriousness) * multiplier;
        }
        
        public override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.equipment == null || pawn.equipment.Primary == null || !pawn.equipment.Primary.def.IsMeleeWeapon)
            {
                return null;
            }

            IntVec3 position = IntVec3.Invalid;
            if (pawn.ownership != null)
            {
                Room room = pawn.ownership.OwnedRoom;
                if (room != null)
                {
                    if (!room.Cells.Where(x => x.Standable(pawn.Map) && !x.IsForbidden(pawn) && pawn.CanReserveAndReach(x, PathEndMode.OnCell, Danger.None)).TryRandomElement(out position))
                    {
                        position = AIUtility.FindRandomSpotOutsideColony(pawn, canReach: true, canReserve: true);
                    }
                }
            }

            if (position.IsValid)
            {
                return new Job(IdleJobDefOf.IdleJob_PracticeMelee, pawn.equipment.Primary, position)
                {
                    locomotionUrgency = LocomotionUrgency.Walk
                };
            }

            return null;
        }
    }
}
