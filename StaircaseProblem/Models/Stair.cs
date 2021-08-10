using System.Collections.Generic;

namespace StaircaseProblem.Models
{
    public class Stair
    {
        public int NumberOfSteps { get; set; }

        public int StepWidth { get; set; }

        public int StepHeight { get; set; }

        public string WaysToReachTheTop { get; set; }

        public List<Step> Step { get; set; }

        public List<string> WaysToClimb { get; set; }
    }
}
