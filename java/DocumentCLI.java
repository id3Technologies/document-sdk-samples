import eu.id3.document.*;

public class DocumentCLI {

    /*
     * This basic sample shows how to extract information from document
     */

    public static void main(String[] args) {
        System.out.println("-----------------------");
        System.out.println("id3.Document.Sample.CLI");
        System.out.println("-----------------------");

        /*
         * Before calling any function of the SDK you must first check a valid l.icense
         * file. To get such a file please use the provided activation tool.
         */
        DocumentLicense.checkLicense("../id3Document.lic");

        /*
         * The Document SDK heavily relies on deep learning technics and hence requires
         * trained models to run.
         * It also relies on document template files for each model and each document
         * type it might need to process.
         * Fill in the correct path to the downloaded models and the document templates.
         */
        String modelPath = "../sdk/models/";
        String documentTemplatesPath = "../sdk/document_templates/";
        /*
         * Once a model is loaded in the desired processing unit (CPU or GPU) several
         * instances of the associated processor can be created.
         */
        System.out.println("Loading models... ");
        DocumentLibrary.loadModel(modelPath, DocumentModel.DOCUMENT_DETECTOR_2B, ProcessingUnit.CPU); // mandatory
        System.out.println("Done.\n");
        /*
         * It is also required to load documents template files you want to detect.
         * It only has to be called once at the beginning of the code.
         */
        System.out.println("Loading document templates... ");
        DocumentLibrary.loadDocumentTemplate(documentTemplatesPath + "ALB_BO_01001_detector_2B_2.2.0.0.id3dr");
        System.out.println("Done.\n");
        /*
         * Load sample images from files.
         */
        System.out.println("Loading image from files... ");
        DocumentImage image1 = DocumentImage.fromFile("../data/image1.jpg", PixelFormat.BGR_24_BITS);
        System.out.println("Done.\n");

        /*
         * Initialize an instance of document detector that will run on the CPU.
         * This instance has several parameters that can be set:
         * - Model: default value is DocumentDetector1A, but we are using
         * DocumentDetector1B
         * - ThreadCount: allocating more than 1 thread here can increase the speed of
         * the process.
         * - DocumentWidthRatio : approximate width of the document to detect in the
         * image. For mobile capture, 1 is a good choice.
         */
        DocumentDetector documentDetector = new DocumentDetector();
        documentDetector.setModel(DocumentModel.DOCUMENT_DETECTOR_2B);
        documentDetector.setThreadCount(4);
        documentDetector.setDocumentWidthRatio(1);
        /*
         * To speed up the algorithm, you can optionally define a detection rectangle
         * zone in the image.
         * Make sure the coordinates of the 4 points make a straight rectangle.
         * If you don't want to restrict a zone, replace the coordinates by the default
         * class value :
         * Rectangle detectionZone = Rectangle.From...();
         */
        Rectangle detectionZone = Rectangle.fromXywh(0, 800, 2268, 2200);

        /*
         * Detect document in the images.
         */
        System.out.println("Detecting document... ");
        StringList subset = new StringList();
        DetectedDocument detectedDocument = documentDetector.detectDocument(image1, detectionZone, subset);
        System.out.println("Done.\n");
        /*
         * Print score and bounds
         */
        int score = detectedDocument.getConfidence();
        if (score > 0) {
            System.out.println("Detected with score : " + score);
            System.out.println("Corners of the detected document(starting at top - left, then clock - wise) : ");
            for (Point point : detectedDocument.getBounds()) {
                System.out.println("X = " + point.x + ", Y = " + point.y);
            }

            /*
             * Realign detected document in another image
             */
            String image1RealignedPath = "../data/output_image1_aligned.jpg";
            System.out.println("Saving realigned image to " + image1RealignedPath);
            DocumentImage image1Realigned = documentDetector.alignDocument(image1, detectedDocument);
            image1Realigned.toFile(image1RealignedPath, 50);
            image1Realigned.close();
        } else {
            System.out.println("Document not found.");
        }

        System.out.println("-----MRZ Reading module----------");
        /*
         * Load sample images from files.
         */
        System.out.println("Loading images from files... ");
        DocumentImage image2 = DocumentImage.fromFile("../data/image2.jpg", PixelFormat.BGR_24_BITS);
        System.out.println("Done.\n");

        /*
         * To use the mrz reader it is required to first load the model files into the
         * RAM of the desired processing unit.
         * It only has to be called once and then multiple instances of
         * ID3_DOCUMENT_MRZ_READER can be created.
         */
        System.out.println("Loading models... ");
        DocumentLibrary.loadModel(modelPath, DocumentModel.MRZ_READER_2A, ProcessingUnit.CPU);
        System.out.println("Done.\n");

        /*
         * Initialze MRZ Reader and read MRZ
         */
        MrzReader reader = new MrzReader();
        MrzReadingResult result = reader.readMrz(image2);

        /*
         * Print MRZ Type and MRZ string
         */

        if (result.getMrzType() == MrzType.NONE) {
            System.out.println("No MRZ found in the image");
        } else {
            System.out.println("Found MRZ of type " + result.getMrzType() + " :");
            System.out.println(result.getMrz());

            /*
             * Check MRZ Validity
             */

            Boolean validity = MrzHelper.check(result.getMrz(), result.getMrzType());
            System.out.println("MRZ Validity : " + validity);

            /*
             * Decode MRZ and print fields
             */
            System.out.println("Decoding fields from MRZ...");
            StringDict fieldDict = MrzHelper.decode(result.getMrz(), result.getMrzType());
            for (String key : fieldDict.getKeys()) {
                System.out.println(key + " : " + fieldDict.get(key));
            }

        }

        /*
         * Close all SDK ressources
         */
        image1.close();
        reader.close();
        detectedDocument.close();
        documentDetector.close();
        DocumentLibrary.unloadModel(DocumentModel.DOCUMENT_DETECTOR_2B, ProcessingUnit.CPU);
        DocumentLibrary.unloadDocumentTemplate("ALB_BO_01001_detector_2B_2.1.0.0.id3dr");
        System.out.println("Sample terminated successfully.");
    }
}
