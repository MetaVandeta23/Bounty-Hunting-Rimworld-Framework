using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using System.Linq;
using UnityEngine;
using Verse;

namespace BountiesMod
{
    [HarmonyPatch(typeof(QuestGen_Rewards), "GiveRewards")]
    public static class QuestGen_Rewards_GiveRewards_Patch
    {
        public static bool shouldChangeRewards;
        public static void Postfix(QuestPart_Choice __result)
        {
            if (shouldChangeRewards is false || __result == null || __result.choices.Count == 0)
                return;
            float silverAmount = QuestGen.slate.Get("bountyReward", 0f);
            if (silverAmount <= 0f)
                return;
            Thing silver = ThingMaker.MakeThing(ThingDefOf.Silver);
            int stackCount = Mathf.Max(1, (int)silverAmount);
            silver.stackCount = stackCount;

            if (__result.choices.Count >= 3)
            {
                var idx = __result.choices.FindIndex(c => c.rewards.Any(r => r is Reward_Goodwill || r is Reward_RoyalFavor));
                __result.choices.RemoveAt(idx >= 0 ? idx : __result.choices.Count - 1);
            }

            var silverChoice = new QuestPart_Choice.Choice();
            Reward_Items silverReward = new Reward_Items();
            silverReward.items.Add(silver);
            silverChoice.rewards.Add(silverReward);
            __result.choices.Add(silverChoice);
        }
    }
}
