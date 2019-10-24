using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace DSFI.Toils
{
    public static class DSFIToils_Moving
    {
        public static Toil GotoNearTarget(TargetIndex target, Danger danger, float moveDistance, float lookDistance)
        {
            Toil toil = new Toil();
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.initAction = delegate ()
            {
                Pawn pawn = toil.actor;
                Pawn targetPawn = pawn.CurJob.GetTarget(target).Thing as Pawn;
                if (pawn.Position.InHorDistOf(targetPawn.Position, lookDistance))
                {
                    pawn.jobs.curDriver.ReadyForNextToil();
                }
                else
                {
                    pawn.pather.StartPath(targetPawn, PathEndMode.OnCell);
                }
            };

            toil.tickAction = delegate ()
            {
                Pawn pawn = toil.actor;
                Pawn targetPawn = pawn.CurJob.GetTarget(target).Thing as Pawn;
                Map map = pawn.Map;
                if (pawn.Position.InHorDistOf(targetPawn.Position, lookDistance))
                {
                    pawn.pather.StopDead();
                    pawn.jobs.curDriver.ReadyForNextToil();
                }
                else if (!pawn.pather.Moving)
                {
                    IntVec3 destVec = IntVec3.Invalid;
                    
                    foreach (IntVec3 t in GenRadial.RadialPatternInRadius(moveDistance))
                    {
                        IntVec3 v = targetPawn.Position + t;
                        if (v.InBounds(map) && v.Walkable(map) && v != pawn.Position && 
                            pawn.CanReach(v, PathEndMode.OnCell, danger) && 
                            (!destVec.IsValid || pawn.Position.DistanceToSquared(v) >= pawn.Position.DistanceToSquared(destVec)) &&
                            pawn.CanSee(targetPawn))
                        {
                            destVec = v;
                        }
                    }

                    if (destVec.IsValid)
                    {
                        pawn.pather.StartPath(destVec, PathEndMode.OnCell);
                    }
                    else
                    {
                        pawn.jobs.curDriver.EndJobWith(JobCondition.Incompletable);
                    }
                }
            };
            
            return toil;
        }

        public static Toil GotoNearTargetAndWait(TargetIndex target, float moveDistance, float lookDistance)
        {
            Toil toil = new Toil();
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.initAction = delegate ()
            {
                Pawn pawn = toil.actor;
                Pawn targetPawn = pawn.CurJob.GetTarget(target).Thing as Pawn;
                if (pawn.Position.InHorDistOf(targetPawn.Position, lookDistance))
                {
                    pawn.jobs.curDriver.ReadyForNextToil();
                }
                else
                {
                    pawn.pather.StartPath(targetPawn, PathEndMode.OnCell);
                }
            };

            toil.tickAction = delegate ()
            {
                Pawn pawn = toil.actor;
                Pawn targetPawn = pawn.CurJob.GetTarget(target).Thing as Pawn;
                Map map = pawn.Map;
                if (pawn.Position.InHorDistOf(targetPawn.Position, moveDistance))
                {
                    pawn.pather.StopDead();
                    pawn.jobs.curDriver.ReadyForNextToil();
                }
                else if (!pawn.pather.Moving)
                {
                    IntVec3 destVec = IntVec3.Invalid;

                    foreach (IntVec3 t in GenRadial.RadialPatternInRadius(moveDistance))
                    {
                        IntVec3 v = targetPawn.Position + t;
                        if (v.InBounds(map) && v.Walkable(map) && v != pawn.Position &&
                            pawn.CanReach(v, PathEndMode.OnCell, Danger.Some) &&
                            (!destVec.IsValid || pawn.Position.DistanceToSquared(v) >= pawn.Position.DistanceToSquared(destVec)) &&
                            pawn.CanSee(targetPawn))
                        {
                            destVec = v;
                        }
                    }

                    if (destVec.IsValid)
                    {
                        pawn.pather.StartPath(destVec, PathEndMode.OnCell);
                    }
                    else
                    {
                        pawn.jobs.curDriver.EndJobWith(JobCondition.Incompletable);
                    }
                }
            };

            return toil;
        }
    }
}
