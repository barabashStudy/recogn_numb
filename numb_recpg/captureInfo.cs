using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace numb_recpg
{
    public class CaptureInfo
    {
        public Bitmap FrameBmp { get; }
        public Rectangle[] Faces { get; }
        public string[] RecognizedPersons { get; set; }

        internal CaptureInfo(Bitmap frameBmp, Rectangle[] faces)
        {
            FrameBmp = frameBmp;
            Faces = faces;
        }
    }
}
