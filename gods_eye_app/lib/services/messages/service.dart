import 'dart:convert';

import 'package:gods_eye_app/services/models/common/model.dart';
import 'package:gods_eye_app/services/models/remote_worker/model.dart';

class MessageParsingService {

  //implement the singleton pattern
  static MessageParsingService _singletonInstance = MessageParsingService._internal();

  //declare a private constructor
  MessageParsingService._internal();

  //implement the factory method
  factory MessageParsingService() {
    return _singletonInstance;
  }

  /// This method it is used for parsing the string [message] in an instance of the object
  IAbstractModel parseModelFromJson(final String message) {
    //treat the null case
    if (message == null || message.isEmpty) {
      return null;
    }

    //decode the json into an instance of dynamic
    dynamic decodedJson = jsonDecode(message);

    //depending on the message type, create the proper instance of the object
    switch (decodedJson["MessageType"]) {
      //treat the ActiveWorkerMessageResponse message type
      case "ActiveWorkerMessageResponse":
        {
          //convert the json into the right instance of object
          return RemoteWorkerModel.convertFromJson(decodedJson);
        }
    }

    return null;
  }
}
