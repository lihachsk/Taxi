using System;
using System.Collections.Generic;

namespace Help.Models
{

    public class coord
    {
        public String x { get; set; }
        public String y { get; set; }
    }

    public class coordsRegister
    {
        public coord coordsA { get; set; }
        public coord coordsB { get; set; }
        public coordsRegister()
        {
            this.coordsA = new coord();
            this.coordsB = new coord();
        }
    }

    public class distanceItem
    {
        public String text;
        public int value;
    }

    public class elementsElement
    {
        public distanceItem distance;
        public distanceItem duration;
        public String status;

    }
    public class rowsItem
    {
        public elementsElement[] elements;
    }
    //data
    public class pointsItem
    {
        public String street;
        public String streetNum;
        public String houseNum;
    }
}