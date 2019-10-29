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
    public class IdleJobDriver_CleaningGun : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedNullOrForbidden(TargetIndex.A);

            Toil mending = new Toil();
            mending.defaultCompleteMode = ToilCompleteMode.Never;
            mending.socialMode = RandomSocialMode.SuperActive;
            mending.WithProgressBar(TargetIndex.A, () => workDone / mendingWorks);
            mending.PlaySustainerOrSound(SoundDefOf.Interact_CleanFilth);
            mending.FailOn(() => this.pawn.Position.IsForbidden(this.pawn));
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
