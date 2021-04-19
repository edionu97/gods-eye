class FacialAttributeAnalysisModel:

    @property
    def age(self) -> int:
        return self.__age

    @property
    def gender(self) -> str:
        return self.__gender

    @property
    def race(self) -> str:
        return self.__race

    @property
    def emotion(self) -> str:
        return self.__emotion

    def __init__(self, face_attribute_result: dict):

        self.__age = None
        self.__gender = None
        self.__race = None
        self.__emotion = None

        if 'age' in face_attribute_result:
            self.__age = face_attribute_result['age']

        if 'gender' in face_attribute_result:
            self.__gender = face_attribute_result['gender']

        if 'race' in face_attribute_result:
            self.__race = face_attribute_result['dominant_race']

        if 'emotion' in face_attribute_result:
            self.__emotion = face_attribute_result['dominant_emotion']



