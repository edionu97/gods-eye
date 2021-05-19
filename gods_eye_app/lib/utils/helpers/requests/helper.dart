import 'dart:convert';
import 'package:gods_eye_app/utils/constants/message/constants.dart';
import 'package:http/http.dart' as http;

class RequestHelper {
  ///This function can be used for sending a post request to [requestUrl] address
  ///The [requestBody] contains the data that will be send to the [requestUrl]
  static Future<dynamic> doPostRequestAsync(
      {String requestUrl, Object requestBody}) {
    //define the uri from the request url
    final Uri url = Uri.parse(requestUrl);
    //return the future response result
    return http
        //initialize the post request
        .post(url,
            headers: {"Content-Type": "application/json"},
            body: jsonEncode(requestBody))
        //set the timeout to 2 minutes
        .timeout(const Duration(seconds: 2))
        //if any error occurs then send a proper message
        .catchError((error) =>
            throw Exception(MessageConstants.couldNotGetAnyResponseMessage))
        //if no error is encountered just process the request
        .then((http.Response response) {
      //decode the response
      var decodedResponse = json.decode(response.body);
      //if the status is not 200
      if (response.statusCode != 200) {
        throw Exception(decodedResponse["detail"]);
      }
      //return the response
      return decodedResponse;
    });
  }

  static Future<dynamic> doGetRequestAsync({String requestUrl}) {
    //define the uri from the request url
    final Uri url = Uri.parse(requestUrl);
    //return the future response result
    return http
        //initialize the post request
        .get(url, headers: {"Content-Type": "application/json"})
        //set the timeout to 2 minutes
        .timeout(const Duration(seconds: 2))
        //if any error occurs then send a proper message
        .catchError((error) =>
            throw Exception(MessageConstants.couldNotGetAnyResponseMessage))
        //if no error is encountered just process the request
        .then((http.Response response) {
          //try to decode the response
          try {
            //decode the response (try to decode)
            var decodedResponse = json.decode(response.body);
            //if the status is not 200
            if (response.statusCode != 200) {
              throw Exception(decodedResponse["detail"]);
            }
            //return the response
            return decodedResponse;

            //handle the case in which the response could not be parsed
          } on Exception {
            return {
              "responseCode": response.statusCode,
              "content": response.body
            };
          }
        });
  }
}
