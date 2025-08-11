using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
namespace DSFI.Jobs
{
    public class IdleJobDriver_MessingAround : IdleJobDriver
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

            Toil waiting = Toils_General.Wait(2000);
            waiting.socialMode = RandomSocialMode.SuperActive;
            waiting.handlingFacing = true;
            waiting.initAction = () =>
            {
                pawn.Rotation = Rot4.Random;
            };
            waiting.tickAction = () =>
            {
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
