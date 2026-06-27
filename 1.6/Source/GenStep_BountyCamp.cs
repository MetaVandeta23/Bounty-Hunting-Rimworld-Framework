using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace BountiesMod
{
    public class GenStep_BountyCamp : GenStep_Outpost
    {
        public override void Generate(Map map, GenStepParams parms)
        {
            base.Generate(map, parms);
            if (parms.sitePart != null && parms.sitePart.things != null && parms.sitePart.things.Any)
            {
                var pawn = (Pawn)parms.sitePart.things.Take(parms.sitePart.things[0]);
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
