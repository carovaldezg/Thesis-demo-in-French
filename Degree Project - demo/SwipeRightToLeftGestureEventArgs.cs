using GestureDetector.Events;
using GestureDetector.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestureDetector
{
    class SwipeRightToLeftGestureEventArgs : GestureEventArgs
    {
        /// <summary>
        /// Direction of the swipe
        /// </summary>
        public Direction Direction { get; set; }
    }
}
