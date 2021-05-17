import 'package:flutter/material.dart';

import 'dialog/component.dart';

class Modal {

  /// This function it is used for extracting a message from exception
  static String extractMessageFromException(Exception e){

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
}
