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
    public class IdleJobGiver_Thinking : IdleJobGiver<IdleJobGiverDef_Thinking>
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.ownership == null)
            {
                return null;
            }

            if (pawn.ownership.OwnedRoom == null)
            {
                return null;
            }

            Room room = pawn.ownership.OwnedRoom;

            var spotCandidates = room.Cells.Where(c => c.Standable(pawn.Map) && !c.IsForbidden(pawn) && pawn.CanReserveAndReach(c, PathEndMode.OnCell, Danger.None));

            LocalTargetInfo destination = LocalTargetInfo.Invalid;
            if (spotCandidates.Count() > 0)
            {
                destination = spotCandidates.RandomElement();
            }
            else
            {
                return null;
            }
            
            Pawn targetPawn = null;
            if (!(from x in pawn.needs.mood.thoughts.memories.Memories
                  where x.otherPawn != null && pawn.relations.OpinionOf(x.otherPawn) >= def.requiredOpinion
                  select x.otherPawn).TryRandomElementByWeight(delegate (Pawn other)
                  {
                      return pawn.relations.OpinionOf(other);
                  }, out targetPawn))
            {
                return null;
            }

#if DEBUG
            Log.Message("idlejob thinking: " + pawn.Name + " => " + targetPawn.Name);
#endif
            return new Job(IdleJobDefOf.IdleJob_Thinking, targetPawn, destination)
            {
                locomotionUrgency = modSettings.wanderMovePolicy
            };
        }
        
        HashSet<Pair<Room, Building>> rooms = new HashSet<Pair<Room, Building>>();
    }
}
