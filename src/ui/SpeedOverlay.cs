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
            // Copy of the current user theme
            Theme theme = Theme.GetTheme();
            theme.overlayOpacity = 0.95f;
            theme.overlayFadeTime = 0.2f;

            overlay = new Overlay(300f, 100f);
            overlay.SetAnchor(AnchorType.TopMiddle);
            overlay.SetOffset(0f, -200f);
            overlay.SetLockMode(LockMode.None);
            overlay.SetTheme(theme);

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
        }

        /**
         * <summary>
         * Updates the currently displayed speed.
         * </summary>
         */
        private IEnumerator DisplayRoutine() {
            float waitTime = 2.5f;
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
