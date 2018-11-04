using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureDetector
{
    class LeapListener : Listener
    {
        Controller leapsensor;
        public LeapListener(Controller sensor)
        {
            leapsensor = sensor;

        }


        public override void OnFrame(Controller controller)
        {

        }
    }
}
