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
    public class IdleJobDriver_Gardening : IdleJobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch).FailOnForbidden(TargetIndex.A);

            var labelLoopStart = Toils_General.Label();
            var labelLoopEnd = Toils_General.Label();
            
            yield return labelLoopStart;
            yield return Toils_General.Do(() =>
            {
                Zone_Growing zone = this.TargetA.Cell.GetZone(this.Map) as Zone_Growing;
                Thing thing = null;

                if (zone == null || !zone.AllContainedThings.Where(x => x is Plant && !x.HostileTo(this.pawn) && !x.IsForbidden(this.pawn) &&
                    !((Plant)x).HarvestableNow && this.pawn.CanReserveAndReach(x, PathEndMode.Touch, Danger.None) && !listChecked.Contains(x.Position)).TryRandomElement(out thing))
                {
                    this.job.SetTarget(TargetIndex.A, LocalTargetInfo.Invalid);
                }
                else
                {
                    this.Map.reservationManager.Reserve(this.pawn, this.job, thing);
                    this.job.SetTarget(TargetIndex.A, thing);
                }
            });

            yield return Toils_Jump.JumpIf(labelLoopEnd, () => !this.job.targetA.IsValid || this.plantCount >= this.plantWant);
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch).FailOnDestroyedOrNull(TargetIndex.A);

            Toil gardening = new Toil().FailOnDestroyedNullOrForbidden(TargetIndex.A);
            gardening.defaultCompleteMode = ToilCompleteMode.Never;
            gardening.socialMode = RandomSocialMode.SuperActive;
            gardening.WithProgressBar(TargetIndex.A, () => (float)workDone / workTotal);
            gardening.PlaySustainerOrSound(SoundDefOf.Interact_Sow);
            gardening.WithEffect(() => EffecterDefOf.Harvest, TargetIndex.A);
            gardening.initAction = () =>
            {
                this.workDone = 0;
                this.reportState = Rand.Bool ? ReportStringState.A : ReportStringState.B;
            };
            gardening.tickAction = () =>
            {
                this.workDone++;
                this.pawn.skills.Learn(SkillDefOf.Plants, 0.01f);
                if (this.workDone >= workTotal)
                {
                    this.ReadyForNextToil();
                }
            };
            gardening.AddFinishAction(() =>
            {
                this.plantCount++;
                this.Map.reservationManager.Release(this.job.targetA, this.pawn, this.job);

                var plant = this.job.targetA.Thing as Plant;
                plant.Growth += plant.GrowthRate * 2000f / (60000f * plant.def.plant.growDays);

                this.listChecked.Add(this.job.targetA.Cell);
                this.reportState = ReportStringState.None;
            });

            yield return gardening;
            yield return Toils_Jump.Jump(labelLoopStart);
            yield return labelLoopEnd;
            yield break;
        }

        public override string GetReport()
        {
            if (this.reportState == ReportStringState.None)
            {
                Zone zone = this.TargetA.Cell.GetZone(this.Map);
                if (zone != null)
                {
                    return this.job.def.reportString.Replace("TargetA", zone.label);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return base.GetReport();
            }
        }

        private int plantCount = 0;
        private int plantWant = Rand.RangeInclusive(5, 8);
        private int workDone = 0;
        private const int workTotal = 600;

        private List<IntVec3> listChecked = new List<IntVec3>();
    }
}
