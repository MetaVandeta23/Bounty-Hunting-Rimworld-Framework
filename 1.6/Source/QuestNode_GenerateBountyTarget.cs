using RimWorld;
using RimWorld.QuestGen;
using System.Linq;
using Verse;

namespace BountiesMod
{
    public class QuestNode_GenerateBountyTarget : QuestNode
    {
        public SlateRef<string> storeAs;
        public SlateRef<Faction> faction;

        public override void RunInt()
        {
            var f = faction.GetValue(QuestGen.slate);
            var kind = f.def.pawnGroupMakers.SelectMany(gm => gm.options).Where(opt => opt.kind.RaceProps.Humanlike).RandomElementByWeight(opt => opt.selectionWeight).kind;
            var pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(kind, f, PawnGenerationContext.NonPlayer, forceGenerateNewPawn: true));
            Find.WorldPawns.PassToWorld(pawn);
            QuestGen.slate.Set(storeAs.GetValue(QuestGen.slate), pawn);
        }
        public override bool TestRunInt(Slate slate) => true;
    }
}
