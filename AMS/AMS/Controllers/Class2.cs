using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;




namespace AMS.Controllers
{
    public class Rootobject
    {
        public Class2[] Property1 { get; set; }
    }

    public class Class2
    {
        public string faceId { get; set; }
        public Facerectangle faceRectangle { get; set; }
    }

    public class Class3
    {
        public bool isIdentical { get; set; }
        public float confidence { get; set; }
    }


    public class Facerectangle
    {
        public int top { get; set; }
        public int left { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
}