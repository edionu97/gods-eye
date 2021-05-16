import 'package:flutter/material.dart';

class Decorator {
  /// Creates the decoration as a rounded bottom left square
  static BoxDecoration getDecoration() {
    return BoxDecoration(
      gradient: LinearGradient(
        begin: Alignment.topLeft,
        end: Alignment.bottomRight,
        stops: [0.1, 0.5, 0.7, 0.9],
        colors: [
          Colors.blueGrey[700],
          Colors.blueGrey[600],
          Colors.blueGrey[500],
          Colors.blueGrey[400],
        ],
      ),
      borderRadius:
      new BorderRadius.only(bottomLeft: const Radius.circular(150)),
    );
  }

  static getAllRoundedCornersDecoration() {
    return BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.bottomRight,
          end: Alignment.topCenter,
          stops: [0.1, .4, .7, .9],
          colors: [
            Colors.blueGrey[700],
            Colors.blueGrey[600],
            Colors.blueGrey[500],
            Colors.blueGrey[400],
          ],
        ),
        borderRadius: new BorderRadius.all(const Radius.circular(15)));
  }

  static BoxDecoration getDialogDecoration() {
    return BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          stops: [0.1, 0.5, 0.7, 0.9],
          colors: [
            Colors.white12,
            Colors.white12,
            Colors.white12,
            Colors.white12,
          ],
        ),
        borderRadius: BorderRadius.only(
            bottomLeft: Radius.circular(30),
            bottomRight: Radius.circular(30),
            topLeft: Radius.circular(30),
            topRight: Radius.circular(30)));
  }

  static BoxDecoration getImageDecoration(final Image img) {
    return BoxDecoration(
        shape: BoxShape.circle,
        border: Border.all(color: Colors.white),
        image: new DecorationImage(
          fit: BoxFit.fill,
          image: img.image,
        ));
  }

  static BoxDecoration getDefaultImageDecoration() {
    return new BoxDecoration(
        shape: BoxShape.circle,
        border: Border.all(color: Colors.white),
        image: new DecorationImage(
          fit: BoxFit.fill,
          image: AssetImage("user.png"),
        ));
  }

  static BoxDecoration getSimpleDecoration() {
    return BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomCenter,
          stops: [0.2, .5, .8, .8],
          colors: [
            Colors.blueGrey[700],
            Colors.blueGrey[800],
            Colors.blueGrey[800],
            Colors.blueGrey[800],
          ],
        ));
  }
}
