import 'package:flutter/material.dart';

class RemoteWorkersScreen extends StatefulWidget {
  final String userToken;

  RemoteWorkersScreen(this.userToken);

  @override
  State<StatefulWidget> createState() => _RemoteWorkersScreenState();
}

class _RemoteWorkersScreenState extends State<RemoteWorkersScreen> {
  @override
  Widget build(BuildContext context) {
    return Container(
      height: MediaQuery.of(context).size.height,
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Center(
            child: Text(
              "Home Screen",
              style: TextStyle(
                  fontWeight: FontWeight.w300,
                  fontStyle: FontStyle.normal,
                  fontSize: 25,
                  color: Colors.black),
            ),
          )
        ],
      ),
    );
  }
}
