import 'package:flutter/material.dart';

class AnimatedButton extends StatefulWidget {
  //properties
  final String name;
  final Function action;

  //constructor
  //sets those two properties (the button name and the action)
  AnimatedButton({this.name = "", @required this.action});

  @override
  State<StatefulWidget> createState() => _AnimatedButtonState();
}

class _AnimatedButtonState extends State<AnimatedButton>
    with SingleTickerProviderStateMixin {
  //properties
  Animation<num> _buttonSqueezeAnimation;
  AnimationController _animationController;

  //override the init state method
  @override
  void initState() {
    //call the base class
    super.initState();

    //create the animation controller
    _animationController = AnimationController(
        vsync: this, duration: Duration(milliseconds: 1500));

    //create the squeeze animation
    _buttonSqueezeAnimation = Tween(begin: 380, end: 70.0).animate(
        CurvedAnimation(
            parent: _animationController, curve: Interval(0.0, 0.150)));

    //add the listener (just refresh the object)
    _buttonSqueezeAnimation.addListener(() => setState(() {}));
  }

  @override
  Widget build(BuildContext context) {
    return Material(
        color: Colors.white,
        //create an ink well instance
        child: InkWell(
            //when we press over it play the animation
            onTap: () => _playAnimation(),
            //create the button
            child: Container(
                //width will updated based on the values from the animations
                width: _buttonSqueezeAnimation.value.toDouble(),
                height: 50,
                alignment: FractionalOffset.center,
                decoration: BoxDecoration(
                    color: Colors.blueGrey[500],
                    borderRadius:
                        BorderRadius.all(const Radius.circular(30.0))),
                //the child is either a text or a progress bar
                child: _buttonSqueezeAnimation.value.toDouble() > 75
                    ? Text(widget.name,
                        style: TextStyle(
                          color: Colors.white,
                          fontSize: 20.0,
                          fontWeight: FontWeight.w300,
                          letterSpacing: 0.3,
                        ))
                    : CircularProgressIndicator(
                        valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
                      ))));
  }

  // This method it is used for starting the animation
  Future<Null> _playAnimation() async {
    //try to execute animation
    try {
      //do the animation in the frontal way
      await _animationController.forward();

      //do the animation in reverse way
      await _animationController.reverse();

      //if the action is specified then
      if (widget.action == null) {
        return;
      }

      //execute the callback
      widget.action();
    } on TickerCanceled catch (e) {
      print("Play animation " + e.toString());
    }
  }
}
