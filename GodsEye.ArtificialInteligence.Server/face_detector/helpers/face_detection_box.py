
class FaceDetectionBox:

    @property
    def top_x(self) -> int:
        return self.__top_x

    @property
    def top_y(self) -> int:
        return self.__top_y

    @property
    def bottom_x(self) -> int:
        return self.__bottom_x

    @property
    def bottom_y(self) -> int:
        return self.bottom_y

    def __init__(self, detection_box: list):
        # check if the length of the list is 4
        if len(detection_box) != 4:
            return

        # set the top left and right
        self.__top_x, self.__top_y, width, height = detection_box

        # set the bottom and left
        self.__bottom_x = self.top_x + width
        self.__bottom_y = self.top_y + height