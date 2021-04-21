class AppSettings:

    @property
    def recognition_model(self) -> str:
        return self.__recognition_model

    @recognition_model.setter
    def recognition_model(self, value: str):
        self.__recognition_model = value

    @property
    def facial_attribute_analysis_models(self) -> list[str]:
        return self.__facial_attribute_analysis_models

    @facial_attribute_analysis_models.setter
    def facial_attribute_analysis_models(self, value: list[str]):
        self.__facial_attribute_analysis_models = value

    @property
    def certificate_name(self) -> str:
        return self.__certificate_name

    @certificate_name.setter
    def certificate_name(self, value: str):
        self.__certificate_name = value

    @property
    def certificate_key(self) -> str:
        return self.__certificate_key

    @certificate_key.setter
    def certificate_key(self, value: str):
        self.__certificate_key = value

    def __init__(self):
        """
        Construct the object, set the properties to default values
        """

        self.__recognition_model = ""
        self.__certificate_name = ""
        self.__certificate_key = ""
        self.__facial_attribute_analysis_models = []


def parse_app_settings_from_json_object(json_object) -> AppSettings:
    """
    Parse the json_object and converts it into an instance of app settings model
    :param json_object: the python object from the json
    :return: a new instance of the model with all the fields populated
    """

    # create the model
    model = AppSettings()

    # set the recognition model
    model.recognition_model = json_object.recognition_dnn_model

    # set the facial attribute analysis model
    model.facial_attribute_analysis_models = json_object.facial_attribute_analysis

    # set the certificate name
    model.certificate_name = json_object.grpc_server.certificate_name

    # set the certificate key
    model.certificate_key = json_object.grpc_server.certificate_key

    # return the model
    return model
