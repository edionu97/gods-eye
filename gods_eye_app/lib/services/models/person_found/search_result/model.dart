import 'package:flutter/material.dart';
import 'package:gods_eye_app/utils/helpers/conversion/image/convertor.dart';

import 'attribute_analysis/model.dart';
import 'facial_detection_details_bnd_box/model.dart';
import 'facial_detection_details_face_keypoints/model.dart';

class SearchResultModel {
  Image foundImage;

  final FaceBoundingBoxModel boundingBox;

  final FacialKeyPointsModel faceKeyPoints;

  final AttributeAnalysisModel attributeAnalysis;

  final String foundImageString;

  ///convert the message into dart object
  static SearchResultModel convertFromJson(final dynamic json) {
    return SearchResultModel(
        boundingBox: FaceBoundingBoxModel.convertFromJson(
            json['SearchDetails']['BoundingBox']),
        faceKeyPoints: FacialKeyPointsModel.convertFromJson(
            json['SearchDetails']['FaceKeypoints']),
        foundImageString: json['Frame'],
        attributeAnalysis: AttributeAnalysisModel.convertFromJson(
            json['FacialAnalysisResult']));
  }

  SearchResultModel(
      {this.boundingBox,
      this.faceKeyPoints,
      this.attributeAnalysis,
      this.foundImageString}) {
    //convert the image from base 64 into img
    foundImage = null;
    if (foundImageString != null && foundImageString.isNotEmpty) {
      //get the image and the format
      var imageAndFormat =
          ImageConvertor.imageFromBase64String(foundImageString);

      //create the image from base64 string
      foundImage = imageAndFormat.first;
    }
  }
}
