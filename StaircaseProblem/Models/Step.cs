using SkiaSharp;

namespace StaircaseProblem.Models
{
    public class Step
    {
        public SKPoint StartPoint { get; set; }

        public SKPoint EndPoint { get; set; }

        public bool IsStepped { get; set; }
    }
}
