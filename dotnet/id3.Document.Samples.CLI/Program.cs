using System;
using id3.Document;

// This basic sample shows how to detect a document and read a MRZ in an image

Console.WriteLine("-------------------------------");
Console.WriteLine("id3.Document.Samples.CLI");
Console.WriteLine("-------------------------------");

Console.WriteLine("-----Detection module-----------");



try
{
    // Before calling any function of the SDK you must first check a valid license file.
    // To get such a file please use the provided activation tool.
    DocumentLibrary.CheckLicense(@"your_license_path_here");
}
catch (DocumentException ex)
{
    Console.WriteLine("Error during license check" + ex.Message);
    Environment.Exit(-1);
}

/*
 * The Document SDK heavily relies on deep learning technics and hence requires trained models to run.
 * It also relies on document reference files for each model and each document type it might need to process.
 * Fill in the correct path to the downloaded models and the document references.
 */
string modelPath = "../../../../../sdk/models";
string documentReferencesPath = @"../../../../../sdk/document_references";

/*
* Once a model is loaded in the desired processing unit (CPU or GPU) several instances of the associated processor can be created.
* For instance in this sample, we load a detector and an encoder.
*/
Console.Write("Loading models... ");
DocumentLibrary.LoadModel(modelPath, DocumentModel.DocumentDetector1B, ProcessingUnit.Cpu);
Console.Write("Done.\n");
/*
 * It is also required to load documents reference files you want to detect.
 * It only has to be called once at the beginning of the code.
*/
Console.Write("Loading document references... ");
DocumentLibrary.LoadReferenceDocument(documentReferencesPath, "alb_id_detection_1b_1.0.0.0");
Console.Write("Done.\n");

/*
 * Load sample images from files.
 */
Console.Write("Loading images from files... ");
Image image1 = Image.FromFile("../../../../../data/image1.jpg", PixelFormat.Bgr24Bits);
Console.Write("Done.\n");

/*
 * Initialize an instance of document detector that will run on the CPU.
 * This instance has several parameters that can be set:
  * - Model: default value is DocumentDetector1A, but we are using DocumentDetector1B
  * - ThreadCount: allocating more than 1 thread here can increase the speed of the process.
  * - DocumentWidthRatio : approximate width of the document to detect in the image. For mobile capture, 1 is a good choice.
 */
DocumentDetector documentDetector = new DocumentDetector()
{
    Model = DocumentModel.DocumentDetector1B,
    ThreadCount = 4,
    DocumentWidthRatio = 1
};

/*
  * To speed up the algorithm, you can optionally define a detection rectangle zone in the image.
  * Make sure the coordinates of the 4 points make a straight rectangle.
  * If you don't want to restrict a zone, replace the coordinates by the default struct value :
  *  Rectangle detectionZone = new Rectangle();
 */
Rectangle detectionZone = new Rectangle(
    new Point(0, 3000),
    new Point(2268, 3000),
    new Point(0, 800),
    new Point(2268, 800));

/*
 * Detect document in the images.
 */
Console.Write("Detecting document... ");
DetectedDocument detectedDocument = documentDetector.DetectDocument(image1, "alb_id", detectionZone);
Console.Write("Done.\n");

/*
 * Print score and bounds
 */
int score = detectedDocument.DetectionScore;
if(score > 0)
{
    Console.WriteLine("Detected with score : " + score);
    Console.WriteLine("Corners of the detected document(starting at top - left, then clock - wise) : ");
    foreach (Point point in detectedDocument.Bounds)
    {
        Console.WriteLine("X = " + point.X + ", Y = " + point.Y);
    }

    /*
    * Realign detected document in another image
    */
    string image1RealignedPath = "../../../../../data/output_image1_aligned.jpg";
    Console.WriteLine("Saving realigned image to " + image1RealignedPath);
    Image image1Realigned = documentDetector.AlignDocument(image1, detectedDocument);
    image1Realigned.ToFile(image1RealignedPath, 50);

    
}
else
{
    Console.WriteLine("Document not found.");
}

Console.WriteLine("-----MRZ Reading module----------");
/*
 * Load sample images from files.
 */
Console.Write("Loading images from files... ");
Image image2 = Image.FromFile("../../../../../data/image2.jpg", PixelFormat.Bgr24Bits);
Console.Write("Done.\n");

/*
 * To use the mrz reader it is required to first load the model files into the RAM of the desired processing unit.
 * It only has to be called once and then multiple instances of ID3_DOCUMENT_MRZ_READER can be created.
*/
Console.Write("Loading models... ");
DocumentLibrary.LoadModel(modelPath, DocumentModel.MrzReader2A, ProcessingUnit.Cpu);
Console.Write("Done.\n");

/*
 * Initialze MRZ Reader and read MRZ
*/
MrzReader reader = new MrzReader();
MrzReadingResult result =  reader.ReadMrz(image2);

/*
 * Print MRZ Type and MRZ string
 */

if (result.MrzType == MrzType.None)
{
    Console.WriteLine("No MRZ found in the image");
}
else
{
    Console.WriteLine("Found MRZ of type "+result.MrzType + " :");
    Console.WriteLine(result.Mrz);


    /*
     * Check MRZ Validity
     */

    bool validity = MrzHelper.Check(result.Mrz, result.MrzType);
    Console.WriteLine("MRZ Validity : " + validity);


    /*
     * Decode MRZ and print fields
     */
    Console.WriteLine("Decoding fields from MRZ...");
    StringDict fieldDict = MrzHelper.Decode(result.Mrz, result.MrzType);
    foreach (string key in fieldDict.GetKeys())
    {
        Console.WriteLine(key + " : " + fieldDict.Get(key));
    }

}

Console.WriteLine("Sample terminated successfully.");
Console.ReadKey();
