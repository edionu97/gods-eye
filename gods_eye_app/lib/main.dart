import 'package:flutter/material.dart';
import 'package:gods_eye_app/screens/login/component.dart';

/// This represents the app screen
class AppScreen extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      home: Container(
        child: Scaffold(
          body: LoginScreen(),
        ),
      ),
    );
  }
}

/// App entry point
void main() => runApp(AppScreen());
