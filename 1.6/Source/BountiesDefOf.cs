using RimWorld;

namespace BountiesMod
{
    [DefOf]
    public static class BountiesDefOf
    {
        public static SitePartDef META_DebtEnforcementCamp;

        static BountiesDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BountiesDefOf));
        }
    }
}
