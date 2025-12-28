using BepInEx.Configuration;
using ModMenu.Config;
using UnityEngine;

namespace Freecam {
    /**
     * <summary>
     * Holds the freecam config.
     * </summary>
     */
    internal static class Config {
        internal const float minMovementSpeed = 1f;
        internal const float maxMovementSpeed = 1000f;

        // General
        [Listener(typeof(Freecam.Camera), nameof(Freecam.Camera.UpdateFov))]
        [Field("Field of View", FieldType.Slider, min=50f, max=150f)]
        internal static ConfigEntry<float> fov;

        [Listener(typeof(Freecam.Camera), nameof(Freecam.Camera.UpdateFarClip))]
        [Field("Far Clip Plane", min=0f)]
        internal static ConfigEntry<float> farClipPlane;

        [Listener(typeof(Freecam.Camera), nameof(Freecam.Camera.UpdatePostProcess))]
        [Field("Use Post Processing")]
        internal static ConfigEntry<bool> postProcess;

        [Field(
            "Movement Speed",
            FieldType.Slider,
            min=minMovementSpeed,
            max=maxMovementSpeed
        )]
        internal static ConfigEntry<float> movementSpeed;

        [Field("Sensitivity", FieldType.Slider, min=1f, max=20f)]
        internal static ConfigEntry<float> sensitivity;

        [Field("Boost Multiplier", FieldType.Slider, min=1f, max=10f)]
        internal static ConfigEntry<float> boostMult;

        [Field("Speed Change Multiplier", FieldType.Slider, min=1f, max=10f)]
        internal static ConfigEntry<float> speedChangeMult;

        [Field("Remember Position")]
        internal static ConfigEntry<bool> rememberPosition;

        // Keybinds
        [Field("Toggle Keybind")]
        internal static ConfigEntry<KeyCode> toggleKeybind;

        [Field("Move Forward")]
        internal static ConfigEntry<KeyCode> moveForward;

        [Field("Move Backward")]
        internal static ConfigEntry<KeyCode> moveBackward;

        [Field("Move Left")]
        internal static ConfigEntry<KeyCode> moveLeft;

        [Field("Move Right")]
        internal static ConfigEntry<KeyCode> moveRight;

        [Field("Move Up")]
        internal static ConfigEntry<KeyCode> moveUp;

        [Field("Move Down")]
        internal static ConfigEntry<KeyCode> moveDown;

        [Field("Boost")]
        internal static ConfigEntry<KeyCode> boostKeybind;

        [Category("Keybinds")]
        [Field("Increase Speed")]
        internal const string increaseSpeed = "Mouse Wheel Up";

        [Category("Keybinds")]
        [Field("Decrease Speed")]
        internal const string decreaseSpeed = "Mouse Wheel Down";


        /**
         * <summary>
         * Initializes the config, binding to the provided config file.
         * </summary>
         * <param name="configFile">The config file to bind to</param>
         */
        internal static void Init(ConfigFile configFile) {
            // General
            fov = configFile.Bind(
                "General", "fov", 100f,
                "The field of view."
            );

            farClipPlane = configFile.Bind(
                "General", "farClipPlane", 100000f,
                "The far clip plane."
            );

            postProcess = configFile.Bind(
                "General", "postProcess", true,
                "Whether post processing should be applied."
            );

            movementSpeed = configFile.Bind(
                "General", "movementSpeed", 20f,
                "The regular movement speed."
            );

            sensitivity = configFile.Bind(
                "General", "sensitivity", 4f,
                "The camera sensitivity."
            );

            boostMult = configFile.Bind(
                "General", "boostMult", 3f,
                "How much the boost keybind should multiply speed."
            );

            speedChangeMult = configFile.Bind(
                "General", "speedChangeMult", 2f,
                "How much the movement speed should change when the"
                + " mouse wheel is scrolled up/down."
            );

            rememberPosition = configFile.Bind(
                "General", "rememberPosition", false,
                "Whether the freecam should remember its last position,"
                + " instead of teleporting to the player's current"
                + " position when enabled."
            );

            // Keybinds
            toggleKeybind = configFile.Bind(
                "Keybinds", "toggleKeybind", KeyCode.F9,
                "The keybind to toggle the freecam."
            );

            moveForward = configFile.Bind(
                "Keybinds", "moveForward", KeyCode.W,
                "The keybind to move forward."
            );

            moveBackward = configFile.Bind(
                "Keybinds", "moveBackward", KeyCode.S,
                "The keybind to move forward."
            );

            moveLeft = configFile.Bind(
                "Keybinds", "moveLeft", KeyCode.A,
                "The keybind to move left."
            );

            moveRight = configFile.Bind(
                "Keybinds", "moveRight", KeyCode.D,
                "The keybind to move right."
            );

            moveUp = configFile.Bind(
                "Keybinds", "moveUp", KeyCode.E,
                "The keybind to move up."
            );

            moveDown = configFile.Bind(
                "Keybinds", "moveDown", KeyCode.Q,
                "The keybind to move down."
            );

            boostKeybind = configFile.Bind(
                "Keybinds", "boostKeybind", KeyCode.LeftShift,
                "The keybind to go faster."
            );
        }
    }
}
