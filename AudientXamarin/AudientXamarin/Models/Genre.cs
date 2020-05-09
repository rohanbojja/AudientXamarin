using System;
using System.Collections.Generic;
using System.Text;
using MvvmHelpers;

namespace AudientXamarin.Models
{
    public class Genre : ObservableObject
    {
        float _score;
        public float Score
        {
            get
            {
                return _score;
            }
            set
            {
                SetProperty(ref _score, value);
            }
        }

        string _label=String.Empty;
        public string Label
        {
            get
            {
                return _label;
            }
            set
            {
                SetProperty(ref _label, value);
            }
        }

        string _color = String.Empty;
        public string Color
        {
            get
            {
                return _label;
            }
            set
            {
                SetProperty(ref _label, value);
            }
        }
        Genre(float score, string label, string color= "#FFFFFF" )
        {
            this.Score = score;
            this.Label = label;
            this.Color = color;
        }
    }
}
