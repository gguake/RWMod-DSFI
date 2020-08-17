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
    public class IdleJobGiver_Gardening : IdleJobGiver<IdleJobGiverDef>
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            int plantSkill = pawn.skills.GetSkill(SkillDefOf.Plants).Level;

            Zone growZone = null;
            if (!pawn.Map.zoneManager.AllZones.Where((Zone x) => 
            {
                Zone_Growing zone = x as Zone_Growing;
                if (zone == null)
                {
                    return false;
                }

                if (zone.cells.Count == 0)
                {
                    return false;
                }

                if (zone.GetPlantDefToGrow() == null || zone.GetPlantDefToGrow().plant == null)
                {
                    return false;
                }

                if (plantSkill < Mathf.RoundToInt(zone.GetPlantDefToGrow().plant.sowMinSkill * 0.75f))
                {
                    return false;
                }

                return true;

            }).TryRandomElement(out growZone))
            {
                return null;
            }

            IntVec3 position = AIUtility.FindRandomSpotInZone(pawn, growZone, true, true);
            if (position != IntVec3.Invalid)
            {
                return new Job(IdleJobDefOf.IdleJob_Gardening, position)
                {
                    locomotionUrgency = modSettings.wanderMovePolicy
                };
            }
            else
            {
                return null;
            }
        }

    }
}
