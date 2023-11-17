using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace MechHumanlikes
{
    public class ScenPart_ConfigPage_ConfigureStartingPawns_Drones : ScenPart_ConfigPage_ConfigureStartingPawnsBase
    {
        public List<PawnKindCount> droneCounts = new List<PawnKindCount>();

        [MustTranslate]
        public string customSummary;

        private float ElementHeight => RowHeight * 4f;

        protected override int TotalPawnCount => droneCounts.Sum((PawnKindCount x) => x.count);

        public override string Summary(Scenario scen)
        {
            return customSummary ?? ((string)"ScenPart_StartWithSpecificColonists".Translate(droneCounts.Select((PawnKindCount x) => x.Summary).ToCommaList(useAnd: true), pawnChoiceCount));
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect scenPartRect = listing.GetScenPartRect(this, ElementHeight * droneCounts.Count + RowHeight);
            scenPartRect.height = RowHeight;
            for (int i = droneCounts.Count - 1; i >= 0; i--)
            {
                PawnKindCount droneCount = droneCounts[i];
                Widgets.TextFieldNumeric(scenPartRect, ref droneCount.count, ref droneCount.countBuffer, 1f, 10f);
                scenPartRect.y += RowHeight;
                Rect rect = scenPartRect;
                rect.xMax -= RowHeight;
                if (Widgets.ButtonText(rect, droneCount.kindDef.LabelCap))
                {
                    List<FloatMenuOption> list = new List<FloatMenuOption>();
                    foreach (PawnKindDef def in MUD_Utils.CachedFriendlyDroneKinds)
                    {
                        PawnKindDef localPawnKindDef = def;
                        list.Add(new FloatMenuOption(localPawnKindDef.LabelCap, delegate
                        {
                            droneCount.kindDef = localPawnKindDef;
                        }));
                    }
                    if (list.Any())
                    {
                        Find.WindowStack.Add(new FloatMenu(list));
                    }
                }
                Rect butRect = scenPartRect;
                butRect.xMin = rect.xMax;
                if (Widgets.ButtonImage(butRect, TexButton.DeleteX))
                {
                    droneCounts.RemoveAt(i);
                    return;
                }
                scenPartRect.y += RowHeight;
                Rect rect2 = scenPartRect;
                TaggedString taggedString = "Required".Translate();
                Vector2 vector = Text.CalcSize(taggedString);
                rect2.width = vector.x;
                Text.Anchor = TextAnchor.MiddleRight;
                Widgets.Label(rect2, taggedString);
                Text.Anchor = TextAnchor.UpperLeft;
                Widgets.Checkbox(scenPartRect.xMin + vector.x + 4f, scenPartRect.y, ref droneCount.requiredAtStart);
                scenPartRect.y += RowHeight * 2f;
            }
            if (Widgets.ButtonText(scenPartRect, "Add"))
            {
                droneCounts.Add(new PawnKindCount
                {
                    kindDef = MUD_Utils.CachedFriendlyDroneKinds.RandomElement(),
                    count = 1
                });
            }
        }

        protected override void GenerateStartingPawns()
        {
            StartingPawnUtility.ClearAllStartingPawns();
            int generationRequestIndex = 0;
            foreach (PawnKindCount droneCount in droneCounts)
            {
                for (int i = 0; i < droneCount.count; i++)
                {
                    PawnGenerationRequest generationRequest = StartingPawnUtility.GetGenerationRequest(generationRequestIndex);
                    generationRequest.KindDef = droneCount.kindDef;
                    StartingPawnUtility.SetGenerationRequest(generationRequestIndex, generationRequest);
                    StartingPawnUtility.AddNewPawn(generationRequestIndex);
                    generationRequestIndex++;
                }
            }
        }

        public override void PostGameStart()
        {
            base.PostGameStart();
            foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonists)
            {
                if (MDR_Utils.IsProgrammableDrone(pawn))
                {
                    ChoiceLetter_ReprogramDrone choiceLetter = (ChoiceLetter_ReprogramDrone)LetterMaker.MakeLetter(MHUD_LetterDefOf.MHUD_ReprogramDroneLetter);
                    choiceLetter.subject = pawn;
                    choiceLetter.Label = "MHUD_ReprogramDrone".Translate(pawn.LabelShortCap);
                    choiceLetter.Text = "MHUD_ReprogramDroneDesc".Translate(pawn.LabelShortCap);
                    choiceLetter.lookTargets = pawn;
                    Find.LetterStack.ReceiveLetter(choiceLetter);
                }
            }
        }

        public override void PostIdeoChosen()
        {
            Find.GameInitData.startingPawnsRequired = droneCounts.Where((PawnKindCount c) => c.requiredAtStart).ToList();
            Find.GameInitData.startingPawnKind = null;
            Find.GameInitData.startingXenotypesRequired = null;
            base.PostIdeoChosen();
        }

        public override bool HasNullDefs()
        {
            if (base.HasNullDefs())
            {
                return true;
            }
            foreach (PawnKindCount droneCount in droneCounts)
            {
                if (droneCount.kindDef == null)
                {
                    return true;
                }
            }
            return false;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref droneCounts, "droneCounts", LookMode.Deep);
        }

        public override int GetHashCode()
        {
            int num = base.GetHashCode();
            foreach (PawnKindCount droneCount in droneCounts)
            {
                num ^= droneCount.GetHashCode();
            }
            return num;
        }
    }

}
