import 'dart:convert';

import 'package:auto_animated/auto_animated.dart';
import 'package:flutter/material.dart';
import 'package:gods_eye_app/screens/home_screen/common/active_search_request/component.dart';
import 'package:gods_eye_app/screens/home_screen/components/workers_component/active_search_requests/component.dart';
import 'package:gods_eye_app/services/models/active_search_request/model.dart';
import 'package:gods_eye_app/utils/components/loader/component.dart';
import 'package:gods_eye_app/utils/components/top_corner_button/component.dart';
import 'package:gods_eye_app/utils/helpers/conversion/image/convertor.dart';
import 'package:intl/intl.dart';
import 'dart:io' as Io;

class PersonSearchScreen extends StatefulWidget {
  final String userToken;

  const PersonSearchScreen({this.userToken});

  @override
  State<StatefulWidget> createState() => _PersonSearchScreenState();
}

class _PersonSearchScreenState extends State<PersonSearchScreen> {
  //the request list
  List<ActiveSearchRequestModel> _activeSearchRequests = [
    ActiveSearchRequestModel(
      startedAt:
          DateFormat("dd-MM-yyyy HH:mm:ss").format(DateTime.now().toUtc()),
    )
  ];

  @override
  void initState() {
    //call the super init state logic
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Stack(clipBehavior: Clip.none, children: [
      //define the column with grid
      Column(children: [
        SizedBox(height: 10),
        //expand the widget
        Expanded(
            child: Padding(
                //set the padding
                padding: const EdgeInsets.only(left: 25, right: 25),
                //create the centered item
                child: Center(
                    //if display the loader only if the list is empty
                    child: CircularSpinningLoader(
                        //if the list is empty, display the spinner
                        displayLoaderIf: () => _activeSearchRequests.isEmpty,
                        //otherwise display the grid
                        elseDisplay: GridView.builder(
                            clipBehavior: Clip.none,
                            // the number of items from grid is equal with the number of items from list
                            itemCount: _activeSearchRequests.length,
                            //the items are instances of remote workers
                            itemBuilder: (BuildContext context, int index) =>
                                //create the active search request instance
                                ActiveSearchRequest(
                                    //set the data model
                                    activeSearchRequestModel:
                                        _activeSearchRequests[index],
                                    //set the font size
                                    fontSize: 14,
                                    //set the max value of the opacity
                                    opacityValue: 1,
                                    //set the widget that will appear on top
                                    onTopWidget: TopCornerButton(
                                        onTap: () => _deleteButtonClicked(
                                            index, context))),
                            //specifies the grid alignment
                            gridDelegate:
                                SliverGridDelegateWithFixedCrossAxisCount(
                                    crossAxisCount: 2,
                                    crossAxisSpacing: 10,
                                    mainAxisSpacing: 10))))))
      ]),
      //create the bottom button
      _createBottomButton(context)
    ]);
  }

  /// Create the bottom button
  Widget _createBottomButton(BuildContext context) {
    //position the button bottom right
    return Positioned(
        bottom: 40,
        right: 30,
        //wrap the button in a container
        child: Container(
            height: 60,
            width: 60,
            //set a little opacity
            child: Opacity(
                opacity: .95,
                //create the button
                child: ElevatedButton(
                    //set the on pressed function
                    onPressed: () => _addButtonClicked(context),
                    //set the button style
                    style: ElevatedButton.styleFrom(
                        primary: Colors.blueGrey[400],
                        shadowColor: Colors.blueGrey[700],
                        visualDensity: VisualDensity.comfortable,
                        shape: RoundedRectangleBorder(
                            borderRadius:
                                BorderRadius.all(Radius.circular(20))),
                        elevation: 20),
                    //put the icon
                    child: ImageIcon(AssetImage("new_search.png"))))));
  }


  /// Create the handler for item deletion
  void _deleteButtonClicked(int itemIndex, BuildContext context) {
    print(itemIndex);
  }

  /// Define the handler for adding a new search request
  void _addButtonClicked(BuildContext context) {}
}
