using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Harmony;
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
            base.ExposeData();
        }

        public float wanderMultiplier = 1.0f;
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
            listing.Label("DSFI_ConfigWanderMultiplier".Translate(), tooltip: "DSFI_TT_ConfigWanderMultiplier".Translate());

            settings.wanderMultiplier = Widgets.HorizontalSlider(listing.GetRect(22f), settings.wanderMultiplier, 0.01f, 2f, false, rightAlignedLabel: string.Format("{0:f2}", settings.wanderMultiplier));
            listing.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Do Something for Idle";
        }
    }
}
