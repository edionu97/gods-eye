import os
import grpc
from concurrent import futures
from server.impl.base.grpc_server_base import add_FacialRecognitionAndAnalysisServicer_to_server
from server.services.face_analysis.facial_recognition_and_analysis_server import FacialRecognitionAndAnalysisService

# set the logging
os.environ["GRPC_VERBOSITY"] = "DEBUG"
# os.environ["GRPC_TRACE"] = "transport_security,tsi"


def start_server(service: FacialRecognitionAndAnalysisService):
    # creat the grp server
    grpc_server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))

    # add the service to the server
    add_FacialRecognitionAndAnalysisServicer_to_server(service, grpc_server)

    # specify the address and the port
    grpc_server.add_insecure_port('[::]:50051')

    # start the server at a specific address and port
    grpc_server.start()

    # block until the server is finished
    grpc_server.wait_for_termination()


# start the server
start_server(service=FacialRecognitionAndAnalysisService())