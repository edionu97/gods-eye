from abc import ABC, abstractmethod
from server.services.face_analysis.components.face_detector.helpers.detection_summary import FaceDetectionSummary
from server.services.face_analysis.components.face_detector.helpers.face_detection_box import FaceDetectionBox
from server.services.face_analysis.components.face_recogniser.helpers.face_analiser_summary import \
    FacialAttributeAnalysisModel


class IAnalyser(ABC):

    @abstractmethod
    def search_person_in_image(self,
                               searched_person_base64: str,
                               image_base64: str) -> list[(FaceDetectionSummary, [])]:
        """
        Search the person in one image
        :param searched_person_base64: the image that contains the searched person face
        :param image_base64: the image in which we are searching the person
        :return: the face recognition summary for each identified face
        """
        raise Exception(f"Method {self.search_person_in_image.__name__} is not implemented")

    @abstractmethod
    def analyze_faces_from_image(self,
                                 person_image_base64: str,
                                 detection_boxes: list[FaceDetectionBox]) -> list[FacialAttributeAnalysisModel]:
        """
        Analyse the face of the persons from that list
        :param detection_boxes: face detection boxes
        :param person_image_base64: the image containing the person
        :return: the result about persons analyzing as instances of FacialAttributeAnalysisModel
        """
        raise Exception(f"Method {self.analyze_faces_from_image.__name__} is not implemented")
