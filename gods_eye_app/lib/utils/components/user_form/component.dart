import 'package:flutter/material.dart';
import 'package:gods_eye_app/utils/components/text_input/component.dart';

class UserForm extends StatefulWidget {
  final GlobalKey<FormState> formKey;
  final TextEditingController controllerUsername;
  final TextEditingController controllerPassword;
  final TextEditingController controllerConfirmPassword;

  UserForm(
      {this.controllerUsername,
      this.controllerPassword,
      this.controllerConfirmPassword,
      this.formKey});

  @override
  State<StatefulWidget> createState() => _UserFormState();
}

class _UserFormState extends State<UserForm> {
  final FocusNode focusNodeUsername = new FocusNode();
  final FocusNode focusNodePassword = new FocusNode();
  final FocusNode focusNodeConfirm = new FocusNode();

  @override
  Widget build(BuildContext context) {
    //depending on the values of the controller create a list with up to 3 elements
    return new Form(
        key: widget.formKey,
        child: Container(
          height: 250,
          padding: EdgeInsets.symmetric(horizontal: 20),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.spaceEvenly,
            children: <Widget>[
              //add the username
              widget.controllerUsername != null
                  ? Material(
                      child: TextInput(
                          name: "Username",
                          iconData: Icons.person_outline,
                          controller: widget.controllerUsername,
                          focusNode: focusNodeUsername,
                          validator: (value) {
                            String val = value.toString();
                            if (val.isEmpty) {
                              return "Username is empty";
                            }
                          },
                          fieldSubmitted: () {
                            focusNodeUsername.unfocus();
                            FocusScope.of(context)
                                .requestFocus(focusNodePassword);
                          }))
                  : null,
              //add the password
              widget.controllerPassword != null
                  ? Material(
                      child: TextInput(
                          name: "Enter password",
                          iconData: Icons.vpn_key,
                          isHidden: true,
                          validator: (value) {
                            String val = value.toString();
                            if (val.isEmpty) {
                              return "Password is empty";
                            }
                          },
                          controller: widget.controllerPassword,
                          focusNode: focusNodePassword,
                          fieldSubmitted: () {
                            focusNodePassword.unfocus();
                            FocusScope.of(context)
                                .requestFocus(focusNodeConfirm);
                          }))
                  : null,
              //add the confirm password
              widget.controllerConfirmPassword != null
                  ? Material(
                      child: TextInput(
                      name: "Confirm password",
                      iconData: Icons.vpn_key,
                      isHidden: true,
                      validator: (value) {
                        String val = value.toString();
                        if (val.isEmpty) {
                          return "Confirm is empty";
                        }
                        if (val != widget.controllerPassword.text) {
                          return "Passwords must be identical";
                        }
                      },
                      controller: widget.controllerConfirmPassword,
                      action: TextInputAction.next,
                      focusNode: focusNodeConfirm,
                      fieldSubmitted: () {
                        focusNodeConfirm.unfocus();
                      },
                    ))
                  : null,
              //select only the not null elements
            ].where((element) => element != null).toList(),
          ),
        ));
  }
}
