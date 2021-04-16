from mtcnn import MTCNN

from face_detector.helpers.detection_summary import FaceDetectionSummary
from face_detector.idetector import IDetector
from helpers.image_helpers.image_helpers import ImageConversionHelpers


class MtcnnFaceDetector(IDetector):

    def __init__(self):
        """
        Constructs the face detector based on mtcnn model
        """
        self.__mtcnn_detector = MTCNN()

    def identify_faces(self, image_bytes: [], is_bgr=True) -> list[FaceDetectionSummary]:
        # convert the image in the rgb format
        if is_bgr:
            image_bytes = ImageConversionHelpers.convert_bgr_to_rgb(image_bytes)

        # detect the faces
        faces_metadata = self.__mtcnn_detector.detect_faces(image_bytes)

        # iterate the faces_metadata
        for face_metadata in faces_metadata:
            yield FaceDetectionSummary(detection_result=face_metadata)
