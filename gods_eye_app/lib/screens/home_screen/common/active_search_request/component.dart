import 'package:flutter/material.dart';
import 'package:gods_eye_app/services/models/active_search_request/model.dart';
import 'package:intl/intl.dart';

class ActiveSearchRequest extends StatefulWidget {
  final ActiveSearchRequestModel activeSearchRequestModel;

  final double fontSize;
  final double opacityValue;
  final Widget onTopWidget;
  final String extraText;

  //set the active search request model
  ActiveSearchRequest(
      {@required this.activeSearchRequestModel,
      this.extraText,
      this.onTopWidget,
      this.fontSize = 9,
      this.opacityValue = .8});

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
    _animation = Tween<double>(begin: 0, end: widget.opacityValue)
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
    //create the transition
    return FadeTransition(
        opacity: _animation,
        //wrap everything a in scale transition
        child: ScaleTransition(
            scale: _animation,
            //create a stack in order to place the top widget
            child: _createCard(context)));
  }

  /// This method creates the card
  Widget _createCard(BuildContext context) {
    //declare the border radius
    const BorderRadius borderRadius = BorderRadius.only(
        topLeft: Radius.circular(5),
        topRight: Radius.circular(5),
        bottomLeft: Radius.circular(10),
        bottomRight: Radius.circular(10));

    //create the widgets
    final widgetList = [
      //put the top widget
      _createCardBody(context),
      widget.onTopWidget
      //create the card
    ];

    //create a new instance of the card
    return Card(
        //set the card border
        shape: RoundedRectangleBorder(borderRadius: borderRadius),
        //put a shadow
        shadowColor: Colors.blueGrey[600],
        //set the elevation
        elevation: 5,
        //create the elements of the card
        child: Stack(
            clipBehavior: Clip.none,
            //set the children ignoring the null values
            children: widgetList.where((element) => element != null).toList()));
  }

  /// Create all the items from card
  Widget _createCardBody(BuildContext context) {
    //get the value that will be displayed in the ui
    String startedAtDateTime = "unknown";
    if (widget.activeSearchRequestModel?.startedAt != null) {
      startedAtDateTime = DateFormat("dd-MM-yyyy (HH:mm)")
          .format(widget.activeSearchRequestModel?.startedAt);
    }

    //put the extra message
    if(widget.extraText != null && widget.extraText.isNotEmpty){
      startedAtDateTime = "${widget.extraText}$startedAtDateTime";
    }

    //get the image
    var image = widget.activeSearchRequestModel?.image ??
        Image.asset("unknown.png", fit: BoxFit.fill);

    //create the column
    return Column(crossAxisAlignment: CrossAxisAlignment.stretch, children: [
      //put the image on top
      Expanded(
          child: ClipRRect(
              borderRadius: BorderRadius.only(
                  topLeft: Radius.circular(5), topRight: Radius.circular(5)),
              child: image)),
      //on bottom put the date
      Container(
          height: 20,
          decoration: BoxDecoration(
              color: Colors.white70,
              borderRadius: const BorderRadius.only(
                  bottomLeft: Radius.circular(10),
                  bottomRight: Radius.circular(10))),
          child: Center(
              child: Text(startedAtDateTime,
                  style: TextStyle(
                      fontWeight: FontWeight.bold,
                      fontStyle: FontStyle.normal,
                      fontSize: widget.fontSize,
                      color: Colors.blueGrey[700]))))
    ]);
  }
}
