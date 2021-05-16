import 'package:flutter/material.dart';

class TextInput extends StatelessWidget {

  final TextEditingController controller;
  final String name;
  final IconData iconData;
  final bool isHidden;
  final TextInputAction action;
  final FocusNode focusNode;
  final Function fieldSubmitted;
  final Function validator;

  //constructor (initialize all the properties)
  TextInput(
      {this.controller,
        this.name,
        this.iconData,
        this.isHidden = false,
        this.action = TextInputAction.next,
        this.focusNode,
        this.fieldSubmitted,
        this.validator});

  @override
  Widget build(BuildContext context) {
    //create the custom text form field
    return Container(
      padding: EdgeInsets.only(top: 4, left: 16, right: 16, bottom: 4),
      child: TextFormField(
        //if the validator is set execute the validation otherwise do nothing
        validator: (value) => validator != null ? validator(value) : null,
        textInputAction: action,
        obscureText: isHidden,
        focusNode: focusNode,
        //execute the submit code
        onFieldSubmitted: (v) => fieldSubmitted != null ? fieldSubmitted() : (){},
        decoration: InputDecoration(
          hintText: this.name,
          icon: Icon(
            iconData,
            color: Colors.grey,
          ),
          border: InputBorder.none,
        ),
        controller: controller,
      ),
      decoration: BoxDecoration(
          borderRadius: BorderRadius.all(Radius.circular(50)),
          color: Colors.white,
          boxShadow: [BoxShadow(color: Colors.black26, blurRadius: 5)]),
    );
  }


}