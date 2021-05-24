import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';
import 'package:gods_eye_app/services/models/person_found/model.dart';
import 'package:gods_eye_app/utils/components/collapsibile_application_bar/component.dart';

import 'search_result_detail/component.dart';

class PersonSearchRequestDetails extends StatefulWidget {
  //declare the fields that are required for this component
  final String userToken;
  final Image searchRequestImage;
  final List<PersonFoundMessageModel> personFoundResponses;
  final Object heroTag;

  const PersonSearchRequestDetails(
      {Key key,
      @required this.userToken,
      @required this.personFoundResponses,
      @required this.searchRequestImage,
      @required this.heroTag})
      : super(key: key);

  @override
  _StatePersonSearchRequestDetailsState createState() =>
      _StatePersonSearchRequestDetailsState();
}

class _StatePersonSearchRequestDetailsState
    extends State<PersonSearchRequestDetails> {
  final List<PersonFoundMessageModel> _responses = [];

  @override
  void initState() {
    super.initState();

    for (var a in widget.personFoundResponses) {
      _responses.add(a);
      _responses.add(a);
      _responses.add(a);
      _responses.add(a);
      _responses.add(a);
      _responses.add(a);
      _responses.add(a);
    }

    setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    //create the scaffold
    return Scaffold(
        body: CollapsableApplicationBar(
            title: "Search results",
            appBarSize: 120,
            background: Hero(
                tag: widget.heroTag,
                child: ClipRRect(
                    borderRadius: BorderRadius.all(Radius.circular(30)),
                    child: widget.searchRequestImage)),
            children: [_buildGridWidget(context)]));
  }

  /// The [context] represents the build context
  Widget _buildGridWidget(BuildContext context) {
    //create a list of response ids
    final availableResponses = _responses ?? [];
    //return the gridview
    return SliverToBoxAdapter(
        //create the grid
        child: Padding(
            padding: const EdgeInsets.only(left: 10, right: 10),
            child: GridView.builder(
                //clip the items if required
                clipBehavior: Clip.none,
                primary: false,
                shrinkWrap: true,
                // the number of items from grid is equal with the number of items from list
                itemCount: availableResponses.length,
                //the items are instances of remote workers
                itemBuilder: (BuildContext context, int index) =>
                    SearchResultDetail(
                      foundPersonInfo: availableResponses[index],
                    ),
                //specifies the grid alignment
                gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
                    crossAxisCount: 2,
                    crossAxisSpacing: 10,
                    mainAxisSpacing: 10))));
  }
}
