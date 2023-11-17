using System.Collections.Generic;
using Verse;

namespace MechHumanlikes
{
    public class ChoiceLetter_ReprogramDrone : ChoiceLetter
    {
        public Pawn subject;

        public override bool CanDismissWithRightClick => false;

        public override IEnumerable<DiaOption> Choices
        {
            get
            {
                if (ArchivedOnly)
                {
                    yield return Option_Close;
                    yield break;
                }
                DiaOption diaOption = new DiaOption("AcceptButton".Translate());
                diaOption.action = delegate
                {
                    Find.WindowStack.Add(new Dialog_ReprogramDrone(subject));
                    Find.LetterStack.RemoveLetter(this);
                    // If the unit had the no programming hediff, remove that hediff.
                    Hediff hediff = subject.health.hediffSet.GetFirstHediffOfDef(MDR_HediffDefOf.MDR_NoProgramming);
                    if (hediff != null)
                    {
                        subject.health.RemoveHediff(hediff);
                    }
                };
                diaOption.resolveTree = true;
                if (!subject.Spawned)
                {
                    diaOption.disabled = true;
                    diaOption.disabledReason = "MHUD_InvalidUntilSpawned".Translate();
                }
                yield return diaOption;
                if (lookTargets.IsValid())
                {
                    yield return Option_JumpToLocationAndPostpone;
                }
                yield return Option_Postpone;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref subject, "MHUD_subject");
        }
    }
}
