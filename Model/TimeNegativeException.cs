using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrajectoryOfSensorVisualization.Model
{
    public class TimeNegativeException : Exception
    {
        public TimeNegativeException(string message)
            : base(message) { }
    }
}
