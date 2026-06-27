using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;
using Verse.Grammar;

namespace BountiesMod
{
    public class SitePartWorker_BountyItem : SitePartWorker
    {
        public override void Notify_GeneratedByQuestGen(SitePart part, Slate slate, List<Rule> outExtraDescriptionRules, Dictionary<string, string> outExtraDescriptionConstants)
        {
            base.Notify_GeneratedByQuestGen(part, slate, outExtraDescriptionRules, outExtraDescriptionConstants);
            part.things = new ThingOwner<Thing>(part, oneStackOnly: true);
            if (slate.TryGet<Thing>("item", out var item))
                part.things.TryAdd(item);
        }
    }
}
