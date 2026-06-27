using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace BountiesMod
{
    public class QuestNode_GenerateRetrievalItem : QuestNode
    {
        public SlateRef<string> storeAs;

        public override void RunInt()
        {
            var defs = DefDatabase<ThingDef>.AllDefsListForReading.Where(d => d.tradeTags != null && d.tradeTags.Contains("ExoticMisc")).ToList();
            var item = ThingMaker.MakeThing(defs.RandomElement());
            QuestGen.slate.Set(storeAs.GetValue(QuestGen.slate), item);
            QuestGen.slate.Set("ItemName", item.LabelShort);
        }
        public override bool TestRunInt(Slate slate) => true;
    }
}
