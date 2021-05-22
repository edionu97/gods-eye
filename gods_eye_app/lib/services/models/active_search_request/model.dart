import 'package:flutter/material.dart';
import 'package:gods_eye_app/utils/helpers/conversion/image/convertor.dart';
import 'package:intl/intl.dart';

class ActiveSearchRequestModel {
  //this represents the date time as string
  String _startedAt;

  Image image;

  //the image base 64
  final String imageBase64;

  //get the request hash id
  final String searchRequestHashId;

  /// Convert the [json] into a dart object instance
  static ActiveSearchRequestModel convertFromJson(final dynamic jsonObject) {
    //set the date value
    String dateValue;
    try {
      dateValue = DateFormat("dd-MM-yyyy HH:mm:ss")
          .format(DateTime.parse(jsonObject["SubmittedOn"]));
    } on Exception {
      //empty
    }
    //create the model
    return ActiveSearchRequestModel(
        startedAt: dateValue,
        imageBase64: jsonObject["SearchedImage"],
        searchRequestHashId: jsonObject["JobHashId"]);
  }

  ActiveSearchRequestModel(
      {String startedAt, this.imageBase64, this.searchRequestHashId}) {
    //set the image to null by default
    image = null;

    //convert the image from base 64 into img
    if (imageBase64 != null && imageBase64.isNotEmpty) {
      //create the image from base64 string
      image = ImageConvertor.imageFromBase64String(imageBase64);
    }

    //set the started at
    _startedAt = startedAt;
  }

  DateTime get startedAt {
    //check the string for null or empty
    if (_startedAt == null || _startedAt.isEmpty) {
      return null;
    }

    return DateFormat("dd-MM-yyyy HH:mm:ss").parse(_startedAt, true).toLocal();
  }
}
