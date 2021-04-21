from pathlib import Path
from abc import ABC, abstractmethod
from resources.models.app_settings_model import AppSettings


class AbstractResourcesManager(ABC):

    def __init__(self):
        """
        Creates the object instance
        :raises Exception: if the file does not exist
        """

        # initialize the file path
        self._settings_json_path = Path(__file__) \
            .parent \
            .parent \
            .joinpath('files', 'app_settings.json')

        # initialize the search location for security
        self._security_search_location = Path(__file__) \
            .parent \
            .parent \
            .joinpath('files', 'security')

        # throw exception if the file does not exist
        if not Path(self._settings_json_path).is_file():
            raise Exception(f"The app settings file does not exist in the specified path")

        # throw exception if the directory does not exist
        if not Path(self._security_search_location).is_dir():
            raise Exception(f"The security folder could not be found in the specified path")

    @abstractmethod
    def parse_settings(self) -> AppSettings:
        """
        This method it is used for parsing the app settings model
        :return: an instance of app settings
        """
        raise Exception(f"Method {self.parse_settings.__name__} is not implemented")

    @abstractmethod
    def get_security_info(self, settings: AppSettings) -> (str, str):
        """
        This method it is used for getting the content of the certificate and key
        :return: a tuple of two string elements (certificate content and the certificate key)
        """
        raise Exception(f"Method {self.get_security_info.__name__} is not implemented")
