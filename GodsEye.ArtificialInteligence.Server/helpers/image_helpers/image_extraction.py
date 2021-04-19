from face_detector.helpers.face_detection_box import FaceDetectionBox


class ImageExtractionHelpers:

    @staticmethod
    def crop_face_from_image(image: [], detection_box: FaceDetectionBox) -> []:

        """
        Crops the image, extracting the roi
        :param image: the image that will be cropped
        :param detection_box: the cropping information
        :return: the roi or none if the image could not be cropped
        """

        # check if image array has any values
        if not image.any():
            return None

        # get the height and  width
        height, width, _ = image.shape

        # the top x, y values should be always positive
        top_x = max(detection_box.top_x, 0)
        top_y = max(detection_box.top_y, 0)

        # bottom x, y should not exceed the image boundaries
        bottom_x = min(detection_box.bottom_x, width)
        bottom_y = min(detection_box.bottom_y, height)

        # crop the image
        cropped_image = image[top_y:bottom_y, top_x: bottom_x]

        # if there are no elements in image return none
        if not cropped_image.any():
            return None

        # return the cropped image
        return cropped_image
