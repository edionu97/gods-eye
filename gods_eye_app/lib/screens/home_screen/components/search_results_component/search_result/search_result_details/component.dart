import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';
import 'package:gods_eye_app/services/models/person_found/model.dart';
import 'package:gods_eye_app/utils/components/animated_opacity_widget/component.dart';
import 'package:gods_eye_app/utils/components/collapsibile_application_bar/component.dart';
import 'package:gods_eye_app/utils/components/loader/component.dart';

import 'search_result_detail/component.dart';

class PersonSearchRequestDetails extends StatefulWidget {
  //declare the fields that are required for this component
  final String userToken;
  final Image searchRequestImage;
  final Object heroTag;
  final List<PersonFoundMessageModel> personFoundResponses;

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
  int _currentSelectedItemIdx = 0;

  bool _isHideMode = false;

  @override
  void initState() {
    super.initState();

    //call async method for sorting the values
    (() async {
      //sort the responses by date descending
      setState(() {
        widget.personFoundResponses.sort((x, y) {
          //null dates are in front
          if (x.foundAt == null || y.foundAt == null) {
            return -1;
          }
          //compare dates descending
          return y.foundAt.compareTo(x.foundAt);
        });
      });
    })();
  }

  @override
  Widget build(BuildContext context) {
    //create the scaffold
    return Scaffold(
        bottomNavigationBar: _createNavigationBar(),
        body: Center(
          child: CircularSpinningLoader(
            displayLoaderIf: () => widget.personFoundResponses.isEmpty,
            elseDisplay: CollapsableApplicationBar(
                title: "Search results",
                appBarSize: 120,
                background: Hero(
                    tag: widget.heroTag,
                    child: ClipRRect(
                        borderRadius: BorderRadius.all(Radius.circular(30)),
                        child: widget.searchRequestImage)),
                children: [_buildGridWidget(context)]),
          ),
        ));
  }

  ///This function it is used for creating the navbar
  Widget _createNavigationBar() {
    //wrap in container
    return Container(
        //set the margin of the nav bar
        margin: EdgeInsets.only(left: 1, right: 1),
        //set it's border radius
        decoration: BoxDecoration(
            borderRadius: BorderRadius.only(
                topLeft: Radius.circular(80.0),
                topRight: Radius.circular(80.0)),
            color: Colors.white,
            //create the box shadow
            boxShadow: [
              BoxShadow(
                color: Colors.blueGrey[700],
                blurRadius: .5,
                spreadRadius: 2.0,
                offset: Offset(-1.0, -1.0), // shadow direction: bottom right
              )
            ]),
        //create the nav bar
        child: ClipRRect(
            //set the radius
            borderRadius: BorderRadius.only(
                topLeft: Radius.circular(80.0),
                topRight: Radius.circular(80.0)),
            //define the navbar structure
            child: BottomNavigationBar(
                unselectedFontSize: 12,
                selectedFontSize: 12,
                type: BottomNavigationBarType.fixed,
                currentIndex: _currentSelectedItemIdx,
                //items from navigation bar
                items: [
                  // the network menu entry
                  BottomNavigationBarItem(
                      icon: ImageIcon(AssetImage("assets/search_results.png"),
                          size: 25),
                      label: 'Go back'),
                  // the network menu entry
                  BottomNavigationBarItem(
                      icon: Icon(Icons.notifications_off_outlined, size: 28),
                      label: 'Mark all as seen'),
                  // the network menu entry
                  BottomNavigationBarItem(
                      icon: Icon(Icons.visibility_outlined, size: 28),
                      label: 'Modify visibility'),
                ],
                selectedItemColor: Colors.white70,
                elevation: 10.0,
                unselectedItemColor: Colors.white70,
                backgroundColor: Colors.blueGrey[700],
                onTap: (i) => _onOptionClicked(i, context))));
  }

