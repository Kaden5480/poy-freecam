using UnityEngine;

namespace Freecam {
    /**
     * <summary>
     * Patches relating to cameras.
     * </summary>
     */
    internal static class CamFixes {
        /**
         * <summary>
         * Makes the distance render camera on ST untagged
         * otherwise it does some really weird things.
         * </summary>
         */
        private static void FixDistanceCamera() {
            GameObject obj = GameObject.Find("DistanceRenderCam");
            if (obj == null) {
                return;
            }

            obj.tag = "Untagged";
        }

        /**
         * <summary>
         * Applies per-scene patches.
         * </summary>
         */
        internal static void SceneLoad() {
            FixDistanceCamera();
        }
    }
}
