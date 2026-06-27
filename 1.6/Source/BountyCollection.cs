using System.Collections.Generic;
using Verse;

namespace BountiesMod
{
    public class BountyCollection : IExposable
    {
        public List<BountyDef> bounties = new List<BountyDef>();

        public void ExposeData()
        {
            Scribe_Collections.Look(ref bounties, "bounties", LookMode.Def);
        }
    }
}
