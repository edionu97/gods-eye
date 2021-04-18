import cv2
from face_detector.helpers.detection_summary import FaceDetectionSummary


class ImageDrawerHelper:

    @staticmethod
    def draw_faces_on_image(bgr_image: [],
                            faces_information: list[FaceDetectionSummary],
                            put_key_points: bool = True,
                            color: (int, int, int) = (0, 0, 255)):
        """
        Draw the the bounding boxes around the identified faces
        :param put_key_points: if true also the eye, mouth and nose will be identified
        :param color: the color of the rectangle and key_points
        :param faces_information: the face information
        :param bgr_image: the image itself
        :return: the modified brg image
        """

        # treat the empty image case
        if not bgr_image.any():
            return None

        # copy the image
        new_image = bgr_image.copy()

        # iterate the faces and draw the faces
        for face_info in faces_information:
            # draw values over the image
            new_image = ImageDrawerHelper.__draw_info_around_img_result(bgr_image=new_image,
                                                                        face_detection_summary=face_info,
                                                                        color=color,
                                                                        include_keypoints=put_key_points)
        # return the image
        return new_image

    @staticmethod
    def __draw_info_around_img_result(bgr_image: [],
                                      face_detection_summary: FaceDetectionSummary,
                                      color: (int, int, int),
                                      include_keypoints: bool = True) -> []:

        # identify the starting point and ending point
        from_x = face_detection_summary.box.top_x
        from_y = face_detection_summary.box.top_y
        to_x = face_detection_summary.box.bottom_x
        to_y = face_detection_summary.box.bottom_y

        # draw the rectangle
        face_with_rectangle = cv2.rectangle(bgr_image, (from_x, from_y), (to_x, to_y), color, thickness=2)

        # if is not required to draw the keypoints then return
        if not include_keypoints:
            return face_with_rectangle

        # get the face keypoints
        face_keypoints = face_detection_summary.face_points

        # draw the left eye
        face_with_keypoints = cv2.circle(img=face_with_rectangle,
                                         center=face_keypoints.left_eye_coord,
                                         color=color,
                                         thickness=2,
                                         radius=1)

        # draw the right eye
        face_with_keypoints = cv2.circle(img=face_with_keypoints,
                                         center=face_keypoints.right_eye_coord,
                                         color=color,
                                         thickness=2,
                                         radius=1)

        # draw the nose coord
        face_with_keypoints = cv2.circle(img=face_with_keypoints,
                                         center=face_keypoints.nose_coord,
                                         color=color,
                                         thickness=2,
                                         radius=1)
        # draw the mouse left coord
        face_with_keypoints = cv2.circle(img=face_with_keypoints,
                                         center=face_keypoints.mouth_left_coord,
                                         color=color,
                                         thickness=2,
                                         radius=1)

        # draw mouth right
        face_with_keypoints = cv2.circle(img=face_with_keypoints,
                                         center=face_keypoints.mouth_right_coord,
                                         color=color,
                                         thickness=2,
                                         radius=1)

        # return the face
        return face_with_keypoints
