import grpc

from server.messages.grpc.server_messages_pb2 import *
from server.services.face_analysis.helpers.message_conversion import *
from helpers.image_helpers.image_conversion import ImageConversionHelpers
from helpers.image_helpers.image_extraction import ImageExtractionHelpers
from server.impl.base.grpc_server_base import FacialRecognitionAndAnalysisServicer
from server.services.face_analysis.components.face_detector.idetector import IDetector
from server.services.face_analysis.components.face_recogniser.ianalyser import IAnalyser


class FacialRecognitionAndAnalysisService(FacialRecognitionAndAnalysisServicer):

    def __init__(self, face_detector: IDetector, face_analyser: IAnalyser):
        """
        Constructor of the face recognition and analysis server
        :param face_detector: the face detector
        :param face_analyser: the face analyser
        """

        # set the face detector component
        self.__face_detector = face_detector

        # set the face analyser component
        self.__face_analyser = face_analyser

    def DoFacialRecognition(self, request, context):
        """
            This represents the method that handles the facial recognition
            As input it takes an instance of SearchForPersonRequest
            As output returns an instance of SearchForPersonRequest
        :param request: the SearchForPersonRequest
        :param context: the grpc context
        :return: an instance of SearchForPersonRequest
        """

        # catch the exception and set the status code accordingly
        try:
            # handle the facial recognition task
            return self.__handle_the_facial_recognition(person_b64=request.person_image_b64,
                                                        image_b64=request.location_image_b64,
                                                        include_faces=request.include_cropped_faces_in_response)
        except Exception as e:
            # if any exception occur then we set the code accordingly
            context.set_details = str(e)
            context.set_code = grpc.StatusCode.INVALID_ARGUMENT
            raise e

    def DoFacialAttributeAnalysis(self, request, context):

        # catch the exception and set the status code accordingly
        try:
            # handle the facial recognition task
            return self.__handle_the_facial_analysis_request(is_face_location_known=request.is_face_location_known,
                                                             face_bounding_box=request.face_bounding_box,
                                                             image_b64=request.analyzed_image_containing_the_face_b64)
        except Exception as e:
            # if any exception occur then we set the code accordingly
            context.set_details = str(e)
            context.set_code = grpc.StatusCode.INVALID_ARGUMENT
            raise e

    def __handle_the_facial_recognition(self,
                                        person_b64: str,
                                        image_b64: str,
                                        include_faces: bool) -> SearchForPersonResponse:
        """
        This function is responsible for handling the facial recognition request
        :param person_b64: the image or searched person as base 64
        :param image_b64: the image in which we are searching the person
        :param include_faces: if true in response we will include also the faces as base64
        :return: an instance of SearchForPersonResponse
        """

        # declare the face recognition information list
        face_recognition_infos = []

        # get the image extension
        original_image_extension = ImageExtractionHelpers.extract_image_extension_from_base64_string(image_b64)

        # iterate the result
        for face_info, face in self.__face_analyser.search_person_in_image(image_base64=image_b64,
                                                                           searched_person_base64=person_b64):
            # create the face location bounding box
            face_location_bounding_box = face_detection_box_model_to_grpc_message_grpc_message(face_info.box)

            # convert the face points into an grpc message
            face_points = face_points_model_to_grpc_message(face_info.face_points)

            # if the faces are not required
            if not include_faces:
                face_recognition_infos.append(FaceRecognitionInformation(face_bounding_box=face_location_bounding_box,
                                                                         face_points=face_points))
                continue

            # convert the face into an base64 img
            # use the same extension as original image
            cropped_face_image_b64 = ImageConversionHelpers \
                .convert_bgr_image_to_base64_string(image_array=face,
                                                    image_extension=original_image_extension)

            # add all the information in face recognition
            face_recognition_infos.append(FaceRecognitionInformation(face_bounding_box=face_location_bounding_box,
                                                                     face_points=face_points,
                                                                     cropped_face_image_b64=cropped_face_image_b64))

        # return the response
        return SearchForPersonResponse(is_found=len(face_recognition_infos) > 0,
                                       face_recognition_info=face_recognition_infos)

    def __handle_the_facial_analysis_request(self,
                                             is_face_location_known: bool,
                                             face_bounding_box: FaceLocationBoundingBox,
                                             image_b64: str) -> FacialAttributeAnalysisResponse:

        # if the face location is unknown then detect it
        if not is_face_location_known:
            face_bounding_box = self.__identify_face(image_b64=image_b64)

        # if the face bounding box is none => the face does not exist
        if face_bounding_box is None:
            raise Exception("The face could not be identified in the given picture, analysis stopped...")

        # convert the message in an instance of FaceDetectionBox
        face_detection_box = grpc_message_to_face_detection_box(grpc_message=face_bounding_box)

        # iterate the results
        # keep only first item
        for analysis_result, _ in self.__face_analyser.analyze_faces_from_image(person_image_base64=image_b64,
                                                                                detection_boxes=[face_detection_box]):
            # return the result
            return facial_attribute_analysis_model_to_grpc_message(model=analysis_result)

        # return empty response
        return FacialAttributeAnalysisResponse()

    def __identify_face(self, image_b64: str) -> FaceLocationBoundingBox:
        """
        This function it is used for selecting the face with higher confidence
        :param image_b64: the image in which the faces are searched
        :return: none if in image could not be identified any face or the face with higher confidence
        """

        # convert the image from base64 into an bgr image
        (img_image_as_bgr, _) = ImageConversionHelpers.convert_base64_string_to_bgr_image(image_b64)

        # detect the faces
        detected_faces = [(face_summary.box, face_summary.confidence)
                          for face_summary in self.__face_detector.identify_faces(image_bytes=img_image_as_bgr,
                                                                                  is_bgr=True)]

        # sort the faces by confidence
        sorted_faces_by_confidence = sorted(detected_faces, key=lambda pair: pair[1], reverse=True)

        # get the first face
        # the face with the highest confidence
        if len(sorted_faces_by_confidence) > 0:
            # get the face
            selected_face_box = sorted_faces_by_confidence[0][0]

            # convert to message
            return face_detection_box_model_to_grpc_message_grpc_message(model=selected_face_box)

        # none if the face could not be identified
        return None
