using RimWorld;
using RimWorld.QuestGen;
using System.Linq;
using UnityEngine;
using Verse;

namespace BountiesMod
{
    public class QuestPart_HalveReward : QuestPart
    {
        public string inSignal;
        public float factor = 0.55f;

        public override void Notify_QuestSignalReceived(Signal signal)
        {
            if (signal.tag == inSignal)
            {
                foreach (var choice in quest.PartsListForReading.OfType<QuestPart_Choice>())
                {
                    foreach (var opt in choice.choices)
                    {
                        foreach (var reward in opt.rewards.OfType<Reward_Items>())
                        {
                            foreach (var thing in reward.items)
                            {
                                thing.stackCount = Mathf.Max(1, Mathf.RoundToInt(thing.stackCount * factor));
                            }
                        }
                    }
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref inSignal, "inSignal");
            Scribe_Values.Look(ref factor, "factor", 0.55f);
        }
    }
}
