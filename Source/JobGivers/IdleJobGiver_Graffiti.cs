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
    public class IdleJobGiver_Graffiti : IdleJobGiver<IdleJobGiverDef>
    {
        public override float GetWeight(Pawn pawn, Trait traitIndustriousness)
        {
            if (pawn.skills.GetSkill(SkillDefOf.Artistic).passion == Passion.None)
            {
                return 0f;
            }

            return base.GetWeight(pawn, traitIndustriousness);
        }

        public override Job TryGiveJob(Pawn pawn)
        {
            if (!JoyUtility.EnjoyableOutsideNow(pawn.Map))
            {
                return null;
            }

            IntVec3 position = IntVec3.Invalid;

            RCellFinder.TryFindRandomSpotJustOutsideColony(pawn.Position, pawn.Map, pawn, out position, (IntVec3 x) =>
            {
                if (!x.InBounds(pawn.Map) || !x.Walkable(pawn.Map))
                {
                    return false;
                }

                if (x.IsForbidden(pawn))
                {
                    return false;
                }

                TerrainDef terrainDef = x.GetTerrain(pawn.Map);
                if (terrainDef.fertility <= 0)
                {
                    return false;
                }

                int distSq = x.DistanceToSquared(pawn.Position);
                if (distSq < 9 && distSq > 225)
                {
                    return false;
                }
                
                if(!pawn.CanReserveAndReach(x, PathEndMode.Touch, Danger.None))
                {
                    return false;
                }

                return true;
            });

            if (position.IsValid)
            {
                return new Job(IdleJobDefOf.IdleJob_Graffiti, position)
                {
                    locomotionUrgency = modSettings.wanderMovePolicy
                };
            }
            
            return null;
        }
    }
}
