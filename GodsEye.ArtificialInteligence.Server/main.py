from helpers.image_helpers.image_helpers import ImageConversionHelpers
from face_detector.impl.mtcnndetector import MtcnnFaceDetector

import matplotlib.pyplot as plt
import json
import jsonpickle

with open(r"C:\Users\Eduard\Desktop\New Text Document (2).txt", 'r') as file:
    data = file.read()

(image, ext) = ImageConversionHelpers.convert_base64_string_to_bgr_image(data)

detector = MtcnnFaceDetector()

for a in detector.identify_faces(image, is_bgr=True):
    print(json.dumps(a.__dict__, default=lambda x: x.__dict__, indent=4))
    pass

plt.imshow(ImageConversionHelpers.convert_bgr_to_rgb(image))
plt.show()
