import 'package:flutter/material.dart';
import 'package:gods_eye_app/screens/home_screen/components/workers_component/worker/component.dart';
import 'package:gods_eye_app/services/models/person_found/model.dart';
import 'package:gods_eye_app/services/models/remote_worker/model.dart';
import 'package:gods_eye_app/utils/components/animated_size_widget/component.dart';
import 'package:intl/intl.dart';

class SearchResultDetail extends StatefulWidget {
  final PersonFoundMessageModel foundPersonInfo;
  final Function removeNotificationBellAction;

  const SearchResultDetail(
      {Key key,
      @required this.foundPersonInfo,
      this.removeNotificationBellAction})
      : super(key: key);

  @override
  _SearchResultDetailState createState() => _SearchResultDetailState();
}

class _SearchResultDetailState extends State<SearchResultDetail>
    with SingleTickerProviderStateMixin {
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

    // adapted the remote worker component
    return RemoteWorker(
        bottomRightLabel: "FOUND AT",
        cardTitle: "Face match",
        middleWidget: Center(
          child: Text("Here"),
        ),
        bottomRightValue: foundAt,
        onCardClicked: () {
          widget.foundPersonInfo.isNewToUser =
              !widget.foundPersonInfo.isNewToUser;
          //call the parent callback
          widget.removeNotificationBellAction?.call();
          print("da");
        },
        workerModel: RemoteWorkerModel(
            workerHashId: widget.foundPersonInfo?.findByWorkerId,
            geolocation: widget.foundPersonInfo?.geoLocation,
            startedAt: startedAt));
  }
}
