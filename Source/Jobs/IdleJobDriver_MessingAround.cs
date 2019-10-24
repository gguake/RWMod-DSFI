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
    public class IdleJobDriver_MessingAround : JobDriver
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
            Building building = TargetA.Thing as Building;
            
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            pawn.Rotation = Rot4.Random;

            Toil waiting = Toils_General.Wait(2000);
            waiting.socialMode = RandomSocialMode.SuperActive;
            waiting.tickAction = () =>
            {
                waiting.handlingFacing = true;
                if (pawn.IsHashIntervalTick(150))
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

            yield return waiting;
            yield break;
        }
    }
}
