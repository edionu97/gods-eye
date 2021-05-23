import 'package:intl/intl.dart';

class PrimitivesConvertor {

  /// Converts the date from utc to local time, in the proper format
  static DateTime convertUtcStringDateToLocal(String stringToBeConverted) {
    //check the string for null or empty
    if (stringToBeConverted == null || stringToBeConverted.isEmpty) {
      return null;
    }

    //return the format in the right format
    return DateFormat("dd-MM-yyyy HH:mm:ss")
        .parse(stringToBeConverted, true)
        .toLocal();
  }

  /// Converts the received from it's format into the expected date format
  static String convertResponseDateToExpectedFormat(String utcDate) {
    //set the date value
    String dateValue;
    try {
      dateValue =
          DateFormat("dd-MM-yyyy HH:mm:ss").format(DateTime.parse(utcDate));
    } on Exception {
      //empty
    }

    return dateValue;
  }
}
