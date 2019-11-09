using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Harmony;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

using DSFI.Toils;
namespace DSFI.Jobs
{
    public class IdleJobDriver_PracticeMelee : IdleJobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo target = this.job.GetTarget(TargetIndex.B);
            Job job = this.job;
            return pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
        }

        private FieldInfo fieldJitterer = AccessTools.Field(typeof(Pawn_DrawTracker), "jitterer");
        protected override IEnumerable<Toil> MakeNewToils()
        {
            JitterHandler jitterer = fieldJitterer.GetValue(pawn.Drawer) as JitterHandler;
            yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);

            Toil practice = Toils_General.Wait(2000);
            practice.socialMode = RandomSocialMode.Normal;
            practice.FailOn(() => this.TargetB.Cell.IsForbidden(this.pawn));
            practice.tickAction = () =>
            {
                if (pawn.IsHashIntervalTick(200))
                {
                    pawn.skills.Learn(SkillDefOf.Melee, 5f);
                    if (jitterer != null)
                    {
                        jitterer.AddOffset(Rand.Range(0.25f, 0.75f), pawn.Rotation.AsAngle);
                        if (Rand.Value > 0.7f)
                        {
                            practice.handlingFacing = true;
                            pawn.Rotation = Rot4.Random;
                        }
                    }
                }
            };
            
            yield return practice;
            yield break;
        }

        public override string GetReport()
        {
            return this.job.def.reportString.Replace("TargetA", this.TargetA.Thing.def.LabelCap);
        }
    }
}
