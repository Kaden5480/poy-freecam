using System.Collections;

using UILib;
using UILib.Components;
using UILib.Layouts;
using UILib.Patches;
using UnityEngine;

namespace Freecam {
    /**
     * <summary>
     * The UI which pops up when modifying speed to display
     * the current speed information.
     * </summary>
     */
    internal class SpeedOverlay {
        private static SpeedOverlay instance;
        private IEnumerator coroutine;

        private Overlay overlay;
        private Image background;

        private Label currentSpeed;
        private Label currentBoost;

        /**
         * <summary>
         * Initializes the speed overlay.
         * </summary>
         */
        internal SpeedOverlay() {
            instance = this;

            Theme theme = Theme.GetTheme();

            overlay = new Overlay(300f, 100f);
            overlay.SetAnchor(AnchorType.TopMiddle);
            overlay.SetOffset(0f, -200f);
            overlay.SetLockMode(LockMode.None);

            background = new Image(theme.background);
            background.SetFill(FillType.All);
            background.SetInheritTheme(false);
            background.SetContentLayout(LayoutType.Vertical);
            background.SetContentPadding(10);
            background.SetElementSpacing(0f);
            overlay.Add(background);

            currentSpeed = new Label("", 30);
            currentSpeed.SetSize(200f, 40f);
            background.Add(currentSpeed);

            currentBoost = new Label("", 18);
            currentBoost.SetSize(200f, 20f);
            currentBoost.SetColor(theme.selectAltHighlight);
            background.Add(currentBoost);

            // Set the theme
            UpdateTheme(0f);
        }

        /**
         * <summary>
         * Updates the opacity of the overlay.
         * </summary>
         */
        internal static void UpdateTheme(float _) {
            Theme theme = Theme.GetTheme();
            theme.overlayOpacity = Config.overlayOpacity.Value;
            theme.overlayFadeTime = Config.overlayFadeTime.Value;

            instance.overlay.SetTheme(theme);
        }

        /**
         * <summary>
         * Updates the currently displayed speed.
         * </summary>
         */
        private IEnumerator DisplayRoutine() {
            float waitTime = Config.overlayWaitTime.Value;
            overlay.Show();

            // Wait a bit
            float timer = 0f;
            while (timer < waitTime) {
                timer += Time.deltaTime;
                yield return null;
            }

            // Start fading
            overlay.Hide();
            coroutine = null;
            yield break;
        }

        /**
         * <summary>
         * Displays the speed overlay.
         * </summary>
         */
        internal void Show() {
            if (Plugin.instance == null) {
                return;
            }

            if (coroutine != null) {
                Plugin.instance.StopCoroutine(coroutine);
                coroutine = null;
            }

            currentSpeed.SetText($"Speed: {Config.movementSpeed.Value}");
            currentBoost.SetText($"Boost: x{Config.boostMult.Value}");

            coroutine = DisplayRoutine();
            Plugin.instance.StartCoroutine(coroutine);
        }
    }
}
