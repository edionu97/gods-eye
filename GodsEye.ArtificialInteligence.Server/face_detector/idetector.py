from abc import ABC, abstractmethod
from face_detector.helpers.detection_summary import FaceDetectionSummary


class IDetector(ABC):

    @abstractmethod
    def identify_faces(self, image_bytes: [], is_bgr=True) -> list[FaceDetectionSummary]:
        """
        This method it is used to identify all the faces from the image
        :param is_bgr: if true is assumed that the image is in bgr format, otherwise
        :param image_bytes: the image pixels
        :return: the identified faces, as a list of FaceDetectionSummary obj
        """
        raise Exception(f"Method {self.identify_faces.__name__} is not implemented")
