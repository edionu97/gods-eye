from helpers.image_helpers.image_conversion import ImageConversionHelpers
from face_detector.impl.mtcnndetector import MtcnnFaceDetector

import matplotlib.pyplot as plt
import json
import jsonpickle
import cv2

from helpers.image_helpers.image_drawer import ImageDrawerHelper

with open(r"C:\Users\Eduard\Desktop\New Text Document (2).txt", 'r') as file:
    data = file.read()

(image, ext) = ImageConversionHelpers.convert_base64_string_to_bgr_image(data)

detector = MtcnnFaceDetector()

# for a in detector.identify_faces(image, is_bgr=True):
#     print(json.dumps(a.__dict__, default=lambda x: x.__dict__, indent=4))
#     pass

image = ImageDrawerHelper.draw_faces_on_image(image, detector.identify_faces(image), put_key_points=True)

rgb_image = ImageConversionHelpers.convert_bgr_to_rgb(image)

with open(r"C:\Users\Eduard\Desktop\New Text Document (2) result.txt", 'w') as file:
    file.write(ImageConversionHelpers.convert_bgr_image_to_base64_string(image, ext))
plt.imshow(rgb_image)
plt.show()
