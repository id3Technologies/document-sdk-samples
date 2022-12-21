using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using OpenCvSharp;
using OpenCvSharp.Extensions;




namespace id3.Document.Samples.MRZReaderWF
{
    using System.Runtime.InteropServices;
    using id3.Document;

    public partial class Form1 : Form
    {
        class WorkerProgress
        {
            public long TrackTime;
            public long IndexToDraw;
            public MrzType MrzType;
            public string Mrz;
            public bool IsMrzValid;
            public StringDict MrzDecodedDict;

        }


        Bitmap[] bitmapBuffer; 
        BackgroundWorker camera;
        VideoCapture capture;
        bool isCameraRunning = false;

        /*
         * id3Document SDK objects.
        */
        Image image;

        MrzReader mrzReader;
        bool isTemplateEnrolled = false;

        public Form1()
        {
            InitializeComponent();

            FormClosed += Form1_FormClosed;

            // UI elements
            buttonStartCapture.Click += ButtonStartCapture_Click;
            buttonStartCapture.Enabled = IsCameraPlugged();

            if (!buttonStartCapture.Enabled)
            {
                MessageBox.Show("Please plug-in a webcam before to use this sample.");
                Environment.Exit(-1);
            }

            // Camera background worker
            camera = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            camera.DoWork += Camera_DoWork;
            camera.ProgressChanged += Camera_ProgressChanged;
            camera.RunWorkerCompleted += Camera_RunWorkerCompleted;

            // Bitmap buffer for UI/process
            bitmapBuffer = new Bitmap[2];

            // id3 Document SDK objects
            try
            {
                /*
                 * Before calling any function of the SDK you must first check a valid license file.
                 * To get such a file please use the provided activation tool.
                 */
                DocumentLibrary.CheckLicense(@"your_license_path_here");
            }
            catch (DocumentException ex)
            {
                MessageBox.Show("Error during license check: " + ex.Message);
                Environment.Exit(-1);
            }

            /*
             * The Document SDK heavily relies on deep learning technics and hence requires trained models to run.
             * Fill in the correct path to the downloaded models.
             */
            string modelPath = "..\\..\\..\\..\\..\\sdk\\models";

            try
            {
                /*
                * Once a model is loaded in the desired processing unit (CPU or GPU) several instances of the associated processor can be created.
                */
                DocumentLibrary.LoadModel(modelPath, DocumentModel.MrzReader2A, ProcessingUnit.Cpu);


                /*
                 * Init objects.
                 */
                mrzReader = new MrzReader()
                {
                    ThreadCount = 1
                };
               
            }
            catch (DocumentException ex)
            {
                MessageBox.Show("Error during document objects initialization: " + ex.Message);
                Environment.Exit(-1);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (isCameraRunning)
            {
                StopCaptureCamera();
            }
        }

        // UI events

        private void ButtonStartCapture_Click(object sender, EventArgs e)
        {
            if (buttonStartCapture.Text.Equals("Start capture"))
            {
                StartCaptureCamera();
                buttonStartCapture.Text = "Stop capture";
                
            }
            else
            {
                StopCaptureCamera();
                buttonStartCapture.Text = "Start capture";
               
            }
        }

        // Camera events
        private void Camera_DoWork(object sender, DoWorkEventArgs e)
        {
            Mat frame = new Mat();
            capture = new VideoCapture(0);
            capture.Open(0);

            int bitmapIndex = 0;

            // For real-time processing, consider downscaling the image if necessary
            int maxSize = 2048;

            if (capture.IsOpened())
            {
                while (!camera.CancellationPending)
                {
                    capture.Read(frame);
                    Bitmap bitmap = BitmapConverter.ToBitmap(frame);

                    // Create image from the first frame
                    byte[] pixels = new byte[3 * frame.Width * frame.Height];
                    Marshal.Copy(frame.Data, pixels, 0, 3 * frame.Width * frame.Height);
                    image = Image.FromRawBuffer(pixels, frame.Width, frame.Height, 3 * frame.Width, PixelFormat.Bgr24Bits, PixelFormat.Bgr24Bits);

                    // Resize for real-time capacity
                    float scale = image.Downscale(maxSize);

                    Stopwatch stopWatch = Stopwatch.StartNew();

                    // Detect and read MRZ
                    var result = mrzReader.ReadMrz(image);
                    long trackTime = stopWatch.ElapsedMilliseconds;



                    StringDict decodedDict = new StringDict();
                    bool isMrzValid = false;
                    
                    if(result.MrzType != MrzType.None)
                    {
                        isMrzValid = MrzHelper.Check(result.Mrz, result.MrzType);
                        if (isMrzValid)
                        {
                            decodedDict = MrzHelper.Decode(result.Mrz, result.MrzType);
                        }
                    }

                    bitmapBuffer[bitmapIndex] = bitmap;

                    WorkerProgress workerProgress = new WorkerProgress()
                    {
                        TrackTime = trackTime,
                        IndexToDraw = bitmapIndex,
                        MrzType = result.MrzType,
                        Mrz = result.Mrz,
                        MrzDecodedDict = decodedDict,
                        IsMrzValid = isMrzValid
                    };

                    camera.ReportProgress(0, workerProgress);
                }
            }
        }

        private void Camera_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WorkerProgress workerProgress = (WorkerProgress)e.UserState;
            pictureBoxPreview.Image = bitmapBuffer[workerProgress.IndexToDraw];
            labelDetectionTime.Text = string.Format("Detection time: {0} ms", workerProgress.TrackTime);
            
            labelMrzType.Text = workerProgress.MrzType == MrzType.None ? "No MRZ found" : "Found MRZ of type " + workerProgress.MrzType;
            labelMRZ.ForeColor = workerProgress.IsMrzValid ? Color.Green : Color.Red;

            /* Split lines in the MRZ for display */
            if (workerProgress.MrzType == MrzType.Td1)
            {
                labelMRZ.Text = workerProgress.Mrz.Substring(0, 30) + "\n"
                              + workerProgress.Mrz.Substring(30, 30) + "\n"
                              + workerProgress.Mrz.Substring(60, 30);
            }
            else if (workerProgress.MrzType == MrzType.Td2)
            {
                labelMRZ.Text = workerProgress.Mrz.Substring(0, 36) + "\n" + workerProgress.Mrz.Substring(36, 36);
            }
            else if (workerProgress.MrzType == MrzType.Td3)
            {
                labelMRZ.Text = workerProgress.Mrz.Substring(0, 44) + "\n" + workerProgress.Mrz.Substring(44, 44);
            }
            else
            {
                labelMRZ.Text = workerProgress.Mrz;
            }

            labelMrzDecode.Text = "Decoded fields :\n";
            if (workerProgress.IsMrzValid)
            {
              
                StringDict fieldDict = MrzHelper.Decode(workerProgress.Mrz, workerProgress.MrzType);
                foreach (string key in fieldDict.GetKeys())
                {
                    labelMrzDecode.Text += (key + " : " + workerProgress.MrzDecodedDict.Get(key)) + '\n';
                }

            }

        }

        private void Camera_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isCameraRunning = false;
        }

        // Utils

        private bool IsCameraPlugged()
        {
            capture = new VideoCapture(0);
            bool ret = capture.Open(0);
            if (ret)
            {
                capture.Release();
            }
            return ret;
        }

        private void StartCaptureCamera()
        {
            camera.RunWorkerAsync();
            isCameraRunning = true;
        }

        private void StopCaptureCamera()
        {
            camera.CancelAsync();
            while (isCameraRunning)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }
        }


        private System.Drawing.Rectangle ConvertRectangle(Rectangle rectangle)
        {
            return new System.Drawing.Rectangle(rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.TopRight.X - rectangle.TopLeft.X,
                rectangle.BottomLeft.Y - rectangle.TopLeft.Y);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}