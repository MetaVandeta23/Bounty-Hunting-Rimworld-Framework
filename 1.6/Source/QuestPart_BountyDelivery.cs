using RimWorld;
using Verse;

namespace BountiesMod
{
    public class QuestPart_BountyDelivery : QuestPartActivable
    {
        public Faction askerFaction;
        public ThingDef targetItemDef;
        public Pawn targetPawn;
        public bool captureAlive;
        public bool deadOrAlive;
        public bool alivePreferred;
        public bool deadPreferred;
        public string outSignalDeliveredAlive;
        public string outSignalDeliveredDead;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref askerFaction, "askerFaction");
            Scribe_Defs.Look(ref targetItemDef, "targetItemDef");
            Scribe_References.Look(ref targetPawn, "targetPawn");
            Scribe_Values.Look(ref captureAlive, "captureAlive", false);
            Scribe_Values.Look(ref deadOrAlive, "deadOrAlive", false);
            Scribe_Values.Look(ref alivePreferred, "alivePreferred", false);
            Scribe_Values.Look(ref deadPreferred, "deadPreferred", false);
            Scribe_Values.Look(ref outSignalDeliveredAlive, "outSignalDeliveredAlive");
            Scribe_Values.Look(ref outSignalDeliveredDead, "outSignalDeliveredDead");
        }
    }
}
