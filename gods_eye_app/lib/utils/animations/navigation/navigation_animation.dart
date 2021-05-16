import 'package:flutter/cupertino.dart';

/// This represents an navigation animation
class NavigationAnimation extends CupertinoPageRoute {
  //property
  final Function toPage;

  //constructor of the object
  NavigationAnimation({@required this.toPage})
      : super(builder: (BuildContext context) => toPage());

  @override
  // execute this method when the route is first build
  Widget buildPage(BuildContext context, Animation<double> animation,
          Animation<double> secondaryAnimation) =>
      FadeTransition(
          opacity: animation,
          child: ScaleTransition(
            scale: animation,
            alignment: Alignment.center,
            child: toPage(),
          ));
}
