using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace BountiesMod
{
    public class QuestNode_SetupBountyDelivery : QuestNode
    {
        public SlateRef<Thing> targetItem;
        public SlateRef<Faction> askerFaction;
        public SlateRef<MapParent> site;

        public override void RunInt()
        {
            var part = new QuestPart_BountyDelivery();
            part.inSignalEnable = QuestGen.slate.Get<string>("inSignal");
            part.askerFaction = askerFaction.GetValue(QuestGen.slate);
            part.targetItemDef = targetItem.GetValue(QuestGen.slate).def;
            part.targetSite = site.GetValue(QuestGen.slate);
            part.outSignalDeliveredAlive = QuestGen.GenerateNewSignal("ItemDelivered");
            QuestGen.quest.AddPart(part);
            QuestGen.slate.Set("outDelivered", part.outSignalDeliveredAlive);
        }
        public override bool TestRunInt(Slate slate) => true;
    }
}
