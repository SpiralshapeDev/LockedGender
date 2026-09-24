using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Thor;
using UnityEngine;

namespace LockedGender
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class LockedGenderBase : BaseUnityPlugin
    {
        private const string modGUID = "SpiralMods." + modName;
        private const string modName = "LockedGender";
        private const string modVersion = "1.0.1";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static LockedGenderBase Instance;

        private ManualLogSource mls;

        private static BepInEx.Configuration.ConfigEntry<int> Gender;

        void Awake()
        {
            Instance = this;
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo($"{modName} has loaded (ModVersion: {modVersion}, ModGUID: {modGUID})!");

            ConfigCreate();
            harmony.PatchAll(typeof(LockedGenderBase));
        }

        void ConfigCreate()
        {
            Gender = Config.Bind<int>("Settings", "Player Gender", Random.Range(0,1), "0 = Male, 1 = Female");
            mls.LogInfo($"Loaded Config for gender, Value: '{Gender.Value}', Gender: '{(PlayerCharacterData.GenderType)Gender.Value}'");
        }
        
        [HarmonyPatch(typeof(PlayerCharacterData), nameof(PlayerCharacterData.Gender), MethodType.Getter)]
        static bool Prefix(ref PlayerCharacterData.GenderType __result)
        {
            Instance.ConfigCreate();
            Instance.Config.Reload();
            PlayerCharacterData.GenderType gender = (PlayerCharacterData.GenderType)Gender.Value;
            if (__result == gender) return false;
            
            __result = gender;
            return false;
        }
    }
}
