using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace BountiesMod
{
    public class WorldObjectComp_DebtEnforcement : WorldObjectComp
    {
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
        {
            if (parent is Site site && site.parts.Any(p => p.def == BountiesDefOf.META_DebtEnforcementCamp))
            {
                foreach (var option in CaravanArrivalAction_VisitSiteNoMap.GetFloatMenuOptions(caravan, site))
                {
                    yield return option;
                }
            }
        }

        public override IEnumerable<Gizmo> GetCaravanGizmos(Caravan caravan)
        {
            if (caravan.Tile != parent.Tile || caravan.pather.Moving)
                yield break;

            if (parent is Site site && site.parts.Any(p => p.def == BountiesDefOf.META_DebtEnforcementCamp))
            {
                var negotiator = BestCaravanPawnUtility.FindBestNegotiator(caravan);
                if (negotiator != null)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "META_NegotiateDebt".Translate(),
                        defaultDesc = "META_NegotiateDebt".Translate(),
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/FulfillTradeRequest"),
                        action = () =>
                        {
                            var baseChance = 0.40f;
                            var negotiationStat = negotiator.GetStatValue(StatDefOf.NegotiationAbility);
                            var techLevel = (int)parent.Faction.def.techLevel;
                            var chance = Mathf.Clamp(baseChance + (negotiationStat - 1f) * 0.5f - (techLevel - 3) * 0.1f, 0.05f, 0.95f);
                            if (Rand.Chance(chance))
                            {
                                QuestUtility.SendQuestTargetSignals(parent.questTags, "DiplomacySuccess", parent.Named("SUBJECT"));
                                Find.LetterStack.ReceiveLetter("META_NegotiateSuccess".Translate(), "META_NegotiateSuccessDesc".Translate(), LetterDefOf.PositiveEvent, parent);
                                parent.Destroy();
                            }
                            else
                            {
                                LongEventHandler.QueueLongEvent(delegate
                                {

                                    parent.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -100);
                                    CaravanEnterMapUtility.Enter(caravan, GetOrGenerateMapUtility.GetOrGenerateMap(parent.Tile, parent.def), CaravanEnterMode.Edge);
                                    Find.LetterStack.ReceiveLetter("META_NegotiateFailure".Translate(), "META_NegotiateFailureDesc".Translate(), LetterDefOf.NegativeEvent, parent);
                                }, "GeneratingMap", doAsynchronously: false, GameAndMapInitExceptionHandlers.ErrorWhileGeneratingMap);
                            }
                        }
                    };
                }
                yield return new Command_Action
                {
                    defaultLabel = "META_AssaultDebt".Translate(),
                    defaultDesc = "META_AssaultDebt".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/AttackSettlement"),
                    action = () =>
                    {
                        parent.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -100);
                        CaravanEnterMapUtility.Enter(caravan, GetOrGenerateMapUtility.GetOrGenerateMap(parent.Tile, parent.def), CaravanEnterMode.Edge);
                    }
                };
            }
        }
    }

    public class WorldObjectCompProperties_DebtEnforcement : WorldObjectCompProperties
    {
        public WorldObjectCompProperties_DebtEnforcement()
        {
            compClass = typeof(WorldObjectComp_DebtEnforcement);
        }
    }
}
