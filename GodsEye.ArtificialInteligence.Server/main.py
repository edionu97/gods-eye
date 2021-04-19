import matplotlib.pyplot as plt

from face_detector.impl.mtcnndetector import MtcnnFaceDetector
from face_recogniser.impl.analyzer import DnnFaceAnalyser
from helpers.image_helpers.image_conversion import ImageConversionHelpers
from helpers.image_helpers.image_drawer import ImageDrawerHelper
from resources.manager.impl.resources_manager import ResourcesManager

try:
    # read the face of rob
    with open(r"C:\Users\Eduard\Desktop\adam base64.txt") as file_rob:
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

    # image
    img, _ = ImageConversionHelpers.convert_base64_string_to_bgr_image(searched_base64)

    infos = []
    # iterate the result and display the faces
    for face_info, face in face_analyser.search_person_in_image(searched_person_base64=face_base64,
                                                                image_base64=searched_base64):
        infos.append(face_info)

    # draw the faces over the image
    img = ImageDrawerHelper.draw_faces_on_image(img, infos)
    plt.imshow(ImageConversionHelpers.convert_bgr_to_rgb(img))
    plt.show()

    # print the emotion of the person from each face
    for result_info, rgb_face in face_analyser.analyze_faces_from_image(searched_base64, infos):
        plt.imshow(rgb_face)
        plt.show()
        print(result_info.race, result_info.age, result_info.gender, result_info.emotion)


except Exception as e:
    print(e)
