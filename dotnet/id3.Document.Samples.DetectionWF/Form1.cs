
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using OpenCvSharp;
using OpenCvSharp.Extensions;




namespace id3.Document.Samples.DetectionWF
{
    using System.Runtime.InteropServices;
    using id3.Document;


    public partial class Form1 : Form
    {
        class WorkerProgress
        {
            public long TrackTime;
            public int DetectionScore;
        }

        class ImagesToDraw
        {
            public Bitmap BitmapPreview;
            public System.Drawing.Image ImageAligned;
        }

        /*
         * Global variables
         */
        BackgroundWorker camera;
        VideoCapture capture;
        bool isCameraRunning = false;
        ImagesToDraw imagesToDraw;

        /*
         * Parameters
         */
        string document_name = "alb_id";
        int detectionThreshold = 24;

        /*
         * id3Document SDK objects.
         */
        Image image;

        DocumentDetector documentDetector;

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
            imagesToDraw= new ImagesToDraw();

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
             * It also relies on document reference files for each model and each document type it might need to process.
             * Fill in the correct path to the downloaded models and the document references.
             */

            string modelPath = "..\\..\\..\\..\\..\\sdk\\models";
            string documentReferencesPath = "..\\..\\..\\..\\..\\sdk\\document_references";
            try
            {
                /*
                 * Once a model and a document reference is loaded in the desired processing unit (CPU or GPU) several instances of the associated processor can be created.
                 */
                DocumentLibrary.LoadModel(modelPath, DocumentModel.DocumentDetector1B, ProcessingUnit.Cpu);
                DocumentLibrary.LoadReferenceDocument(documentReferencesPath, "alb_id_detection_1b_1.0.0.0");

                /*
                 * Initialize an instance of document detector that will run on the CPU.
                 * This instance has several parameters that can be set:
                  * - Model: default value is DocumentDetector1A, but we are using DocumentDetector1B
                  * - ThreadCount: allocating more than 1 thread here can increase the speed of the process.
                  * - DocumentWidthRatio : approximate width of the document to detect in the image. For mobile capture, 1 is a good choice.
                 */
                documentDetector = new DocumentDetector()
                {
                    Model = DocumentModel.DocumentDetector1B,
                    ProcessingUnit = ProcessingUnit.Cpu,
                    DocumentWidthRatio = 1f,
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

                    /*
                      * To speed up the algorithm, you can optionally define a detection rectangle zone in the image.
                      * Make sure the coordinates of the 4 points make a straight rectangle.
                      * If you don't want to restrict a zone, replace the coordinates by the default struct value :
                      *  Rectangle detectionZone = new Rectangle();
                     */

                    int bw = bitmap.Width * 2 / 3;
                    int bh = bitmap.Height * 2 / 3;

                    int bx = bitmap.Width / 2 - bw / 2;
                    int by = bitmap.Height / 2 - bh / 2;

                    Point tl = new Point(bx, by);
                    Point tr = new Point(bx + bw, by);
                    Point bl = new Point(bx, by + bh);
                    Point br = new Point(bx + bw, by + bh);
                    Rectangle rectangle = new Rectangle(bl, br, tl, tr);

                    using (Graphics gr = Graphics.FromImage(bitmap))
                    {
                        gr.DrawRectangle(new Pen(Color.Blue, 2), ConvertRectangle(rectangle));
                    }




                    // Add preview for UI/process
                    imagesToDraw.BitmapPreview = bitmap;


                    Stopwatch stopWatch = Stopwatch.StartNew();

                    // Detect document of a known type
                    var detectedDocument = documentDetector.DetectDocument(image, document_name, rectangle);

                    //Check detection score
                    if (detectedDocument.DetectionScore > detectionThreshold)
                    {

                        // ...draw results
                        using (Graphics gr = Graphics.FromImage(imagesToDraw.BitmapPreview))
                        {
                            System.Drawing.Point[] ptList =
                            {
                                ConvertPoint(detectedDocument.Bounds.Get(0)),
                                ConvertPoint(detectedDocument.Bounds.Get(1)),
                                ConvertPoint(detectedDocument.Bounds.Get(2)),
                                ConvertPoint(detectedDocument.Bounds.Get(3))

                            };
                            gr.DrawPolygon(new Pen(Color.Green, 2), ptList);
                        }
                        // Align document
                        var alignedImage = documentDetector.AlignDocument(image, detectedDocument);



                        // Draw result
                        float ratio = 300f / alignedImage.GetWidth();
                        alignedImage.Resize(300, 190);
                        using (MemoryStream memStream = new MemoryStream(alignedImage.ToBuffer(ImageFormat.Jpeg, 0)))
                        {
                            imagesToDraw.ImageAligned = System.Drawing.Image.FromStream(memStream);
                        }

                    }

                    long trackTime = stopWatch.ElapsedMilliseconds;


                    WorkerProgress workerProgress = new WorkerProgress()
                    {
                        TrackTime = trackTime,
                        DetectionScore = detectedDocument.DetectionScore
                    };

                    camera.ReportProgress(0, workerProgress);
                }
            }
        }
        
        //Update UI after each detection
        private void Camera_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WorkerProgress workerProgress = (WorkerProgress)e.UserState;
            pictureBoxPreview.Image = imagesToDraw.BitmapPreview;
            pictureBoxAligned.Image = imagesToDraw.ImageAligned;

            labelDetectionTime.Text = string.Format("Detection time: {0} ms", workerProgress.TrackTime);
            labelDetectionScore.Text = String.Format("Detection score : {0}", workerProgress.DetectionScore);

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
        private System.Drawing.Point ConvertPoint(Point pt)
        {
            return new System.Drawing.Point(pt.X, pt.Y);
        }


    }
}