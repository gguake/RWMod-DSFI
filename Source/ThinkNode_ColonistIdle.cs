using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using RimWorld;

namespace DSFI
{
    public class ThinkNode_ColonistIdle : ThinkNode
    {
        public ThinkNode_ColonistIdle()
        {
            Log.Message("[DSFI] Initialized");
        }

        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            Lord lord = pawn.GetLord();
            if (lord == null)
            {
                IdleJobGiverBase jobGiver = null;
                this.JobGivers.TryRandomElementByWeight((IdleJobGiverBase x) => x.GetWeight(pawn, pawn.story.traits.GetTrait(TraitDefOf.Industriousness)), out jobGiver);
                if (jobGiver != null)
                {
                    Job job = jobGiver.TryGiveJob(pawn);
                    if (job != null)
                    {
#if DEBUG
                        Log.Message(string.Format("idle job {0} to {1}", jobGiver.GetType().Name, pawn.Name));
#endif
                        return new ThinkResult(job, this, null, false);
                    }
#if DEBUG
                    else
                    {
                        Log.Message(string.Format("idle job {0} to {1} but job is null", jobGiver.GetType().Name, pawn.Name));
                    }
#endif
                }
            }

            return wanderJobGiver.TryIssueJobPackage(pawn, new JobIssueParams());
        }

        private List<IdleJobGiverBase> JobGivers
        {
            get
            {
                if (this.cachedJobGivers == null)
                {
                    this.cachedJobGivers = new List<IdleJobGiverBase>();
                    foreach (var def in DefDatabase<IdleJobGiverDef>.AllDefs.ToList())
                    {
                        var jobGiver = Activator.CreateInstance(def.giverClass) as IdleJobGiverBase;
                        if (jobGiver != null)
                        {
                            jobGiver.LoadDef(def);
                            this.cachedJobGivers.Add(jobGiver);
                        }
                        else
                        {
                            Log.Warning("Failed to create IdleJobGiver with named '" + def.giverClass.ToString() + "'. so ignoring that.");
                        }
                    }
                }

                return cachedJobGivers;
            }
        }

        private List<IdleJobGiverBase> cachedJobGivers = null;

        private JobGiver_WanderColony wanderJobGiver = new JobGiver_WanderColony();
    }
}
