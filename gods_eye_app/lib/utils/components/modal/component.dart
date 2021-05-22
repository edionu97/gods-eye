import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:gods_eye_app/utils/components/decoration_form/component.dart';

import 'dialog/component.dart';

class Modal {
  /// This function it is used for extracting a message from exception
  static String extractMessageFromException(Exception e) {
    //get the regex
    final regex = RegExp(r":(?<message>.*)$");

    //check if the
    if (regex.hasMatch(e.toString())) {
      //get the with that name
      return regex.firstMatch(e.toString()).namedGroup("message");
    }

    //return the exception
    return e?.toString();
  }

  /// Shows the cupertino dialog
  static Future showDialogWithNoActionsAsync(BuildContext context,
      {Widget title, Widget content}) async {
    //treat the null case
    if (context == null) {
      throw Exception("The context must not be null");
    }

    //show the cupertino dialog
    await showDialog(
        context: context,
        //build the cupertino box
        builder: (BuildContext context) => CupertinoAlertDialog(
            //set the title
            title: title,
            //set the box constraints
            content: content));
  }

  static Future showExceptionalDialogWithNoActionsAsync(
      BuildContext context, String title, String message) async {
    //treat the null case
    if (context == null) {
      throw Exception("The context must not be null");
    }

    //show the cupertino dialog
    await showDialog(
        context: context,
        //build the cupertino box
        builder: (BuildContext context) => CupertinoAlertDialog(
            //set the title
            title: Container(
                child: Align(
                    alignment: Alignment.center,
                    child: Text(title ?? "",
                        style: TextStyle(color: Colors.blueGrey[500])))),
            //set the box constraints
            content: Column(children: [
              Divider(
                color: Colors.blueGrey,
              ),
              Padding(
                  padding: const EdgeInsets.only(top: 15),
                  child: Text(message ?? "",
                      style:
                          TextStyle(color: Colors.blueGrey[500], fontSize: 16)))
            ])));
  }

  static Future showBottomWithActionsDialogAsync(BuildContext context,
      {List<Widget> actions}) async {
    //the action list must not be null
    actions = actions ?? [];

    //define the border radius
    const border = Radius.circular(30);
    //display a modal on the bottom of the phone
    await showModalBottomSheet(
        //this is used to clip the items
        clipBehavior: Clip.antiAliasWithSaveLayer,
        //create the border
        shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.only(topLeft: border, topRight: border)),
        //set the context
        context: context,
        //set the background color
        backgroundColor: Colors.blueGrey[800],
        //the build context
        builder: (BuildContext context) =>
            SafeArea(child: Wrap(children: actions)));
  }
}
