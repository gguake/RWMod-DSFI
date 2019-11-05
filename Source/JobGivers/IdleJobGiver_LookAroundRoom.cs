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
    public class IdleJobGiver_LookAroundRoom : IdleJobGiver<IdleJobGiverDef_LookAroundRoom>
    {
        public override float GetWeight(Pawn pawn, Trait traitIndustriousness)
        {
            if (pawn.story.traits.HasTrait(TraitDefOf.Psychopath))
            {
                return 0f;
            }

            return base.GetWeight(pawn, traitIndustriousness);
        }
        
        public override Job TryGiveJob(Pawn pawn)
        {
            Map map = pawn.Map;
            
            roomOwners.Clear();
            foreach (Pawn owner in pawn.relations.RelatedPawns.Where(x => x.Map == pawn.Map))
            {
                if (pawn.relations.OpinionOf(owner) < this.def.requiredOpinion)
                {
                    continue;
                }

                if (owner.jobs.curDriver != null && owner.jobs.curDriver.asleep)
                {
                    continue;
                }

                if (owner.ownership == null)
                {
                    continue;
                }

                Room room = owner.ownership.OwnedRoom;
                if (room != null && room.Role == RoomRoleDefOf.Bedroom)
                {
                    roomOwners.Add(owner);
                }
            }

            if (roomOwners.Count > 0)
            {
                Pawn owner = roomOwners.RandomElement();
                IntVec3 position = IntVec3.Invalid;

                Room room = owner.ownership.OwnedRoom;
                room.Cells.Where(x => x.Standable(map) && !x.IsForbidden(pawn) &&
                                pawn.CanReserveAndReach(x, PathEndMode.OnCell, Danger.None)).TryRandomElement(out position);

                if (position.IsValid)
                {
                    return new Job(IdleJobDefOf.IdleJob_LookAroundRoom, owner, position)
                    {
                        locomotionUrgency = LocomotionUrgency.Walk
                    };
                }
            }

            return null;
        }

        private HashSet<Pawn> roomOwners = new HashSet<Pawn>();
    }
}
