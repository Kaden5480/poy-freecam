using System.Reflection;

using HarmonyLib;
using UILib;
using UILib.Patches;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UECamera = UnityEngine.Camera;

namespace Freecam {
    internal class Camera {
        private static Camera instance;

        // The GameObject this camera is attached to
        private GameObject root;

        // The camera itself
        private UECamera camera;

        // Audio listener for this camera
        private AudioListener listener;

        // Post processing for this camera
        private PostProcessLayer processLayer;

        // Lock for managing pausing
        private Lock @lock;

        // Camera parameters
        private const float minY = -89f;
        private const float maxY = 89f;
        private float rotX = 0f;
        private float rotY = 0f;

        /**
         * <summary>
         * Initializes the freecam.
         * </summary>
         */
        internal Camera() {
            instance = this;

            // Create the camera
            root = new GameObject("Freecam Camera");
            root.tag = "MainCamera";
            GameObject.DontDestroyOnLoad(root);

            camera = root.AddComponent<UECamera>();
            listener = root.AddComponent<AudioListener>();
            processLayer = root.AddComponent<PostProcessLayer>();

            camera.enabled = false;
            root.SetActive(false);

            // Register shortcuts
            Shortcut toggle = new Shortcut(new[] { Config.toggleKeybind });
            toggle.onTrigger.AddListener(Toggle);
            UIRoot.AddShortcut(toggle);

            // Apply defaults
            UpdateFov(Config.fov.Value);
            UpdateFarClip(Config.farClipPlane.Value);
            UpdatePostProcess(Config.postProcess.Value);
        }

        /**
         * <summary>
         * Updates the post processing state on this camera.
         * </summary>
         */
        internal static void UpdatePostProcess(bool use) {
            if (instance == null) {
                return;
            }

            instance.processLayer.enabled = use;
        }

        /**
         * <summary>
         * Updates the fov of this camera.
         * </summary>
         */
        internal static void UpdateFov(float fov) {
            if (instance == null) {
                return;
            }

            instance.camera.fieldOfView = fov;
        }

        /**
         * <summary>
         * Updates the far clip plane of this camera.
         * </summary>
         */
        internal static void UpdateFarClip(float farClip) {
            if (instance == null) {
                return;
            }

            instance.camera.farClipPlane = farClip;
        }

        /**
         * <summary>
         * Copies post processing from the current camera.
         * </summary>
         */
        private void CopyPostProcessing() {
            if (Cache.playerCamera == null) {
                return;
            }

            // Copy camera settings
            camera.renderingPath = Cache.playerCamera.renderingPath;

            // Copy post processing
            PostProcessLayer originalLayer = Cache.playerCamera.GetComponent<PostProcessLayer>();
            if (originalLayer == null) {
                return;
            }

            string[] fieldNames = new[] {
                "m_ActiveEffects", "m_Resources", "m_OldResources",
            };

            foreach (string name in fieldNames) {
                FieldInfo info = AccessTools.Field(typeof(PostProcessLayer), name);
                info.SetValue(processLayer, info.GetValue(originalLayer));
            }

            processLayer.antialiasingMode = originalLayer.antialiasingMode;
            processLayer.volumeLayer = originalLayer.volumeLayer;
        }

        /**
         * <summary>
         * Goes to the player's current position and rotation.
         * </summary>
         */
        private void GoToPlayer() {
            root.transform.position = Cache.playerCamera.transform.position;
            LookAt(
                Cache.playerCamX.transform.eulerAngles.y,
                Cache.playerCamY.desiredRotationY
            );
        }

