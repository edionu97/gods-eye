
import 'point/model.dart';

class FacialKeyPointsModel {
  final FacialPoint rightEyePoint;
  final FacialPoint leftEyePoint;
  final FacialPoint nosePoint;
  final FacialPoint mouthLeftPoint;
  final FacialPoint mouthRightPoint;

  /// convert the json in a dart object
  static FacialKeyPointsModel convertFromJson(final dynamic json) {
    return FacialKeyPointsModel(
      rightEyePoint:  FacialPoint.convertFromJson(json['RightEyePoint']),
      leftEyePoint:  FacialPoint.convertFromJson(json['LeftEyePoint']),
      nosePoint:  FacialPoint.convertFromJson(json['NosePoint']),
      mouthLeftPoint:  FacialPoint.convertFromJson(json['MouthLeftPoint']),
      mouthRightPoint:  FacialPoint.convertFromJson(json['MouthRightPoint']),
    );
  }

  FacialKeyPointsModel(
      {this.rightEyePoint,
      this.leftEyePoint,
      this.nosePoint,
      this.mouthLeftPoint,
      this.mouthRightPoint});
  
}
