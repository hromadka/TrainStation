using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainStation
{
    class Airplane
    {
        // Properties
        public String label { get; set; }
        public float height { get; set; }
        public float width { get; set; }
        public float length { get; set; }

        public Airplane() {
            label = "dummy";
            height = 0.0f;
            width = 0.0f;
            length = 0.0f;
        }

        public Airplane(String lbl, float h, float w, float l) {
            label = lbl;
            height = h;
            width = w;
            length = l;
        }

    }
}
