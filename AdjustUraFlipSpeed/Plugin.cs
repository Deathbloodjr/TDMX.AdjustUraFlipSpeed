using AdjustUraFlipSpeed.Patches;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SaveProfileManager.Patches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if IL2CPP
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP;
#endif

namespace AdjustUraFlipSpeed
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, ModName, MyPluginInfo.PLUGIN_VERSION)]
#if MONO
    public class Plugin : BaseUnityPlugin
#elif IL2CPP
    public class Plugin : BasePlugin
#endif
    {
        public const string ModName = "AdjustUraFlipSpeed";

        public static Plugin Instance;
        private Harmony _harmony;
        public new static ManualLogSource Log;

        public ConfigEntry<bool> ConfigEnabled;

        public ConfigEntry<float> ConfigAdjustMultiplier;
        public ConfigEntry<float> ConfigSongSelectScrollNormalSpeed;
        public ConfigEntry<float> ConfigSongSelectScrollFastSpeed;


#if MONO
        private void Awake()
#elif IL2CPP
        public override void Load()
#endif
        {
            Instance = this;

#if MONO
            Log = Logger;
#elif IL2CPP
            Log = base.Log;
#endif

            SetupConfig(Config, Path.Combine("BepInEx", "data", ModName));
            SetupHarmony();

            var isSaveManagerLoaded = IsSaveManagerLoaded();
            if (isSaveManagerLoaded)
            {
                AddToSaveManager();
            }
        }

        private void SetupConfig(ConfigFile config, string saveFolder, bool isSaveManager = false)
        {
            string dataFolder = Path.Combine("BepInEx", "data", ModName);

            if (!isSaveManager)
            {
                ConfigEnabled = config.Bind("General",
                   "Enabled",
                   true,
                   "Enables the mod.");
            }

            ConfigAdjustMultiplier = config.Bind("UraFlipSpeed",
                "AdjustMultiplier",
                1f,
                "Multiplies the animation speed. Higher number is faster.");

            ConfigSongSelectScrollNormalSpeed = config.Bind("SongSelectScrollSpeed",
                "SongSelectScrollNormalSpeed",
                1f,
                "Sets the animation speed for the normal scroll speed. Higher number is faster.");

            ConfigSongSelectScrollFastSpeed = config.Bind("SongSelectScrollSpeed",
                "SongSelectScrollFastSpeed",
                2f,
                "Sets the animation speed for the faster scroll speed. Higher number is faster.");
        }

        private void SetupHarmony()
        {
            // Patch methods
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

            LoadPlugin(ConfigEnabled.Value);
        }

        public static void LoadPlugin(bool enabled)
        {
            if (enabled)
            {
                bool result = true;
                // If any PatchFile fails, result will become false
                result &= Instance.PatchFile(typeof(AdjustUraFlipSpeedPatch));
                result &= Instance.PatchFile(typeof(SongSelectScrollSpeedPatch));
                if (result)
                {
                    ModLogger.Log($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");
                }
                else
                {
                    ModLogger.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} failed to load.", LogType.Error);
                    // Unload this instance of Harmony
                    // I hope this works the way I think it does
                    Instance._harmony.UnpatchSelf();
                }
            }
            else
            {
                ModLogger.Log($"Plugin {MyPluginInfo.PLUGIN_NAME} is disabled.");
            }
        }

        private bool PatchFile(Type type)
        {
            if (_harmony == null)
            {
                _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            }
            try
            {
                _harmony.PatchAll(type);
                ModLogger.Log("File patched: " + type.FullName, LogType.Debug);
                return true;
            }
            catch (Exception e)
            {
                ModLogger.Log("Failed to patch file: " + type.FullName);
                ModLogger.Log(e.Message);
                return false;
            }
        }

        public static void UnloadPlugin()
        {
            LogPatchedMethods(LogType.Debug);
            Instance._harmony.UnpatchSelf();
            var items = PatchProcessor.GetAllPatchedMethods().ToList();
            foreach (var method in items)
            {
                HarmonyLib.Patches patchInfo = PatchProcessor.GetPatchInfo(method);
                foreach (var post in patchInfo.Postfixes)
                {
                    if (post.owner == Instance._harmony.Id)
                    {
                        //ModLogger.Log("Post patch found: " + method.Name, LogType.Debug);
                        // The patch SongSelectScrollSpeedPatch.Animator_speed_setter_Postfix for Animator.set_speed doesn't have a MethodBody
                        // So it gets skipped by UnpatchSelf
                        // I need to unpatch it myself as so
                        if (method.Name.Contains("speed"))
                        {
                            //ModLogger.Log("method.HasMethodBody(): " + method.HasMethodBody(), LogType.Debug);
                            PatchProcessor patchProcessor = new PatchProcessor(null, method);
                            patchProcessor.Unpatch(post.PatchMethod);
                            ModLogger.Log("Unpatch: " + method.Name, LogType.Debug);
                        }
                    }
                }
            }
            LogPatchedMethods(LogType.Debug);
            ModLogger.Log($"Plugin {MyPluginInfo.PLUGIN_NAME} has been unpatched.");
        }

        public static void ReloadPlugin()
        {
            // Reloading will always be completely different per mod
            // You'll want to reload any config file or save data that may be specific per profile
            // If there's nothing to reload, don't put anything here, and keep it commented in AddToSaveManager
            //SwapSongLanguagesPatch.InitializeOverrideLanguages();
            //TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.MusicData.Reload();
        }

        public void AddToSaveManager()
        {
            // Add SaveDataManager dll path to your csproj.user file
            // https://github.com/Deathbloodjr/TDMX.SaveProfileManager
            var plugin = new PluginSaveDataInterface(MyPluginInfo.PLUGIN_GUID);
            plugin.AssignLoadFunction(LoadPlugin);
            plugin.AssignUnloadFunction(UnloadPlugin);

            // Reloading will always be completely different per mod
            // You'll want to reload any config file or save data that may be specific per profile
            // If there's nothing to reload, don't put anything here, and keep it commented in AddToSaveManager
            //plugin.AssignReloadSaveFunction(ReloadPlugin);

            // Comment this if the only config option is ConfigEnabled
            plugin.AssignConfigSetupFunction(SetupConfig);
            plugin.AddToManager(ConfigEnabled.Value);
        }

        private bool IsSaveManagerLoaded()
        {
            try
            {
                Assembly loadedAssembly = Assembly.Load("com.DB.TDMX.SaveProfileManager");
                var isLoaded = loadedAssembly != null;
                return isLoaded;
            }
            catch
            {
                return false;
            }
        }

        private static void LogPatchedMethods(LogType logType = LogType.Debug)
        {
            List<string> output = new List<string>()
            {
                "Current patched methods: ",
            };
            foreach (MethodBase item in PatchProcessor.GetAllPatchedMethods().ToList())
            {
                //bool num = item.HasMethodBody();
                HarmonyLib.Patches patchInfo2 = PatchProcessor.GetPatchInfo(item);
                PatchProcessor patchProcessor = new PatchProcessor(null, item);
                patchInfo2.Postfixes.DoIf((Patch patchInfo) => patchInfo.owner == Instance._harmony.Id, delegate (Patch patchInfo)
                {
                    string value = item.DeclaringType.Name + "." + item.Name + " Postfix";
                    output.Add(value);
                });
                patchInfo2.Prefixes.DoIf((Patch patchInfo) => patchInfo.owner == Instance._harmony.Id, delegate (Patch patchInfo)
                {
                    string value = item.DeclaringType.Name + "." + item.Name + " Prefix";
                    output.Add(value);
                });
                patchInfo2.ILManipulators.DoIf((Patch patchInfo) => patchInfo.owner == Instance._harmony.Id, delegate (Patch patchInfo)
                {
                    string value = item.DeclaringType.Name + "." + item.Name + " ILManipulator";
                    output.Add(value);
                });
                patchInfo2.Transpilers.DoIf((Patch patchInfo) => patchInfo.owner == Instance._harmony.Id, delegate (Patch patchInfo)
                {
                    string value = item.DeclaringType.Name + "." + item.Name + " Transpiler";
                    output.Add(value);
                });
                patchInfo2.Finalizers.DoIf((Patch patchInfo) => patchInfo.owner == Instance._harmony.Id, delegate (Patch patchInfo)
                {
                    string value = item.DeclaringType.Name + "." + item.Name + " Finalizer";
                    output.Add(value);
                });
            }
            ModLogger.Log(output, logType);
        }

        public static MonoBehaviour GetMonoBehaviour() => TaikoSingletonMonoBehaviour<CommonObjects>.Instance;

        public void StartCustomCoroutine(IEnumerator enumerator)
        {
#if MONO
            GetMonoBehaviour().StartCoroutine(enumerator);
#elif IL2CPP
            GetMonoBehaviour().StartCoroutine(enumerator);
#endif
        }
    }
}