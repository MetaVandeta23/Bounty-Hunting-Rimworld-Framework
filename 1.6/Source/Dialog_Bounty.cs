using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BountiesMod
{
    public class Dialog_Bounty : Window
    {
        public Faction faction;
        public Pawn pawn;

        public override Vector2 InitialSize => new Vector2(800f, 400f);

        public Dialog_Bounty(Faction faction, Pawn pawn)
        {
            this.faction = faction;
            this.pawn = pawn;
            doCloseX = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            float bountyCost = BountyUtilities.CalculateBountyCost(pawn);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.Label("META_BrowseBounties_Description".Translate(pawn.LabelShort, faction.Name, bountyCost));
            listing.GapLine();

            GameComponent_Bounties comp = Current.Game.GetComponent<GameComponent_Bounties>();
            BountyCollection bountyCollection = comp.GetBountiesForFaction(faction);

            if (bountyCollection != null)
            {
                foreach (BountyDef bounty in bountyCollection.bounties.ToList())
                {
                    listing.Gap();

                    if (listing.ButtonTextLabeledPct(bounty.label, "META_Accept".Translate(), 0.8f))
                    {
                        if (!CanAffordBounty((int)bountyCost))
                        {
                            Messages.Message("META_CannotAffordBounty".Translate((int)bountyCost), MessageTypeDefOf.RejectInput);
                        }
                        else
                        {
                            ConsumeSilver((int)bountyCost);

                            QuestPart_InvolvedFactions involvedFactions = new QuestPart_InvolvedFactions();
                            involvedFactions.factions.Add(faction);

                            Slate slate = new Slate();
                            slate.Set("faction", faction);
                            slate.Set("faction_name", faction.Name);
                            slate.Set("asker", faction.leader);
                            slate.Set("points", 100f);

                            Quest quest = QuestUtility.GenerateQuestAndMakeAvailable(bounty.firesQuest, slate);
                            quest.AddPart(involvedFactions);
                            QuestUtility.SendLetterQuestAvailable(quest);

                            comp.factionBounties[faction.def.defName].bounties.Remove(bounty);
                            Close();
                        }
                    }

                    listing.SubLabel(bounty.description, 0.6f);
                    listing.Gap();
                }
            }
            else
            {
                listing.Label("META_NoBounties".Translate(faction.Name));
            }

            listing.End();
        }

        public virtual bool CanAffordBounty(int amount)
        {
            Map map = Find.CurrentMap;

            if (map == null)
                return false;

            int totalSilverOnMap = map.listerThings.ThingsOfDef(ThingDefOf.Silver).Sum(thing => thing.stackCount);
            return totalSilverOnMap >= amount;
        }

        // a better modder would have merged CanAffordBounty and ConsumeSilver into one bool function that checks whether a colony can afford a bounty but also consumes the bountyCost if they can, so you can do something like 'if (TryConsumeBountyCost(amount)) fire quest'
		// I am not a better modder
		
        // this and the caravan version are just differently wired versions of the same thing. I'd have used a for each loop here myself but the code I copied here from vanilla trading stuff worked pretty cleanly, so
        public virtual void ConsumeSilver(int amount)
        {
            Map map = Find.CurrentMap;

            if (map == null)
                return;

            TransferableUtility.TransferNoSplit(map.listerThings.ThingsOfDef(ThingDefOf.Silver).ToList(), amount,
                (thing, count) =>
                {
                    thing.SplitOff(count).Destroy(DestroyMode.Vanish);
                });
        }
    }


    public class Dialog_Bounty_Caravan : Dialog_Bounty
    {
        public Caravan caravan;

        public Dialog_Bounty_Caravan(Faction faction, Pawn pawn, Caravan caravan) : base(faction, pawn)
        {
            this.caravan = caravan;
        }

        public override bool CanAffordBounty(int amount)
        {
            if (caravan == null || caravan.pawns == null)
                return false;

            int totalSilverInCaravan = CaravanInventoryUtility.AllInventoryItems(caravan).Where(thing => thing.def == ThingDefOf.Silver).Sum(thing => thing.stackCount);

            return totalSilverInCaravan >= amount;
        }

        public override void ConsumeSilver(int amount)
        {
            if (caravan == null)
                return;

            TransferableUtility.TransferNoSplit(CaravanInventoryUtility.AllInventoryItems(caravan).Where(thing => thing.def == ThingDefOf.Silver).ToList(), amount,
                (thing, count) =>
                {
                    Thing taken = thing.SplitOff(count);
                    taken.Destroy(DestroyMode.Vanish);
                });

            caravan.RecacheInventory();
        }
    }
}
