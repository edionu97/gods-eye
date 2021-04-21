import cv2
from mtcnn import MTCNN
from server.services.face_analysis.components.face_detector.idetector import IDetector
from server.services.face_analysis.components.face_detector.helpers.detection_summary import FaceDetectionSummary
from helpers.image_helpers.image_conversion import ImageConversionHelpers


class MtcnnFaceDetector(IDetector):

    def __init__(self):
        """
        Constructs the face detector based on mtcnn model
        """
        self.__mtcnn_detector = MTCNN()

    def identify_faces(self,
                       image_bytes: [],
                       is_bgr: bool = True,
                       resize_to: (int, int) = None) -> list[FaceDetectionSummary]:
        # convert the image in the rgb format
        if is_bgr:
            image_bytes = ImageConversionHelpers.convert_bgr_to_rgb(image_bytes)

        # resize the image if specified
        if resize_to:
            image_bytes = cv2.resize(image_bytes, resize_to)

        # detect the faces
        faces_metadata = self.__mtcnn_detector.detect_faces(image_bytes)

        # iterate the faces_metadata
        for face_metadata in faces_metadata:
            yield FaceDetectionSummary(detection_result=face_metadata)
