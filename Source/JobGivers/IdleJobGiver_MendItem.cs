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
    public class IdleJobGiver_MendItem : IdleJobGiver<IdleJobGiverDef>
    {
        public override float GetWeight(Pawn pawn, Trait traitIndustriousness)
        {
            if (pawn.story.WorkTypeIsDisabled(WorkTypeDefOf.Crafting))
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
            Region region = pawn.GetRegion(RegionType.Set_Passable);
            if (region == null)
            {
                return null;
            }

            targets.Clear();
            foreach (var zone in pawn.Map.zoneManager.AllZones.Where(x => x is Zone_Stockpile))
            {
                foreach (var thing in zone.AllContainedThings)
                {
                    Apparel apparel = thing as Apparel;
                    if (apparel == null || apparel.WornByCorpse)
                    {
                        continue;
                    }

                    if (!apparel.IsInValidBestStorage())
                    {
                        continue;
                    }

                    if (apparel.HitPoints >= apparel.MaxHitPoints)
                    {
                        continue;
                    }

                    if (!pawn.CanReserveAndReach(apparel, PathEndMode.Touch, Danger.None))
                    {
                        continue;
                    }
                    
                    if (!apparel.def.stuffCategories.Any(x => x.defName == "Fabric" || x.defName == "Leathery"))
                    {
                        continue;
                    }

                    targets.Add(apparel);
                }
            }

            if (targets.Count > 0)
            {
                var target = targets.RandomElement();
                return new Job(IdleJobDefOf.IdleJob_MendItem, target)
                {
                    locomotionUrgency = LocomotionUrgency.Walk
                };
            }

            return null;
        }

        private List<Thing> targets = new List<Thing>();
    }
}
