using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace BountiesMod
{
    public class QuestNode_SetupBountyContract : QuestNode
    {
        public SlateRef<Pawn> target;
        public SlateRef<Faction> askerFaction;
        public SlateRef<string> forceContractType;
        public SlateRef<MapParent> site;

        public override void RunInt()
        {
            var slate = QuestGen.slate;
            var forceTypeStr = forceContractType.GetValue(slate);
            int type = Rand.RangeInclusive(0, 3);
            if (!string.IsNullOrEmpty(forceTypeStr) && int.TryParse(forceTypeStr, out int parsed))
            {
                type = parsed;
            }

            slate.Set("CaptureAlive", type == 0);
            slate.Set("DeadOrAlive", type == 1);
            slate.Set("AlivePreferred", type == 2);
            slate.Set("DeadPreferred", type == 3);

            var deliveryPart = new QuestPart_BountyDelivery();
            deliveryPart.inSignalEnable = QuestGen.slate.Get<string>("inSignal");
            deliveryPart.askerFaction = askerFaction.GetValue(slate);
            deliveryPart.targetPawn = target.GetValue(slate);
            deliveryPart.targetSite = site.GetValue(slate);
            deliveryPart.captureAlive = type == 0;
            deliveryPart.deadOrAlive = type == 1;
            deliveryPart.alivePreferred = type == 2;
            deliveryPart.deadPreferred = type == 3;

            var outDeliveredAlive = QuestGen.GenerateNewSignal("BountyDeliveredAlive");
            var outDeliveredDead = QuestGen.GenerateNewSignal("BountyDeliveredDead");
            deliveryPart.outSignalDeliveredAlive = outDeliveredAlive;
            deliveryPart.outSignalDeliveredDead = outDeliveredDead;
            QuestGen.quest.AddPart(deliveryPart);

            if (type == 0)
            {
                var targetKilledSignal = QuestGen.GenerateNewSignal("TargetKilled");
                var isDeadPart = new QuestPart_IsDead
                {
                    pawn = target.GetValue(slate),
                    inSignalEnable = QuestGen.slate.Get<string>("inSignal")
                };
                isDeadPart.outSignalsCompleted.Add(targetKilledSignal);
                QuestGen.quest.AddPart(isDeadPart);

                QuestGen.quest.Letter(
                    LetterDefOf.NegativeEvent,
                    inSignal: targetKilledSignal,
                    text: "META_BountyTargetKilledLetterText".Translate(target.GetValue(slate).LabelCap),
                    label: "META_BountyTargetKilledLetter".Translate()
                );
                QuestGen.quest.End(QuestEndOutcome.Fail, 0, null, targetKilledSignal, sendStandardLetter: true);
            }

            if (type == 2)
            {
                var halvePart = new QuestPart_HalveReward
                {
                    inSignal = outDeliveredDead,
                    factor = 0.55f
                };
                QuestGen.quest.AddPart(halvePart);
            }
            
            slate.Set("outDeliveredAlive", outDeliveredAlive);
            slate.Set("outDeliveredDead", outDeliveredDead);
        }
        public override bool TestRunInt(Slate slate) => true;
    }
}
