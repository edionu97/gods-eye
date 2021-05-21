import 'dart:convert';

import 'package:crclib/catalog.dart';
import 'package:gods_eye_app/services/models/active_search_request/model.dart';
import 'package:gods_eye_app/services/models/common/model.dart';
import 'package:gods_eye_app/services/models/geolocation/model.dart';
import 'package:intl/intl.dart';

class RemoteWorkerModel implements IAbstractModel {
  //started at property
  String _startedAt;

  //worker hash
  final String workerHashId;

  //set the active searching jobs
  final int activeSearchingJobs;

  //create the geolocation
  final GeolocationModel geolocation;

  //get the active search requests
  final List<ActiveSearchRequestModel> activeSearchRequests;

  /// Convert the json into dart object
  static RemoteWorkerModel convertFromJson(final dynamic jsonObject) {
    //create the requests
    final List<ActiveSearchRequestModel> activeRequests = [];

    //set the date value
    String dateValue;
    try {
      dateValue = DateFormat("dd-MM-yyyy HH:mm:ss")
          .format(DateTime.parse(jsonObject["StartedAt"]));
    } on Exception {
      //empty
    }

    //if the information about the active search requests
    if (jsonObject["WorkerInfo"]["RunningJobs"] != null) {
      //iterate each active search
      for (var activeSearchRequestJson in jsonObject["WorkerInfo"]
          ["RunningJobs"]) {
        //convert the json into the dart object
        activeRequests.add(
            ActiveSearchRequestModel.convertFromJson(activeSearchRequestJson));
      }
    }

    //create the remote worker model
    return RemoteWorkerModel(
        workerHashId: jsonObject["WorkerInfo"]["WorkerId"],
        geolocation:
            GeolocationModel.convertFromJson(jsonObject["GeoLocation"]),
        startedAt: dateValue,
        activeSearchingJobs: activeRequests.length,
        activeSearchRequests: activeRequests);
  }

  /// Constructor
  RemoteWorkerModel(
      {this.workerHashId,
      this.geolocation,
      String startedAt,
      this.activeSearchRequests,
      this.activeSearchingJobs}) {
    _startedAt = startedAt;
  }

  String get workerId {
    //check the string for null or empty
    if (workerHashId == null || workerHashId.isEmpty) {
      return null;
    }

    //encode the worker id
    final encodedWorkerId = utf8.encode(workerHashId);

    //return the crc worker id
    return Crc16X().convert(encodedWorkerId).toString();
  }

  DateTime get startedAt {
    //check the string for null or empty
    if (_startedAt == null || _startedAt.isEmpty) {
      return null;
    }

    return DateFormat("dd-MM-yyyy HH:mm:ss").parse(_startedAt, true).toLocal();
  }

  String get runningFor {
    //check the string for null or empty
    if (_startedAt == null || _startedAt.isEmpty) {
      return null;
    }

    //get the utc date
    final utcDate = DateFormat("dd-MM-yyyy HH:mm:ss").parse(_startedAt, true);

    //get the difference of dates in minutes
    final int minutes = DateTime.now().difference(utcDate.toLocal()).inMinutes;

    //get the hours
    final int hours = (minutes / 60).floor();

    //get the minutes
    final int min = minutes - hours * 60;

    //get the hours:and minutes
    return "${hours < 10 ? "0$hours" : hours}:${min < 10 ? "0$min" : min} h";
  }
}
