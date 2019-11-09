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
    public class IdleJobDriver_LookAroundRoom : IdleJobDriver
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
            this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);

            Toil looking = Toils_General.Wait(1500);
            looking.FailOn(() => this.TargetB.Cell.IsForbidden(this.pawn));
            looking.tickAction = () =>
            {
                
                looking.handlingFacing = true;
                if (pawn.IsHashIntervalTick(250))
                {
                    if (pawn.Rotation == Rot4.North)
                    {
                        pawn.Rotation = Rot4.East;
                    }
                    else if (pawn.Rotation == Rot4.East)
                    {
                        pawn.Rotation = Rot4.South;
                    }
                    else if (pawn.Rotation == Rot4.South)
                    {
                        pawn.Rotation = Rot4.West;
                    }
                    else
                    {
                        pawn.Rotation = Rot4.North;
                    }
                }
            };

            looking.AddFinishAction(() =>
            {
                Pawn owner = TargetA.Thing as Pawn;
                pawn.interactions.TryInteractWith(owner, InteractionDefOf.Chitchat);
            });

            yield return looking;
            yield break;
        }
    }
}
