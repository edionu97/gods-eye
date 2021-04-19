from abc import ABC, abstractmethod
from pathlib import Path

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

        # throw exception if the file does not exist
        if not Path(self._settings_json_path).is_file():
            raise Exception(f"The app settings file does not exist in the specified path")

    @abstractmethod
    def parse_settings(self) -> AppSettings:
        """
        This method it is used for parsing the app settings model
        :return: an instance of app settings
        """
        raise Exception(f"Method {self.parse_settings.__name__} is not implemented")
