using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;
using Verse.Grammar;

namespace BountiesMod;

public abstract class SitePartWorker_BountyAnimalsBase : SitePartWorker
{
    protected abstract bool IsExotic { get; }

    public override SitePartParams GenerateDefaultParams(float myThreatPoints, PlanetTile tile, Faction faction)
    {
        var sitePartParams = base.GenerateDefaultParams(myThreatPoints, tile, faction);

        IEnumerable<PawnKindDef> candidates;

        if (IsExotic)
        {
            candidates = DefDatabase<PawnKindDef>.AllDefsListForReading.Where(x =>
                x.RaceProps.Animal && !x.RaceProps.neverIncludeInQuests &&
                x.race.tradeTags != null && x.race.tradeTags.Contains("AnimalExotic"));

            if (!candidates.Any())
            {
                candidates = DefDatabase<PawnKindDef>.AllDefsListForReading.Where(x =>
                    x.RaceProps.Animal && !x.RaceProps.neverIncludeInQuests && x.combatPower >= 250f);
            }
        }
        else
        {
            candidates = DefDatabase<PawnKindDef>.AllDefsListForReading.Where(x =>
                x.RaceProps.Animal && !x.RaceProps.neverIncludeInQuests &&
                x.combatPower >= 40f && x.canArriveManhunter);
        }

        var validCandidates = candidates.Where(x => x.combatPower <= sitePartParams.threatPoints).ToList();
        if (!validCandidates.Any())
        {
            validCandidates = candidates.ToList();
        }

        if (!validCandidates.TryRandomElementByWeight(x => x.combatPower, out sitePartParams.animalKind))
        {
            ManhunterPackGenStepUtility.TryGetAnimalsKind(sitePartParams.threatPoints, tile, out sitePartParams.animalKind);
        }

        if (sitePartParams.animalKind != null)
        {
            sitePartParams.threatPoints = Mathf.Max(sitePartParams.threatPoints, sitePartParams.animalKind.combatPower);
        }
        return sitePartParams;
    }

    public override void Notify_GeneratedByQuestGen(SitePart part, Slate slate, List<Rule> outExtraDescriptionRules, Dictionary<string, string> outExtraDescriptionConstants)
    {
        base.Notify_GeneratedByQuestGen(part, slate, outExtraDescriptionRules, outExtraDescriptionConstants);
        if (part.parms.animalKind != null)
        {
            var animalsCount = AggressiveAnimalIncidentUtility.GetAnimalsCount(part.parms.animalKind, part.parms.threatPoints);
            slate.Set("animalCount", animalsCount);
            slate.Set("animalKind_labelPlural", part.parms.animalKind.GetLabelPlural(animalsCount));
            slate.Set("animalKind_label", part.parms.animalKind.label);
        }
    }
}
