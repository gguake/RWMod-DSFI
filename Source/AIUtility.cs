using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace DSFI
{
    public static class AIUtility
    {
        public static IntVec3 FindRandomSpotOutsideColony(Pawn pawn, float distance = -1f, bool canReach = false, bool canReserve = false)
        {
            IntVec3 position = IntVec3.Invalid;
            RCellFinder.TryFindRandomSpotJustOutsideColony(pawn.Position, pawn.Map, pawn, out position, (IntVec3 x) =>
            {
                if (!x.InBounds(pawn.Map) || !x.Walkable(pawn.Map))
                {
                    return false;
                }

                if (x.IsForbidden(pawn))
                {
                    return false;
                }

                if (distance > 0f && x.DistanceToSquared(pawn.Position) > distance * distance)
                {
                    return false;
                }

                if (canReach && !pawn.CanReach(x, PathEndMode.OnCell, Danger.None))
                {
                    return false;
                }

                if (canReserve && !pawn.CanReserve(x))
                {
                    return false;
                }

                return true;
            });
            
            return position;
        }

        public static IEnumerable<IntVec3> FindAroundSpotFromTarget(Pawn pawn, IntVec3 target, float maxRadius, float minRadius, bool canSee = true, bool canReach = false, bool canReserve = false)
        {
            return GenRadial.RadialCellsAround(target, maxRadius, true).Where((IntVec3 x) =>
            {
                if (!x.InBounds(pawn.Map) || !x.Walkable(pawn.Map))
                {
                    return false;
                }

                if (x.IsForbidden(pawn))
                {
                    return false;
                }

                if (x.DistanceToSquared(target) <= minRadius * minRadius)
                {
                    return false;
                }

                if (canReach && !pawn.CanReach(x, PathEndMode.OnCell, Danger.None))
                {
                    return false;
                }

                if (canReserve && !pawn.CanReserve(x))
                {
                    return false;
                }

                if (canSee & !GenSight.LineOfSight(target, x, pawn.Map))
                {
                    return false;
                }

                return true;
            });
        }

        public static IntVec3 FindRandomSpotInZone(Pawn pawn, Zone zone, bool canReach = false, bool canReserve = false)
        {
            IntVec3 position = IntVec3.Invalid;
            if (zone.cells.Where((IntVec3 x) =>
            {
                if (!x.InBounds(pawn.Map) || !x.Walkable(pawn.Map))
                {
                    return false;
                }

                if (x.IsForbidden(pawn))
                {
                    return false;
                }

                if (canReach && !pawn.CanReach(x, PathEndMode.OnCell, Danger.None))
                {
                    return false;
                }

                if (canReserve && !pawn.CanReserve(x))
                {
                    return false;
                }

                return true;

            }).TryRandomElement(out position))
            {
                return position;
            }
            else
            {
                return IntVec3.Invalid;
            }
        }
    }
}
