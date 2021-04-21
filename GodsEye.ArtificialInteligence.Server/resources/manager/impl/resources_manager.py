import os
from resources.manager.abs_resources_manager import AbstractResourcesManager
from helpers.serializers.json.jsonserializerdeserializer import JsonSerializerDeserializer
from resources.models.app_settings_model import AppSettings, parse_app_settings_from_json_object


class ResourcesManager(AbstractResourcesManager):

    def get_security_info(self, settings: AppSettings) -> (str, str):

        # read the content of the private key
        with open(os.path.join(self._security_search_location, settings.certificate_key), 'rb') as file_private_key:
            private_key = file_private_key.read()

        # read the certificate chain
        with open(os.path.join(self._security_search_location, settings.certificate_name), 'rb') as file_certificate:
            certificate_chain = file_certificate.read()

        # return the pairs
        return certificate_chain, private_key

    def parse_settings(self) -> AppSettings:
        # read the file and store the content in it
        with open(self._settings_json_path, 'r') as settings_file:
            app_settings_as_json = settings_file.read()

        # convert from string to json object
        json_object = JsonSerializerDeserializer.deserialize_object(json_rep=app_settings_as_json)

        # parse the json object in app settings object
        return parse_app_settings_from_json_object(json_object=json_object)
