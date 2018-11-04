using GestureDetector.DataSources;
using GestureDetector.Gestures;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureDetector
{
    class SwipeRightToLeftChecker : GestureChecker
    {
        protected const int ConditionTimeout = 1500;

        public SwipeRightToLeftChecker(Person p)
            : base(new List<Condition> {

                new SwipeRightToLeftCondition(p, JointType.HandRight)

            }, ConditionTimeout, p) { }
    }
    
}
