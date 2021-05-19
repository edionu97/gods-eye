import 'package:flutter/cupertino.dart';
import 'package:gods_eye_app/screens/home_screen/components/workers_component/active_search_requests/component.dart';
import 'package:gods_eye_app/services/models/remote_worker/model.dart';
import 'package:gods_eye_app/utils/components/modal/component.dart';
import 'package:intl/intl.dart';
import 'package:flutter/material.dart';

class RemoteWorker extends StatefulWidget {
  //the worker model
  final RemoteWorkerModel workerModel;

  //construct the worker
  const RemoteWorker({Key key, @required this.workerModel}) : super(key: key);

  @override
  State<StatefulWidget> createState() => _RemoteWorkerState();
}

class _RemoteWorkerState extends State<RemoteWorker>
    with TickerProviderStateMixin {
  //define the animation controller
  AnimationController _animationController;
  Animation<double> _opacityAnimation;

  @override
  void initState() {
    //execute the super logic
    super.initState();

    //create the animation controller
    _animationController = AnimationController(
        vsync: this, duration: const Duration(milliseconds: 1000));

    //create the opacity controller
    _opacityAnimation =
        Tween<double>(begin: 0, end: 1).animate(_animationController);

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
    //put a padding around the card
    return FadeTransition(
      opacity: _opacityAnimation,
      child: Padding(
          padding: const EdgeInsets.all(8.0),
          //create a container and place the card inside
          child: Container(
              child: Card(
                  //set the card border
                  shape: RoundedRectangleBorder(
                      side: BorderSide(color: Colors.white70, width: 1),
                      borderRadius: BorderRadius.all(Radius.circular(30))),
                  //put a shadow
                  shadowColor: Colors.blueGrey[700],
                  //set the elevation
                  elevation: 12,
                  //create the elements of the card
                  child: _createCardElements(context)))),
    );
  }

  ///Create the card element
  Widget _createCardElements(BuildContext context) {
    //return the column
    return InkWell(
        onTap: () => _onCardClickedAsync(context),
        customBorder:
            RoundedRectangleBorder(borderRadius: BorderRadius.circular(30)),
        child: Column(crossAxisAlignment: CrossAxisAlignment.center, children: [
          //define the top part of the card
          _createCartTopElement(),
          //define the bottom part of the card
          _createCardBottomElement()
        ]));
  }

  /// Create the cart top element
  Widget _createCartTopElement() {
    return Container(
        height: 35,
        decoration: BoxDecoration(
            color: Colors.blueGrey[400],
            borderRadius: const BorderRadius.only(
                topLeft: Radius.circular(30), topRight: Radius.circular(30))),
        //define the text
        child: Center(
            child: Column(children: [
          //put the title of the card
          Padding(
              padding: const EdgeInsets.only(top: 5),
              child: Text("Remote worker",
                  style: TextStyle(
                      fontWeight: FontWeight.w400,
                      fontStyle: FontStyle.normal,
                      fontSize: 11,
                      color: Colors.white))),
          //put the worker id
          Padding(
              padding: const EdgeInsets.all(2.5),
              child: Text(
                  "CRC SHA of the worker id: ${widget.workerModel?.workerId}",
                  style: TextStyle(
                      fontWeight: FontWeight.w400,
                      fontStyle: FontStyle.normal,
                      fontSize: 10,
                      color: Colors.white)))
        ])));
  }

  /// Create the card bottom element
  Widget _createCardBottomElement() {
    //set the location info (assume that the value is unknown)
    String locationInfo = "Location unknown";

    //if the value is not null, set the properties
    final geo = widget.workerModel?.geolocation;
    if (geo != null) {
      locationInfo = "${geo.countryName} (${geo.countryCode}), "
          "${geo.regionName} (${geo.regionCode}), "
          "${geo.city}, ${geo.zipCode}";
    }

    //set the started at date time
    String startedAtDatetime = "unknown";
    if (widget.workerModel?.startedAt != null) {
      startedAtDatetime =
          DateFormat("MM-yyyy HH:mm").format(widget.workerModel.startedAt);
    }

    //create the child
    return Expanded(
        child: Padding(
            padding: const EdgeInsets.only(left: 8.0, right: 8.0, top: 8.0),
            //define the widget container
            child: Container(
                //add the decoration
                decoration: BoxDecoration(
                    color: Colors.transparent,
                    borderRadius: const BorderRadius.only(
                        bottomLeft: Radius.circular(30),
                        bottomRight: Radius.circular(30))),
                //set the child
                child: Column(
                    crossAxisAlignment: CrossAxisAlignment.center,
                    children: [
                      //put the location info
                      Expanded(
                          child: Row(children: [
                        Icon(Icons.add_location_alt_outlined,
                            color: Colors.blueGrey[500], size: 25),
                        //put the extra details
                        Expanded(
                            child: Padding(
                                padding: const EdgeInsets.only(left: 3),
                                child: Text(locationInfo,
                                    overflow: TextOverflow.ellipsis,
                                    style: TextStyle(
                                        fontWeight: FontWeight.w400,
                                        fontStyle: FontStyle.normal,
                                        fontSize: 11,
                                        color: Colors.blueGrey[300]),
                                    maxLines: 3)))
                      ])),
                      //put the searching jobs
                      Expanded(
                          child: Padding(
                              padding: const EdgeInsets.only(top: 5),
                              child: Align(
                                  alignment: Alignment.centerLeft,
                                  child: Row(children: [
                                    Icon(Icons.person_search_outlined,
                                        color: Colors.blueGrey[500], size: 25),
                                    //put the extra details
                                    Expanded(
                                        child: Padding(
                                            padding:
                                                const EdgeInsets.only(left: 3),
                                            child: RichText(
                                                maxLines: 2,
                                                text: TextSpan(
                                                    // Note: Styles for TextSpans must be explicitly defined.
                                                    // Child text spans will inherit styles from parent
                                                    style: TextStyle(
                                                        fontWeight:
                                                            FontWeight.w400,
                                                        fontStyle:
                                                            FontStyle.normal,
                                                        fontSize: 11,
                                                        color: Colors
                                                            .blueGrey[300]),
                                                    children: <TextSpan>[
                                                      //put with bold the number of jobs
                                                      TextSpan(
                                                          text: widget
                                                                  .workerModel
                                                                  ?.activeSearchingJobs
                                                                  ?.toString() ??
                                                              "unknown",
                                                          style: TextStyle(
                                                              fontWeight:
                                                                  FontWeight
                                                                      .bold)),
                                                      //put the rest of the text
                                                      TextSpan(
                                                          text:
                                                              " is the number of active searching jobs")
                                                    ]))))
                                  ])))),
                      //add the bottom right value
                      Expanded(
                          child: Padding(
                              padding: const EdgeInsets.only(top: 8.0),
                              child: Row(children: [
                                Expanded(
                                    child: Column(children: [
                                  Text("STARTED AT",
                                      style: TextStyle(
                                          fontWeight: FontWeight.bold,
                                          fontStyle: FontStyle.normal,
                                          fontSize: 11,
                                          color: Colors.blueGrey[300])),
                                  Center(
                                      child: Padding(
                                          padding: const EdgeInsets.all(2),
                                          child: Text(startedAtDatetime,
                                              style: TextStyle(
                                                  fontWeight: FontWeight.w400,
                                                  fontStyle: FontStyle.normal,
                                                  fontSize: 9,
                                                  color:
                                                      Colors.blueGrey[300]))))
                                ])),
                                Expanded(
                                    child: Column(children: [
                                  Text("RUNNING",
                                      style: TextStyle(
                                          fontWeight: FontWeight.bold,
                                          fontStyle: FontStyle.normal,
                                          fontSize: 11,
                                          color: Colors.blueGrey[300])),
                                  Padding(
                                      padding: const EdgeInsets.all(2),
                                      child: Center(
                                          child: Text(
                                              widget.workerModel?.runningFor ??
                                                  "unknown",
                                              style: TextStyle(
                                                  fontWeight: FontWeight.w400,
                                                  fontStyle: FontStyle.normal,
                                                  fontSize: 9,
                                                  color:
                                                      Colors.blueGrey[300]))))
                                ]))
                              ])))
                    ]))));
  }

  /// Used to handle the card click event
  void _onCardClickedAsync(BuildContext context) async {
    //open the modal
    await Modal.showDialogWithNoActionsAsync(context,
        title: Text("Your active search requests on selected worker",
            style: TextStyle(color: Colors.blueGrey[600])),
        content: ActiveSearchRequests(
            activeSearchRequestModels:
                widget.workerModel?.activeSearchRequests ?? []));
  }
}
