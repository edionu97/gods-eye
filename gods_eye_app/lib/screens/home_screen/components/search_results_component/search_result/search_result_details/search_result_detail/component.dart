import 'package:flutter/material.dart';
import 'package:gods_eye_app/services/models/person_found/model.dart';
import 'package:gods_eye_app/utils/components/animated_size_widget/component.dart';

class SearchResultDetail extends StatefulWidget {
  final PersonFoundMessageModel foundPersonInfo;

  const SearchResultDetail({Key key, @required this.foundPersonInfo})
      : super(key: key);

  @override
  _SearchResultDetailState createState() => _SearchResultDetailState();
}

class _SearchResultDetailState extends State<SearchResultDetail> {
  @override
  Widget build(BuildContext context) {
    return AnimatedSizeWidget(
      duration: const Duration(milliseconds: 800),
      widget: Card(
          elevation: 10,
          shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.all(Radius.circular(25)))),
    );
  }
}
