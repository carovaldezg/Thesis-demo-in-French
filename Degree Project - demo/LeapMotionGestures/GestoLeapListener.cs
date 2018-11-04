using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureDetector
{
    class GestoLeapListener : Listener
    {
        public delegate void GestureEvent(GestoLeap gesture);

        public event GestureEvent onGesture;

        public float sensitivity { get; set; }

        public GestoLeapListener(int sensitivity)
        {
            this.sensitivity = sensitivity;
        }

       
        public override void OnFrame(Controller controller)
        {
            Frame frame = controller.Frame();
            // Get the first hand
            Hand hand = frame.Hands[0];

            // Check if the hand has any fingers
            FingerList fingers = hand.Fingers;
            if (!fingers.IsEmpty)
            {
                // Calculate the hand's average finger tip position
                Vector avgPos = Vector.Zero;
                Vector avgVelocity = Vector.Zero;
                foreach (Finger finger in fingers)
                {
                    avgPos += finger.TipPosition;
                    avgVelocity += finger.TipVelocity;
                }
                avgPos /= fingers.Count;
                avgVelocity /= fingers.Count;
                List<GestoLeap.Direction> directions = new List<GestoLeap.Direction>();

                if (avgVelocity.y > sensitivity)
                {
                    directions.Add(GestoLeap.Direction.Up);
                }
                else if (avgVelocity.y < -sensitivity)
                {
                    directions.Add(GestoLeap.Direction.Down);
                }

                if (avgVelocity.x > sensitivity)
                {
                    directions.Add(GestoLeap.Direction.Right);
                }
                else if (avgVelocity.x < -sensitivity)
                {
                    directions.Add(GestoLeap.Direction.Left);
                }


                if (avgVelocity.z > sensitivity)
                {
                    directions.Add(GestoLeap.Direction.Backward);
                }
                else if (avgVelocity.z < -sensitivity)
                {
                    directions.Add(GestoLeap.Direction.Forward);
                }

                if (directions.Count > 0)
                {
                    GestoLeap gesture = new GestoLeap(directions.ToArray(), fingers.Count);
                    onGesture(gesture);
                }

                //Console.WriteLine("Hand has " + fingers.Count
                //            + " fingers, average finger tip Velocity: " + avgVelocity);
            }
        }
      
    }
}
