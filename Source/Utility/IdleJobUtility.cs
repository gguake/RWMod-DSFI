using RimWorld;
using Verse;
using Verse.AI;

namespace DSFI
{
    public static class IdleJobUtility
    {
        public static IntVec3 FindNearSittableForIdle(Pawn pawn)
        {
            if (!pawn.IsColonist) { return IntVec3.Invalid; }

            var map = pawn.Map;
            if (map == null) { return IntVec3.Invalid; }

            var thing = GenClosest.ClosestThingReachable(
                pawn.Position, 
                pawn.Map, 
                ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), 
                PathEndMode.OnCell, 
                TraverseParms.For(pawn), 
                15f, 
                (Thing t) => ValidateChair(t, pawn) && 
                t.Position.GetDangerFor(pawn, t.Map) == Danger.None);

            if (thing != null)
            {
                if (TryFindFreeSittingSpotOnThing(thing, pawn, true, out var cell))
                {
                    return cell;
                }
            }

            return IntVec3.Invalid;
        }

        private static bool ValidateChair(Thing t, Pawn pawn)
        {
            if (t.def.building == null || !t.def.building.isSittable)
            {
                return false;
            }
            if (!TryFindFreeSittingSpotOnThing(t, pawn, true, out var _))
            {
                return false;
            }
            if (t.Fogged())
            {
                return false;
            }
            if (t.IsForbidden(pawn))
            {
                return false;
            }
            if (!pawn.CanReserve(t))
            {
                return false;
            }
            if (!t.IsSociallyProper(pawn))
            {
                return false;
            }
            if (t.IsBurning())
            {
                return false;
            }
            if (t.HostileTo(pawn))
            {
                return false;
            }
            return true;
        }

        private static bool TryFindFreeSittingSpotOnThing(Thing t, Pawn pawn, bool skipInteractionCells, out IntVec3 cell)
        {
            foreach (var item in t.OccupiedRect())
            {
                if (!item.IsBuildingInteractionCell(pawn.Map) && !item.Fogged(pawn.Map) && pawn.CanReserveSittableOrSpot(item))
                {
                    cell = item;
                    return true;
                }
            }

            cell = default;
            return false;
        }
    }
}
