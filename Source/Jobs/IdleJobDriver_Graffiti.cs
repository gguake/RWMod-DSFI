using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace DSFI.Jobs
{
    public class IdleJobDriver_Graffiti : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo target = this.job.GetTarget(TargetIndex.A);
            Job job = this.job;
            return pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch);
            
            Toil doWork = new Toil();
            doWork.defaultCompleteMode = ToilCompleteMode.Never;
            doWork.FailOn(() => !JoyUtility.EnjoyableOutsideNow(this.pawn, null));
            doWork.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            doWork.initAction = delegate ()
            {
                this.workLeft = 1800f;
            };

            doWork.tickAction = delegate ()
            {
                if (pawn.IsHashIntervalTick(20))
                {
                    pawn.skills.Learn(SkillDefOf.Artistic, 5f);
                }

                this.workLeft -= doWork.actor.GetStatValue(StatDefOf.ConstructionSpeed, true);
                if (this.workLeft <= 0f)
                {
                    Thing thing = ThingMaker.MakeThing(ThingDefOf.Snowman, null);
                    thing.SetFaction(this.pawn.Faction, null);
                    GenSpawn.Spawn(thing, this.TargetLocA, this.Map, WipeMode.Vanish);
                    this.ReadyForNextToil();
                    return;
                }
            };

            yield return doWork;
            yield break;
        }

        private float workLeft;
    }
}
