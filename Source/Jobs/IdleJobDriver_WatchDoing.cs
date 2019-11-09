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
    public class IdleJobDriver_WatchDoing : IdleJobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Pawn targetPawn = this.job.targetA.Thing as Pawn;
            this.watchingTicks = Rand.Range(watchingTicksMin, watchingTicksMax);
            
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            this.learnings.Clear();
            if (targetPawn.CurJob != null)
            {
                if (targetPawn.CurJob.RecipeDef != null && targetPawn.CurJob.RecipeDef.requiredGiverWorkType != null)
                {
                    List<SkillDef> skillDefs = targetPawn.CurJob.RecipeDef.requiredGiverWorkType.relevantSkills;
                    foreach (SkillDef skillDef in skillDefs)
                    {
                        this.learnings.Add(new Pair<SkillDef, float>(skillDef,
                            (targetPawn.skills.GetSkill(skillDef).Level / (float)SkillRecord.MaxLevel * xp) / skillDefs.Count()));
                    }
                }
                else if (targetPawn.CurJob.def.defName == "Research")
                {
                    this.learnings.Add(new Pair<SkillDef, float>(SkillDefOf.Intellectual, targetPawn.skills.GetSkill(SkillDefOf.Intellectual).Level / (float)SkillRecord.MaxLevel * xp));
                }
            }

            Toil watching = new Toil().FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            watching.defaultCompleteMode = ToilCompleteMode.Never;
            watching.socialMode = RandomSocialMode.SuperActive;
            watching.tickAction = () =>
            {
                ticks++;
                
                if (pawn.IsHashIntervalTick(20))
                {
                    foreach (var x in this.learnings)
                    {
                        pawn.skills.Learn(x.First, x.Second);
                    }
                }
                else if (ticks > watchingTicks)
                {
                    this.ReadyForNextToil();
                }
            };

            yield return watching;
            yield break;
        }
        
        private const float xp = 10f;

        private List<Pair<SkillDef, float>> learnings = new List<Pair<SkillDef, float>>();
        private float ticks;
        private float watchingTicks;
        private const int watchingTicksMin = 1000;
        private const int watchingTicksMax = 1500;
    }
}
