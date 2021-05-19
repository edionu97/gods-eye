import 'package:flutter/material.dart';
import 'package:gods_eye_app/screens/home_screen/components/workers_component/activer_search_requests/active_search_request/component.dart';

class ActiveSearchRequests extends StatefulWidget {
  @override
  State<StatefulWidget> createState() => ActiveSearchRequestsState();
}

class ActiveSearchRequestsState extends State<ActiveSearchRequests> {
  final List<ActiveSearchRequest> _activeSearchRequests = [
    ActiveSearchRequest(),
  ];

  @override
  Widget build(BuildContext context) {
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
            child: GridView.count(
                crossAxisCount: 2,
                crossAxisSpacing: 10,
                mainAxisSpacing: 10,
                clipBehavior: Clip.antiAliasWithSaveLayer,
                children: _activeSearchRequests),
          ),
          Padding(
              padding: const EdgeInsets.only(top: 10.0),
              child: Text(
                  "You have ${_activeSearchRequests.length} request${_activeSearchRequests.length > 1 ? "s" : ""} running on this worker",
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
