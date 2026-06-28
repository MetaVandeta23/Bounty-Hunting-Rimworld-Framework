using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BountiesMod
{
    public class GameComponent_Bounties : GameComponent
    {
        public int tickCounter = 0;
        public int ticksPerBountyRefresh = 3 * 60000;
        public Dictionary<string, BountyCollection> factionBounties = new Dictionary<string, BountyCollection>();

        public GameComponent_Bounties(Game game)
        {}

        public override void StartedNewGame()
        {
            GenerateBounties();
        }

        public override void GameComponentTick()
        {
            tickCounter++;
            if (tickCounter >= ticksPerBountyRefresh)
            {
                tickCounter = 0;
                GenerateBounties();
            }
        }

        public void GenerateBounties()
        {
            factionBounties.Clear();
            foreach (var faction in Find.FactionManager.AllFactionsListForReading)
            {
                if (BountyUtilities.OffersBounties(faction))
                {
                    var collection = new BountyCollection
                    {
                        bounties =
                        {
                            DefDatabase<BountyDef>.AllDefsListForReading.RandomElement(),
                            DefDatabase<BountyDef>.AllDefsListForReading.RandomElement(),
                            DefDatabase<BountyDef>.AllDefsListForReading.RandomElement()
                        }
                    };
                    factionBounties[faction.def.defName] = collection;
                }
            }
        }

        public BountyCollection GetBountiesForFaction(Faction faction)
        {
            if (factionBounties.TryGetValue(faction.def.defName, out BountyCollection collection))
            {
                return collection;
            }
            return null;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref tickCounter, "tickCounter", 0);
            Scribe_Collections.Look(ref factionBounties, "factionBounties", LookMode.Value, LookMode.Deep);
        }
    }
}
