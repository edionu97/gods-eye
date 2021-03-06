import os
import grpc
import multiprocessing

from concurrent import futures
from resources.manager.abs_resources_manager import AbstractResourcesManager
from resources.manager.impl.resources_manager import ResourcesManager
from resources.models.app_settings_model import AppSettings
from server.impl.base.grpc_server_base import add_FacialRecognitionAndAnalysisServicer_to_server
from server.services.face_analysis.components.face_detector.impl.mtcnndetector import MtcnnFaceDetector
from server.services.face_analysis.components.face_recogniser.impl.analyzer import DnnFaceAnalyser
from server.services.face_analysis.impl \
    .facial_recognition_and_analysis_service import FacialRecognitionAndAnalysisService

# set the logging level
os.environ["GRPC_VERBOSITY"] = "INFO"


def start_server(service: FacialRecognitionAndAnalysisService,
                 settings: AppSettings,
                 manager: AbstractResourcesManager):
    """
    Starts the server on a specific address and port
    :param service: the service used
    :param settings: the app settings
    :param manager: the resources manager
    """
    # create the grp server
    grpc_server = grpc.server(futures.ThreadPoolExecutor(max_workers=multiprocessing.cpu_count()))

    # add the service to the server
    add_FacialRecognitionAndAnalysisServicer_to_server(service, grpc_server)

    # get the certificate chain and the private key
    # add the server credentials
    (certificate_chain, private_key) = manager.get_security_info(settings=app_settings)
    server_credentials = grpc.ssl_server_credentials([(private_key, certificate_chain)],
                                                     root_certificates=None,
                                                     require_client_auth=False)

    # start the server on the desired address and port
    # also add the credentials
    grpc_server.add_secure_port(server_credentials=server_credentials,
                                address=f'{app_settings.server_address}:{app_settings.server_port}')

    # start the server at a specific address and port
    grpc_server.start()

    # block until the server is finished
    grpc_server.wait_for_termination()


try:

    # create the resources manager
    resources_manager = ResourcesManager()

    # get the app settings
    app_settings = resources_manager.parse_settings()

    # create the face detector
    face_detector = MtcnnFaceDetector()

    # create the face analyzer
    face_analyser = DnnFaceAnalyser(face_detector=face_detector, app_settings=app_settings)

    # start the server
    start_server(service=FacialRecognitionAndAnalysisService(face_detector=face_detector,
                                                             face_analyser=face_analyser),
                 settings=app_settings,
                 manager=resources_manager)

except Exception as e:
    print(e)
