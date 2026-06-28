using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace BountiesMod
{
    public class WorldObjectComp_Bounties : WorldObjectComp
    {
        public override IEnumerable<Gizmo> GetCaravanGizmos(Caravan caravan)
        {
            if (caravan.Tile != parent.Tile || caravan.pather.Moving)
                yield break;

            if (parent.Faction == null)
                yield break;

            if (!BountyUtilities.OffersBounties(parent.Faction))
                yield break;

            yield return new Command_Action
            {
                defaultLabel = "META_ViewBounties".Translate(),
                defaultDesc = "META_BrowseBounties".Translate(parent.Faction.Name),
                icon = ContentFinder<Texture2D>.Get("UI/META_FulfillBounty"),
                action = () =>
                {
                    Find.WindowStack.Add(new Dialog_Bounty_Caravan(parent.Faction, caravan.RandomOwner(), caravan));
                }
            };

            var parts = Find.QuestManager.QuestsListForReading
                .Where(x => x.State == QuestState.Ongoing)
                .SelectMany(q => q.PartsListForReading.OfType<QuestPart_BountyDelivery>())
                .Where(qp => qp.askerFaction == parent.Faction);
            foreach (var part in parts)
            {
                if (part.targetItemDef != null && CaravanInventoryUtility.HasThings(caravan, part.targetItemDef, 1))
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "META_DeliverBounty".Translate(part.targetItemDef.label),
                        defaultDesc = "META_DeliverBounty".Translate(part.targetItemDef.label),
                        icon = part.targetItemDef.uiIcon,
                        action = () =>
                        {
                            var thing = CaravanInventoryUtility.AllInventoryItems(caravan).First(t => t.def == part.targetItemDef);
                            Find.SignalManager.SendSignal(new Signal(part.outSignalDeliveredAlive));
                            thing.Destroy();
                        }
                    };
                }
                if (part.targetPawn != null)
                {
                    var hasAlive = caravan.PawnsListForReading.Contains(part.targetPawn) && !part.targetPawn.Dead;
                    var hasDead = CaravanInventoryUtility.AllInventoryItems(caravan).OfType<Corpse>().Any(c => c.InnerPawn == part.targetPawn);
                    if (hasAlive && !part.deadPreferred)
                    {
                        yield return new Command_Action
                        {
                            defaultLabel = "META_DeliverBounty".Translate(part.targetPawn.LabelShort),
                            defaultDesc = "META_DeliverBounty".Translate(part.targetPawn.LabelShort),
                            icon = ContentFinder<Texture2D>.Get("UI/META_FulfillBounty"),
                            action = () =>
                            {
                                caravan.RemovePawn(part.targetPawn);
                                Find.SignalManager.SendSignal(new Signal(part.outSignalDeliveredAlive));
                                part.targetPawn.Destroy();
                            }
                        };
                    }
                    if (hasDead && !part.captureAlive)
                    {
                        yield return new Command_Action
                        {
                            defaultLabel = "META_DeliverBounty".Translate(part.targetPawn.LabelShort),
                            defaultDesc = "META_DeliverBounty".Translate(part.targetPawn.LabelShort),
                            icon = ContentFinder<Texture2D>.Get("UI/META_FulfillBounty"),
                            action = () =>
                            {
                                var corpse = CaravanInventoryUtility.AllInventoryItems(caravan).OfType<Corpse>().First(c => c.InnerPawn == part.targetPawn);
                                Find.SignalManager.SendSignal(new Signal(part.outSignalDeliveredDead));
                                corpse.Destroy();
                            }
                        };
                    }
                }
            }
        }
    }

    public class WorldObjectCompProperties_Bounties : WorldObjectCompProperties
    {
        public WorldObjectCompProperties_Bounties()
        {
            compClass = typeof(WorldObjectComp_Bounties);
        }
    }
}
