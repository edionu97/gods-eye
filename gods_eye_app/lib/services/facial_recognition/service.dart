import 'package:gods_eye_app/utils/constants/api/constants.dart';
import 'package:gods_eye_app/utils/helpers/requests/helper.dart';

class FacialRecognitionService {
  //implement the singleton pattern
  static FacialRecognitionService _singletonInstance =
      FacialRecognitionService._internal();

  //declare a private constructor
  FacialRecognitionService._internal();

  //implement the factory method
  factory FacialRecognitionService() {
    return _singletonInstance;
  }

  /// Ping all the workers for [userId]
  Future pingAllWorkersAsync(String userId) async {
    //do the post request to the server for pinging all the active workers
    await RequestHelper.doPostRequestAsync(
        //provide the request url
        requestUrl:
            "${ApiConstants.SERVER_ADDRESS}${ApiConstants.PING_ACTIVE_WORKERS}",
        //send the body
        requestBody: {"userId": userId});
  }
}
