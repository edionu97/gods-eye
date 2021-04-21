from deepface import DeepFace
from deepface.commons import functions
from server.services.face_analysis.components.face_detector.idetector import IDetector
from server.services.face_analysis.components.face_recogniser.ianalyser import IAnalyser
from resources.models.app_settings_model import AppSettings
from helpers.image_helpers.image_drawer import ImageDrawerHelper
from server.services.face_analysis.components.face_detector.helpers.face_detection_box import FaceDetectionBox
from server.services.face_analysis.components.face_detector.helpers.detection_summary import FaceDetectionSummary
from helpers.image_helpers.image_extraction import ImageExtractionHelpers
from helpers.image_helpers.image_conversion import ImageConversionHelpers
from server.services.face_analysis.components.face_recogniser.helpers.face_analiser_summary import FacialAttributeAnalysisModel


class DnnFaceAnalyser(IAnalyser):

    def __init__(self, face_detector: IDetector, app_settings: AppSettings):
        """
        Constructor of the object
        :param face_detector: the face detector
        """
        # initialize the face detector
        self.__face_detector = face_detector

        # build the face recognition model
        # use the model from the app settings or if is not provided use the vgg face model
        self.__face_recognition_model = DeepFace.build_model(app_settings.recognition_model or "VGG-Face")

        # set the app settings
        self.__app_settings = app_settings

        # define the models
        models = {
            'age': lambda: DeepFace.build_model('Age'),
            'gender': lambda: DeepFace.build_model('Gender'),
            'emotion': lambda: DeepFace.build_model('Emotion'),
            'race': lambda: DeepFace.build_model('Race')
        }

        # initialize the dictionary
        self.__facial_attribute_analysis_models = {}

        # iterate the models and build the required models
        for model_key in app_settings.facial_attribute_analysis_models:
            # construct the model
            self.__facial_attribute_analysis_models[model_key] = models[model_key]()

    def search_person_in_image(self,
                               searched_person_base64: str,
                               image_base64: str) -> list[(FaceDetectionSummary, [])]:

        # check if both the searched person and image base 64 are not null or empty
        if not searched_person_base64 or not image_base64:
            raise Exception("Both base64 images must not be null or empty")

        # convert the image with searched person in bgr image
        (img_searched_person_as_bgr, _) = ImageConversionHelpers \
            .convert_base64_string_to_bgr_image(searched_person_base64)

        # convert the image in which we are searching the person in bgr image
        (img_image_as_bgr, _) = ImageConversionHelpers.convert_base64_string_to_bgr_image(image_base64)

        # find the face of the person in the picture
        # align and isolate the face
        # also the image is rgb
        face_of_the_searched_person_rgb = ImageDrawerHelper \
            .draw_border_around_image(DeepFace.detectFace(img_searched_person_as_bgr))

        # detect faces from image
        # each face is isolated and aligned
        detected_face_infos = [
            (processed_face, detection_summary)
            for detection_summary, processed_face in
            self.__detect_faces_and_preprocess_them(img_image_as_bgr)
        ]

        # create the face recognition pairs
        # image pairs must be rgb
        # a pair is an association between the searched face and each face from the image
        face_recognition_pairs = [
            [ImageConversionHelpers.convert_bgr_to_rgb(processed_face), face_of_the_searched_person_rgb]
            for processed_face, _ in detected_face_infos
        ]

        # get the verification result of face recognition
        # it is more efficient to call the function once instead of multiple times in a for each
        face_recognition_result = \
            DeepFace.verify(img1_path=face_recognition_pairs,
                            enforce_detection=False,
                            model=self.__face_recognition_model)

        # process the face recognition result
        return self.__process_the_result(face_recognition_result, detected_face_infos)

    def analyze_faces_from_image(self,
                                 source_image_base64: str,
                                 face_detection_summaries: list[FaceDetectionSummary]):

        # check if both the searched person and image base 64 are not null or empty
        if not source_image_base64:
            raise Exception("The face image must not be null or empty")

        # convert the image into an bgr image
        (source_image_base64_as_bgr, _) = ImageConversionHelpers \
            .convert_base64_string_to_bgr_image(source_image_base64)

        # extract the faces from the images
        detected_faces = self.__extract_faces_from_bgr_image(face_detection_summaries, source_image_base64_as_bgr)

        # convert the faces to rgb
        rgb_detected_faces = [ImageConversionHelpers.convert_bgr_to_rgb(face) for face in detected_faces]

        # convert the faces in rgb format and run the face analysis task
        face_analysis_result = DeepFace.analyze(
            img_path=rgb_detected_faces,
            models=self.__facial_attribute_analysis_models,
            enforce_detection=False,
            actions=self.__app_settings.facial_attribute_analysis_models)

        # analise the faces
        index = -1
        for instance_id in face_analysis_result:
            index += 1
            yield FacialAttributeAnalysisModel(face_analysis_result[instance_id]), rgb_detected_faces[index]

    def __detect_faces_and_preprocess_them(self, bgr_image: []) -> [(FaceDetectionSummary, [])]:

        """
        This function it is used to detect, crop, isolate and align the faces from an image
        :param bgr_image: the image
        :return: a lists of images
        """

        # detect the faces from the image and preprocess them
        for detected_face_summary in self.__face_detector.identify_faces(image_bytes=bgr_image):

            # process the face
            # crop, isolate and align
            processed_face = self.__crop_isolate_and_align_the_face(bgr_image=bgr_image,
                                                                    face_box=detected_face_summary.box)

            # if the face could not be cropped
            # ignore
            if processed_face is None:
                continue

            # get the result (tuple of two elements)
            yield detected_face_summary, processed_face

    @staticmethod
    def __process_the_result(recognition_result: dict,
                             detected_face_infos: [(FaceDetectionSummary, [])]) -> list[(FaceDetectionSummary, [])]:

        # create the index
        pair_index = -1

        # iterate the dictionary
        for face_pair_result in recognition_result:
            # increment the pair index
            pair_index += 1

            # get the face match result
            face_match_result = recognition_result[face_pair_result]

            # ignore the faces that are not matching
            if not face_match_result['verified']:
                continue

            # get the detected face info
            detected_face, detected_face_info = detected_face_infos[pair_index]

            # return the recognition info
            yield detected_face_info, detected_face

    @staticmethod
    def __extract_faces_from_bgr_image(face_detection_summaries: list[FaceDetectionSummary], bgr_image: []) -> []:
        """
        This function it is used for cropping, isolating and aligning the faces using
        Uses the face detection summaries
        :param face_detection_summaries: the list of face detection tips
        :param bgr_image: the image in bgr format
        :return: the list of brg images representing the cropped faces
        """

        # iterate the face detection summaries
        for face_detection_summary in face_detection_summaries:
            # process the face
            # crop, isolate and align
            processed_face = DnnFaceAnalyser.__crop_isolate_and_align_the_face(bgr_image=bgr_image,
                                                                               face_box=face_detection_summary.box)

            # if the face could not be cropped
            # ignore
            if processed_face is None:
                continue

            # return the face
            yield processed_face

    @staticmethod
    def __crop_isolate_and_align_the_face(face_box: FaceDetectionBox, bgr_image: []) -> []:

        """
        This function it is used for cropping, isolating and alignment of one specific face in the image
        :param face_box: the box which contains the image
        :param bgr_image: the bgr image
        :return: none if the image could not be cropped or the processed face
        """

        # get the cropped face
        cropped_face = ImageExtractionHelpers.crop_face_from_image(bgr_image, face_box)

        # if the face could not be cropped
        # ignore
        if cropped_face is None:
            return None

        # draw a border around the image
        # used to better isolate the identified face
        face_with_border = ImageDrawerHelper.draw_border_around_image(cropped_face)

        # align the face
        # for better results
        return functions.align_face(face_with_border, detector_backend='mtcnn')
