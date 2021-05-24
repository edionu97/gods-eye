import 'dart:async';

import 'package:flutter/material.dart';
import 'package:gods_eye_app/services/facial_recognition/service.dart';
import 'package:gods_eye_app/services/messages/service.dart';
import 'package:gods_eye_app/services/models/active_search_request/model.dart';
import 'package:gods_eye_app/services/models/common/model.dart';
import 'package:gods_eye_app/services/models/person_found/model.dart';
import 'package:gods_eye_app/services/models/remote_worker/model.dart';
import 'package:gods_eye_app/services/notifications/service.dart';
import 'package:gods_eye_app/utils/components/animated_opacity_widget/component.dart';
import 'package:gods_eye_app/utils/components/loader/component.dart';

class SearchRequest extends StatefulWidget {
  //declare the fields that are required for this component
  final String userToken;
  final List<PersonFoundMessageModel> responses;

  //set the values
  const SearchRequest({Key key, this.userToken, this.responses})
      : super(key: key);

  @override
  _SearchRequestState createState() => _SearchRequestState();
}

class _SearchRequestState extends State<SearchRequest>
    with TickerProviderStateMixin {
  //define the animation controller
  AnimationController _animationController;
  Animation<double> _animation;

  bool _hasNewNotification = false;
  bool _isStillSearching = true;

  //create the ui update job timer
  Timer _notificationChecker;

  final Map<String, bool> _jobSummary = {};

  @override
  void initState() {
    //execute the super logic
    super.initState();

    //create the animation controller
    _animationController =
        AnimationController(vsync: this, duration: const Duration(seconds: 1));

    //create the opacity controller
    _animation = Tween<double>(begin: 0, end: 1).animate(_animationController);

    //start the animation
    _animationController.forward();

    //start the checking loop
    _notificationChecker =
        Timer.periodic(const Duration(milliseconds: 300), (t) {
      //get the response list
      final List<PersonFoundMessageModel> list = widget.responses ?? [];

      //the notification is new only if the values are not yet seen by user
      setState(() {
        _hasNewNotification = list.any((element) => element.isNewToUser);
      });
    });

    //register the observer
    NotificationService().registerObserver(_onNotification);

    //do the initial interface update
    FacialRecognitionService().pingAllWorkersAsync(widget.userToken);
  }

  @override
  void dispose() {
    //unregister the observer
    NotificationService().unregisterObserver(_onNotification);
    //cancel the timer
    _notificationChecker?.cancel();
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
        child: _createCard(context));
  }

  /// Create the card on the given build context
  Widget _createCard(BuildContext context) {
    //declare the border radius
    final BorderRadius borderRadius = BorderRadius.circular(20);
    //get the image
    var image = widget.responses?.first?.searchedPersonImage ??
        Image.asset("unknown.png", fit: BoxFit.fill);

    //children
    final List<Widget> children = [
      //make the card clickable
      InkWell(
        //set a custom border
        customBorder: RoundedRectangleBorder(borderRadius: borderRadius),
        //set the handler for on card clicked
        onTap: () => _onCardClicked(context),
        //create the card
        child: Card(
            //set the card border
            shape: RoundedRectangleBorder(borderRadius: borderRadius),
            //put a shadow
            shadowColor: Colors.blueGrey[600],
            //set the elevation
            elevation: 10,
            //create the elements of the card
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Expanded(
                    child: ClipRRect(borderRadius: borderRadius, child: image))
              ],
            )),
      ),
      //create the top right badge
      _hasNewNotification
          ? _createTopRightNotificationBadge(borderRadius)
          : null,

      //put a progress bar in center or none otherwise
      Center(
          child: CircularSpinningLoader(
              spinningDotsColor: Colors.white24,
              centerDotColor: Colors.transparent,
              displayLoaderIf: () => _isStillSearching,
              elseDisplay: Container())),
    ];

    //create the stack
    return Stack(
        clipBehavior: Clip.none,
        children: children.where((element) => element != null).toList());
  }

  /// This function it is used for creating the top button
  /// The border radius defines the shape of the top button
  Widget _createTopRightNotificationBadge(BorderRadius borderRadius) {
    return //define the top bell icon
        Positioned(
            right: -11,
            top: -11,
            // create the container
            child: AnimatedOpacityWidget(
              duration: const Duration(milliseconds: 1500),
              //if start visible
              widget: Card(
                  //set the elevation
                  elevation: 2,
                  //create the white color
                  color: Colors.white,
                  //set the shape
                  shape: RoundedRectangleBorder(borderRadius: borderRadius),
                  //create container
                  child: Container(
                      width: 25,
                      height: 25,
                      //set the box decoration
                      decoration: BoxDecoration(borderRadius: borderRadius),
                      //position the icon in center
                      child: Center(
                          //create a new container
                          child: Container(
                              width: 22,
                              height: 22,
                              //create the same border
                              decoration: BoxDecoration(
                                  borderRadius: borderRadius,
                                  color: Colors.white10),
                              //in center place the image
                              child: Center(
                                  child: ImageIcon(
                                      AssetImage("assets/bell.png"),
                                      color: Colors.blueGrey[600],
                                      size: 17)))))),
            ));
  }

  /// Handle the card clicked event
  void _onCardClicked(BuildContext context) {
    //clear the notification
    setState(() {
      //clear the notification
      //widget.responses?.forEach((element) => element.isNewToUser = false);
    });
  }

  /// Change the status of the spinning bar
  void _onNotification(final String message) {
    //convert the json into an object
    IAbstractModel convertedObject =
        MessageParsingService().parseModelFromJson(message);

    //check if the object is the right instance
    if (convertedObject is! RemoteWorkerModel) {
      return;
    }

    //convert the worker back
    final RemoteWorkerModel remoteWorkerDetails =
        convertedObject as RemoteWorkerModel;

    //get the active search requests for the model
    final List<ActiveSearchRequestModel> requestsFromWorker =
        remoteWorkerDetails?.activeSearchRequests ?? [];

    //get the response id
    final String responseId = widget.responses?.first?.responseId;

    //check if the worker is active on this request
    final bool isSearchingActiveOnWorkerThisWorker = requestsFromWorker
        .any((element) => element.searchRequestHashId == responseId);

    //set the values
    _jobSummary[remoteWorkerDetails.workerId] =
        isSearchingActiveOnWorkerThisWorker;

    //set the state to true only if we have this search req active on one worker
    setState(() {
      _isStillSearching = _jobSummary.values.any((x) => x);
    });
  }
}
