from face_detector.helpers.face_detection_box import FaceDetectionBox
from face_detector.helpers.face_points import FacePoints


class FaceDetectionSummary:

    @property
    def box(self) -> FaceDetectionBox:
        return self.__bounding_box

    @property
    def confidence(self) -> float:
        return self.__confidence

    @property
    def face_points(self) -> FacePoints:
        return self.__face_points

    def __init__(self, detection_result: dict):
        # set the bounding box
        self.__bounding_box = FaceDetectionBox(detection_box=detection_result['box'])

        # set the confidence
        self.__confidence = detection_result['confidence']

        # set the face points
        self.__face_points = FacePoints(key_points=detection_result['keypoints'])
