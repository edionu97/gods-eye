import json
from types import SimpleNamespace


class JsonSerializerDeserializer:

    @staticmethod
    def deserialize_object(json_rep: str) -> object:
        """
        Deserialize the object from json
        :param json_rep: the representation of object
        :return: the object
        """

        # if the json rep is null or empty
        # return none
        if not json_rep:
            return None

        # deserialize the object
        return json.loads(json_rep, object_hook=lambda d: SimpleNamespace(**d))
