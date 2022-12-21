/**
 * This sample shows how to perform basic document processing tasks using id3 Document SDK.
 */

#include <iostream>
#include "id3DocumentLib.h"
#include <vector>

void check(int err, const std::string& func_name)
{
	if (err != id3DocumentError_Success)
	{
		std::cout << "Error " << err << " in " << func_name.c_str() << std::endl;
		exit(1);
	}
}

int main(int argc, char **argv)
{
	std::string data_dir = "data/";
	/**
   	 * Fill in the correct path to the license.
   	 */
	std::string license_path = "your_license_path_here";
	/**
   	 * Fill in the correct path to the downloaded models.
   	 */
	std::string models_dir = "models/";
	/**
   	 * Fill in the correct path to the downloaded document reference files.
   	 */
	std::string document_references_dir = "document_references/";
	/**
   	 * All functions of the API return an error code.
   	 */
	int err = id3DocumentError_Success;
	/**
   	 * The id3 Document SDK needs a valid license to work.
   	 * It is required to provide a path to this license file.
	 * This file can either be retrived using the provided activation tools or the DocumentLicense activation APIs.
   	 * It is required to call the id3DocumentLibrary_CheckLicense() function before calling any other function of the SDK.
   	 */
	std::cout << "Checking license" << std::endl;
	err = id3DocumentLibrary_CheckLicense(license_path.c_str(), nullptr);
	check(err, "id3DocumentLibrary_CheckLicense");
    char version[10];
    int s=10;
    id3DocumentLibrary_GetVersion(version, &s);
    std::cout << "Version : " <<version <<std::endl;

	/**
	 *  ---------------------- Detection Module -----------------------------------
	 */



	/**
   	 * Load one document image from file. Document images are always loaded with BGR 24 bits pixel format.
   	 */
	std::string image1_path = data_dir + "image1.jpg";
	std::cout << "Loading reference image: " << image1_path.c_str() << std::endl;
	ID3_DOCUMENT_IMAGE image1;
	err = id3DocumentImage_Initialize(&image1);
	err = id3DocumentImage_FromFile(image1, image1_path.c_str(), id3DocumentPixelFormat_Bgr24Bits);
	check(err, "id3DocumentImage_FromFile 1");



    ID3_DOCUMENT_DETECTOR detector;
    /**
     * To use the document detector it is required to first load the model files into the RAM of the desired processing unit.
     * It only has to be called once and then multiple instances of ID3_DOCUMENT_DETECTOR can be created.
     */
    std::cout << "Loading document detector 1B model" << std::endl;
    err = id3DocumentLibrary_LoadModel(models_dir.c_str(), id3DocumentModel_DocumentDetector1B, id3DocumentProcessingUnit_Cpu);
    check(err, "id3DocumentLibrary_LoadModel");
    /**
     * It is also required to load documents reference files you want to detect.
       It only has to be called once at the beginning of the code.
     */
    std::cout << "Loading albanian ID reference file" << std::endl;
    err = id3DocumentLibrary_LoadReferenceDocument(document_references_dir.c_str(), "alb_id_detection_1b_1.0.0.0");
    check(err, "id3DocumentLibrary_LoadReferenceDocument");


    /**
    * Once the model is loaded, it is now possible to instantiate an ID3_DOCUMENT_DETECTOR object.
    */
    err = id3DocumentDetector_Initialize(&detector);
    check(err, "id3DocumentDetector_Initialize");
    /**
    * Once the instance is initialized, it is now possible to set its parameters.
        * - Model: default value is DocumentDetector1A, but we are using DocumentDetector1B
        * - ThreadCount: allocating more than 1 thread here can increase the speed of the process.
        * - DocumentWidthRatio : approximate width of the document to detect in the image. For mobile capture, 1 is a good choice.
    */
    err = id3DocumentDetector_SetModel(detector, id3DocumentModel_DocumentDetector1B);
    check(err, "id3DocumentDetector1ADetector_SetModel");
    err = id3DocumentDetector_SetThreadCount(detector, 4);
    check(err, "id3DocumentDetector_SetThreadCount");
    err = id3DocumentDetector_SetDocumentWidthRatio(detector, 1);
    check(err, "id3DocumentDetector_SetDocumentWidthRatio");

    /**
     * To speed up the algorithm, you can optionally define a detection rectangle zone in the image.
     * Make sure the coordinates of the 4 points make a straight rectangle.
     * If you don't want to restrict a zone, replace the coordinates by the default struct value :
        *  id3DocumentRectangle detection_zone = {}
     */
    id3DocumentRectangle detection_zone = {
        {0,3000},
        {2268, 3000},
        {0,800},
        {2268,800 }
    };

    ID3_DETECTED_DOCUMENT detected_document;
    err = id3DetectedDocument_Initialize(&detected_document);
    check(err, "id3DetectedDocument_Initialize");

    /**
     * Detect document in the image
     */
    std::cout << "Detecting document image" << std::endl;
    err = id3DocumentDetector_DetectDocument(detector, image1, "alb_id", &detection_zone, detected_document);
    check(err, "id3DocumentDetector_DetectDocument");

    /**
     * Check detection score
     */
    int score;
    err = id3DetectedDocument_GetDetectionScore(detected_document, &score);
    check(err, "id3DetectedDocument_GetDetectionScore");
    std::cout << "Detected with score : " << score << std::endl;

    /**
     * Get coordinates of the document corners
     */
    ID3_DOCUMENT_POINT_LIST corners_list;
    err = id3DocumentPointList_Initialize(&corners_list);
    err = id3DetectedDocument_GetBounds(detected_document, corners_list);
    check(err, "id3DetectedDocument_GetBounds");
    std::cout << "Corners of the detected document (starting at top-left, then clock-wise) : " << std::endl;

    for (int corner_index=0; corner_index<4; ++corner_index){
        id3DocumentPoint corner = {};
        err = id3DocumentPointList_Get(corners_list, corner_index, &corner);
        check(err, "id3DocumentPointList_Get");
        std::cout << "X = " << corner.X << ", Y = " <<corner.Y<< std::endl;

    }


    /**
     * Realign detected document in another image
     */
    std::string image1_aligned_path = data_dir + "output_image1_aligned.jpg";
    std::cout << "Align document and save new image to "<< image1_aligned_path.c_str() << std::endl;
    ID3_DOCUMENT_IMAGE aligned_image;
    err = id3DocumentImage_Initialize(&aligned_image);
    err = id3DocumentDetector_AlignDocument(detector, image1, detected_document, aligned_image);
    check(err, "id3DocumentDetector_AlignDocument");

    err = id3DocumentImage_ToFile(aligned_image, image1_aligned_path.c_str(), 50);
    check(err, "id3DocumentImage_ToFile");


    /**
     *  ---------------------- MRZ Reading Module -----------------------------------
     */


    /**
   	 * Load one document image from file. Document images are always loaded with BGR 24 bits pixel format.
   	 */
    std::string image2_path = data_dir + "image2.jpg";
    std::cout << "Loading reference image: " << image2_path.c_str() << std::endl;
    ID3_DOCUMENT_IMAGE image2;
    err = id3DocumentImage_Initialize(&image2);
    err = id3DocumentImage_FromFile(image2, image2_path.c_str(), id3DocumentPixelFormat_Bgr24Bits);
    check(err, "id3DocumentImage_FromFile 2");


    ID3_DOCUMENT_MRZ_READER reader;
    /**
     * To use the mrz reader it is required to first load the model files into the RAM of the desired processing unit.
     * It only has to be called once and then multiple instances of ID3_DOCUMENT_MRZ_READER can be created.
     */
    std::cout << "Loading document detector 1B model" << std::endl;
    err = id3DocumentLibrary_LoadModel(models_dir.c_str(), id3DocumentModel_MrzReader2A, id3DocumentProcessingUnit_Cpu);
    check(err, "id3DocumentLibrary_LoadModel 2");


    /**
    * Once the model is loaded, it is now possible to instantiate an ID3_DOCUMENT_DETECTOR object.
    */
    err = id3DocumentMrzReader_Initialize(&reader);
    check(err, "id3DocumentMrzReader_Initialize");

    /**
     * Initialize result object
     */
    ID3_DOCUMENT_MRZ_READING_RESULT reading_result;
    err = id3DocumentMrzReadingResult_Initialize(&reading_result);
    check(err, "id3DocumentMrzReadingResult_Initialize");

    /**
     * Read MRZ in the image
     */
    std::cout << "Reading MRZ in image" << std::endl;
    err = id3DocumentMrzReader_ReadMrz(reader, image2, reading_result);
    check(err, "id3DocumentMrzReader_ReadMrz");

    /**
     * Check detection score
     */
    id3DocumentMrzType mrz_type;
    err = id3DocumentMrzReadingResult_GetMrzType(reading_result, &mrz_type);
    check(err, "id3DocumentMrzReadingResult_GetMrzType");

    if (mrz_type == id3DocumentMrzType_None){
        std::cout << "No MRZ found in the image " << std::endl;
    }
    else{
        /**
         * Retrieve MRZ string. Pre-allocation of 91 characters is enough for all MRZ Types.
         */
        char mrz[91];
        int size= 91;
        err = id3DocumentMrzReadingResult_GetMrz(reading_result, mrz, &size);
        check(err, "id3DocumentMrzReadingResult_GetMrz");
        std::cout << "Found MRZ of type " << mrz_type << " :" <<std::endl;
        std::cout << mrz << std::endl;

        /**
         * Check MRZ validity
         */
        bool validity;
        err = id3DocumentMrzHelper_Check(mrz, mrz_type, &validity);
        check(err, "id3DocumentMrzHelper_Check");
        std::cout<<"MRZ Validity : " << validity << std::endl;


        /**
         * Decode MRZ and print fields
         */
        ID3_DOCUMENT_STRING_DICT field_dict;
        err = id3DocumentStringDict_Initialize(&field_dict);
        err = id3DocumentMrzHelper_Decode(mrz, mrz_type, field_dict);
        check(err, "id3DocumentMrzHelper_Decode");

        /**
         * Display keys and fields from the decoded dictionary
         */
        ID3_DOCUMENT_STRING_ARRAY keys;
        err = id3DocumentStringArray_Initialize(&keys);
        err = id3DocumentStringDict_GetKeys(field_dict ,keys);
        check(err, "id3DocumentStringDict_GetKeys");

        int nb_keys;
        err = id3DocumentStringArray_GetCount(keys, &nb_keys);

        for (int i=0; i<nb_keys ; i++){
            int size=44;
            char key[44];
            id3DocumentStringArray_Get(keys, i, key, &size);

            char field[44];
            size=44;
            err = id3DocumentStringDict_Get(field_dict, key, field, &size);
            check(err, "id3DocumentStringDict_Get");
            std::cout << key <<" : " << field << std::endl;

        }

    }



    std::cout << std::endl
		 << "Press any key..." << std::endl;
	std::cin.get();

	/**
	 * Dispose of all objects and unload models.
	 */
	err = id3DocumentDetector_Dispose(&detector);
    err = id3DocumentImage_Dispose(&image1);
    err = id3DocumentImage_Dispose(&image2);
    err = id3DetectedDocument_Dispose(&detected_document);
    err = id3DocumentPointList_Dispose(&corners_list);
    err = id3DocumentMrzReader_Dispose(&reader);
    err = id3DocumentMrzReadingResult_Dispose(&reading_result);

	err = id3DocumentLibrary_UnloadModel(id3DocumentModel_DocumentDetector1B, id3DocumentProcessingUnit_Cpu);
    err  = id3DocumentLibrary_UnloadReferenceDocument("alb_id");

}
