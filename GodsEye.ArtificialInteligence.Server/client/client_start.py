import os
import grpc
import matplotlib.pyplot as plt

from threading import *
from resources.models.app_settings_model import AppSettings
from resources.manager.impl.resources_manager import ResourcesManager
from helpers.image_helpers.image_conversion import ImageConversionHelpers
from resources.manager.abs_resources_manager import AbstractResourcesManager
from server.impl.base.grpc_server_base import FacialRecognitionAndAnalysisStub
from server.messages.grpc.server_messages_pb2 import FacialAttributeAnalysisRequest, SearchForPersonRequest

# set the logging
os.environ["GRPC_VERBOSITY"] = "INFO"

# define de lock
lock = Lock()


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


def start_recognition_job(searched_person_path: str, in_file_path: str):
    """
    Start a searching client
    :param searched_person_path: the searched person
    :param in_file_path: the picture in which the searched person may appear
    :return:
    """
    try:
        # create the resources manager
        resources_manager = ResourcesManager()

        # get the app settings
        app_settings = resources_manager.parse_settings()

        # create the client
        server_stub = create_client(manager=resources_manager, settings=app_settings)

        # read the face of rob
        with open(searched_person_path) as file_rob:
            face_base64 = file_rob.read()

        # read the image with rob and adam
        with open(in_file_path) as file_rob_and_adam:
            searched_base64 = file_rob_and_adam.read()

        # call the method through grpc
        response = server_stub \
            .DoFacialRecognition(SearchForPersonRequest(person_image_b64=face_base64,
                                                        location_image_b64=searched_base64,
                                                        include_cropped_faces_in_response=False))

        # iterate in the face recognition results
        for face_recognition_info in response.face_recognition_info:

            image = None

            if face_recognition_info.cropped_face_image_b64:
                # get the image
                image, _ = ImageConversionHelpers \
                    .convert_base64_string_to_bgr_image(face_recognition_info.cropped_face_image_b64)

            analysis_response = server_stub.DoFacialAttributeAnalysis(
                FacialAttributeAnalysisRequest(is_face_location_known=True,
                                               analyzed_image_containing_the_face_b64=searched_base64,
                                               face_bounding_box=face_recognition_info.face_bounding_box))

            # critical section
            lock.acquire()
            print(analysis_response)
            if image is not None and image.any():
                plt.imshow(ImageConversionHelpers.convert_bgr_to_rgb(image))
                plt.show()
            lock.release()

    except Exception as e:
        print(e)


def start_clients():
    # declare the files that contain  b64 images of the searched persons
    searched_persons_paths = [r"C:\Users\Eduard\Desktop\rob.txt",
                              r"C:\Users\Eduard\Desktop\eduard.txt",
                              r"C:\Users\Eduard\Desktop\adam.txt"]

    # declare the files that contain  b64 images of pictures in which the person are searched
    searched_in_paths = [r"C:\Users\Eduard\Desktop\adam sandler and rob .txt",
                         r"C:\Users\Eduard\Desktop\eduard si sorina.txt",
                         r"C:\Users\Eduard\Desktop\adam sandler and rob .txt"]

    # declare the clients
    client_jobs = []

    # iterate the persons
    for index in range(0, len(searched_persons_paths)):
        # create the client job
        client_job = Thread(target=start_recognition_job,
                            args=[searched_persons_paths[index % len(searched_persons_paths)],
                                  searched_in_paths[index % len(searched_persons_paths)]])

        # start the client job
        client_job.start()

        # add it into list
        client_jobs.append(client_job)

    # join the clients
    for client_job in client_jobs:
        client_job.join()


start_clients()
