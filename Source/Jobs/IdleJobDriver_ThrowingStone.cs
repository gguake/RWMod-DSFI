using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

using DSFI.Toils;
namespace DSFI.Jobs
{
    public class IdleJobDriver_ThrowingStone : IdleJobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo target = this.job.GetTarget(TargetIndex.B);
            Job job = this.job;
            return pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
            
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);

            Toil throwing = Toils_General.Wait(2000, TargetIndex.A);
            throwing.socialMode = RandomSocialMode.Normal;
            throwing.FailOn(() => !JoyUtility.EnjoyableOutsideNow(this.pawn, null) || this.TargetB.Cell.IsForbidden(this.pawn));
            throwing.handlingFacing = true;
            throwing.initAction = () =>
            {
                pawn.rotationTracker.FaceCell(TargetLocA);
            };

            throwing.tickAction = () =>
            {
                if (this.pawn.IsHashIntervalTick(400))
                {
                    pawn.skills.Learn(SkillDefOf.Shooting, 5f);
                    MoteMaker.ThrowStone(pawn, TargetLocA);
                }
            };

            yield return throwing;
            yield break;
        }
    }
}
