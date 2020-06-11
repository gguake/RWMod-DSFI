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
    public class IdleJobDriver_TakeNap : IdleJobDriver
    {
        public Building_Bed Bed
        {
            get
            {
                return (Building_Bed)this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (!this.pawn.Reserve(this.Bed, this.job, this.Bed.SleepingSlotsCount, 0, null, errorOnFailed))
            {
                return false;
            }

            return true;
        }

        public override bool CanBeginNowWhileLyingDown()
        {
            return JobInBedUtility.InBedOrRestSpotNow(this.pawn, this.job.GetTarget(TargetIndex.A));
        }

        // A: TargetThing
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Bed.ClaimBedIfNonMedical(TargetIndex.A, TargetIndex.None);
            yield return Toils_Bed.GotoBed(TargetIndex.A);
            this.goalNeedRest = napRestRate + (1 - napRestRate) * pawn.needs.rest.CurLevelPercentage;


            Toil nap = new Toil();
            nap.defaultCompleteMode = ToilCompleteMode.Never;
            nap.FailOnBedNoLongerUsable(TargetIndex.A);
            nap.socialMode = RandomSocialMode.Off;
            nap.initAction = () =>
            {
                pawn.pather.StopDead();
                if (!Bed.OccupiedRect().Contains(pawn.Position))
                {
                    Log.Error("Can't start LayDown toil because pawn is not in the bed. pawn=" + pawn, false);
                    pawn.jobs.EndCurrentJob(JobCondition.Errored, true);
                    return;
                }

                pawn.jobs.posture = PawnPosture.LayingInBed;

                this.asleep = false;
                if (pawn.mindState.applyBedThoughtsTick == 0)
                {
                    pawn.mindState.applyBedThoughtsTick = Find.TickManager.TicksGame + Rand.Range(2500, 10000);
                    pawn.mindState.applyBedThoughtsOnLeave = false;
                }

                if (pawn.ownership != null && pawn.CurrentBed() != pawn.ownership.OwnedBed)
                {
                    ThoughtUtility.RemovePositiveBedroomThoughts(pawn);
                }
            };

            nap.tickAction = () =>
            {
                pawn.GainComfortFromCellIfPossible();

                if (!this.asleep)
                {
                    if (pawn.needs.rest != null && pawn.needs.rest.CurLevel < RestUtility.FallAsleepMaxLevel(pawn))
                    {
                        this.asleep = true;
                    }
                }
                else if (pawn.needs.rest == null || pawn.needs.rest.CurLevelPercentage >= this.goalNeedRest)
                {
                    this.asleep = false;
                }

                if (this.asleep && pawn.needs.rest != null)
                {
                    float restEffectiveness;
                    if (Bed != null && Bed.def.statBases.StatListContains(StatDefOf.BedRestEffectiveness))
                    {
                        restEffectiveness = Bed.GetStatValue(StatDefOf.BedRestEffectiveness, true);
                    }
                    else
                    {
                        restEffectiveness = 0.8f;
                    }

                    pawn.needs.rest.TickResting(restEffectiveness);
                }

                if (pawn.mindState.applyBedThoughtsTick != 0 && pawn.mindState.applyBedThoughtsTick <= Find.TickManager.TicksGame)
                {
                    ApplyBedThoughts(pawn);
                    pawn.mindState.applyBedThoughtsTick += 60000;
                    pawn.mindState.applyBedThoughtsOnLeave = true;
                }

                if (pawn.IsHashIntervalTick(100) && !pawn.Position.Fogged(pawn.Map))
                {
                    if (this.asleep)
                    {
                        MoteMaker.ThrowMetaIcon(pawn.Position, pawn.Map, ThingDefOf.Mote_SleepZ);
                    }

                    if (pawn.health.hediffSet.GetNaturallyHealingInjuredParts().Any<BodyPartRecord>())
                    {
                        MoteMaker.ThrowMetaIcon(pawn.Position, pawn.Map, ThingDefOf.Mote_HealingCross);
                    }
                }

                if (pawn.ownership != null && Bed != null && !Bed.Medical && !Bed.OwnersForReading.Contains(pawn))
                {
                    if (pawn.Downed)
                    {
                        pawn.Position = CellFinder.RandomClosewalkCellNear(pawn.Position, pawn.Map, 1, null);
                    }

                    pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true);
                    return;
                }

                if (pawn.IsHashIntervalTick(211))
                {
                    pawn.jobs.CheckForJobOverride();
                    return;
                }
            };

            nap.AddFinishAction(() =>
            {
                if (pawn.mindState.applyBedThoughtsOnLeave)
                {
                    ApplyBedThoughts(pawn);
                }

                this.asleep = false;
            });

            yield return nap;
            yield break;
        }

        private static void ApplyBedThoughts(Pawn actor)
        {
            if (actor.needs.mood == null)
            {
                return;
            }

            var memories = actor.needs.mood.thoughts.memories;
            Building_Bed building_Bed = actor.CurrentBed();
            memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInBedroom);
            memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInBarracks);
            memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptOutside);
            memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptOnGround);
            memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInCold);
            memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInHeat);

            if (actor.GetRoom(RegionType.Set_Passable).PsychologicallyOutdoors)
            {
                memories.TryGainMemory(ThoughtDefOf.SleptOutside, null);
            }

            if (building_Bed == null || building_Bed.CostListAdjusted().Count == 0)
            {
                memories.TryGainMemory(ThoughtDefOf.SleptOnGround, null);
            }

            if (actor.AmbientTemperature < actor.def.GetStatValueAbstract(StatDefOf.ComfyTemperatureMin, null))
            {
                memories.TryGainMemory(ThoughtDefOf.SleptInCold, null);
            }

            if (actor.AmbientTemperature > actor.def.GetStatValueAbstract(StatDefOf.ComfyTemperatureMax, null))
            {
                memories.TryGainMemory(ThoughtDefOf.SleptInHeat, null);
            }

            if (building_Bed != null && building_Bed == actor.ownership.OwnedBed && !building_Bed.ForPrisoners && !actor.story.traits.HasTrait(TraitDefOf.Ascetic))
            {
                ThoughtDef thoughtDef = null;
                if (building_Bed.GetRoom(RegionType.Set_Passable).Role == RoomRoleDefOf.Bedroom)
                {
                    thoughtDef = ThoughtDefOf.SleptInBedroom;
                }
                else if (building_Bed.GetRoom(RegionType.Set_Passable).Role == RoomRoleDefOf.Barracks)
                {
                    thoughtDef = ThoughtDefOf.SleptInBarracks;
                }

                if (thoughtDef != null)
                {
                    int scoreStageIndex = RoomStatDefOf.Impressiveness.GetScoreStageIndex(building_Bed.GetRoom(RegionType.Set_Passable).GetStat(RoomStatDefOf.Impressiveness));
                    if (thoughtDef.stages[scoreStageIndex] != null)
                    {
                        memories.TryGainMemory(ThoughtMaker.MakeThought(thoughtDef, scoreStageIndex), null);
                    }
                }
            }
        }

        private float goalNeedRest;

        private const float napRestRate = 0.5f;
    }
}
