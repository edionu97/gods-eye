import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:gods_eye_app/utils/helpers/objects/pair/object.dart';

class ImageConvertor {
  /// Convert the base64 string into an image
  static Pair<Image, String> imageFromBase64String(String base64String) {
    //treat the null case
    if (base64String == null || base64String.isEmpty) {
      return null;
    }

    //try to convert the image from base64 into dart img
    try {
      //get the base64 string
      var base64WithoutFormat = base64String.split(',').last;
      //get the base64 image format
      var base64ImageFormat = base64String.split(',').first;
      //return the base64Image and the image format
      return Pair(
          //load the image in memory
          Image.memory(
            base64Decode(base64WithoutFormat),
            fit: BoxFit.cover,
            filterQuality: FilterQuality.high,
          ),
          //set the image format
          base64ImageFormat);
    } on Exception {
      //on exception return null
      return null;
    }
  }
}
