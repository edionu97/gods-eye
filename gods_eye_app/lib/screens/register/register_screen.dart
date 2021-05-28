import 'package:flutter/material.dart';
import 'package:gods_eye_app/screens/home_screen/home_screen.dart';
import 'package:gods_eye_app/services/notifications/service.dart';
import 'package:gods_eye_app/services/user_service/service.dart';
import 'package:gods_eye_app/utils/animations/navigation/navigation_animation.dart';
import 'package:gods_eye_app/utils/components/animated_button/component.dart';
import 'package:gods_eye_app/utils/components/modal/component.dart';
import 'package:gods_eye_app/utils/components/upper_element/component.dart';
import 'package:gods_eye_app/utils/components/user_form/component.dart';

class RegisterScreenWidget extends StatefulWidget {
  @override
  State<StatefulWidget> createState() => _RegisterScreenWidgetState();
}

class _RegisterScreenWidgetState extends State<RegisterScreenWidget> {
  bool _isButtonEnabled = false;

  //properties
  final TextEditingController controllerPassword = new TextEditingController();
  final TextEditingController controllerConfirmPassword =
      new TextEditingController();
  final TextEditingController controllerUsername = new TextEditingController();

  final UserService _userService = UserService();

  final GlobalKey<FormState> _formKey = new GlobalKey<FormState>();

  //override the dispose method
  @override
  void dispose() {
    //also dispose all the controllers
    controllerPassword.dispose();
    controllerConfirmPassword.dispose();
    controllerUsername.dispose();

    //execute the base logic
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    //the scroll view is required when the keyboard will be on top
    return Scaffold(
        body: SingleChildScrollView(
            //this represents the container
            child: Container(
                height: MediaQuery.of(context).size.height,
                //register page is composed of 4 parts
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: <Widget>[
                    //the upper element that contains a decoration
                    UpperElement(name: 'Register', path: 'assets/register.png'),
                    //the form (with the input fields)
                    UserForm(
                      controllerConfirmPassword: controllerConfirmPassword,
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
                        child: GestureDetector(
                            onTap: () => _signInPressed(context),
                            child: Text.rich(
                              TextSpan(
                                text: 'Already Having An Account? ',
                                style: TextStyle(fontSize: 17),
                                children: [
                                  TextSpan(
                                      text: 'Sign In',
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
                                  name: "Register",
                                  action: () => _registerPressed(context)))),
                      padding: EdgeInsets.only(bottom: 25),
                    )
                  ],
                ))));
  }

  void _signInPressed(BuildContext context) {
    Navigator.pop(context);
  }

  void _registerPressed(BuildContext context) async {
    try {
      //if the fields are null do nothing
      if (!_formKey.currentState.validate()) {
        return;
      }
      //get the user token
      final String userToken = await _userService.registerAsync(
          controllerUsername.text?.trim(), controllerPassword.text?.trim());

      //register the user token
      await NotificationService().registerAsync(userToken);

      //navigate to home screen after the successful register
      Navigator.of(context).push(
          NavigationAnimation(toPage: () => HomeScreenWidget(userToken: userToken)));
    } on Exception catch (e) {
      //get the message
      final message = Modal.extractMessageFromException(e);
      //get the message and report it
      await Modal.showExceptionalDialogWithNoActionsAsync(
          context, "Register error", message);
    }
  }
}
