class Singleton(type):

    # declare the instance dictionary
    _instances = {}

    def __call__(cls, *args, **kwargs):
        """
        This will be called when the class is instantiated
        :param args:
        :param kwargs:
        :return: a new instance of the object or the same object
        """

        # if it's the first time when the object is initialised
        # create a new instance
        if cls not in cls._instances:
            cls._instances[cls] = super(Singleton, cls).__call__(*args, **kwargs)

        # return the instance
        return cls._instances[cls]
