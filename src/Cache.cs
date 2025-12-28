using UnityEngine;
using UECamera = UnityEngine.Camera;

namespace Freecam {
    /**
     * <summary>
     * Contains scene objects needed by freecam.
     * </summary>
     */
    internal static class Cache {
        internal static LeavePeakScene leavePeakScene { get; private set; }
        internal static UECamera playerCamera         { get; private set; }
        internal static CameraLook playerCamX         { get; private set; }
        internal static CameraLook playerCamY         { get; private set; }
        internal static Climbing climbing             { get; private set; }
        internal static Rigidbody playerRb            { get; private set; }
        internal static Transform playerTransform     { get; private set; }

        /**
         * <summary>
         * Finds objects in the scene.
         * </summary>
         */
        internal static void FindObjects() {
            climbing = GameObject.FindObjectOfType<Climbing>();
            LeavePeakScene[] bags = Resources.FindObjectsOfTypeAll<LeavePeakScene>();
            if (bags.Length > 0) {
                leavePeakScene = bags[0];
            }

            if (climbing != null) {
                playerRb = climbing.playerBody;
            }

            if (playerRb != null) {
                playerTransform = playerRb.transform;
            }

            // Access the player's camera
            GameObject cameraHolderObj = GameObject.Find("PlayerCameraHolder");
            if (cameraHolderObj == null) {
                Plugin.LogDebug("No camera holder found");
                return;
            }

            // The camera has two components, X and Y
            foreach (CameraLook cameraLook in cameraHolderObj.GetComponentsInChildren<CameraLook>()) {
                if ("PlayerCameraHolder".Equals(cameraLook.name) == true) {
                    Plugin.LogDebug("Found camX");
                    playerCamX = cameraLook;
                }
                else {
                    Plugin.LogDebug("Found camY");
                    playerCamY = cameraLook;
                }
            }

            // Find the main camera through an audio listener
            foreach (AudioListener listener in GameObject.FindObjectsOfType<AudioListener>()) {
                if ("CamY".Equals(listener.name) == false
                    && "MainCamera".Equals(listener.name) == false
                ) {
                    continue;
                }

                playerCamera = listener.GetComponent<UECamera>();
                break;
            }
        }

        /**
         * <summary>
         * Clears the cache.
         * </summary>
         */
        internal static void Clear() {
            leavePeakScene = null;
            playerCamera = null;
            playerCamX = null;
            playerCamY = null;
            climbing = null;
            playerRb = null;
            playerTransform = null;
        }
    }
}
