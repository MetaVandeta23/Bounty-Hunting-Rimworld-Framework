using RimWorld;
using Verse;

namespace BountiesMod
{
    public static class BountyUtilities
    {
        public static float CalculateBountyCost(Pawn pawn)
        {
            float price = 1200f;

            price -= pawn.skills.GetSkill(SkillDefOf.Shooting).Level * 10f;
            price -= pawn.skills.GetSkill(SkillDefOf.Melee).Level * 10f;

            price -= pawn.skills.GetSkill(SkillDefOf.Social).Level * 20f;

            return price;
        }
    }
}
