using HarmonyLib;
using System.Reflection;
using Verse;

namespace MechHumanlikes
{
    public class MechUtilityDrones : Mod
    {
        public static MechUtilityDrones ModSingleton { get; private set; }

        public MechUtilityDrones(ModContentPack content) : base(content)
        {
            ModSingleton = this;
            new Harmony("MechUtilityDrones").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}