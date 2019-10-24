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
    public class IdleJobDriver_MendItem : JobDriver
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
            this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            Toil mending = new Toil();
            mending.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            mending.defaultCompleteMode = ToilCompleteMode.Never;
            mending.socialMode = RandomSocialMode.SuperActive;
            mending.WithProgressBar(TargetIndex.A, () => workDone / mendingWorks);
            mending.PlaySustainerOrSound(this.job.targetA.Thing.def.recipeMaker.soundWorking);
            mending.tickAction = () =>
            {
                workDone++;
                if (workDone >= mendingWorks)
                {
                    var thing = this.job.targetA.Thing;
                    float repairRateMin = 1f / SkillRecord.MaxLevel * 0.1f;
                    float repairRateMax = Math.Max(1, pawn.skills.GetSkill(SkillDefOf.Crafting).Level) / (float)SkillRecord.MaxLevel * 0.1f;
                    float repairRate = Rand.Range(repairRateMin, repairRateMax);
                    thing.HitPoints = Math.Min(thing.MaxHitPoints, thing.HitPoints + (int)(thing.MaxHitPoints * repairRate));
                    this.ReadyForNextToil();
                }
            };

            yield return mending;
            yield break;
        }

        private float workDone = 0f;
        private const float mendingWorks = 1800f;
    }
}
