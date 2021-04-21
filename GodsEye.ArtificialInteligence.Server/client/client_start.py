import grpc

from resources.manager.abs_resources_manager import AbstractResourcesManager
from resources.manager.impl.resources_manager import ResourcesManager
from resources.models.app_settings_model import AppSettings
from server.impl.base.grpc_server_base import FacialRecognitionAndAnalysisStub
from server.messages.grpc.server_messages_pb2 import FacialAttributeAnalysisRequest, SearchForPersonRequest


def create_client(manager: AbstractResourcesManager,
                  settings: AppSettings) -> FacialRecognitionAndAnalysisStub:
    """
    This function is used for creating a new client
    :param manager: the resources manager
    :param settings: the app settings
    :return: the server stub
    """

    # get the certificate
    (trusted_certs, _) = manager.get_security_info(settings=settings)

    # create the client credentials
    client_credentials = grpc.ssl_channel_credentials(trusted_certs)

    # create the channel
    channel = grpc.secure_channel(f'{settings.server_address}:{settings.server_port}', client_credentials)

    # return the stub
    return FacialRecognitionAndAnalysisStub(channel)


try:
    # create the resources manager
    resources_manager = ResourcesManager()

    # get the app settings
    app_settings = resources_manager.parse_settings()

    # create the client
    server_stub = create_client(manager=resources_manager, settings=app_settings)

    # call the method through grpc
    server_stub.DoFacialRecognition(SearchForPersonRequest(person_image_b64="ana are mere",
                                                           location_image_b64="ana",
                                                           include_cropped_faces_in_response=True))

except Exception as e:
    print(e)
