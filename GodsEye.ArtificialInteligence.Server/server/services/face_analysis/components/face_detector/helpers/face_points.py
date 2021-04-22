from server.messages.grpc.server_messages_pb2 import FaceKeypointsLocation, FacePoint


class FacePoints:

    @property
    def right_eye_coord(self) -> (int, int):
        return self.__right_eye

    @property
    def left_eye_coord(self) -> (int, int):
        return self.__left_eye

    @property
    def nose_coord(self) -> (int, int):
        return self.__nose

    @property
    def mouth_left_coord(self) -> (int, int):
        return self.__mouth_left

    @property
    def mouth_right_coord(self) -> (int, int):
        return self.__mouth_right

    def __init__(self, key_points: dict):
        # set the face points
        self.__right_eye = key_points['right_eye']
        self.__left_eye = key_points['left_eye']
        self.__nose = key_points['nose']
        self.__mouth_left = key_points['mouth_left']
        self.__mouth_right = key_points['mouth_right']
