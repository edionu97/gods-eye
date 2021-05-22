import 'dart:convert';
import 'dart:io';
import 'dart:typed_data';

import 'package:gods_eye_app/utils/constants/api/constants.dart';
import 'package:gods_eye_app/utils/helpers/requests/helper.dart';
import 'package:mime/mime.dart';

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
  Future pingAllWorkersAsync(final String userId) async {
    //do the post request to the server for pinging all the active workers
    await RequestHelper.doPostRequestAsync(
        //provide the request url
        requestUrl:
            "${ApiConstants.SERVER_ADDRESS}${ApiConstants.PING_ACTIVE_WORKERS}",
        //send the body
        requestBody: {"userId": userId});
  }

  /// Stop an active search request for [userId] on [imageBase64] person
  Future stopActiveSearchRequestAsync(
      final String userId, final String imageBase64) async {
    //do the post request to the server for stopping the search request for every single worker
    await RequestHelper.doPostRequestAsync(
        //provide the request url
        requestUrl:
            "${ApiConstants.SERVER_ADDRESS}${ApiConstants.STOP_SEARCH_REQUEST}",
        //send the body
        requestBody: {"userId": userId, "searchedPerson": imageBase64});
  }


  /// Start a new search request for [userId]. The content of the [filePath] it is
  /// sent to server only if the file is either an jpeg file or a png file
  /// Otherwise an exception will be thrown
  Future startSearchingForANewPersonAsync(
      final String userId, final String filePath) async {
    //get the image type
    final String fileType = lookupMimeType(filePath);

    //push only jpeg and png files to the server
    if (!fileType.contains("jpeg") && !fileType.contains("png")) {
      throw Exception(
          "This file category ($fileType category) is not supported, try with a png or jpg file instead");
    }

    //read the image bytes from the image, and encode in base 64
    final Uint8List imageBytes = await File(filePath).readAsBytes();
    final String encodedBytes = base64Encode(imageBytes);

    //create the format of the payload
    final String payload = "data:$fileType;base64,$encodedBytes";

    //send the request to server
    await RequestHelper.doPostRequestAsync(
        //provide the request url
        requestUrl:
            "${ApiConstants.SERVER_ADDRESS}${ApiConstants.START_SEARCH_REQUEST}",
        //send the body
        requestBody: {"userId": userId, "searchedPerson": payload});
  }
}
