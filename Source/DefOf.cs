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
    [DefOf]
    public static class IdleJobDefOf
    {
        public static JobDef IdleJob_TakeNap;
        public static JobDef IdleJob_MendItem;
        public static JobDef IdleJob_ObservingAnimal;
        public static JobDef IdleJob_MessingAround;
        public static JobDef IdleJob_LookAroundRoom;
        public static JobDef IdleJob_PracticeMelee;
        public static JobDef IdleJob_ThrowingStone;
        public static JobDef IdleJob_Graffiti;
        public static JobDef IdleJob_CleaningGun;
    }

    [DefOf]
    public static class MoreTraitDefOf
    {
        public static TraitDef QuickSleeper;
    }
}
