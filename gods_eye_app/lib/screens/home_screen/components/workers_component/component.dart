import 'package:flutter/material.dart';
import 'package:gods_eye_app/screens/home_screen/components/workers_component/worker/component.dart';
import 'package:gods_eye_app/services/models/active_search_request/model.dart';
import 'package:gods_eye_app/services/models/geolocation/model.dart';
import 'package:gods_eye_app/services/models/remote_worker/model.dart';
import 'package:gods_eye_app/utils/components/loader/component.dart';
import 'package:intl/intl.dart';

class RemoteWorkersScreen extends StatefulWidget {
  final String userToken;

  RemoteWorkersScreen(this.userToken);

  @override
  State<StatefulWidget> createState() => _RemoteWorkersScreenState();
}

class _RemoteWorkersScreenState extends State<RemoteWorkersScreen> {
  //the list with remote workers
  final List<RemoteWorker> _remoteWorkers = [
    RemoteWorker(
      workerModel: RemoteWorkerModel(
          workerId: "ana are mere",
          geolocation: GeolocationModel(
              countryName: "Romania",
              countryCode: "RO",
              regionName: "Cluj",
              regionCode: "CJ",
              city: "Cluj-Napoca",
              zipCode: "400001"),
          startedAt:
              DateFormat("dd-MM-yyyy HH:mm:ss").format(DateTime.now().toUtc()),
          activeSearchingJobs: 10,
          activeSearchRequests: [
            ActiveSearchRequestModel(
                startedAt: DateFormat("dd-MM-yyyy HH:mm:ss")
                    .format(DateTime.now().toUtc()))
          ]),
    ),
  ];

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        SizedBox(height: 15),
        Expanded(
          child: Padding(
              padding: const EdgeInsets.only(left: 15, right: 15),
              child: Center(
                //if display the loader only if the list is empty
                child: CircularSpinningLoader(
                    // if the list is empty display the loader
                    displayLoaderIf: () => _remoteWorkers.isEmpty,
                    //otherwise display the grid
                    elseDisplay: GridView.count(
                        crossAxisCount: 2,
                        crossAxisSpacing: 10,
                        mainAxisSpacing: 10,
                        children: _remoteWorkers)),
              )),
        ),
      ],
    );
  }
}
