using System;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System.Collections.Generic;

namespace StritWalk
{
    public class CustomMap : Map
    {
        public List<Position> ShapeCoordinates { get; set; }
        public List<CustomPin> CustomPins { get; set; }
        public List<Pin> PinList { get; set; }

        public CustomMap()
        {
            ShapeCoordinates = new List<Position>();
            PinList = new List<Pin>();
        }
    }
}
    