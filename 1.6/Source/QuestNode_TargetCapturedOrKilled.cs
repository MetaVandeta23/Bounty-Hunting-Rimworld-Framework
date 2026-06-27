using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace BountiesMod
{
    public class QuestNode_TargetCapturedOrKilled : QuestNode
    {
        public SlateRef<Pawn> target;
        public SlateRef<string> inSignalEnable;
        public SlateRef<string> outSignal;

        public override void RunInt()
        {
            var p = target.GetValue(QuestGen.slate);
            var inSig = inSignalEnable.GetValue(QuestGen.slate);
            var outSig = outSignal.GetValue(QuestGen.slate);
            var isDeadPart = new QuestPart_IsDead { pawn = p, inSignalEnable = inSig };
            isDeadPart.outSignalsCompleted.Add(outSig);
            QuestGen.quest.AddPart(isDeadPart);
            var isPrisonerPart = new QuestPart_IsPrisoner { pawn = p, inSignalEnable = inSig };
            isPrisonerPart.outSignalsCompleted.Add(outSig);
            QuestGen.quest.AddPart(isPrisonerPart);
        }
        public override bool TestRunInt(Slate slate) => true;
    }
}
