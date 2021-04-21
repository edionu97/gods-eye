import grpc

from helpers.image_helpers.image_conversion import ImageConversionHelpers
from helpers.image_helpers.image_extraction import ImageExtractionHelpers
from server.messages.grpc.server_messages_pb2 import *
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
        print(self.DoFacialAttributeAnalysis.__name__)
        return FacialAttributeAnalysisResponse()

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
            face_location_bounding_box = face_info.box.convert_to_grpc_message()

            # if the faces are not required
            if not include_faces:
                face_recognition_infos.append(FaceRecognitionInformation(face_bounding_box=face_location_bounding_box))
                continue

            # convert the face into an base64 img
            # use the same extension as original image
            cropped_face_image_b64 = ImageConversionHelpers \
                .convert_bgr_image_to_base64_string(image_array=face,
                                                    image_extension=original_image_extension)

            # add all the information in face recognition
            face_recognition_infos.append(FaceRecognitionInformation(face_bounding_box=face_location_bounding_box,
                                                                     cropped_face_image_b64=cropped_face_image_b64))

        # return the response
        return SearchForPersonResponse(is_found=len(face_recognition_infos) > 0,
                                       face_recognition_info=face_recognition_infos)
