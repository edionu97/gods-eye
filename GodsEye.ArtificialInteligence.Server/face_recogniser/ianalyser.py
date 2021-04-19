from abc import ABC, abstractmethod


class IAnalyser(ABC):

    @abstractmethod
    def search_person_in_image(self, searched_person_base64: str, image_base64: str):
        """
        Search the person in one image
        :param searched_person_base64: the image that contains the searched person face
        :param image_base64: the image in which we are searching the person
        :return: the face recognition summary
        """
        raise Exception(f"Method {self.search_person_in_image.__name__} is not implemented")

    @abstractmethod
    def analyze_person(self, person_image_base64: str):
        """
        Analyse the person
        :param person_image_base64: the image containing the person
        :return: the result about person analyzing
        """
        raise Exception(f"Method {self.analyze_person.__name__} is not implemented")
