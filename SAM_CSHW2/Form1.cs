using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SAM_CSHW2
{

    public partial class Form1 : Form
    {
        private VideoCapture _capture;
        private Thread _captureThread;
       


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _capture = new VideoCapture(0);
            _captureThread = new Thread(DisplayWebcam);
            _captureThread.Start(); 

        }
        private void DisplayWebcam()
        {
            while(_capture.IsOpened)
            {
                //Raw Image code
                Mat frame = _capture.QueryFrame();
                

                int newHeight = (frame.Size.Height * PictureBox.Size.Width) / frame.Size.Width;
                Size newSize = new Size(PictureBox.Size.Width, newHeight);
               
                CvInvoke.Resize(frame, frame, newSize);


                PictureBox.Image = frame.ToBitmap();


                //Code for a grayscale and binary-threshholding for GrayPictureBox

                int newgrayHeight = (frame.Size.Height * GrayPictureBox.Size.Width) / frame.Size.Width;
                Size newgraySize = new Size(GrayPictureBox.Size.Width, newgrayHeight);

                CvInvoke.Resize(frame, frame, newgraySize);


                Mat grayFrame = new Mat();
                CvInvoke.CvtColor(frame, grayFrame, ColorConversion.Bgr2Gray);

                
                Mat binaryFrame = new Mat();
                //CvInvoke.Threshold(grayFrame, binaryFrame, 120, 255, ThresholdType.Binary | ThresholdType.Otsu);
                CvInvoke.AdaptiveThreshold(grayFrame, binaryFrame,255, AdaptiveThresholdType.MeanC, ThresholdType.BinaryInv, 11, 2);


                //Split GrayFrame into 5 slices...

                int width = binaryFrame.Width;
                int height = binaryFrame.Height;
                int Slicewidth = width / 5;
                int[] whitePixelCounts = new int[5];


                for (int i = 0; i < 5; i++) 
                {
                    Rectangle roi = new Rectangle(i * Slicewidth, 0, Slicewidth, height);
                    Mat slice = new Mat(binaryFrame, roi);
                    whitePixelCounts[i] = CvInvoke.CountNonZero(slice);
                    //Update lable with white pixle count 

                    this.Invoke((MethodInvoker)delegate {
                        textBox1.Text = $"1st: {whitePixelCounts[0]}";
                        textBox2.Text = $"2nd: {whitePixelCounts[1]}";
                        textBox3.Text = $"3rd: {whitePixelCounts[2]}";
                        textBox4.Text = $"4th: {whitePixelCounts[3]}";
                        textBox5.Text = $"5th: {whitePixelCounts[4]}";
                    });



                    CvInvoke.Rectangle(binaryFrame, roi, new MCvScalar(0, 255, 0), 2);
                
                }


                GrayPictureBox.Image = binaryFrame.ToBitmap();

                



            }



        }
        private void Form1_FromClosing(object sender, FormClosingEventArgs e)
        {
            _captureThread.Abort();
        }





       

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void GrayPictureBox_Click(object sender, EventArgs e)
        {

        }
    }
}
