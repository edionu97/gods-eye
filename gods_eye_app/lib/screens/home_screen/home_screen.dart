import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:gods_eye_app/persistence/search_responses/repo.dart';
import 'package:gods_eye_app/services/messages/service.dart';
import 'package:gods_eye_app/services/models/common/model.dart';
import 'package:gods_eye_app/services/models/failure/model.dart';
import 'package:gods_eye_app/services/models/person_found/model.dart';
import 'package:gods_eye_app/services/notifications/service.dart';
import 'package:gods_eye_app/utils/components/loader/component.dart';
import 'package:gods_eye_app/utils/components/modal/component.dart';
import 'package:gods_eye_app/utils/components/notification_badge/component.dart';
import 'package:gods_eye_app/utils/helpers/objects/pair/object.dart';
import 'package:intl/intl.dart';
import 'components/network_screen_component/network_screen_component.dart';
import 'components/person_search_screen_component/person_search_screen_component.dart';
import 'components/search_results_screen_component/search_results_screen_component.dart';

/// Create the home screen
class HomeScreenWidget extends StatefulWidget {
  //represents the user token
  final String userToken;

  //constructs the object
  HomeScreenWidget({this.userToken});

  @override
  State<StatefulWidget> createState() => _HomeScreenWidgetState();
}

class _HomeScreenWidgetState extends State<HomeScreenWidget> {
  //menu items
  List<Pair<Function, String>> _menuItems;

  //the number of new notifications
  int _newNotifications = 0;

  //the menu index
  int _currentMenuItemIdx = 0;

  @override
  void initState() {
    //call the initialisation logic
    super.initState();

    //set the menu items by defining the function that creates the element and the title of the page
    _menuItems = [
      //create the page for the remote workers
      Pair((userToken) => RemoteWorkersScreenWidget(userToken), "Remote Workers"),
      //create the page for the person search
      Pair((userToken) => PersonSearchScreenWidget(userToken: userToken),
          "Person search"),
      //create the search results page
      Pair((userToken) => SearchResultsScreenWidget(userToken: userToken),
          "Search results"),
      Pair((userToken) => Center(child: CircularSpinningLoader()),
          "App settings")
    ];

    //register a new observer
    PersonSearchResponseRepository().registerObserver(_onItemAdded);

    //register the observer
    NotificationService().registerObserver(_onMessage);
  }

  @override
  void dispose() {
    //unregister the observer
    PersonSearchResponseRepository().unregisterObserver(_onItemAdded);
    //clear the repository
    PersonSearchResponseRepository().clearRepository();
    //unregister the observer
    NotificationService().unregisterObserver(_onMessage);
    //on dispose unregister the user
    NotificationService().unregisterAsync();
    //execute the dispose logic
    super.dispose();
  }

  /// This function it is used when a new message is received from client
  /// Treats only the person found message
  void _onMessage(String message) async {
    //convert the json into an object
    IAbstractModel convertedObject =
        MessageParsingService().parseModelFromJson(message);

    //treat the case when the response is a error response
    //some notification
    if (convertedObject is ActiveWorkerFailedMessage) {
      //handle the error message
      await _onErrorMessageEncountered(context, convertedObject);
      return;
    }

    //check if the object is the right instance
    if (!(convertedObject is PersonFoundMessageModel)) {
      return;
    }

    //convert the abstract worker in specific instance
    var personFoundMessage = convertedObject as PersonFoundMessageModel;

    //the model is new the first time it entered into the system
    //otherwise it means that he saw it
    personFoundMessage.isNewToUser = true;

    //add a  new item into database and notify all the observers
    await PersonSearchResponseRepository().addItemAsync(personFoundMessage);
  }

