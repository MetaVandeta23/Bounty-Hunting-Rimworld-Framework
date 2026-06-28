using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI.Group;

namespace BountiesMod
{
    public class GenStep_BountyCamp : GenStep_Outpost
    {
        public override void Generate(Map map, GenStepParams parms)
        {
            base.Generate(map, parms);
            SpawnTargetPawn(map, parms.sitePart.site);
        }

        public static void SpawnTargetPawn(Map map, Site site)
        {
            var deliveryPart = Find.QuestManager.QuestsListForReading
                .SelectMany(q => q.PartsListForReading.OfType<QuestPart_BountyDelivery>())
                .FirstOrDefault(p => p.targetSite == site);

            if (deliveryPart != null && deliveryPart.targetPawn != null && !deliveryPart.targetPawn.Spawned && !deliveryPart.targetPawn.Dead)
            {
                var pawn = deliveryPart.targetPawn;
                
                if (pawn.IsWorldPawn())
                {
                    Find.WorldPawns.RemovePawn(pawn);
                }

                var faction = map.ParentFaction;
                if (pawn.Faction != faction)
                {
                    pawn.SetFaction(faction);
                }

                var pawns = map.mapPawns.SpawnedPawnsInFaction(faction);
                var cell = pawns.Any() ? CellFinder.RandomClosewalkCellNear(pawns.RandomElement().Position, map, 5) : CellFinder.RandomClosewalkCellNear(map.Center, map, 5);
                
                GenSpawn.Spawn(pawn, cell, map);
                pawns.FirstOrDefault()?.GetLord()?.AddPawn(pawn);
            }
        }
    }
}
