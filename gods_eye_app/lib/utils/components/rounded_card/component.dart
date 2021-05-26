import 'package:flutter/material.dart';
import 'package:gods_eye_app/utils/components/animated_opacity_widget/component.dart';

class RoundedCorneredCard extends StatelessWidget {
  final double width;
  final double height;
  final double elevation;
  final double borderRadius;
  final List<Widget> cardTitleChildren;
  final List<Widget> children;
  final Color titleBackgroundColor;

  const RoundedCorneredCard(
      {Key key,
      this.width = 150,
      this.height = 150,
      this.elevation = 10,
      @required this.cardTitleChildren,
      @required this.children,
      this.borderRadius,
      this.titleBackgroundColor})
      : super(key: key);

  @override
  Widget build(BuildContext context) {
    return AnimatedOpacityWidget(
      //set the opacity
      //set the animation duration
      duration: const Duration(seconds: 2),
      //create the card element
      widget: Card(
          //create the card shape
          shape: RoundedRectangleBorder(
              side: BorderSide(color: Colors.white70, width: 1),
              borderRadius: BorderRadius.all(Radius.circular(30))),
          //put a shadow color
          shadowColor: Colors.blueGrey[700],
          //set the elevation
          elevation: elevation,
          child: Container(
              height: height,
              width: width,
              child: Column(
                  crossAxisAlignment: CrossAxisAlignment.center,
                  children: [
                    //define the top part of the card
                    _createCartTopElement(),
                    //unpack the children of the card
                    ...children
                  ]))),
    );
  }

  /// Create the cart top element
  Widget _createCartTopElement() {
    //create a new container
    return Container(
        height: 35,
        decoration: BoxDecoration(
            color: titleBackgroundColor ?? Colors.blueGrey[400],
            borderRadius:
                const BorderRadius.vertical(top: Radius.circular(30))),
        //define the text
        child: Center(child: Column(children: cardTitleChildren)));
  }
}
