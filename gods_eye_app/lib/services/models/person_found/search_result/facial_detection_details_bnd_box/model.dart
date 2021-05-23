class FaceBoundingBoxModel {
  final int topX;
  final int topY;
  final int bottomX;
  final int bottomY;

  /// Convert the json into a dart object instance
  static FaceBoundingBoxModel convertFromJson(final dynamic json) {
    return FaceBoundingBoxModel(
      topX: json["TopX"],
      topY: json["TopY"],
      bottomX: json["BottomX"],
      bottomY: json["BottomY"]
    );
  }

  FaceBoundingBoxModel({this.topX, this.topY, this.bottomX, this.bottomY});
}