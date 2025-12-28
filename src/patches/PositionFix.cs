using UnityEngine;

namespace Freecam {
    /**
     * <summary>
     * A class with some helper methods to deal with
     * the custom origin shifter.
     * </summary>
     */
    internal class PositionFix {
        /**
         * <summary>
         * Converts a "real" position to an offset
         * from the LeavePeakScene object.
         * </summary>
         * <param name="real">The object's current position</param>
         * <returns>The offset from LeavePeakScene</returns>
         */
        internal static Vector3 RealToOffset(Vector3 real) {
            if (Cache.leavePeakScene == null) {
                Plugin.LogDebug("LeavePeakScene not found, positions may be scuffed");
                return real;
            }

            return real - Cache.leavePeakScene.transform.position;
        }

        /**
         * <summary>
         * Converts an offset from the LeavePeakScene object
         * to a "real" position.
         * </summary>
         * <param name="offset">The offset from the LeavePeakScene object</param>
         * <returns>The "real" position</returns>
         */
        internal static Vector3 OffsetToReal(Vector3 offset) {
            if (Cache.leavePeakScene == null) {
                Plugin.LogDebug("LeavePeakScene not found, positions may be scuffed");
                return offset;
            }

            return offset + Cache.leavePeakScene.transform.position;
        }
    }
}
