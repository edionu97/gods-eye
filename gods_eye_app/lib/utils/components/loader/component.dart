import 'dart:math';

import 'package:flutter/material.dart';

class CircularSpinningLoader extends StatefulWidget {
  //property fields
  final Function displayLoaderIf;
  final Widget elseDisplay;
  final Color spinningDotsColor;
  final Color centerDotColor;

  //display the spinning loader
  CircularSpinningLoader(
      {this.displayLoaderIf,
      this.elseDisplay,
      this.spinningDotsColor,
      this.centerDotColor});

  @override
  _CircularSpinningLoaderState createState() => _CircularSpinningLoaderState();
}

class _CircularSpinningLoaderState extends State<CircularSpinningLoader>
    with SingleTickerProviderStateMixin {
  //the initial radius
  final double _initialRadius = 30;

  //the value of the radius
  double _radius = 0;

  //create the animation controller
  AnimationController _animationController;

  //create the animation
  Animation<double> _animationRotation;
  Animation<double> _animationRadiusIn;
  Animation<double> _animationRadiusOut;

  @override
  void initState() {
    //execute the super init logic
    super.initState();

    //create the animation controller
    _animationController =
        AnimationController(vsync: this, duration: Duration(seconds: 3));

    //create the animation radius
    _animationRadiusIn = Tween(begin: 1.0, end: 0.0).animate(CurvedAnimation(
        parent: _animationController,
        curve: Interval(0.5, 1, curve: Curves.elasticIn)));

    _animationRadiusOut = Tween(begin: 0.0, end: 1.0).animate(CurvedAnimation(
        parent: _animationController,
        curve: Interval(0, 0.5, curve: Curves.elasticOut)));

    //add the animation rotation
    _animationRotation = new Tween(begin: 0.0, end: 1.0).animate(
        CurvedAnimation(
            parent: _animationController,
            curve: Interval(0.0, 1.0, curve: Curves.linear)));

    //add listener to controller
    _animationController.addListener(_animationControllerListener);

    //repeat the animations
    _animationController.repeat();
  }

  @override
  void dispose() {
    //remove the listener
    _animationController.removeListener(_animationControllerListener);

    //dispose the controller
    _animationController.dispose();

    //execute the dispose logic
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    //display the object only if the predicate is defined
    //and its value is false
    bool result = widget.displayLoaderIf?.call();
    if (result != null && !result) {
      return widget.elseDisplay;
    }

    final double _dotRadius = 5.5;
    final double _radianStep = 4;

    //compute the color values
    final Color centerDotColor = widget.centerDotColor ?? Colors.blueGrey[400];
    final Color spinningDotsColor =
        widget.spinningDotsColor ?? Colors.blueGrey[600];

    //return the loader
    return Container(
        width: 150,
        height: 150,
        child: Center(
            child: Stack(children: <Widget>[
          Center(
            child: Dot(
              radius: 10,
              color: centerDotColor,
            ),
          ),
          RotationTransition(
              turns: _animationRotation,
              child: Stack(children: <Widget>[
                Transform.translate(
                  offset: Offset(cos(pi / _radianStep) * _radius,
                      sin(pi / _radianStep) * _radius),
                  child: Dot(radius: _dotRadius, color: spinningDotsColor),
                ),
                Transform.translate(
                  offset: Offset(cos(2 * pi / _radianStep) * _radius,
                      sin(2 * pi / _radianStep) * _radius),
                  child: Dot(radius: _dotRadius, color: spinningDotsColor),
                ),
                Transform.translate(
                  offset: Offset(cos(3 * pi / _radianStep) * _radius,
                      sin(3 * pi / _radianStep) * _radius),
                  child: Dot(radius: _dotRadius, color: spinningDotsColor),
                ),
                Transform.translate(
                  offset: Offset(cos(4 * pi / _radianStep) * _radius,
                      sin(4 * pi / _radianStep) * _radius),
                  child: Dot(radius: _dotRadius, color: spinningDotsColor),
                ),
                Transform.translate(
                  offset: Offset(cos(5 * pi / _radianStep) * _radius,
                      sin(5 * pi / _radianStep) * _radius),
                  child: Dot(radius: _dotRadius, color: spinningDotsColor),
                ),
                Transform.translate(
                  offset: Offset(cos(6 * pi / _radianStep) * _radius,
                      sin(6 * pi / _radianStep) * _radius),
                  child: Dot(radius: _dotRadius, color: spinningDotsColor),
                ),
                Transform.translate(
                  offset: Offset(cos(7 * pi / _radianStep) * _radius,
                      sin(7 * pi / _radianStep) * _radius),
                  child: Dot(radius: _dotRadius, color: spinningDotsColor),
                ),
                Transform.translate(
                  offset: Offset(cos(8 * pi / _radianStep) * _radius,
                      sin(8 * pi / _radianStep) * _radius),
                  child: Dot(radius: _dotRadius, color: spinningDotsColor),
                )
              ]))
        ])));
  }

  void _animationControllerListener() {
    //set the state values
    setState(() {
      //execute the first logic (forward)
      if (_animationController.value >= 0.5 &&
          _animationController.value <= 1.0) {
        _radius = _animationRadiusIn.value * _initialRadius;
        return;
      }

      //execute the out login (reverse)
      if (_animationController.value >= 0 && _animationController.value <= .5) {
        _radius = _animationRadiusOut.value * _initialRadius;
      }
    });
  }
}

class Dot extends StatelessWidget {
  final double radius;
  final Color color;

  Dot({this.color, this.radius});

  @override
  Widget build(BuildContext context) {
    return Center(
        child: Container(
            width: this.radius,
            height: this.radius,
            decoration: BoxDecoration(color: color, shape: BoxShape.circle)));
  }
}
