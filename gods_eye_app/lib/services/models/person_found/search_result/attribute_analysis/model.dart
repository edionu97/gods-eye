class AttributeAnalysisModel {
  final int age;
  final String emotion;
  final String gender;
  final String race;

  /// convert the json into dart object
  static AttributeAnalysisModel convertFromJson(final dynamic json) {
    return AttributeAnalysisModel(
      age: json["Age"],
      emotion: json["Emotion"],
      gender: json["Gender"],
      race: json["Emotion"]
    );
  }

  AttributeAnalysisModel({this.age, this.emotion, this.gender, this.race});
}
