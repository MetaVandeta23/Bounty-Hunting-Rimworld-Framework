using RimWorld;
using RimWorld.QuestGen;

namespace BountiesMod
{
    public class QuestNode_GetEnemyFactionLeader : QuestNode
    {
        public SlateRef<string> storeAs;
        public SlateRef<Faction> faction;

        public override void RunInt()
        {
            QuestGen.slate.Set(storeAs.GetValue(QuestGen.slate), faction.GetValue(QuestGen.slate).leader);
        }
        public override bool TestRunInt(Slate slate)
        {
            var f = faction.GetValue(slate);
            return f != null && f.leader != null;
        }
    }
}