  @override
  Widget build(BuildContext context) {
    //create the page
    return Scaffold(
        //add the app bar
        appBar: _createAppBar(),
        //add the bottom navigation bar
        bottomNavigationBar: _createNavigationBar(),
        //set the body
        body: _menuItems[_currentMenuItemIdx].first(widget.userToken));
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
                type: BottomNavigationBarType.fixed,
                currentIndex: _currentMenuItemIdx,
                //items from navigation bar
                items: [
                  // the network menu entry
                  BottomNavigationBarItem(
                      icon:
                          ImageIcon(AssetImage("assets/network.png"), size: 28),
                      label: 'Network'),
                  //the person search menu entry
                  BottomNavigationBarItem(
                      icon: ImageIcon(AssetImage("assets/person_search.png"),
                          size: 28),
                      label: 'Person search'),
                  //the search results menu entry
                  BottomNavigationBarItem(
                    //create the icon badge
                    icon: NotificationBadge(
                        //if this condition is displayed we are displaying the badge
                        displayBadgeIf: () => _newNotifications > 0,
                        //set the badge text
                        badgeText: "$_newNotifications",
                        //if true the text will be displayed
                        displayText: true,
                        // the widget that will be displayed as notification
                        mainWidget: ImageIcon(
                            AssetImage("assets/search_results.png"),
                            size: 28)),
                    //set the label of the badge
                    label: 'Search results',
                  ),
                  // //the application settings menu entry
                  // BottomNavigationBarItem(
                  //     icon: Icon(Icons.settings, size: 28),
                  //     label: 'App settings')
                ],
                selectedItemColor: Colors.white,
                elevation: 10.0,
                unselectedItemColor: Colors.blueGrey[300],
                backgroundColor: Colors.blueGrey[700],
                onTap: _onNavigate)));
  }

  /// This function it is used for creating the application bar
  Widget _createAppBar() {
    return AppBar(
        elevation: 20,
        shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.vertical(bottom: Radius.circular(40))),
        shadowColor: Colors.blueGrey[700],
        backgroundColor: Colors.blueGrey[700],
        title: Align(
          alignment: Alignment.center,
          child: Opacity(
            opacity: .95,
            child: Text(_menuItems[_currentMenuItemIdx].second,
                style: TextStyle(
                    fontWeight: FontWeight.w300,
                    fontStyle: FontStyle.normal,
                    fontSize: 25,
                    color: Colors.white)),
          ),
        ),
        automaticallyImplyLeading: false);
  }

  /// This method it is used for navigating from one menu to another
  /// the [toIndex] is the index to which we want to navigate
  void _onNavigate(int toIndex) {
    //set the state accordingly
    setState(() {
      //reset the value of the notification badge
      _newNotifications = toIndex == 2 ? 0 : _newNotifications;
      //set the current menu index
      _currentMenuItemIdx = toIndex;
    });
  }

  /// This callback it is used for modifying the value of the badge
  void _onItemAdded(_) {
    //if we are displaying the notification screen
    if (_currentMenuItemIdx == 2) {
      //do nothing
      return;
    }

    //set the value of the notification badge text
    setState(() {
      ++_newNotifications;
    });
  }

  /// Handle the error message that are pushed from server
  Future _onErrorMessageEncountered(
      BuildContext context, ActiveWorkerFailedMessage errorMessage) async {
    //try to display the camera
    try {
      var date = "unspecified", time = "unspecified";

      //if date and time are not null
      if (errorMessage.failedSearchReq?.startedAt != null) {
        //get the date
        date = DateFormat("dd MMM yyyy")
            .format(errorMessage.failedSearchReq.startedAt);
        //get the time
        time = DateFormat("hh:mm:ss")
            .format(errorMessage.failedSearchReq.startedAt);
      }

      //get the dialog message
      var dialogMessage =
          "Error processing the request created on $date at $time";
      if (errorMessage.status == "Cancelled") {
        dialogMessage = "Cancelled request created on $date at $time";
      }

      //show the modal
      await Modal.showExceptionalDialogWithNoActionsAsync(
          context, dialogMessage, errorMessage.details);
    } on Exception {
      //ignore
    }
  }
}
