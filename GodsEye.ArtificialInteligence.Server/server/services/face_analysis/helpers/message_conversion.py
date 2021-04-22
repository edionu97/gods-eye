from server.messages.grpc.server_messages_pb2 import FaceLocationBoundingBox, FacialAttributeAnalysisResponse, \
    FaceKeypointsLocation, FacePoint
from server.services.face_analysis.components.face_detector.helpers.face_detection_box import FaceDetectionBox
from server.services.face_analysis.components.face_detector.helpers.face_points import FacePoints
from server.services.face_analysis.components.face_recogniser.helpers.face_analiser_summary import \
    FacialAttributeAnalysisModel


def grpc_message_to_face_detection_box(grpc_message: FaceLocationBoundingBox) -> FaceDetectionBox:
    """
    Convert the face location bounding box grpc message into an face detection box instance
    :param grpc_message: the message to be converted
    :return: a new instance of face detection box
    """

    # extract fields from the face location box
    top_x = grpc_message.top_x
    top_y = grpc_message.top_y
    bottom_x = grpc_message.bottom_x
    bottom_y = grpc_message.bottom_y

    # get the width and height
    width = bottom_x - top_x
    height = bottom_y - top_y

    # get a new instance of face detection box
    return FaceDetectionBox(detection_box=[top_x, top_y, width, height])


def face_detection_box_model_to_grpc_message_grpc_message(model: FaceDetectionBox) -> FaceLocationBoundingBox:
    """
    This method it is used for converting from an instance of FaceDetectionBox into an grpc associate message
    :return: an instance of FaceLocationBoundingBox
    """

    return FaceLocationBoundingBox(top_x=model.top_x,
                                   top_y=model.top_y,
                                   bottom_x=model.bottom_x,
                                   bottom_y=model.bottom_y)


def facial_attribute_analysis_model_to_grpc_message(
        model: FacialAttributeAnalysisModel) -> FacialAttributeAnalysisResponse:
    """
    Convert the instance of FacialAttributeAnalysisModel into an grpc message
    :param model: the instance to be converted
    :return: a new instance of  FacialAttributeAnalysisResponse
    """

    return FacialAttributeAnalysisResponse(emotion=model.emotion,
                                           race=model.race,
                                           age=model.age,
                                           gender=model.gender)


def face_points_model_to_grpc_message(model: FacePoints) -> FaceKeypointsLocation:
    """
    This method it is used for converting from an instance of FacePoints into an grpc associate message
    :return: an instance of FaceKeypointsLocation
    """
    return FaceKeypointsLocation(right_eye_point=FacePoint(x=model.right_eye_coord[0],
                                                           y=model.right_eye_coord[1]),
                                 left_eye_point=FacePoint(x=model.left_eye_coord[0],
                                                          y=model.left_eye_coord[1]),
                                 nose_point=FacePoint(x=model.nose_coord[0],
                                                      y=model.nose_coord[1]),
                                 mouth_left_point=FacePoint(x=model.mouth_left_coord[0],
                                                            y=model.mouth_left_coord[1]),
                                 mouth_right_point=FacePoint(x=model.mouth_right_coord[0],
                                                             y=model.mouth_right_coord[1]))