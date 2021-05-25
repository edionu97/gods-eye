import 'dart:convert';
import 'dart:developer';

import 'package:flutter/material.dart';
import 'package:gods_eye_app/services/models/person_found/search_result/facial_detection_details_bnd_box/model.dart';
import 'package:gods_eye_app/services/models/person_found/search_result/facial_detection_details_face_keypoints/model.dart';
import 'package:gods_eye_app/utils/constants/api/constants.dart';
import 'package:gods_eye_app/utils/helpers/conversion/image/convertor.dart';
import 'package:gods_eye_app/utils/helpers/requests/helper.dart';

class DrawingService {
  //implement the singleton pattern
  static DrawingService _singletonInstance = DrawingService._internal();

  //declare a private constructor
  DrawingService._internal();

  //implement the factory method
  factory DrawingService() {
    return _singletonInstance;
  }

  /// Resize the [base64Image] to [width]x[height]
  Future<Image> resizeImageAsync({
    @required String userToken,
    @required String base64Image,
    @required int width,
    @required int height,
  }) async {
    //await the response
    //send the resize request to server
    var response = await RequestHelper.doPostRequestAsync(
        requestUrl:
            "${ApiConstants.SERVER_ADDRESS}${ApiConstants.IMAGE_RESIZE}",
        requestBody: {
          "imageBase64": base64Image,
          "toWidth": width,
          "toHeight": height
        });

    //return the image instance
    return ImageConvertor.imageFromBase64String(response['image']);
  }

  /// This method it is used for drawing the bounding boxes around the face
  Future<Image> drawFaceBoundingBoxAsync(
      {@required String userToken,
      @required String base64Image,
      @required FaceBoundingBoxModel bndBox,
      int width,
      int height,
      bool resize,
      FacialKeyPointsModel facialKeyPointsModel}) async {
    //define the payload
    final dynamic payload = {
      "imageBase64": base64Image,
      "boundingBox": {
        "topX": bndBox.topX,
        "topY": bndBox.topY,
        "bottomX": bndBox.bottomX,
        "bottomY": bndBox.bottomY
      }
    };

    //include in payload the facial results model
    if(facialKeyPointsModel != null) {
      payload["faceKeypointsLocation"] = {
        "rightEyePoint": {
          "x": facialKeyPointsModel.rightEyePoint?.x,
          "y":  facialKeyPointsModel.rightEyePoint?.y
        },
        "leftEyePoint": {
          "x": facialKeyPointsModel.leftEyePoint?.x,
          "y": facialKeyPointsModel.leftEyePoint?.y
        },
        "nosePoint": {
          "x": facialKeyPointsModel.nosePoint?.x,
          "y": facialKeyPointsModel.nosePoint?.y
        },
        "mouthLeftPoint": {
          "x": facialKeyPointsModel.mouthLeftPoint?.x,
          "y": facialKeyPointsModel.mouthLeftPoint?.y
        },
        "mouthRightPoint": {
          "x": facialKeyPointsModel.mouthRightPoint?.x,
          "y": facialKeyPointsModel.mouthRightPoint?.y
        }
      };
    }

    //await the response
    //send the resize request to server
    var response = await RequestHelper.doPostRequestAsync(
        requestUrl:
            "${ApiConstants.SERVER_ADDRESS}${ApiConstants.HIGHLIGHT_ROI}",
        requestBody: payload);

    // resize the image if we need to do this
    if (resize == true) {
      //resize the image to the desired width
      return await resizeImageAsync(
        userToken: userToken,
        base64Image: response['image'],
        width: width,
        height: height,
      );
    }

    //convert and save the image
    return ImageConvertor.imageFromBase64String(response['image']);
  }
}