  /// The [context] represents the build context
  Widget _buildGridWidget(BuildContext context) {
    //create a list of response ids
    //display only the responses that are not hidden
    final availableResponses = (widget.personFoundResponses ?? [])
       // .where((model) => !model.isHidden || _isHideMode)
        .toList();

    if(availableResponses.isEmpty){
      return SliverFillRemaining(
        hasScrollBody: false,
        child: CircularSpinningLoader(),
      );
    }

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
                //put items in a stack so we can display badge on top
                itemBuilder: (BuildContext context, int index) => AnimatedOpacity(
                  duration: const Duration(milliseconds: 600),
                  opacity: availableResponses[index].isHidden ? .5 : 1,
                  child: Stack(
                        clipBehavior: Clip.none,
                        children: [
                          SearchResultDetail(
                              //this function will be called on the card is clicked
                              //it will check to see if the bell icon can be removed
                              removeNotificationBellAction: () => setState(() {}),
                              foundPersonInfo: availableResponses[index],
                              userToken: widget.userToken),
                          availableResponses[index].isNewToUser || _isHideMode
                              ? _createTopCornerItem(BorderRadius.circular(20),
                                  availableResponses[index])
                              : null
                        ].where((element) => element != null).toList(),
                      ),
                ),
                //specifies the grid alignment
                gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
                    crossAxisCount: 2,
                    crossAxisSpacing: 10,
                    mainAxisSpacing: 10))));
  }

  /// This function it is used for creating the top button
  /// The border radius defines the shape of the top button
  Widget _createTopCornerItem(
      BorderRadius borderRadius, PersonFoundMessageModel model) {
    //check if the card has displayed on top the bell or the hide/view icons
    var isHideEnabled = !model.isNewToUser && _isHideMode;
    //create the proper top item
    return
        //define the top bell icon
        Positioned(
            right: 2,
            top: -3,
            // create the container
            child: AnimatedOpacityWidget(
              duration: const Duration(milliseconds: 1000),
              //if start visible
              widget: GestureDetector(
                //enable the icon tap only if we are in hidden mode
                onTap: _isHideMode ? () => _onHideChanged(model) : null,
                //create the card
                child: Card(
                    //set the elevation
                    elevation: 1,
                    //create the white color
                    color: Colors.white,
                    //set the shape
                    shape: RoundedRectangleBorder(borderRadius: borderRadius),
                    //create container
                    child: Container(
                        width: 25,
                        height: 25,
                        //set the box decoration
                        decoration: BoxDecoration(borderRadius: borderRadius),
                        //position the icon in center
                        child: Center(
                            //create a new container
                            child: Container(
                                width: 22,
                                height: 22,
                                //create the same border
                                decoration: BoxDecoration(
                                    borderRadius: borderRadius,
                                    color: Colors.white10),
                                //in center place the image
                                child: Center(
                                    child: _getTopRightIcon(
                                        isHideEnabled, model)))))),
              ),
            ));
  }

  Widget _getTopRightIcon(bool isHideEnabled, PersonFoundMessageModel model) {
    //it the hide mode is not enabled, display the bell icon if
    if (!isHideEnabled) {
      return ImageIcon(AssetImage("assets/bell.png"),
          color: Colors.blueGrey[600], size: 17);
    }

    //if the model is hidden display the visibility on
    if (!model.isHidden) {
      return Icon(Icons.visibility, color: Colors.blueGrey[600], size: 17);
    }

    //otherwise display the visibility off
    return Icon(Icons.visibility_outlined, color: Colors.blueGrey[600], size: 17);
  }

  /// Handle the event user option clicked on the [index] on [context]
  void _onOptionClicked(int index, BuildContext context) async {
    //set the current selected item
    _currentSelectedItemIdx = index;
    //treat the cases
    switch (index) {
      //treat the case when the user clicks on the go back option
      case 0:
        {
          Navigator.of(context).pop();
          return;
        }
      //treat the case when the user chooses to clear all the notifications
      case 1:
        {
          //set the state
          setState(() {
            //the hide mode must be reset on the user clicks on the card
            _isHideMode = false;
            //clear the notifications
            for (PersonFoundMessageModel personFoundResponse
                in widget.personFoundResponses ?? []) {
              personFoundResponse.isNewToUser = false;
            }
          });
          return;
        }
      case 2:
        {
          //enable/disable hide mode
          setState(() {
            _isHideMode = !_isHideMode;
          });
        }
    }
  }

  /// This method it is executed when the hide action is clicked
  void _onHideChanged(final PersonFoundMessageModel model) {
    //this will change the state of the model (from hidden to visible and vice vesa)
    setState(() {
      model.isHidden = !model.isHidden;
    });
  }
}
