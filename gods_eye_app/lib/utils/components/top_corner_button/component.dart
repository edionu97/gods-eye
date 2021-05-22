import 'package:flutter/material.dart';

class TopCornerButton extends StatefulWidget {
  final Function onTap;

  const TopCornerButton({@required this.onTap});

  @override
  State<StatefulWidget> createState() => _TopCornerButton();
}

class _TopCornerButton extends State<TopCornerButton>
    with TickerProviderStateMixin {
  //define the animation controller
  AnimationController _animationController;
  Animation<double> _animation;

  @override
  void initState() {
    super.initState();

    //create the animation controller
    _animationController = AnimationController(
        vsync: this, duration: const Duration(milliseconds: 600));

    //create the opacity controller
    _animation = Tween<double>(begin: 0, end: 1).animate(_animationController);

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
    return Positioned(
      right: 0,
      top: 0,
      child: FadeTransition(
        opacity: _animation,
        child: ScaleTransition(
            scale: _animation,
            child: Opacity(
              opacity: .6,
              child: Container(
                      height: 31,
                      width: 31,
                      //create the child
                      child: Card(
                        //set the card border
                        shape: RoundedRectangleBorder(
                            side: BorderSide(color: Colors.white70, width: 1),
                            borderRadius: BorderRadius.all(Radius.circular(30))),
                        //put a shadow
                        shadowColor: Colors.blueGrey[700],
                        //set the elevation
                        elevation: 5,
                        //create the inkwell
                        child: InkWell(
                            //put the border
                            customBorder: CircleBorder(),
                            //set the on tap
                            onTap: widget.onTap,
                            //set the icon
                            child: Icon(Icons.clear_rounded,
                                color: Colors.red[700], size: 23)),
                      )),
            ),
          ),
      ),
    );
  }
}
