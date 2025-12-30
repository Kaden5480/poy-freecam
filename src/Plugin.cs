using System;
using System.Linq;

using BepInEx;
using HarmonyLib;
using ModMenu;
using UILib;
using UILib.Patches;

namespace Freecam {
    [BepInDependency("com.github.Kaden5480.poy-ui-lib")]
    [BepInDependency(
        "com.github.Kaden5480.poy-mod-menu",
        BepInDependency.DependencyFlags.SoftDependency
    )]
    [BepInPlugin("com.github.Kaden5480.poy-freecam", "Freecam", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {
        internal static Plugin instance { get; private set; }
        internal SpeedOverlay speedOverlay { get; private set; }

        private Camera camera;

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        private void Awake() {
            instance = this;

            // Initialize config
            Freecam.Config.Init(this.Config);

            // Initialize the freecam and overlay
            UIRoot.onInit.AddListener(() => {
                speedOverlay = new SpeedOverlay();
                camera = new Camera();
            });

            // Add listeners for caching
            SceneLoads.AddLoadListener(delegate {
                Cache.FindObjects();
                CamFixes.SceneLoad();
            });
            SceneLoads.AddUnloadListener(delegate {
                if (camera != null) {
                    camera.Disable();
                }

                Cache.Clear();
            });

            // Register with Mod Menu
            if (AccessTools.AllAssemblies().FirstOrDefault(
                    a => a.GetName().Name == "ModMenu"
                ) != null
            ) {
                Register();
            }
        }

        /**
         * <summary>
         * Registers with Mod Menu.
         * </summary>
         */
        private void Register() {
            ModInfo info = ModManager.Register(this);
            info.license = "GPL-3.0";
            info.Add(typeof(Freecam.Config));
        }

        /**
         * <summary>
         * Executes each frame.
         * </summary>
         */
        private void Update() {
            if (camera != null) {
                camera.Update();
            }
        }

        /**
         * <summary>
         * Logs a debug message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        internal static void LogDebug(string message) {
#if DEBUG
            if (instance == null) {
                Console.WriteLine($"[Debug] Freecam: {message}");
                return;
            }

            instance.Logger.LogInfo(message);
#else
            if (instance != null) {
                instance.Logger.LogDebug(message);
            }
#endif
        }

        /**
         * <summary>
         * Logs an informational message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        internal static void LogInfo(string message) {
            if (instance == null) {
                Console.WriteLine($"[Info] Freecam: {message}");
                return;
            }
            instance.Logger.LogInfo(message);
        }

        /**
         * <summary>
         * Logs an error message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        internal static void LogError(string message) {
            if (instance == null) {
                Console.WriteLine($"[Error] Freecam: {message}");
                return;
            }
            instance.Logger.LogError(message);
        }
    }
}
