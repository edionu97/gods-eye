import matplotlib.pyplot as plt

from face_detector.impl.mtcnndetector import MtcnnFaceDetector
from face_recogniser.impl.analyzer import DnnFaceAnalyser
from helpers.image_helpers.image_conversion import ImageConversionHelpers
from resources.manager.impl.resources_manager import ResourcesManager

try:
    # read the face of rob
    with open(r"C:\Users\Eduard\Desktop\rob base 64.txt") as file_rob:
        face_base64 = file_rob.read()

    # read the image with rob and adam
    with open(r"C:\Users\Eduard\Desktop\rob and adam.txt") as file_rob_and_adam:
        searched_base64 = file_rob_and_adam.read()

    # create the resources manager
    resources_manager = ResourcesManager()

    # parse the settings and get the app_settings
    app_settings = resources_manager.parse_settings()

    # create the face detector
    face_detector = MtcnnFaceDetector()

    # create the face analyzer
    face_analyser = DnnFaceAnalyser(face_detector=face_detector, app_settings=app_settings)

    # iterate the result and display the faces
    for face_info, face in face_analyser.search_person_in_image(searched_person_base64=face_base64,
                                                                image_base64=searched_base64):

        plt.imshow(ImageConversionHelpers.convert_bgr_to_rgb(face))
        plt.show()


except Exception as e:
    print(e)
