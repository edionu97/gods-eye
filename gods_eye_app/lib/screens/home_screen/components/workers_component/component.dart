import 'dart:async';

import 'package:flutter/material.dart';
import 'package:gods_eye_app/screens/home_screen/components/workers_component/worker/component.dart';
import 'package:gods_eye_app/utils/components/decoration_form/component.dart';

class RemoteWorkersScreen extends StatefulWidget {
  final String userToken;

  RemoteWorkersScreen(this.userToken);

  @override
  State<StatefulWidget> createState() => _RemoteWorkersScreenState();
}

class _RemoteWorkersScreenState extends State<RemoteWorkersScreen> {

  final List<RemoteWorker> _remoteWorkers = [
    RemoteWorker(),
    RemoteWorker(),
    RemoteWorker(),
    RemoteWorker(),
    RemoteWorker(),
    RemoteWorker(),
    RemoteWorker(),
  ];

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        SizedBox(height: 15),
        Expanded(
          child: Padding(
              padding: const EdgeInsets.only(left: 15, right: 15),
              child: GridView.count(
                  crossAxisCount: 2,
                  crossAxisSpacing: 10,
                  mainAxisSpacing: 10,
                  children: _remoteWorkers)),
        ),
      ],
    );
  }
}
