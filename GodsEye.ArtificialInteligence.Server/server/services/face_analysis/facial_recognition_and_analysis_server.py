from server.messages.grpc.server_messages_pb2 import *
from server.impl.base.grpc_server_base import FacialRecognitionAndAnalysisServicer


class FacialRecognitionAndAnalysisService(FacialRecognitionAndAnalysisServicer):

    def DoFacialRecognition(self, request, context):

        print(request)
        return SearchForPersonResponse()

    def DoFacialAttributeAnalysis(self, request, context):

        print(request)
        return FacialAttributeAnalysisResponse()
