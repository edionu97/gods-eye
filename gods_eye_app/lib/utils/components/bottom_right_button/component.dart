import 'package:flutter/material.dart';

class BottomRightButton extends StatefulWidget {
  final Function onPressed;

  BottomRightButton({@required this.onPressed});

  @override
  State<StatefulWidget> createState() => _BottomRightButtonState();
}

class _BottomRightButtonState extends State<BottomRightButton> with TickerProviderStateMixin {

  //define the animation controller
  AnimationController _animationController;
  Animation<double> _animation;

  @override
  void initState(){
    //init the state
    super.initState();

    //create the animation controller
    _animationController = AnimationController(
        vsync: this, duration: const Duration(milliseconds: 700));

    //create the opacity controller
    _animation = Tween<double>(begin: 0, end: 1)
        .animate(_animationController);

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
    //position the button bottom right
    return Positioned(
      bottom: 40,
      right: 30,
      //add a scale transition
      child: ScaleTransition(
        scale: _animation,
        //add a fade transition
        child: FadeTransition(
          opacity: _animation,
          //create the container
          child: Container(
                height: 60,
                width: 60,
                //set a little opacity
                child: ElevatedButton(
                    //set the on pressed function
                    onPressed: () => widget.onPressed?.call(),
                    //set the button style
                    style: ElevatedButton.styleFrom(
                        primary: Colors.blueGrey[400],
                        shadowColor: Colors.blueGrey[700],
                        visualDensity: VisualDensity.comfortable,
                        //create the button shape
                        shape: RoundedRectangleBorder(
                            borderRadius:
                                BorderRadius.all(Radius.circular(20))),
                        elevation: 20),
                    //put the icon
                    child: ImageIcon(AssetImage("new_search.png")))),
        ),
      ),
    );
  }
}
