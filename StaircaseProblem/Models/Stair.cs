using System;
using System.ComponentModel;
using System.Drawing;

namespace StaircaseProblem.Models
{
    public class Stair : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string text;
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                if (value != this.text)
                {
                    this.text = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Color color;
        public Color Color
        {
            get
            {
                return this.color;
            }

            set
            {
                if (value != this.color)
                {
                    this.color = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
