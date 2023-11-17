using System.Collections.Generic;
using Verse;

namespace MechHumanlikes
{
    public static class MUD_Utils
    {
        private static List<PawnKindDef> cachedFriendlyDroneKinds;

        public static List<PawnKindDef> CachedFriendlyDroneKinds
        {
            get
            {
                if (cachedFriendlyDroneKinds == null)
                {
                    cachedFriendlyDroneKinds = new List<PawnKindDef>();
                    foreach (PawnKindDef pawnKindDef in DefDatabase<PawnKindDef>.AllDefsListForReading)
                    {
                        if (pawnKindDef.defaultFactionType?.isPlayer == true && MHC_Utils.IsConsideredMechanicalDrone(pawnKindDef.race))
                        {
                            cachedFriendlyDroneKinds.Add(pawnKindDef);
                        }
                    }
                }
                return cachedFriendlyDroneKinds;
            }
        }
    }
}
