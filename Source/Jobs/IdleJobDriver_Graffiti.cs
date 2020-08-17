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
    public class IdleJobDriver_Graffiti : IdleJobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(TargetIndex.A), job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch);
            
            Toil doWork = new Toil();
            doWork.defaultCompleteMode = ToilCompleteMode.Never;
            doWork.FailOn(() => !JoyUtility.EnjoyableOutsideNow(this.pawn, null));
            doWork.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            doWork.PlaySustainerOrSound(SoundDefOf.Interact_CleanFilth);
            doWork.initAction = () =>
            {
                this.workLeft = 600f;
            };

            doWork.tickAction = () =>
            {
                if (pawn.IsHashIntervalTick(50))
                {
                    pawn.skills.Learn(SkillDefOf.Artistic, 0.5f);
                }

                this.workLeft -= doWork.actor.GetStatValue(StatDefOf.ConstructionSpeed, true);
                if (this.workLeft <= 0f)
                {
                    Thing thing = ThingMaker.MakeThing(DSFIThingDefOf.DSFI_Scribbling, null);
                    Thing spawnedThing = GenSpawn.Spawn(thing, this.TargetLocA, this.Map, WipeMode.Vanish);
                    spawnedThing.Rotation = Rot4.Random;

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
