using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Harmony;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using RimWorld;

namespace DSFI
{
    public class IdleJobGiverDef : Def
    {
        public Type giverClass;
        public float probabilityWeight;
        public int usefulness;
    }

    public class IdleJobGiverDef_TakeNap : IdleJobGiverDef
    {
        public float restRequirement;
    }

    public class IdleJobGiverDef_WatchDoing : IdleJobGiverDef
    {
        public float searchDistance;
        public JobDef jobDef;
        public Type targetJobDriver;
    }

    public class IdleJobGiverDef_ObservingAnimal : IdleJobGiverDef
    {
        public float searchDistance;
    }

    public class IdleJobGiverDef_LookAroundRoom : IdleJobGiverDef
    {
        public int requiredOpinion;
    }

    public class IdleJobGiverDef_Graffiti : IdleJobGiverDef
    {
        public int requiredOpinion;
    }
}
