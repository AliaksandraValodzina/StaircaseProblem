using System;
using SkiaSharp;

namespace StaircaseProblem.Models
{
    public class Step
    {
        public SKPoint StartPoint { get; set; }

        public SKPoint EndPoint { get; set; }

        public SKColor Color { get; set; }
    }
}
