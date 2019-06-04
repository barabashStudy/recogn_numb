using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
//using Tesseract;
//using tessnet3;
using System.Diagnostics;

namespace numb_recpg
{
    class Program
    {
        static private VideoCapture capture;

        static void Main(string[] args)
        {
            Start();

        }











        internal static Mat GetFrame(bool isGrayscale)
        {
            var frame = new Mat();

            capture.Read(frame);

            if (!isGrayscale)
                return frame;

            Cv2.CvtColor(frame, frame, ColorConversionCodes.BGRA2GRAY);
            return frame;
        }





        static public void Start()
        {
            capture = new VideoCapture(0);

            Window window = new Window("Camera");

            using (Mat image = new Mat()) // Frame image buffer
            {
                // Load the cascade
                var haarCascade = new CascadeClassifier(@"haarcascade_russian_plate_number.xml");
                Mat mat = new Mat();

                Rect[] Rects;
                Rect[] Rects2 = new Rect[0];
                Mat cut_numberMat = new Mat();
                Bitmap cut_numberBitmap;

                Bitmap fullPictureBitmap;
                Mat fullPictureMat = new Mat();
                string numberStr;
                char[] numberChar;

                Tesseract.TesseractEngine ocr = new Tesseract.TesseractEngine("./tessdata", "eng", Tesseract.EngineMode.Default);          //Tesseract.EngineMode.TesseractAndCube
                Tesseract.Page page;



                // When the movie playback reaches end, Mat.data becomes NULL.
                while (true)
                {
                    using (Mat frame = GetFrame(true))
                    {
                        if (frame.Empty()) break;
                        window.ShowImage(frame);

                        // Detect number
                        Rects = haarCascade.DetectMultiScale(frame, 1.1, 4, HaarDetectionType.DoRoughSearch | HaarDetectionType.DoCannyPruning, new OpenCvSharp.Size(100, 100));        //, new OpenCvSharp.Size(100, 100)

                        if (Rects.Length > 0)
                        {

                            MemoryStream stream = new MemoryStream();
                            Bitmap tmpBitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(frame);

                            tmpBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

                            var pic = Image.FromStream(stream);

                            using (Graphics g = Graphics.FromImage(pic))
                            {
                                g.DrawImage(tmpBitmap, 0, 0);
                                Pen p = new Pen(Color.Red, 5);

                                g.DrawRectangle(p, Rects[0].X, Rects[0].Y, Rects[0].Width, Rects[0].Height);

                                g.Save();

                                //tmpBitmap.Save("myfile2.png", System.Drawing.Imaging.ImageFormat.Png);
                                //tmpBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

                                fullPictureBitmap = (Bitmap)pic;

                                Cv2.Canny(frame, fullPictureMat, 150, 300);

                                mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(fullPictureBitmap);


                                //Cv2.Canny(frame, fullPictureMat, 50, 200);
                                fullPictureBitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(fullPictureMat);

                                Rectangle rectangle = new Rectangle(Rects[0].X, Rects[0].Y, Rects[0].Width, Rects[0].Height);
                                //Region region = new Region(rectangle);

                                cut_numberBitmap = fullPictureBitmap.Clone(rectangle, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);


                                

                                page = ocr.Process(cut_numberBitmap);




                                numberStr = page.GetText();


                                if (numberStr.Length > 4 && numberStr.Length < 10)
                                    if (Char.IsDigit(numberStr, 1) == true && Char.IsDigit(numberStr, 2) == true && Char.IsDigit(numberStr, 3) == true)
                                    {
                                        cut_numberBitmap.Save("myfile2.png", System.Drawing.Imaging.ImageFormat.Png);
                                        Console.WriteLine(numberStr);
                                    }
                                page.Dispose();
                                //cut_numberBitmap.Dispose();





                            }

                            window.ShowImage(mat);

                        }


                    }

                    Cv2.WaitKey(30);

                }
            }

        }



    }
}

