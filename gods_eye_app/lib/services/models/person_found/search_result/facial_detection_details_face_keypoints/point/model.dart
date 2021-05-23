class FacialPoint {
  final int x;
  final int y;

  static FacialPoint convertFromJson(final dynamic json) {
    return FacialPoint(
      x: json["X"],
      y: json["Y"]
    );
  }

  FacialPoint({this.x, this.y});
}