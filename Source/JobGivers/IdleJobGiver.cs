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
    public abstract class IdleJobGiverBase
    {
        public abstract float GetWeight(Pawn pawn, Trait traitIndustriousness);
        public abstract Job TryGiveJob(Pawn pawn);
        public abstract void LoadDef(IdleJobGiverDef def);
    }

    public abstract class IdleJobGiver<T> : IdleJobGiverBase where T : IdleJobGiverDef
    {
        public override float GetWeight(Pawn pawn, Trait traitIndustriousness)
        {
            if (this.def != null)
            {
                if ((pawn.story.DisabledWorkTagsBackstoryAndTraits & this.def.workTagsRequirement) != 0)
                {
                    return 0f;
                }
                
                foreach (var workTypeDef in this.def.workTypeRequirement)
                {
                    if ((pawn.story.DisabledWorkTagsBackstoryAndTraits & workTypeDef.workTags) != 0)
                    {
                        return 0f;
                    }
                }
                
                foreach (var pawnCapacityDef in this.def.pawnCapacityRequirement)
                {
                    if (!pawn.health.capacities.CapableOf(pawnCapacityDef))
                    {
                        return 0f;
                    }
                }

                float bonusMultiplier = 1.0f;
                foreach (var relatedSkill in this.def.relatedSkillPassion)
                {
                    SkillRecord skill = pawn.skills.GetSkill(relatedSkill);
                    if (skill.TotallyDisabled)
                    {
                        return 0f;
                    }
                    else if (skill.passion == Passion.Major)
                    {
                        bonusMultiplier *= 1.5f;
                    }
                    else if (skill.passion == Passion.Minor)
                    {
                        bonusMultiplier *= 1.2f;
                    }
                }
                
                if (traitIndustriousness != null && this.def.usefulness != 0)
                {
                    bonusMultiplier = (4.0f - Math.Abs(traitIndustriousness.Degree - this.def.usefulness)) / 2.0f;
                }

                return this.def.probabilityWeight * bonusMultiplier;
            }
            else
            {
                return 0f;
            }
        }

        public override void LoadDef(IdleJobGiverDef def)
        {
            this.def = def as T;
        }

        protected T def;
    }

    public abstract class IdleJobGiverDefaultDef : IdleJobGiver<IdleJobGiverDef> { }
}
