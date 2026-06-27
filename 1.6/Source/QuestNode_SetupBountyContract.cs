using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace BountiesMod
{
    public class QuestNode_SetupBountyContract : QuestNode
    {
        public SlateRef<Pawn> target;
        public SlateRef<Faction> askerFaction;

        public override void RunInt()
        {
            var slate = QuestGen.slate;
            var type = Rand.RangeInclusive(0, 3);
            slate.Set("CaptureAlive", type == 0);
            slate.Set("DeadOrAlive", type == 1);
            slate.Set("AlivePreferred", type == 2);
            slate.Set("DeadPreferred", type == 3);

            var rewardValue = slate.Get("rewardValue", 1000f);
            var rewardAlive = rewardValue;
            var rewardDead = rewardValue;

            if (type == 0)
                rewardDead = 0f;
            else if (type == 2)
                rewardDead = rewardValue * 0.55f;
            else if (type == 3)
                rewardAlive = 0f;

            slate.Set("rewardValueAlive", rewardAlive);
            slate.Set("rewardValueDead", rewardDead);

            var deliveryPart = new QuestPart_BountyDelivery();
            deliveryPart.inSignalEnable = QuestGen.slate.Get<string>("inSignal");
            deliveryPart.askerFaction = askerFaction.GetValue(slate);
            deliveryPart.targetPawn = target.GetValue(slate);
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

            slate.Set("outDeliveredAlive", outDeliveredAlive);
            slate.Set("outDeliveredDead", outDeliveredDead);
        }
        public override bool TestRunInt(Slate slate) => true;
    }
}
