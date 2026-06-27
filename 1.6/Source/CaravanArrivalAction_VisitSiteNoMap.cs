using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BountiesMod
{
    public class CaravanArrivalAction_VisitSiteNoMap : CaravanArrivalAction
    {
        private Site site;

        public override string Label => "VisitSettlement".Translate(site.Label);
        public override string ReportString => "CaravanVisiting".Translate(site.Label);

        public CaravanArrivalAction_VisitSiteNoMap() { }
        public CaravanArrivalAction_VisitSiteNoMap(Site site) => this.site = site;

        public override FloatMenuAcceptanceReport StillValid(Caravan caravan, PlanetTile destinationTile)
        {
            var report = base.StillValid(caravan, destinationTile);
            if (!report) return report;
            return site != null && site.Tile == destinationTile && site.Spawned;
        }

        public override void Arrived(Caravan caravan)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref site, "site");
        }

        public static IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, Site site)
        {
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => true, () => new CaravanArrivalAction_VisitSiteNoMap(site), "VisitSettlement".Translate(site.Label), caravan, site.Tile, site);
        }
    }
}
