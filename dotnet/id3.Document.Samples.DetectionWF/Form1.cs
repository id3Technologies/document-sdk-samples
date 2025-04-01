using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.ComponentModel;
using System.Diagnostics;

namespace id3.Document.Samples.DetectionWF
{
    using id3.Document;
    using System.Data;
    using System.Runtime.InteropServices;

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
        int cameraIndex = 0;

        /*
         * Parameters
         */
        int detectionThreshold = 100;

        /*
         * id3Document SDK objects.
         */
        DocumentImage image;

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
            imagesToDraw = new ImagesToDraw();

            // id3 Document SDK objects
            try
            {
                /*
                 * Before calling any function of the SDK you must first check a valid license file.
                 * To get such a file please use the provided activation tool.
                 */
                DocumentLicense.CheckLicense(@"../../../../../id3Document.lic");
            }
            catch (DocumentException ex)
            {
                MessageBox.Show("Error during license check: " + ex.Message);
                Environment.Exit(-1);
            }

            /*
             * The Document SDK heavily relies on deep learning technics and hence requires trained models to run.
             * It also relies on document template files for each model and each document type it might need to process.
             * Fill in the correct path to the downloaded models and the document template.
             */

            string modelPath = "../../../../../sdk/models";
            string documentTemplatesPath = @"../../../../../sdk/document_templates";
            try
            {
                /*
                 * Once a model and a document template is loaded in the desired processing unit (CPU or GPU) several instances of the associated processor can be created.
                 */
                DocumentLibrary.LoadModel(modelPath, DocumentModel.DocumentDetector2B, ProcessingUnit.Cpu);
                DocumentLibrary.LoadDocumentTemplate(Path.Combine(documentTemplatesPath, "ALB_BO_01001_detector_2B_2.2.0.0.id3dr"));

                /*
                 * Initialize an instance of document detector that will run on the CPU.
                 * This instance has several parameters that can be set:
                  * - Model: default value is DocumentDetector1A, but we are using DocumentDetector1B
                  * - ThreadCount: allocating more than 1 thread here can increase the speed of the process.
                  * - DocumentWidthRatio : approximate width of the document to detect in the image. For mobile capture, 1 is a good choice.
                 */
                documentDetector = new DocumentDetector()
                {
                    Model = DocumentModel.DocumentDetector2B,
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
            capture.Open(cameraIndex);

            if (capture.IsOpened())
            {
                while (!camera.CancellationPending)
                {
                    capture.Read(frame);
                    Bitmap bitmap = BitmapConverter.ToBitmap(frame);

                    // Create image from the first frame
                    byte[] pixels = new byte[3 * frame.Width * frame.Height];
                    Marshal.Copy(frame.Data, pixels, 0, 3 * frame.Width * frame.Height);
                    image = DocumentImage.FromRawBuffer(pixels, frame.Width, frame.Height, 3 * frame.Width, PixelFormat.Bgr24Bits, PixelFormat.Bgr24Bits);

                    /*
                     * To speed up the algorithm, you can optionally define a detection rectangle zone in the image.
                     * Make sure the coordinates of the 4 points make a straight rectangle.
                     * If you don't want to restrict a zone, replace the coordinates by the default struct value :
                     *  Rectangle detectionZone = Rectangle.From...();
                     */

                    int bw = bitmap.Width * 2 / 3;
                    int bh = bitmap.Height * 2 / 3;

                    int bx = bitmap.Width / 2 - bw / 2;
                    int by = bitmap.Height / 2 - bh / 2;

                    Rectangle rectangle = Rectangle.FromXywh(bx, by, bw, bh);

                    using (Graphics gr = Graphics.FromImage(bitmap))
                    {
                        gr.DrawRectangle(new Pen(Color.Blue, 2), ConvertRectangle(rectangle));
                    }

                    // Add preview for UI/process
                    imagesToDraw.BitmapPreview = bitmap;

                    Stopwatch stopWatch = Stopwatch.StartNew();

                    // Detect document of a known type
                    StringList subset = new StringList();
                    var detectedDocument = documentDetector.DetectDocument(image, rectangle, subset);

                    // Check detection score
                    if (detectedDocument.Confidence > detectionThreshold)
                    {
                        // ...draw results
                        PointList bounds = detectedDocument.Bounds;
                        if (bounds != null && bounds.Count >= 4)
                        {
                            using (Graphics gr = Graphics.FromImage(imagesToDraw.BitmapPreview))
                            {
                                System.Drawing.Point[] ptList =
                                {
                                ConvertPoint(bounds.Get(0)),
                                ConvertPoint(bounds.Get(1)),
                                ConvertPoint(bounds.Get(2)),
                                ConvertPoint(bounds.Get(3))

                            };
                                gr.DrawPolygon(new Pen(Color.Green, 2), ptList);
                            }
                        }

                        // Align document
                        var alignedImage = documentDetector.AlignDocument(image, detectedDocument);

                        // Draw result
                        float ratio = 300f / alignedImage.Width;
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
                        DetectionScore = detectedDocument.Confidence
                    };

                    camera.ReportProgress(0, workerProgress);

                    // Release native memory
                    image.Dispose();
                }
            }
        }
        
        // Update UI after each detection
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
            bool ret = capture.Open(cameraIndex);
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