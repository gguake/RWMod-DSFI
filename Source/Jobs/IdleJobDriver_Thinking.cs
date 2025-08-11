using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
namespace DSFI.Jobs
{
    public class IdleJobDriver_Thinking : IdleJobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(TargetIndex.B), job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
            pawn.Rotation = Rot4.Random;

            Toil waiting = Toils_General.Wait(job.def.joyDuration);
            waiting.socialMode = RandomSocialMode.Quiet;
            waiting.FailOn(() => !MeditationUtility.CanMeditateNow(pawn) || !MeditationUtility.SafeEnvironmentalConditions(pawn, TargetB.Cell, base.Map));
            waiting.tickIntervalAction = (delta) =>
            {
                pawn.GainComfortFromCellIfPossible(delta);
                if (pawn.needs.joy != null)
                {
                    JoyUtility.JoyTickCheckEnd(pawn, delta, JoyTickFullJoyAction.None, 0.2f);
                }

                waiting.handlingFacing = true;
                if (pawn.IsHashIntervalTick(400))
                {
                    MakeThinkingBubble(pawn, TargetA.Pawn);
                }
            };

            yield return waiting;
            yield break;
        }

        // TODO
        public RTMoteBubble MakeThinkingBubble(Pawn pawn, Pawn target)
        {
            RTMoteBubble obj = (RTMoteBubble)ThingMaker.MakeThing(MoteDefOf.DSFI_Mote_Thought);
            obj.SetupMoteBubble(job.targetA.Pawn);
            obj.Attach(pawn);
            GenSpawn.Spawn(obj, pawn.Position, pawn.Map);
            return obj;
        }

    }
}
