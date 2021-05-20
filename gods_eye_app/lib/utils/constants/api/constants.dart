class ApiConstants {
  // ignore: non_constant_identifier_names
  static String _SERVER_IP_ADDRESS = "192.168.1.197";

  // ignore: non_constant_identifier_names
  static String SERVER_ADDRESS = "http://$_SERVER_IP_ADDRESS:5000/";

  // ignore: non_constant_identifier_names
  static String WEB_SOCKET = "ws://$_SERVER_IP_ADDRESS:9000";

  // ignore: non_constant_identifier_names
  static String LOGIN_API = "api/users/login";

  // ignore: non_constant_identifier_names
  static String REGISTER_API = "api/users/register";

  // ignore: non_constant_identifier_names
  static String NOTIFICATION_INITIALIZATION_API =
      "api/facial-recognition/initialize";

  // ignore: non_constant_identifier_names
  static String PING_ACTIVE_WORKERS =
      "api/facial-recognition/searching/active-workers/all";
}
