import 'package:flutter/material.dart';
import 'package:gods_eye_app/utils/components/loader/component.dart';

class PersonSearchScreen extends StatefulWidget {
  final String userToken;

  const PersonSearchScreen({this.userToken});

  @override
  State<StatefulWidget> createState() => _PersonSearchScreenState();

}

class _PersonSearchScreenState extends State<PersonSearchScreen> {
  @override
  Widget build(BuildContext context) {
    return Center(child: CircularSpinningLoader());
  }
}