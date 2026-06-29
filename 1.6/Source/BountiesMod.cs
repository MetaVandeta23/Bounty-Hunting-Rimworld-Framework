using HarmonyLib;
using UnityEngine;
using Verse;

namespace BountiesMod
{
    public class BountiesModMod : Mod
    {
        public static BountiesModSettings settings;

        public BountiesModMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<BountiesModSettings>();
            new Harmony("BountiesModMod").PatchAll();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.Label("META_BountyCostMultiplier".Translate(settings.bountyCostMultiplier.ToString("F2")));
            settings.bountyCostMultiplier = listing.Slider(settings.bountyCostMultiplier, 0.1f, 5f);

            listing.Label("META_BountyRefreshRate".Translate(settings.refreshRateDays.ToString("F1")));
            settings.refreshRateDays = listing.Slider(settings.refreshRateDays, 1f, 15f);

            listing.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory() => Content.Name;
    }
}
