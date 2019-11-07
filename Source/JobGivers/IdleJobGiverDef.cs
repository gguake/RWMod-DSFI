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
        public float searchDistance = 16f;

        public WorkTags workTagsRequirement = WorkTags.None;
        public List<WorkTypeDef> workTypeRequirement = new List<WorkTypeDef>();
        public List<PawnCapacityDef> pawnCapacityRequirement = new List<PawnCapacityDef>();
    }

    public class IdleJobGiverDef_TakeNap : IdleJobGiverDef
    {
        public float restRequirement;
    }

    public class IdleJobGiverDef_WatchDoing : IdleJobGiverDef
    {
        public JobDef jobDef;
        public Type targetJobDriver;
    }

    public class IdleJobGiverDef_LookAroundRoom : IdleJobGiverDef
    {
        public int requiredOpinion;
    }
}
