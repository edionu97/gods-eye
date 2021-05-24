import 'package:flutter/material.dart';
import 'package:gods_eye_app/services/models/person_found/model.dart';

class StateRequestDetails extends StatefulWidget {
  //declare the fields that are required for this component
  final String userToken;
  final Image searchRequestImage;
  final List<PersonFoundMessageModel> searchRequests;
  final Object tag;

  const StateRequestDetails(
      {Key key,
      this.userToken,
      this.searchRequests,
      this.searchRequestImage,
      @required this.tag})
      : super(key: key);

  @override
  _StateRequestDetailsState createState() => _StateRequestDetailsState();
}

class _StateRequestDetailsState extends State<StateRequestDetails> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: GestureDetector(
        onTap: () => Navigator.of(context).pop(),
        child: Container(
            child: Center(
                child:
                    Hero(tag: widget.tag, child: widget.searchRequestImage))),
      ),
    );
  }
}
