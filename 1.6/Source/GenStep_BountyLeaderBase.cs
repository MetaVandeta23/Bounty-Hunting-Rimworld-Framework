using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace BountiesMod
{
    public class GenStep_BountyLeaderBase : GenStep_Settlement
    {
        public override void Generate(Map map, GenStepParams parms)
        {
            base.Generate(map, parms);
            GenStep_BountyCamp.SpawnTargetPawn(map, parms.sitePart.site);
        }
    }
}
