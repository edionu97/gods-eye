from deepface import DeepFace
from face_detector.idetector import IDetector
from face_recogniser.ianalyser import IAnalyser
from resources.models.app_settings_model import AppSettings
from helpers.image_helpers.image_conversion import ImageConversionHelpers


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

    def search_person_in_image(self, searched_person_base64: str, image_base64: str):

        # check if both the searched person and image base 64 are not null or empty
        if not searched_person_base64 or not image_base64:
            raise Exception("Both base64 images must not be null or empty")

        # convert the image with searched person in bgr image
        img_searched_person_as_bgr = ImageConversionHelpers.convert_base64_string_to_bgr_image(searched_person_base64)

        # convert the image in which we are searching the person in bgr image
        img_image_as_bgr = ImageConversionHelpers.convert_base64_string_to_bgr_image(image_base64)

        # find the face of the person in the picture
        # align the face
        face_of_the_searched_person_rgb = DeepFace.detectFace(img_searched_person_as_bgr)





    def analyze_person(self, person_image_base64: str):
        pass
