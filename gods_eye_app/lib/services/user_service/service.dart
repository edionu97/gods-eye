import 'package:gods_eye_app/utils/helpers/requests/helper.dart';
import 'package:gods_eye_app/utils/constants/api/constants.dart';

class UserService {
  //implement the singleton pattern
  static UserService _singletonInstance = UserService._internal();

  //declare a private constructor
  UserService._internal();

  //implement the factory method
  factory UserService() {
    return _singletonInstance;
  }

  /// Takes the [username] and the [password] and tries to see if the
  /// user can be authenticated
  Future<String> loginAsync(
      final String username, final String password) async {
    //get the login response
    var loginJsonResponse = await RequestHelper.doPostRequestAsync(
        requestUrl: ApiConstants.SERVER_ADDRESS + ApiConstants.LOGIN_API,
        requestBody: {"password": password, "username": username});
    //return the token
    return loginJsonResponse["userToken"];
  }

  /// Takes the [username] and the [password] and tries to create a new account
  Future<String> registerAsync(
      final String username, final String password) async {
    //get the login response
    var registerResponse = await RequestHelper.doPostRequestAsync(
        requestUrl: ApiConstants.SERVER_ADDRESS + ApiConstants.REGISTER_API,
        requestBody: {"password": password, "username": username});
    //return the token
    return registerResponse["userToken"];
  }
}