        /**
         * <summary>
         * Enables freecam.
         * </summary>
         */
        internal void Enable() {
            UECamera main = UECamera.main;

            if (Cache.playerCamX == null || Cache.playerCamY == null) {
                Plugin.LogDebug(
                    "Unable to enable freecam, the player's camera"
                    + " appears to be missing from this scene"
                );
                return;
            }

            if (main != Cache.playerCamera) {
                Plugin.LogDebug(
                    "Unable to enable freecam, the current main"
                    + " is not the player's camera"
                );
                return;
            }

            Plugin.LogDebug($"Switching from old camera: {Cache.playerCamera}");

            // Acquire pause lock
            if (Config.pauseGame.Value == true && @lock == null) {
                @lock = new Lock(LockMode.Pause);
            }

            // Copy post processing across
            CopyPostProcessing();

            // Go to the player if not remembering
            if (Config.rememberPosition.Value == false) {
                GoToPlayer();
            }

            // Disable the camera
            Cache.playerCamera.enabled = false;

            root.SetActive(true);
            camera.enabled = true;
        }

        /**
         * <summary>
         * Disables freecam.
         * </summary>
         */
        internal void Disable() {
            if (UECamera.main != camera) {
                Plugin.LogDebug("Already disabled");
                return;
            }

            // Close pause handle
            if (@lock != null) {
                @lock.Close();
                @lock = null;
            }

            camera.enabled = false;
            root.SetActive(false);

            if (Cache.playerCamera != null) {
                Cache.playerCamera.enabled = true;
            }
        }

        /**
         * <summary>
         * Toggles freecam.
         * </summary>
         */
        internal void Toggle() {
            if (UECamera.main == camera) {
                Disable();
            }
            else {
                Enable();
            }
        }

        /**
         * <summary>
         * Handles modifying the movement speed.
         * </summary>
         */
        private void UpdateSpeed() {
            // Handle speed changes
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll == 0f) {
                return;
            }

            float speedMult = Mathf.Max(1f, Config.speedChangeMult.Value);
            float currSpeed = Config.movementSpeed.Value;
            float delta = (scroll < 0f) ? 1f / speedMult : speedMult;

            Config.movementSpeed.Value = Mathf.Min(
                Mathf.Max(Config.minMovementSpeed, currSpeed * delta),
                Config.maxMovementSpeed
            );

            Plugin.instance.speedOverlay.Show();
        }

        /**
         * <summary>
         * Looks in a specific rotation.
         * </summary>
         */
        private void LookAt(float x, float y) {
            // Make sure vertical is clamped
            rotY = Mathf.Clamp(y, minY, maxY);
            rotX = x;

            root.transform.localRotation = Quaternion.Euler(
                rotY, rotX, 0f
            );
        }

        /**
         * <summary>
         * Handles looking around.
         * </summary>
         */
        private void LookAround() {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");

            rotX += x * Config.sensitivity.Value;
            rotY -= y * Config.sensitivity.Value;

            LookAt(rotX, rotY);
        }

        /**
         * <summary>
         * Handles moving.
         * </summary>
         */
        private void Move() {
            float x = 0f;
            float y = 0f;
            float z = 0f;

            if (Input.GetKey(Config.moveForward.Value) == true) {
                y += 1f;
            }
            if (Input.GetKey(Config.moveBackward.Value) == true) {
                y -= 1f;
            }
            if (Input.GetKey(Config.moveLeft.Value) == true) {
                x -= 1f;
            }
            if (Input.GetKey(Config.moveRight.Value) == true) {
                x += 1f;
            }
            if (Input.GetKey(Config.moveUp.Value) == true) {
                z += 1f;
            }
            if (Input.GetKey(Config.moveDown.Value) == true) {
                z -= 1f;
            }

            if (x == 0f && y == 0f && z == 0f) {
                return;
            }

            Vector3 moveBy = (Config.movementSpeed.Value * Time.deltaTime)
                * new Vector3(x, z, y).normalized;

            if (Input.GetKey(Config.boostKeybind.Value) == true) {
                moveBy *= Config.boostMult.Value;
            }

            root.transform.Translate(moveBy);
        }

        /**
         * <summary>
         * Handles controls.
         * </summary>
         */
        internal void Update() {
            // Don't control if the camera is currently
            // disabled, or shortcuts can't run
            if (UECamera.main != camera
                || Shortcut.canRun == false
                || LockHandler.isCursorFree == true
            ) {
                return;
            }

            UpdateSpeed();
            LookAround();
            Move();
        }
    }
}
