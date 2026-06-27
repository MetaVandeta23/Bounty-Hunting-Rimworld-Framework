using RimWorld;
using Verse;

namespace BountiesMod;

public abstract class GenStep_BountyAnimalsBase : GenStep
{
    public override int SeedPart => 457293337;

    protected abstract bool ApplyScaria { get; }

    public override void Generate(Map map, GenStepParams parms)
    {
        var traverseParams = TraverseParms.For(TraverseMode.NoPassClosedDoors).WithFenceblocked(forceFenceblocked: true);
        if (!RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((IntVec3 x) => CellValidator(x) && !x.Fogged(map) && map.reachability.CanReachMapEdge(x, traverseParams) && x.GetRoom(map).CellCount >= 100, map, out var result))
        {
            if (!CellFinderLoose.TryGetRandomCellWith((IntVec3 x) => CellValidator(x) && !x.Fogged(map), map, 1000, out result))
            {
                return;
            }
        }

        var points = parms.sitePart != null ? parms.sitePart.parms.threatPoints : new FloatRange(300f, 500f).RandomInRange;
        var animalKind = parms.sitePart?.parms.animalKind;
        if (animalKind == null && !ManhunterPackGenStepUtility.TryGetAnimalsKind(points, map.Tile, out animalKind))
        {
            return;
        }

        var list = AggressiveAnimalIncidentUtility.GenerateAnimals(animalKind, map.Tile, points);
        for (var i = 0; i < list.Count; i++)
        {
            CellFinder.TryFindRandomSpawnCellForPawnNear(result, map, out var result2, 10, CellValidator);
            var pawn = GenSpawn.Spawn(list[i], result2, map, Rot4.Random) as Pawn;
            if (pawn != null)
            {
                if (ApplyScaria)
                {
                    pawn.health.AddHediff(HediffDefOf.Scaria);
                }
                pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
            }
        }

        bool CellValidator(IntVec3 x)
        {
            if (!x.Standable(map))
            {
                return false;
            }
            if (MapGenerator.UsedRects.Any((CellRect r) => r.Contains(x)))
            {
                return false;
            }
            return true;
        }
    }
}
