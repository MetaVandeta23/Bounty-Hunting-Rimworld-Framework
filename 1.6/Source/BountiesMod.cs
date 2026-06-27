using HarmonyLib;
using Verse;

namespace BountiesMod
{
    public class BountiesModMod : Mod
    {
        public BountiesModMod(ModContentPack pack) : base(pack)
        {
            new Harmony("BountiesModMod").PatchAll();
        }
    }
}
