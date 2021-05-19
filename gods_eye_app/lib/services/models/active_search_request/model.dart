import 'package:flutter/material.dart';
import 'package:gods_eye_app/utils/helpers/conversion/image/convertor.dart';
import 'package:intl/intl.dart';

class ActiveSearchRequestModel {
  //this represents the date time as string
  String _startedAt;

  //the image base 64
  String _imageBase64;

  ActiveSearchRequestModel({String startedAt, String imageBase64}) {
    _startedAt = startedAt;
    _imageBase64 = imageBase64;
  }

  Image get image {
    //check the string for null or empty
    if (_imageBase64 == null || _imageBase64.isEmpty) {
      return null;
    }
    //create the image from base64 string
    return ImageConvertor.imageFromBase64String(_imageBase64);
  }

  DateTime get startedAt {

    //check the string for null or empty
    if (_startedAt == null || _startedAt.isEmpty) {
      return null;
    }

    return DateFormat("dd-MM-yyyy HH:mm:ss").parse(_startedAt, true).toLocal();
  }
}
