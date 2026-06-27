using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;
using Verse.Grammar;

namespace BountiesMod
{
    public class SitePartWorker_BountyPawn : SitePartWorker_Outpost
    {
        public override void Notify_GeneratedByQuestGen(SitePart part, Slate slate, List<Rule> outExtraDescriptionRules, Dictionary<string, string> outExtraDescriptionConstants)
        {
            base.Notify_GeneratedByQuestGen(part, slate, outExtraDescriptionRules, outExtraDescriptionConstants);
            part.things = new ThingOwner<Pawn>(part, oneStackOnly: true);
            if (slate.TryGet<Pawn>("target", out var target))
                part.things.TryAdd(target);
            else if (slate.TryGet<Pawn>("leader", out var leader))
                part.things.TryAdd(leader);
        }
    }
}
