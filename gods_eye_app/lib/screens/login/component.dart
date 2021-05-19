import 'package:flutter/material.dart';
import 'package:gods_eye_app/screens/home_screen/component.dart';
import 'package:gods_eye_app/screens/register/component.dart';
import 'package:gods_eye_app/services/notifications/service.dart';
import 'package:gods_eye_app/services/user_service/service.dart';
import 'package:gods_eye_app/utils/animations/navigation/navigation_animation.dart';
import 'package:gods_eye_app/utils/components/animated_button/component.dart';
import 'package:gods_eye_app/utils/components/modal/component.dart';
import 'package:gods_eye_app/utils/components/upper_element/component.dart';
import 'package:gods_eye_app/utils/components/user_form/component.dart';

/// This represents the login screen
/// Everytime the app will be open, this is the first screen that will be shown
class LoginScreen extends StatefulWidget {
  @override
  State<StatefulWidget> createState() => _LoginScreenState();
}

/// This represents the state for the login screen
class _LoginScreenState extends State<LoginScreen> {
  bool _isButtonEnabled = false;

  // controller for username
  final UserService _userService = UserService();
  final GlobalKey<FormState> _formKey = new GlobalKey<FormState>();

  final TextEditingController controllerUsername = new TextEditingController();
  final TextEditingController controllerPassword = new TextEditingController();

  @override
  void dispose() {
    //dispose the controllers
    controllerUsername.dispose();
    controllerPassword.dispose();
    //execute the logic of the base class
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        //the scroll view is required when the keyboard will be on top
        body: SingleChildScrollView(
            //this represents the container
            child: Container(
                height: MediaQuery.of(context).size.height,
                //login page is composed of 4 parts
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: <Widget>[
                    //the upper element that contains a decoration
                    UpperElement(name: 'Login', path: 'assets/login.png'),
                    //the form (with the input fields)
                    UserForm(
                      controllerPassword: controllerPassword,
                      controllerUsername: controllerUsername,
                      formKey: _formKey,
                      onFocusMoved: () {
                        setState(() {
                          _isButtonEnabled = _formKey.currentState.validate();
                        });
                      },
                    ),
                    //the text with the redirect button
                    Align(
                        alignment: Alignment.bottomCenter,
                        //used gesture detector to encapsulate the click over text
                        child: GestureDetector(
                            onTap: () => _registerPressed(context),
                            child: Text.rich(
                              TextSpan(
                                text: 'Not Having An Account? ',
                                style: TextStyle(fontSize: 17),
                                children: [
                                  TextSpan(
                                      text: 'Register',
                                      style: TextStyle(
                                          fontWeight: FontWeight.bold,
                                          color: Colors.blueGrey[700],
                                          fontSize: 17)),
                                ],
                              ),
                            ))),
                    Padding(
                      // the animated button
                      child: Opacity(
                          opacity: _isButtonEnabled ? 1 : 0.7,
                          child: AbsorbPointer(
                              absorbing: !_isButtonEnabled,
                              child: AnimatedButton(
                                  name: "Sign In",
                                  action: () => _loginPressed(context)))),
                      padding: EdgeInsets.only(bottom: 25),
                    )
                  ],
                ))));
  }

  /// Handle the login action
  /// Calls the service and tries to authenticate the current user
  void _loginPressed(BuildContext context) async {
    try {
      //if the fields are null do nothing
      if (!_formKey.currentState.validate()) {
        return;
      }
      //get the user token
      final String userToken = await _userService.loginAsync(
          controllerUsername.text?.trim(), controllerPassword.text?.trim());

      //register the user token
      await NotificationService().registerAsync(userToken);

      //navigate to home screen after the successful authentication
      Navigator.of(context).push(
          NavigationAnimation(toPage: () => HomeScreen(userToken: userToken)));
    } on Exception catch (e) {
      //get message
      final message = Modal.extractMessageFromException(e);

      //get the message and report it
      await Modal.showExceptionalDialogWithNoActionsAsync(
          context, "Authentication error", message);
    }
  }

  /// Handle the register button pressed
  /// Redirects to the register page
  void _registerPressed(BuildContext context) {
    Navigator.of(context)
        .push(NavigationAnimation(toPage: () => RegisterScreen()));
  }
}
