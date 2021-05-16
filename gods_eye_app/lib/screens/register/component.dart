import 'package:flutter/material.dart';
import 'package:gods_eye_app/utils/components/animated_button/component.dart';
import 'package:gods_eye_app/utils/components/upper_element/component.dart';
import 'package:gods_eye_app/utils/components/user_form/component.dart';

class RegisterScreen extends StatefulWidget {
  @override
  State<StatefulWidget> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {

  //properties
  final TextEditingController controllerPassword = new TextEditingController();
  final TextEditingController controllerConfirmPassword =
      new TextEditingController();
  final TextEditingController controllerUsername = new TextEditingController();

  final GlobalKey<FormState> _formKey = new GlobalKey<FormState>();

  //override the dispose method
  @override
  void dispose() {
    //execute the base logic
    super.dispose();

    //also dispose all the controllers
    controllerPassword.dispose();
    controllerConfirmPassword.dispose();
    controllerUsername.dispose();
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
                      child: AnimatedButton(
                          name: "Register",
                          action: () => _registerPressed(context)),
                      padding: EdgeInsets.only(bottom: 25),
                    )
                  ],
                ))));
  }

  void _signInPressed(BuildContext context) {
    Navigator.pop(context);
  }

  void _registerPressed(BuildContext context) async {}
}
