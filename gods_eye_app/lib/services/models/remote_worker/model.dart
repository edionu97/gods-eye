import 'dart:convert';
import 'dart:math';

import 'package:crclib/catalog.dart';
import 'package:gods_eye_app/services/models/geolocation/model.dart';
import 'package:intl/intl.dart';

class RemoteWorkerModel {
  //private field for worker id
  String _workerId;

  //started at property
  String _startedAt;

  //set the active searching jobs
  final int activeSearchingJobs;

  //create the geolocation
  final GeolocationModel geolocation;

  /// Constructor
  RemoteWorkerModel(
      {String workerId,
      this.geolocation,
      String startedAt,
      this.activeSearchingJobs}) {
    _startedAt = startedAt;
    _workerId = workerId;
  }

  String get workerId {
    //encode the worker id
    final encodedWorkerId = utf8.encode(_workerId);

    //return the crc worker id
    return Crc16X().convert(encodedWorkerId).toString();
  }

  DateTime get startedAt {
    return DateFormat("dd-MM-yyyy HH:mm:ss").parse(_startedAt, true).toLocal();
  }

  String get runningFor {
    //get the utc date
    final utcDate = DateFormat("dd-MM-yyyy HH:mm:ss").parse(_startedAt, true);

    //get the difference of dates in minutes
    final int minutes = DateTime.now().difference(utcDate.toLocal()).inMinutes;

    //get the hours
    final int hours = (minutes / 60).floor();

    //get the minutes
    final int min = minutes - hours * 60;

    //get the hours:and minutes
    return "${hours < 10 ? "0$hours" : hours}:${minutes < 10 ? "0$min" : min} h";
  }
}
