import 'dart:async';

import 'package:flutter/material.dart';
import 'package:gods_eye_app/screens/home_screen/components/workers_component/worker/component.dart';
import 'package:gods_eye_app/services/facial_recognition/service.dart';
import 'package:gods_eye_app/services/messages/service.dart';
import 'package:gods_eye_app/services/models/active_search_request/model.dart';
import 'package:gods_eye_app/services/models/common/model.dart';
import 'package:gods_eye_app/services/models/geolocation/model.dart';
import 'package:gods_eye_app/services/models/remote_worker/model.dart';
import 'package:gods_eye_app/services/notifications/service.dart';
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
  final List<RemoteWorkerModel> _remoteWorkersData = [];

  final Map<int, List<RemoteWorkerModel>> _dataFromPingInterval = {};

  //create the ui update job timer
  Timer _uiUpdateJob;

  @override
  void initState() {
    //call the super initialization logic
    super.initState();

    //define the updating task
    _uiUpdateJob =
        Timer.periodic(const Duration(seconds: 5), (time) => _updateUi(time));

    //register the observer
    NotificationService().registerObserver(_onMessage);

    //do the initial interface update
    FacialRecognitionService().pingAllWorkersAsync(widget.userToken);
  }

  @override
  void dispose() {
    //stop the updating job
    _uiUpdateJob?.cancel();
    //unregister the observer
    NotificationService().unregisterObserver(_onMessage);
    //execute the dispose logic
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Column(children: [
      //put a space on top
      SizedBox(height: 15),
      //rest of the space is dedicated to the grid
      Expanded(
          child: Padding(
              padding: const EdgeInsets.only(left: 15, right: 15),
              child: Center(
                  //if display the loader only if the list is empty
                  child: CircularSpinningLoader(
                      //if the list is empty, display the spinner
                      displayLoaderIf: () => _remoteWorkersData.isEmpty,
                      //otherwise display the grid
                      elseDisplay: GridView.builder(
                          // the number of items from grid is equal with the number of items from list
                          itemCount: _remoteWorkersData.length,
                          //the items are instances of remote workers
                          itemBuilder: (BuildContext context, int index) =>
                              RemoteWorker(
                                  workerModel: _remoteWorkersData[index]),
                          //specifies the grid alignment
                          gridDelegate:
                              SliverGridDelegateWithFixedCrossAxisCount(
                                  crossAxisCount: 2,
                                  crossAxisSpacing: 10,
                                  mainAxisSpacing: 10))))))
    ]);
  }

  /// This method is responsible with the ui update
  void _updateUi(Timer time) async {
    //set the displayed data before ping started

    //if in previous state   we have 0 data => there are no available workers
    if (!_dataFromPingInterval.containsKey(time.tick - 1)) {
      //if the list is empty clear the state
      setState(() {
        _remoteWorkersData.clear();
      });
    } else {
      //sync the ui data with the received data
      _syncUiData(_dataFromPingInterval[time.tick - 1]);
      //remove the key (we are interested only in most recent two keys)
      _dataFromPingInterval.remove(time.tick - 1);
    }

    //send the ping request to all the workers
    await FacialRecognitionService().pingAllWorkersAsync(widget.userToken);
  }

  /// This function it is used when a new message is received from client
  void _onMessage(String message) {
    //convert the json into an object
    IAbstractModel convertedObject =
        MessageParsingService().parseModelFromJson(message);

    //check if the object is the right instance
    if (!(convertedObject is RemoteWorkerModel)) {
      return;
    }

    //convert the abstract worker in specific instance
    var remoteWorkerModel = convertedObject as RemoteWorkerModel;

    //get the sync tick
    final int syncTick = _uiUpdateJob?.tick ?? 0;

    //ensure that the key is in dictionary
    if (!_dataFromPingInterval.containsKey(syncTick)) {
      _dataFromPingInterval[syncTick] = [];
    }

    //set the data from the ping interval
    _dataFromPingInterval[syncTick].add(remoteWorkerModel);

    //set the state
    setState(() {
      //iterate the items from the list and try to find that item that has
      //the same hash id with the new model
      for (var index = 0; index < _remoteWorkersData.length; ++index) {
        //search for worker in the list
        if (_remoteWorkersData[index].workerHashId !=
            remoteWorkerModel.workerHashId) {
          continue;
        }

        //set the updated values
        _remoteWorkersData[index] = remoteWorkerModel;
        return;
      }

      //if the element is not in  the list
      _remoteWorkersData.add(remoteWorkerModel);
    });
  }

  /// This function it is used  for ui / workers sync
  void _syncUiData(final List<RemoteWorkerModel> receivedPingList) {
    //convert the list in a dictionary
    final Map<String, RemoteWorkerModel> onlineWorkers = Map.fromIterable(
        receivedPingList,
        key: (x) => (x as RemoteWorkerModel)?.workerHashId);

    //convert the displayed data into a dictionary
    final Map<String, RemoteWorkerModel> displayedData = Map.fromIterable(
        _remoteWorkersData,
        key: (x) => (x as RemoteWorkerModel)?.workerHashId);

    //update the state
    setState(() {
      //remove those workers that are no longer online
      _remoteWorkersData
          .removeWhere((item) => !onlineWorkers.containsKey(item.workerHashId));

      //sync the data of the available workers
      for (var index = 0; index < _remoteWorkersData.length; ++index) {
        //update
        _remoteWorkersData[index] =
            onlineWorkers[_remoteWorkersData[index].workerHashId];
      }

      //add the new elements
      for (final element in receivedPingList) {
        //do not add duplicates
        if (displayedData.containsKey(element.workerHashId)) {
          continue;
        }
        //add the new elements
        _remoteWorkersData.add(element);
      }
    });
  }
}
