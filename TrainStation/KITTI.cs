using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainStation
{
    class KITTI
    {
        // DetectNet uses only the type and bbox values of KITTI.
        // https://devblogs.nvidia.com/parallelforall/deep-learning-object-detection-digits/
        // https://devblogs.nvidia.com/parallelforall/detectnet-deep-neural-network-object-detection-digits/

        // DetectNet is a single-class detection network (only Class 1)  (Class 0 = "dontcare")
        // https://github.com/NVIDIA/DIGITS/blob/digits-4.0/digits/extensions/data/objectDetection/README.md#label-format

        // Below is the original KITTI spec

        //http://www.cvlibs.net/datasets/kitti/eval_object.php

        /*
        #Values    Name      Description
        ----------------------------------------------------------------------------
           1    type         Describes the type of object: 'Car', 'Van', 'Truck',
                             'Pedestrian', 'Person_sitting', 'Cyclist', 'Tram',
                             'Misc' or 'DontCare'
           1    truncated    Float from 0 (non-truncated) to 1 (truncated), where
                             truncated refers to the object leaving image boundaries
           1    occluded     Integer (0,1,2,3) indicating occlusion state:
                             0 = fully visible, 1 = partly occluded
                             2 = largely occluded, 3 = unknown
           1    alpha        Observation angle of object, ranging [-pi..pi]
           4    bbox         2D bounding box of object in the image (0-based index):
                             contains left, top, right, bottom pixel coordinates
           3    dimensions   3D object dimensions: height, width, length (in meters)
           3    location     3D object location x,y,z in camera coordinates (in meters)
           1    rotation_y   Rotation ry around Y-axis in camera coordinates [-pi..pi]
           1    score        Only for results: Float, indicating confidence in
                             detection, needed for p/r curves, higher is better.
        */

        // coordinate axis definition (x = right, y = down, z = forward)
        // http://www.cvlibs.net/publications/Geiger2013.pdf, page 32
        // road plane coordinates = XZ plane

        // example data line:
        // Car 1.00 0 2.52 0.00 222.42 211.82 374.00 1.52 1.54 3.68 -2.80 1.76 1.74 1.57

                
        // Properties
        public String type { get; set; }
        public float truncated { get; set; }
        public int occluded { get; set; }
        public float alpha { get; set; } // +/- pi
        public System.Drawing.Rectangle bbox { get; set; } // need floats? 
        public float height { get; set; }
        public float width { get; set; }
        public float length { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float rotation_y { get; set; } // +/- pi

        public KITTI() {
            type = "dummy";
            height = 0.0f;
            width = 0.0f;
            length = 0.0f;
        }

        public KITTI(String lbl, float h, float w, float l)
        {
            type = lbl;
            height = h;
            width = w;
            length = l;
        }

        // toString 
    }
}
