using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

namespace BountiesMod
{
    [HarmonyPatch(typeof(FactionDialogMaker))]
    [HarmonyPatch("FactionDialogFor")]
    public static class FactionDialogFor_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref DiaNode __result, Pawn negotiator, Faction faction)
        {
            if (faction.IsPlayer || faction.HostileTo(Faction.OfPlayer))
                return;

            BountiesModExtension modExtension = faction.def.GetModExtension<BountiesModExtension>();

            if (modExtension == null || !modExtension.offersBounties)
                return;

            DiaOption bountyOption = new DiaOption("META_BrowseBounties".Translate(faction.Name))
            {
                action = () =>
                {
                    Find.WindowStack.Add(new Dialog_Bounty(faction, negotiator));
                }
            };

            __result.options.Insert(Mathf.Max(0, __result.options.Count - 1), bountyOption);
        }
    }
}