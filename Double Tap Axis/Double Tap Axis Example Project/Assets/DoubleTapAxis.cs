using System.Collections;
using UnityEngine;

namespace DoubeTap
{
    /// <summary>
    /// Calculates taps on the specified axis.
    /// </summary>
    public class DoubleTapAxis : MonoBehaviour
    {
        public enum Axis
        {
            None = 0,
            Horizontal,
            Vertical
        }

        public enum TapVelocity
        {
            None = 0,
            Positive,
            Negative
        }

        /// <summary>
        /// The number of taps required before the method 'OnTapCountMet' is called.
        /// </summary>
        public int tapsRequired = 2;

        // The time between taps. Once this time elapses the tap count is reset.
        public float timeBetweenTaps = 0.4f;

        // Used to make sure that the axis is reset (i.e. the joystick is re-centred) before we count a new tap.
        private bool wasLastFrameInput = false;

        // Stores the previous tap velocity. Makes sure the player taps the joystick in the same direction twice.
        private TapVelocity prevVelocity = TapVelocity.None;

        // Stores the previous tap axis. Makes sure the player taps the joystick on the same axis twice.
        private Axis prevAxis = Axis.None;

        private int currentTapCount;
        private float lastTapTime;
        private bool isRunning;

        void Start()
        {
            // If you don't want to listen for taps at start then remove this line.
            StartChecking();
        }

        /// <summary>
        /// Starts checking for double taps (if not already running).
        /// </summary>
        public void StartChecking()
        {
            isRunning = true;
        }

        /// <summary>
        /// Stops checking for double taps.
        /// </summary>
        public void StopChecking()
        {
            isRunning = false;
        }

        void Update()
        {
            if (!isRunning)
            {
                return;
            }

            float horiAxisVal = Input.GetAxis("Horizontal");
            float vertAxisVal = Input.GetAxis("Vertical");

            float horiAxisAbs = Mathf.Abs(horiAxisVal);
            float vertAxisAbs = Mathf.Abs(vertAxisVal);

            if (horiAxisAbs > 0f || vertAxisAbs > 0f) // We've moved the joystick in either a vertical or horizontal direction.
            {
                Axis currentMove = horiAxisAbs > vertAxisAbs ? Axis.Horizontal : Axis.Vertical;

                if (IsTapOnSameAxis(currentMove)) // Make sure that this tap is on the same axis as the previous tap.
                {
                    float currentVelocity = currentMove == Axis.Horizontal ? horiAxisVal : vertAxisVal;

                    if (IsTapInSameDirection(currentVelocity)) // Ensures that the player tapped in the same direction as the last tap.
                    {
                        if (!wasLastFrameInput)
                        {
                            Vector2 velocity = new Vector2(horiAxisVal, vertAxisVal).normalized;

                            // We've registered a tap on the correct axis and direction.
                            ProcessTap(currentMove, currentVelocity, velocity);
                        }
                    }
                }
            }
            else
            {
                wasLastFrameInput = false; // No axis input registered this frame.

                if (Time.time - lastTapTime > timeBetweenTaps)
                {
                    // Reset tap count if timeBetweenTaps has elapsed.
                    Reset();
                }
            }


        }

        private void Reset()
        {
            currentTapCount = 0;
            prevVelocity = TapVelocity.None;
            prevAxis = Axis.None;
        }

        private bool IsTapOnSameAxis(Axis currentMove)
        {
            if (prevAxis == Axis.None)
            {
                return true;
            }

            return prevAxis == currentMove;
        }

        private bool IsTapInSameDirection(float currentVal)
        {
            return (prevVelocity == TapVelocity.None) ||
                        (currentVal < 0f && prevVelocity == TapVelocity.Negative) ||
                        (currentVal > 0f && prevVelocity == TapVelocity.Positive);
        }

        private void ProcessTap(Axis axis, float axisVal, Vector2 tapVelocity)
        {
            lastTapTime = Time.time;
            wasLastFrameInput = true;

            if (++currentTapCount == tapsRequired)
            {
                Debug.Log(currentTapCount + " taps on axis: " + axis + " with direction: " + tapVelocity);

                OnTapCountMet(tapVelocity);

                Reset();
            }
            else
            {
                prevAxis = axis;
                prevVelocity = axisVal > 0f ? TapVelocity.Positive : TapVelocity.Negative;
            }
        }

        private void OnTapCountMet(Vector2 tapDirection)
        {
            // Place your code here!
        }
    }
}
