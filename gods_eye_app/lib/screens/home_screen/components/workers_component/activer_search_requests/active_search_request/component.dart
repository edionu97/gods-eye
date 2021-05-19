import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

class ActiveSearchRequest extends StatefulWidget {
  @override
  State<StatefulWidget> createState() => _ActiveSearchRequestState();
}

class _ActiveSearchRequestState extends State<ActiveSearchRequest>
    with TickerProviderStateMixin {
  //define the animation controller
  AnimationController _animationController;
  Animation<double> _animation;

  @override
  void initState() {
    //execute the super logic
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
    return FadeTransition(
      opacity: _animation,
      child: ScaleTransition(
        scale: _animation,
        child: Container(
          child: Card(
              //set the card border
              shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.only(
                topLeft: Radius.circular(5),
                topRight: Radius.circular(5),
                bottomLeft: Radius.circular(10),
                bottomRight: Radius.circular(10),
              )),
              //put a shadow
              shadowColor: Colors.blueGrey[600],
              //set the elevation
              elevation: 5,
              //create the elements of the card
              child: _createCardItems(context)),
        ),
      ),
    );
  }

  Widget _createCardItems(BuildContext context) {
    var time = DateTime.now();
    final startedAtDatetime = DateFormat("dd-MM-yyyy (HH:mm)").format(time);

    //create the column
    return Column(crossAxisAlignment: CrossAxisAlignment.stretch, children: [
      //put the image on top
      Expanded(
          child: ClipRRect(
              borderRadius: BorderRadius.only(
                topLeft: Radius.circular(5),
                topRight: Radius.circular(5),
              ),
              child: Image.asset("eli.png", fit: BoxFit.fill))),
      //on bottom put the date
      Container(
          height: 20,
          decoration: BoxDecoration(
              color: Colors.white70,
              borderRadius: const BorderRadius.only(
                  bottomLeft: Radius.circular(10),
                  bottomRight: Radius.circular(10))),
          child: Center(
            child: Text(startedAtDatetime,
                style: TextStyle(
                    fontWeight: FontWeight.bold,
                    fontStyle: FontStyle.normal,
                    fontSize: 9,
                    color: Colors.blueGrey[700])),
          ))
    ]);
  }
}
