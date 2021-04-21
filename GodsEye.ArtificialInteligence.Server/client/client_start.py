import os
import grpc
import matplotlib.pyplot as plt

from helpers.image_helpers.image_conversion import ImageConversionHelpers
from resources.manager.abs_resources_manager import AbstractResourcesManager
from resources.manager.impl.resources_manager import ResourcesManager
from resources.models.app_settings_model import AppSettings
from server.impl.base.grpc_server_base import FacialRecognitionAndAnalysisStub
from server.messages.grpc.server_messages_pb2 import FacialAttributeAnalysisRequest, SearchForPersonRequest

# set the logging
os.environ["GRPC_VERBOSITY"] = "INFO"


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

    # read the face of rob
    with open(r"C:\Users\Eduard\Desktop\rob.txt") as file_rob:
        face_base64 = file_rob.read()

    # read the image with rob and adam
    with open(r"C:\Users\Eduard\Desktop\adam sandler and rob .txt") as file_rob_and_adam:
        searched_base64 = file_rob_and_adam.read()

    # call the method through grpc
    response = server_stub \
        .DoFacialRecognition(SearchForPersonRequest(person_image_b64=face_base64,
                                                    location_image_b64=searched_base64,
                                                    include_cropped_faces_in_response=True))

    for face_recognition_info in response.face_recognition_info:

        # get the image
        image, _ = ImageConversionHelpers \
            .convert_base64_string_to_bgr_image(face_recognition_info.cropped_face_image_b64)

        print(face_recognition_info.face_bounding_box)
        print(face_recognition_info.face_points)

        if not image.any():
            continue

        plt.imshow(ImageConversionHelpers.convert_bgr_to_rgb(image))
        plt.show()

except Exception as e:
    print(e)
