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
    public class IdleJobDriver_ObservingAnimal : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Pawn target = this.TargetA.Thing as Pawn;
            float xp = 5f * Mathf.Pow(5f, target.RaceProps.wildness);

            this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
            yield return DSFIToils_Moving.GotoNearTarget(TargetIndex.A, Danger.None, moveDistance, lookDistance);

            Toil observing = Toils_General.Wait(1500);
            observing.FailOnDestroyedOrNull(TargetIndex.A);
            observing.socialMode = RandomSocialMode.SuperActive;
            observing.handlingFacing = true;
            observing.FailOn(() => !JoyUtility.EnjoyableOutsideNow(this.pawn, null) || this.pawn.Position.IsForbidden(this.pawn));
            observing.tickAction = () =>
            {
                if (pawn.IsHashIntervalTick(20))
                {
                    pawn.skills.Learn(SkillDefOf.Animals, 5f);
                    pawn.rotationTracker.FaceTarget(target);

                    if (!pawn.CanSee(target) || !pawn.Position.InHorDistOf(target.Position, lookDistance))
                    {
                        this.ReadyForNextToil();
                    }
                }
            };

            yield return observing;
            yield break;
        }
        
        private const float moveDistance = 2f;
        private const float lookDistance = 6f;
    }
}
