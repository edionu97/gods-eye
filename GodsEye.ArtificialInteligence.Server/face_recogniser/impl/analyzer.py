from deepface import DeepFace
from deepface.commons import functions
from face_detector.idetector import IDetector
from face_recogniser.ianalyser import IAnalyser
from resources.models.app_settings_model import AppSettings
from helpers.image_helpers.image_drawer import ImageDrawerHelper
from face_detector.helpers.detection_summary import FaceDetectionSummary
from helpers.image_helpers.image_extraction import ImageExtractionHelpers
from helpers.image_helpers.image_conversion import ImageConversionHelpers

import matplotlib.pyplot as plt


class DnnFaceAnalyser(IAnalyser):

    def __init__(self, face_detector: IDetector, app_settings: AppSettings):
        """
        Constructor of the object
        :param face_detector: the face detector
        """
        # initialize the face detector
        self.__face_detector = face_detector

        # set the app settings
        self.__app_settings = app_settings

        # build the face recognition model
        # use the model from the app settings or if is not provided use the vgg face model
        self.__face_recognition_model = DeepFace.build_model(app_settings.recognition_model or "VGG-Face")

    def analyze_person(self, person_image_base64: str):
        pass

    def search_person_in_image(self, searched_person_base64: str, image_base64: str):

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
        # a pair is an association between the searched face and each face from the image
        face_recognition_pairs = [
            [processed_face, face_of_the_searched_person_rgb] for processed_face, _ in detected_face_infos
        ]

        # get the verification result of face recognition
        # it is more efficient to call the function once instead of multiple times in a for each
        face_recognition_result = \
            DeepFace.verify(img1_path=face_recognition_pairs,
                            enforce_detection=False,
                            model=self.__face_recognition_model)

        # process the face recognition result
        return self.__process_the_result(face_recognition_result, detected_face_infos)

    def __detect_faces_and_preprocess_them(self, bgr_image: []) -> [(FaceDetectionSummary, [])]:

        """
        This function it is used to detect, isolate and align the faces from an image
        :param bgr_image: the image
        :return: a lists of images
        """

        # detect the faces from the image and preprocess them
        for detected_face_summary in self.__face_detector.identify_faces(image_bytes=bgr_image):
            # get the face box
            face_box = detected_face_summary.box

            # get the cropped face
            cropped_face = ImageExtractionHelpers.crop_face_from_image(bgr_image, face_box)

            # if the face could not be cropped
            # ignore
            if cropped_face is None:
                continue

            # draw a border around the image
            # used to better isolate the identified face
            face_with_border = ImageDrawerHelper.draw_border_around_image(cropped_face)

            # align the face
            # for better face recognition
            yield detected_face_summary, functions.align_face(face_with_border, detector_backend='mtcnn')

    @staticmethod
    def __process_the_result(recognition_result: dict,
                             detected_face_infos: [(FaceDetectionSummary, [])]) -> (list[FaceDetectionSummary], []):

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
