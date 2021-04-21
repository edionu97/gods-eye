import grpc
from server.impl.base.grpc_server_base import FacialRecognitionAndAnalysisStub
from server.messages.grpc.server_messages_pb2 import FacialAttributeAnalysisRequest, SearchForPersonRequest

# create the channel
channel = grpc.insecure_channel('localhost:50051')

server_stub = FacialRecognitionAndAnalysisStub(channel)

server_stub.DoFacialRecognition(SearchForPersonRequest(person_image_b64="ana are mere",
                                                       location_image_b64="ana",
                                                       include_cropped_faces_in_response=True))

