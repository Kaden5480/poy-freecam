using UnityEngine;
using UECamera = UnityEngine.Camera;

namespace Freecam {
    /**
     * <summary>
     * Contains scene objects needed by freecam.
     * </summary>
     */
    internal static class Cache {
        internal static Climbing climbing { get; private set; }
        internal static Rigidbody playerRb { get; private set; }
        internal static Transform playerTransform { get; private set; }

        /**
         * <summary>
         * Finds objects in the scene.
         * </summary>
         */
        internal static void FindObjects() {
            climbing = GameObject.FindObjectOfType<Climbing>();

            if (climbing != null) {
                playerRb = climbing.playerBody;
            }

            if (playerRb != null) {
                playerTransform = playerRb.transform;
            }
        }

        /**
         * <summary>
         * Clears the cache.
         * </summary>
         */
        internal static void Clear() {
            climbing = null;
            playerRb = null;
            playerTransform = null;
        }
    }
}
