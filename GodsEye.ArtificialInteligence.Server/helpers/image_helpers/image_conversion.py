import io
import re
import cv2
import base64
import numpy as np
from PIL import Image


class ImageConversionHelpers:
    @staticmethod
    def convert_bgr_image_to_base64_string(image_array: [], image_extension: str) -> str:
        """
        Converts the image from bgr into an string
        :param image_array:
        :param image_extension:
        :return:
        """
        # check if the image array is not null or empty
        if not image_array.any():
            return ""

        # check if the image extension is specified
        if not image_extension:
            raise Exception("Extension not found")

        # encode the image
        encoded_image = cv2.imencode(f'.{image_extension}', image_array)[1]

        # return the string that represents the image
        return f'data:image/{image_extension};base64,{str(base64.b64encode(encoded_image), "utf-8")}'

    @staticmethod
    def convert_base64_string_to_bgr_image(base64_string: str) -> ([], str):
        """
        Converts an image from base64 string into an image using opencv
        :param base64_string: string the base64 representation of the image
        :return: returns the bgr image
        """

        # treat the case in which the string is null or empty
        if not base64_string:
            return None

        # split the string into two parts
        split_info = base64_string.split(',')

        # get the extension
        match = re.search('^data:image/(?P<extension>[a-zA-Z]+);base64$', split_info[0])

        # get the image type
        base64_string = split_info[-1]

        # decode the image
        decoded_base64_img = base64.b64decode(base64_string)

        # open the image (RGB)
        rgb_image = Image.open(io.BytesIO(decoded_base64_img))

        # return the brg image
        return cv2.cvtColor(np.array(rgb_image), cv2.COLOR_RGB2BGR), match and match.group('extension')

    @staticmethod
    def convert_bgr_to_rgb(image_array: []):
        """
        Convert the bgr image to rgb
        :param image_array: the image array
        :return: none if the array is empty or null or the reversed array otherwise
        """

        # check if the image is not empty
        if not image_array.any():
            return None

        # reverse the image
        return image_array[:, :, ::-1]
