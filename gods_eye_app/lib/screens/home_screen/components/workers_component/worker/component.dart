import 'dart:convert';

import 'package:crclib/catalog.dart';
import 'package:intl/intl.dart';
import 'package:flutter/material.dart';

class RemoteWorker extends StatefulWidget {
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
          child: _createCardElements(),
        )),
      ),
    );
  }

  ///Create the card element
  Widget _createCardElements() {
    //compute the worker id
    final workerId = Crc16X().convert(utf8.encode('dsdsdsds')).toString();
    //return the column
    return Column(
      crossAxisAlignment: CrossAxisAlignment.center,
      children: [
        //define the top part of the card
        Container(
          height: 35,
          decoration: BoxDecoration(
              color: Colors.blueGrey[400],
              borderRadius: const BorderRadius.only(
                  topLeft: Radius.circular(30), topRight: Radius.circular(30))),
          //define the text
          child: Center(
            child: Column(
              children: [
                //put the title of the card
                Padding(
                  padding: const EdgeInsets.only(top: 5),
                  child: Text("Remote worker",
                      style: TextStyle(
                          fontWeight: FontWeight.w400,
                          fontStyle: FontStyle.normal,
                          fontSize: 11,
                          color: Colors.white)),
                ),
                //put the worker id
                Padding(
                  padding: const EdgeInsets.all(2.5),
                  child: Text("CRC SHA of the worker id: $workerId",
                      style: TextStyle(
                          fontWeight: FontWeight.w400,
                          fontStyle: FontStyle.normal,
                          fontSize: 10,
                          color: Colors.white)),
                ),
              ],
            ),
          ),
        ),
        //define the bottom part of the card
        _createCardBottomElement()
      ],
    );
  }

  ///Create the card bottom element
  Widget _createCardBottomElement() {
    final runningFor = "02:30 h";

    final activeSearchingJobs = "0";

    final locationInfo = "Romania (RO), Cluj (CJ), Cluj-Napoca, 400001";

    var time = DateTime.now();
    final startedAtDatetime = DateFormat("dd-MM-yyyy HH:mm").format(time);

    //create the child
    return Expanded(
        child: Padding(
            padding: const EdgeInsets.all(8.0),
            //define the widget container
            child: Container(
                //add the decoration
                decoration: BoxDecoration(
                    color: Colors.transparent,
                    borderRadius: const BorderRadius.only(
                        bottomLeft: Radius.circular(30),
                        bottomRight: Radius.circular(30))),
                //set the child
                child: Center(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.center,
                    children: [
                      //put the location info
                      Align(
                          alignment: Alignment.centerLeft,
                          child: Row(
                            children: [
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
                                      maxLines: 3),
                                ),
                              ),
                            ],
                          )),
                      //put the searching jobs
                      Padding(
                        padding: const EdgeInsets.only(top: 5),
                        child: Align(
                            alignment: Alignment.centerLeft,
                            child: Row(
                              children: [
                                Icon(Icons.person_search_outlined,
                                    color: Colors.blueGrey[500], size: 25),
                                //put the extra details
                                Expanded(
                                  child: Padding(
                                      padding: const EdgeInsets.only(left: 3),
                                      child: RichText(
                                        maxLines: 2,
                                        text: TextSpan(
                                          // Note: Styles for TextSpans must be explicitly defined.
                                          // Child text spans will inherit styles from parent
                                          style: TextStyle(
                                              fontWeight: FontWeight.w400,
                                              fontStyle: FontStyle.normal,
                                              fontSize: 11,
                                              color: Colors.blueGrey[300]),
                                          children: <TextSpan>[
                                            //put with bold the number of jobs
                                            TextSpan(
                                                text: activeSearchingJobs,
                                                style: TextStyle(
                                                    fontWeight:
                                                        FontWeight.bold)),
                                            //put the rest of the text
                                            TextSpan(
                                                text:
                                                    " is the number of active searching jobs")
                                          ],
                                        ),
                                      )),
                                ),
                              ],
                            )),
                      ),
                      //add the bottom right value
                      Padding(
                        padding: const EdgeInsets.only(top: 10),
                        child: Align(
                            alignment: Alignment.centerLeft,
                            child: Row(
                              children: [
                                Column(
                                  children: [
                                    Align(
                                        alignment: Alignment.centerRight,
                                        child: Text("STARTED AT",
                                            style: TextStyle(
                                                fontWeight: FontWeight.bold,
                                                fontStyle: FontStyle.normal,
                                                fontSize: 11,
                                                color: Colors.blueGrey[300]))),
                                    Align(
                                        alignment: Alignment.center,
                                        child: Text(startedAtDatetime,
                                            style: TextStyle(
                                                fontWeight: FontWeight.w400,
                                                fontStyle: FontStyle.normal,
                                                fontSize: 11,
                                                color: Colors.blueGrey[300])))
                                  ],
                                ),
                                Padding(
                                  padding: const EdgeInsets.only(left: 5),
                                  child: Column(
                                    children: [
                                      Align(
                                          alignment: Alignment.centerRight,
                                          child: Text("RUNNING",
                                              style: TextStyle(
                                                  fontWeight: FontWeight.bold,
                                                  fontStyle: FontStyle.normal,
                                                  fontSize: 11,
                                                  color:
                                                      Colors.blueGrey[300]))),
                                      Align(
                                          alignment: Alignment.center,
                                          child: Text(runningFor,
                                              style: TextStyle(
                                                  fontWeight: FontWeight.w400,
                                                  fontStyle: FontStyle.normal,
                                                  fontSize: 11,
                                                  color: Colors.blueGrey[300])))
                                    ],
                                  ),
                                ),
                              ],
                            )),
                      )
                    ],
                  ),
                ))));
  }
}
