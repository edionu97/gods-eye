import grpc

from resources.manager.impl.resources_manager import ResourcesManager
from server.impl.base.grpc_server_base import FacialRecognitionAndAnalysisStub
from server.messages.grpc.server_messages_pb2 import FacialAttributeAnalysisRequest, SearchForPersonRequest

# # create the channel
# channel = grpc.insecure_channel('localhost:50051')
#
# server_stub = FacialRecognitionAndAnalysisStub(channel)
#
# server_stub.DoFacialRecognition(SearchForPersonRequest(person_image_b64="ana are mere",
#                                                        location_image_b64="ana",
#                                                        include_cropped_faces_in_response=True))

try:
    # create the resources manager
    resources_manager = ResourcesManager()

    # get the app settings
    app_settings = resources_manager.parse_settings()

    # get the certificate chain and the private key
    (certificate_chain, private_key) = resources_manager.get_security_info(settings=app_settings)

except Exception as e:
    print(e)
