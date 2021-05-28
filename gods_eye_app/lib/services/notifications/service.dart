import 'dart:convert';

import 'package:gods_eye_app/utils/constants/api/constants.dart';
import 'package:gods_eye_app/utils/helpers/requests/helper.dart';
import 'package:web_socket_channel/io.dart';

class NotificationService {
  //implement the singleton pattern
  static NotificationService _singletonInstance =
      NotificationService._internal();

  //create the observers list
  final List<Function> _observers = [];

  //the on done callback
  Function _onDoneCallback;

  //get the ws channel
  IOWebSocketChannel _webSocketChannel;

  //declare a private constructor
  NotificationService._internal();

  //implement the factory method
  factory NotificationService() {
    return _singletonInstance;
  }

  /// This method it is used for registering the service to the ws server
  Future registerAsync(final String userId) async {
    //clear all the observers
    _observers.clear();

    //do the get request for notification initialization
    await RequestHelper.doGetRequestAsync(
        requestUrl:
            "${ApiConstants.SERVER_ADDRESS}${ApiConstants.NOTIFICATION_INITIALIZATION_API}");

    //connect to the web socket channel
    _webSocketChannel = IOWebSocketChannel.connect(ApiConstants.WEB_SOCKET);

    //send the registration message
    _webSocketChannel.sink.add(jsonEncode({'clientId': userId}));

    //start listening for incoming connections
    _webSocketChannel.stream.listen(
        //notify all the observers
        (event) => _observers.forEach((obs) => obs?.call(event)),
        //handle the on error
        onError: (error) => print("Error $error"),
        //handle the on done
        onDone: () async {
          //print the message
          print("Client $userId disconnected from the server");
          //call the on done callback
          _onDoneCallback?.call();
        });
  }

  /// Unregister the service
  Future unregisterAsync() async {
    await _webSocketChannel?.sink?.close();
  }

  /// Set the function that is executed on the connection is done
  void setOnDoneCallback(Function function){
    _onDoneCallback = function;
  }

  /// Register the observer
  void registerObserver(final Function observer) {
    //the should be added only once
    if(_observers.contains(_observers)){
      return;
    }
    //register the observer
    _observers.add(observer);
  }

  /// Unregister the observer
  void unregisterObserver(final Function observer) {
    //unregister the observer
    _observers.remove(observer);
  }
}
