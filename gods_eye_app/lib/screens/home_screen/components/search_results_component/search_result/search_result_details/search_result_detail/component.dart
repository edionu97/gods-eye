import 'package:flutter/material.dart';
import 'package:gods_eye_app/screens/home_screen/components/search_results_component/search_result/search_result_details/search_result_face_match_details/component.dart';
import 'package:gods_eye_app/screens/home_screen/components/workers_component/worker/component.dart';
import 'package:gods_eye_app/services/models/person_found/model.dart';
import 'package:gods_eye_app/services/models/remote_worker/model.dart';
import 'package:intl/intl.dart';

class SearchResultDetail extends StatefulWidget {
  final String userToken;
  final PersonFoundMessageModel foundPersonInfo;
  final Function removeNotificationBellAction;

  const SearchResultDetail(
      {Key key,
      @required this.foundPersonInfo,
      this.removeNotificationBellAction,
      this.userToken})
      : super(key: key);

  @override
  _SearchResultDetailState createState() => _SearchResultDetailState();
}

class _SearchResultDetailState extends State<SearchResultDetail>
    with SingleTickerProviderStateMixin {
  //define the animation controller
  AnimationController _animationController;

  final UniqueKey _heroKey = UniqueKey();

  @override
  void initState() {
    //execute the super logic
    super.initState();

    //create the animation controller
    _animationController = AnimationController(
        vsync: this, duration: const Duration(milliseconds: 1000));

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
    //get the start at date
    String startedAt;
    if (widget.foundPersonInfo?.startedAt != null) {
      startedAt = DateFormat("dd-MM-yyyy HH:mm:ss")
          .format(widget.foundPersonInfo?.startedAt?.toUtc());
    }

    //get the found at date
    String foundAt;
    if (widget.foundPersonInfo?.foundAt != null) {
      foundAt =
          DateFormat("MM-yyyy HH:mm").format(widget.foundPersonInfo?.foundAt);
    }

    final RemoteWorkerModel remoteWorkerModel = RemoteWorkerModel(
        workerHashId: widget.foundPersonInfo?.findByWorkerId,
        geolocation: widget.foundPersonInfo?.geoLocation,
        startedAt: startedAt);

    //wrap it in a hero
    return RemoteWorker(
        bottomRightLabel: "FOUND AT",
        cardTitle: "Face match",
        middleWidget: _createMiddleWidget(),
        bottomRightValue: foundAt,
        onCardClicked: () {
          widget.foundPersonInfo.isNewToUser = false;
          //call the parent callback
          widget.removeNotificationBellAction?.call();
          //handle the click event
          _onFaceMatchClicked(context, remoteWorkerModel);
        },
        workerModel: remoteWorkerModel);
  }

  /// Create the middle of the card item
  Widget _createMiddleWidget() {
    //align items in center left
    return Align(
        alignment: Alignment.centerLeft,
        //put an icon and a text in a row
        child: Row(children: [
          Hero(
            tag: _heroKey,
            child: Material(
              elevation: 0,
              color: Colors.transparent,
              child: Padding(
                padding: const EdgeInsets.only(left: 1.0),
                child: Icon(Icons.visibility_outlined,
                    color: Colors.blueGrey[500], size: 24),
              ),
            ),
          ),
          //make the text to grow to occupy all the free space
          Expanded(
            child: Padding(
                padding: const EdgeInsets.only(left: 3),
                child: Text("Click anywhere for seeing the face result",
                      style: TextStyle(
                          fontWeight: FontWeight.w400,
                          fontStyle: FontStyle.normal,
                          fontSize: 11,
                          color: Colors.blueGrey[300])),
              ),
          )
        ]));
  }

  void _onFaceMatchClicked(BuildContext context, RemoteWorkerModel remoteWorkerModel) {
    //push the new page (the details page)
    Navigator.of(context).push(
      //create a new page route builder
        PageRouteBuilder(
          //set the transition duration
            transitionDuration: Duration(milliseconds: 1500),
            //create the page
            pageBuilder: (_, __, ___) => FaceMatchDetails(
              userToken: widget.userToken,
              foundPersonDetails: widget.foundPersonInfo,
              heroKey: _heroKey,
            ),
            //set the transition builder
            transitionsBuilder: (_, animation, __, child) => Align(
              child: FadeTransition(opacity: animation, child: child),
            )));
  }
}
