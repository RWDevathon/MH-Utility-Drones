using RimWorld;
using Verse;

namespace MechHumanlikes
{
    [DefOf]
    public static class MHUD_LetterDefOf
    {
        static MHUD_LetterDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MHUD_LetterDefOf));
        }

        public static LetterDef MHUD_ReprogramDroneLetter;
    }
}
