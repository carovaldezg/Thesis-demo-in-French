using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureDetector
{
    class GestoLeap
    {
        public enum Direction
        {
            Left,
            Right,
            Up,
            Down,
            Forward,
            Backward
        }
        public Direction[] directions { get; set; }
        public int fingers { get; set; }

        public GestoLeap(Direction[] directions, int fingers)
        {
            this.fingers = fingers;
            this.directions = directions;
        }
    }
}
