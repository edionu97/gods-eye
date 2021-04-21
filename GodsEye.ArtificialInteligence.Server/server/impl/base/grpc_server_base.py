# Generated by the gRPC Python protocol compiler plugin. DO NOT EDIT!
"""Client and server classes corresponding to protobuf-defined services."""
import grpc

from server.messages.grpc import server_messages_pb2 as server_dot_messages_dot_grpc_dot_server__messages__pb2


class FacialRecognitionAndAnalysisStub(object):
    """*
    This facial recognition and analysis server
    Contains methods for facial recognition and analysis server
    """

    def __init__(self, channel):
        """Constructor.

        Args:
            channel: A grpc.Channel.
        """
        self.DoFacialRecognition = channel.unary_unary(
                '/gods.eye.server.artificial.intelligence.messaging.FacialRecognitionAndAnalysis/DoFacialRecognition',
                request_serializer=server_dot_messages_dot_grpc_dot_server__messages__pb2.SearchForPersonRequest.SerializeToString,
                response_deserializer=server_dot_messages_dot_grpc_dot_server__messages__pb2.SearchForPersonResponse.FromString,
                )
        self.DoFacialAttributeAnalysis = channel.unary_unary(
                '/gods.eye.server.artificial.intelligence.messaging.FacialRecognitionAndAnalysis/DoFacialAttributeAnalysis',
                request_serializer=server_dot_messages_dot_grpc_dot_server__messages__pb2.FacialAttributeAnalysisRequest.SerializeToString,
                response_deserializer=server_dot_messages_dot_grpc_dot_server__messages__pb2.FacialAttributeAnalysisResponse.FromString,
                )


class FacialRecognitionAndAnalysisServicer(object):
    """*
    This facial recognition and analysis server
    Contains methods for facial recognition and analysis server
    """

    def DoFacialRecognition(self, request, context):
        """*
        This represents the method that handles the facial recognition
        As input it takes an instance of SearchForPersonRequest
        As output returns an instance of SearchForPersonRequest
        """
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def DoFacialAttributeAnalysis(self, request, context):
        """*
        This represents the method that handles the facial attribute analysis
        As input it takes an instance of FacialAttributeAnalysisRequest
        As output returns an instance of FacialAttributeAnalysisResponse
        """
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')


def add_FacialRecognitionAndAnalysisServicer_to_server(servicer, server):
    rpc_method_handlers = {
            'DoFacialRecognition': grpc.unary_unary_rpc_method_handler(
                    servicer.DoFacialRecognition,
                    request_deserializer=server_dot_messages_dot_grpc_dot_server__messages__pb2.SearchForPersonRequest.FromString,
                    response_serializer=server_dot_messages_dot_grpc_dot_server__messages__pb2.SearchForPersonResponse.SerializeToString,
            ),
            'DoFacialAttributeAnalysis': grpc.unary_unary_rpc_method_handler(
                    servicer.DoFacialAttributeAnalysis,
                    request_deserializer=server_dot_messages_dot_grpc_dot_server__messages__pb2.FacialAttributeAnalysisRequest.FromString,
                    response_serializer=server_dot_messages_dot_grpc_dot_server__messages__pb2.FacialAttributeAnalysisResponse.SerializeToString,
            ),
    }
    generic_handler = grpc.method_handlers_generic_handler(
            'gods.eye.server.artificial.intelligence.messaging.FacialRecognitionAndAnalysis', rpc_method_handlers)
    server.add_generic_rpc_handlers((generic_handler,))


 # This class is part of an EXPERIMENTAL API.
class FacialRecognitionAndAnalysis(object):
    """*
    This facial recognition and analysis server
    Contains methods for facial recognition and analysis server
    """

    @staticmethod
    def DoFacialRecognition(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(request, target, '/gods.eye.server.artificial.intelligence.messaging.FacialRecognitionAndAnalysis/DoFacialRecognition',
            server_dot_messages_dot_grpc_dot_server__messages__pb2.SearchForPersonRequest.SerializeToString,
            server_dot_messages_dot_grpc_dot_server__messages__pb2.SearchForPersonResponse.FromString,
            options, channel_credentials,
            insecure, call_credentials, compression, wait_for_ready, timeout, metadata)

    @staticmethod
    def DoFacialAttributeAnalysis(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(request, target, '/gods.eye.server.artificial.intelligence.messaging.FacialRecognitionAndAnalysis/DoFacialAttributeAnalysis',
            server_dot_messages_dot_grpc_dot_server__messages__pb2.FacialAttributeAnalysisRequest.SerializeToString,
            server_dot_messages_dot_grpc_dot_server__messages__pb2.FacialAttributeAnalysisResponse.FromString,
            options, channel_credentials,
            insecure, call_credentials, compression, wait_for_ready, timeout, metadata)