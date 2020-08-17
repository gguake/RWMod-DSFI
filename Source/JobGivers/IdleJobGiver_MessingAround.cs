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
    public class IdleJobGiver_MessingAround : IdleJobGiver<IdleJobGiverDef>
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            foreach (Building building in from x in pawn.Map.listerBuildings.allBuildingsColonist
                                          where x.def.building != null && x.def.building.isSittable && 
                                                x.Position.InHorDistOf(pawn.Position, 20f) && 
                                                pawn.CanReserve(x)
                                          select x)
            {
                Room room = building.Position.GetRoom(pawn.Map);
                if (room != null && (room.Role == RoomRoleDefOf.DiningRoom || room.Role == RoomRoleDefOf.RecRoom))
                {
                    return new Job(IdleJobDefOf.IdleJob_MessingAround, building)
                    {
                        locomotionUrgency = modSettings.wanderMovePolicy
                    };
                }
            }
            
            return null;
        }
        
        HashSet<Pair<Room, Building>> rooms = new HashSet<Pair<Room, Building>>();
    }
}
