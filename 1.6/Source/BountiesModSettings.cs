using Verse;

namespace BountiesMod
{
    public class BountiesModSettings : ModSettings
    {
        public float bountyCostMultiplier = 1f;
        public float refreshRateDays = 3f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref bountyCostMultiplier, "bountyCostMultiplier", 1f);
            Scribe_Values.Look(ref refreshRateDays, "refreshRateDays", 3f);
        }
    }
}
