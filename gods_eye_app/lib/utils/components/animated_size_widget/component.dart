import 'package:flutter/material.dart';

class AnimatedSizeWidget extends StatefulWidget {
  final Duration duration;
  final Widget widget;
  final bool startVisible;

  const AnimatedSizeWidget(
      {Key key,
      @required this.widget,
      this.startVisible = false,
      this.duration = const Duration(milliseconds: 600)})
      : super(key: key);

  @override
  _AnimatedSizeWidgetState createState() => _AnimatedSizeWidgetState();
}

class _AnimatedSizeWidgetState extends State<AnimatedSizeWidget>
    with SingleTickerProviderStateMixin {
  //define the animation controller
  AnimationController _animationController;
  Animation<double> _animation;

  @override
  void initState() {
    //execute the super logic
    super.initState();

    //create the animation controller
    _animationController = AnimationController(
        vsync: this,
        duration: widget.duration ?? const Duration(milliseconds: 600));

    //if the widget must start visible then opacity should be 0
    final double begin = widget.startVisible ? 1 : 0;

    //create the opacity controller
    _animation = Tween<double>(begin: begin, end: 1 - begin)
        .animate(_animationController);

    //start the animation
    _animationController.forward();
  }

  @override
  void dispose() {
    //dispose the animation controller
    _animationController.dispose();
    //execute the base dispose logic
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return ScaleTransition(scale: _animation, child: widget.widget);
  }
}
