import 'package:flutter/material.dart';
import 'package:gods_eye_app/screens/home_screen/common/active_search_request_component/active_search_request_component.dart';
import 'package:gods_eye_app/services/models/active_search_request/model.dart';
import 'package:gods_eye_app/utils/components/loader/component.dart';

/// This component it is displayed when the user clicks on one remote worker
/// that is displayed in the HomeScreen -> Network
class WorkerActiveRequestsWidget extends StatefulWidget {
  //get the list
  final List<ActiveSearchRequestModel> activeSearchRequestModels;

  //set the value of the list
  const WorkerActiveRequestsWidget(
      {Key key, @required this.activeSearchRequestModels})
      : super(key: key);

  @override
  State<StatefulWidget> createState() => _WorkerActiveRequestsWidgetState();
}

class _WorkerActiveRequestsWidgetState extends State<WorkerActiveRequestsWidget> {
  //the request list
  List<ActiveSearchRequestWidget> requests;

  @override
  void initState() {
    //call the super init state logic
    super.initState();

    //iterate the models and populate the requests
    requests = [];
    for (var model in widget.activeSearchRequestModels ?? []) {
      requests.add(ActiveSearchRequestWidget(activeSearchRequestModel: model));
    }

    //set the state
    setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    //set the message
    final message =
        "You have ${requests.length} request${requests.length > 1 ? "s" : ""} "
        "running on this worker. "
        "${requests.isEmpty ? "Waiting for you to create new requests" : ""}";

    //create the container
    return Container(
      height: 250,
      color: Colors.transparent,
      //put the grid on center
      child: Column(
        children: [
          SizedBox(
            height: 10,
          ),
          Expanded(
              child: Center(
            child: CircularSpinningLoader(
                displayLoaderIf: () => requests.isEmpty,
                elseDisplay: GridView.count(
                    crossAxisCount: 2,
                    crossAxisSpacing: 10,
                    mainAxisSpacing: 10,
                    clipBehavior: Clip.antiAliasWithSaveLayer,
                    children: requests)),
          )),
          Padding(
              padding: const EdgeInsets.only(top: 10.0),
              child: Text(message,
                  style: TextStyle(
                      fontWeight: FontWeight.w500,
                      fontStyle: FontStyle.normal,
                      fontSize: 11,
                      color: Colors.blueGrey[600])))
        ],
      ),
    );
  }
}
