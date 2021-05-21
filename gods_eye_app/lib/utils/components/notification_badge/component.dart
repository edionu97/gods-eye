import 'package:flutter/material.dart';

class NotificationBadge extends StatefulWidget {
  final Function displayBadgeIf;
  final Widget mainWidget;
  final String badgeText;

  NotificationBadge({this.displayBadgeIf, this.mainWidget, this.badgeText});

  @override
  State<StatefulWidget> createState() => _NotificationBadgeState();
}

class _NotificationBadgeState extends State<NotificationBadge> {
  @override
  Widget build(BuildContext context) {
    // create the box shadow list
    final boxShadowList = [
      //create the box shadows
      BoxShadow(
        color: Colors.blueGrey[700],
        blurRadius: .5,
        spreadRadius: 2.0,
        offset: Offset(-1.0, -1.0), // shadow direction: bottom right
      ),
      //create the box shadows
      BoxShadow(
        color: Colors.blueGrey[700],
        blurRadius: .5,
        spreadRadius: 2.0,
        offset: Offset(1.0, 1.0), // shadow direction: top left
      ),
      //create the box shadows
      BoxShadow(
        color: Colors.blueGrey[700],
        blurRadius: .5,
        spreadRadius: 2.0,
        offset: Offset(1.0, -1.0), // shadow direction: top right
      ),
      //create the box shadows
      BoxShadow(
        color: Colors.blueGrey[700],
        blurRadius: .5,
        spreadRadius: 2.0,
        offset: Offset(-1.0, 1.0), // shadow direction: bottom left
      )
    ];

    //display the object only if the predicate is defined
    //and its value is false
    bool result = widget.displayBadgeIf?.call();
    if (result != null && !result) {
      return widget.mainWidget;
    }

    //create a stack with two
    return Stack(clipBehavior: Clip.none, children: [
      //set  the main icon
      widget.mainWidget,
      //set the positioned badge on top right
      Positioned(
          right: -5,
          top: -5,
          // create the container
          child: Container(
              padding: EdgeInsets.all(1),
              decoration: BoxDecoration(
                  color: Colors.blueGrey[400],
                  borderRadius: BorderRadius.circular(20),
                  boxShadow: boxShadowList),
              constraints: BoxConstraints(minWidth: 15, minHeight: 15),
              child: Center(
                  child: Text(widget.badgeText ?? "",
                      style: TextStyle(
                        color: Colors.white70,
                        fontSize: 8,
                        fontWeight: FontWeight.bold
                      ),
                      textAlign: TextAlign.center))))
    ]);
  }
}
