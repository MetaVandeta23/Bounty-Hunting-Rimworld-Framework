using LudeonTK;
using RimWorld;
using Verse;

namespace BountiesMod
{
    public static class DebugActions_Bounties
    {
        [DebugAction("Bounties", "Give all bounties to all factions", false, false, false, false, false, 0, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.Playing)]
        private static void GiveAllBountiesToAllFactions()
        {
            var comp = Current.Game.GetComponent<GameComponent_Bounties>();
            foreach (var faction in Find.FactionManager.AllFactionsListForReading)
            {
                var modExtension = faction.def.GetModExtension<BountiesModExtension>();
                if (modExtension != null && modExtension.offersBounties)
                {
                    var collection = new BountyCollection();
                    collection.bounties.AddRange(DefDatabase<BountyDef>.AllDefsListForReading);
                    comp.factionBounties[faction.def.defName] = collection;
                }
            }
        }
    }
}
