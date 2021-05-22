import 'dart:convert';

import 'package:flutter/material.dart';

class ImageConvertor {
  /// Convert the base64 string into an image
  static Image imageFromBase64String(String base64String) {
    //treat the null case
    if (base64String == null || base64String.isEmpty) {
      return null;
    }

    //try to convert the image from base64 into dart img
    try {
      //get the base64 string
      var base64WithoutFormat = base64String.split(',').last;

      //return the base64
      return Image.memory(base64Decode(base64WithoutFormat),
          fit: BoxFit.fill, filterQuality: FilterQuality.high);
    } on Exception {
      //on exception return null
      return null;
    }
  }
}
