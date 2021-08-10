using System;
using System.ComponentModel;
using SkiaSharp;
using StaircaseProblem.Models;

namespace StaircaseProblem.ViewModels
{
    public class StairViewModel : ViewModelBase
    {

        private Stair stair;

        public Stair Stair
        {
            get
            {
                return stair;
            }

            set
            {
                if (value != stair)
                {
                    stair = value;
                    NotifyPropertyChanged("Stair");
                }
            }
        }
    }
}
