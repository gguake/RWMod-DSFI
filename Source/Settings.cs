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
    public class DSFISettings : ModSettings
    {
        public override void ExposeData()
        {
            Scribe_Values.Look(ref wanderMultiplier, "wanderMultiplier", 1.0f);
            Scribe_Values.Look(ref wanderMovePolicy, "wanderMovePolicy", LocomotionUrgency.Walk);
            base.ExposeData();
        }

        public float wanderMultiplier = 1.0f;
        public LocomotionUrgency wanderMovePolicy = LocomotionUrgency.Walk;
    }

    public class DSFIMod : Mod
    {
        DSFISettings settings;

        public DSFIMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<DSFISettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            // wanderMultiplier
            listing.Label("DSFI_ConfigWanderMultiplier".Translate(), tooltip: "DSFI_TT_ConfigWanderMultiplier".Translate());
            settings.wanderMultiplier = Widgets.HorizontalSlider(listing.GetRect(22f), settings.wanderMultiplier, 0.01f, 2f, false, rightAlignedLabel: string.Format("{0:f2}", settings.wanderMultiplier));

            listing.Gap();

            // wanderMovePolicy
            listing.Label("DSFI_ConfigWanderMovePolicy".Translate());

            bool wanderMoveWalk = settings.wanderMovePolicy == LocomotionUrgency.Walk;
            Widgets.CheckboxLabeled(listing.GetRect(22f), "DSFI_ConfigWanderMovePolicy_Walk".Translate(), ref wanderMoveWalk);
            if (wanderMoveWalk)
            {
                settings.wanderMovePolicy = LocomotionUrgency.Walk;
            }

            bool wanderMoveRun = settings.wanderMovePolicy == LocomotionUrgency.Jog;
            Widgets.CheckboxLabeled(listing.GetRect(22f), "DSFI_ConfigWanderMovePolicy_Run".Translate(), ref wanderMoveRun);
            if (wanderMoveRun)
            {
                settings.wanderMovePolicy = LocomotionUrgency.Jog;
            }
            
            listing.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Do Something for Idle";
        }
    }
}
