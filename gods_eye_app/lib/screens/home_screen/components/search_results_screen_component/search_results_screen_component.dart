import 'package:flutter/material.dart';
import 'package:gods_eye_app/persistence/search_responses/repo.dart';
import 'package:gods_eye_app/services/models/person_found/model.dart';
import 'package:gods_eye_app/utils/components/loader/component.dart';

import 'component/possible_search_result_component/possible_search_result_component.dart';

/// This component it is used on HomeScreen => Search Results
class SearchResultsScreenWidget extends StatefulWidget {
  // define the user token
  final String userToken;

  //construct the object
  const SearchResultsScreenWidget({this.userToken});

  @override
  State<StatefulWidget> createState() => _SearchResultsScreenWidgetState();
}

class _SearchResultsScreenWidgetState extends State<SearchResultsScreenWidget> {
  //define the person search repository
  final PersonSearchResponseRepository _personSearchResponseRepository =
      PersonSearchResponseRepository();

  //the list of search responses based on their type
  Map<String, List<PersonFoundMessageModel>> _searchIdToSearchResponses = {};

  @override
  void initState() {
    //initialize the state
    super.initState();

    //register the observer
    _personSearchResponseRepository.registerObserver(_onNewSearchResponse);

    //define an async method for initial populate
    () async {
      //set set the initial values
      _searchIdToSearchResponses = await _personSearchResponseRepository
          .groupItemsBasedOnResponseIdAsync();
      //set state
      setState(() {});
    }();
  }

  @override
  void dispose() {
    //unregister the observer
    _personSearchResponseRepository.unregisterObserver(_onNewSearchResponse);
    //dispose the object
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    //create a column
    return Column(children: [
      //put  a fixed margin
      SizedBox(height: 20),
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
                      displayLoaderIf: () =>
                          (_searchIdToSearchResponses ?? {}).isEmpty,
                      //otherwise display the grid
                      elseDisplay: _buildGridWidget(context)))))
    ]);
  }

  /// This function it is used for building the grid view
  /// The [context] represents the build context
  Widget _buildGridWidget(BuildContext context) {
    //create a list of response ids
    final List<String> responseIds =
        _searchIdToSearchResponses?.keys?.toList() ?? [];
    //return the gridview
    return GridView.builder(
        clipBehavior: Clip.none,
        // the number of items from grid is equal with the number of items from list
        itemCount: _searchIdToSearchResponses.keys.length,
        //the items are instances of remote workers
        itemBuilder: (BuildContext context, int index) {
          //get the responses from the current key index
          var responses = _searchIdToSearchResponses[responseIds[index]];
          //create the widget
          return PossibleSearchResultWidget(
              userToken: widget.userToken, responses: responses);
        },
        //specifies the grid alignment
        gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
            crossAxisCount: 2, crossAxisSpacing: 10, mainAxisSpacing: 10));
  }

  /// This method is called when a new instance is added into the list
  void _onNewSearchResponse(PersonFoundMessageModel message) async {
    //set the state
    setState(() {
      //ensure that the key is in dictionary
      if (!_searchIdToSearchResponses.containsKey(message.responseId)) {
        _searchIdToSearchResponses[message.responseId] = [];
      }
      //add the message in it's category
      _searchIdToSearchResponses[message.responseId].add(message);
    });
  }
}
