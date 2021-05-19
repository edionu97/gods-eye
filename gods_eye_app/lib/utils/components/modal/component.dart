import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';

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

  static Future<dynamic> openDialogAsync(
      {@required BuildContext context,
      String message,
      String title = "Error",
      IconData iconData = Icons.error,
      Color iconColor = Colors.black}) {
    return showDialog(
        context: context,
        builder: (context) {
          return Container(
              color: Colors.transparent,
              // container to set color
              child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: <Widget>[
                    Container(
                      margin: EdgeInsets.symmetric(horizontal: 5),
                      child: DialogCustom(
                        message: message,
                        title: title,
                        icon: Icon(
                          iconData,
                          color: iconColor,
                        ),
                      ),
                    )
                  ]));
        });
  }

  /// Shows the cupertino dialog
  static Future showDialogWithNoActionsAsync(BuildContext context,
      {Widget title, Widget content}) async {

    //treat the null case
    if(context == null){
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
}
