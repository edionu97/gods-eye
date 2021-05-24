import 'package:flutter/material.dart';
import 'package:gods_eye_app/persistence/search_responses/repo.dart';
import 'package:gods_eye_app/screens/home_screen/components/workers_component/component.dart';
import 'package:gods_eye_app/services/messages/service.dart';
import 'package:gods_eye_app/services/models/common/model.dart';
import 'package:gods_eye_app/services/models/person_found/model.dart';
import 'package:gods_eye_app/services/notifications/service.dart';
import 'package:gods_eye_app/utils/components/loader/component.dart';
import 'package:gods_eye_app/utils/components/notification_badge/component.dart';
import 'package:gods_eye_app/utils/helpers/objects/pair/object.dart';

import 'components/person_search_component/component.dart';
import 'components/search_results_component/component.dart';

class HomeScreen extends StatefulWidget {
  //represents the user token
  final String userToken;

  //constructs the object
  HomeScreen({this.userToken});

  @override
  State<StatefulWidget> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  //menu items
  List<Pair<Function, String>> _menuItems;

  //the number of new notifications
  int _newNotifications = 0;

  //the menu index
  int _currentMenuItemIdx = 2;

  @override
  void initState() {
    //call the initialisation logic
    super.initState();

    //set the menu items by defining the function that creates the element and the title of the page
    _menuItems = [
      //create the page for the remote workers
      Pair((userToken) => RemoteWorkersScreen(userToken), "Remote Workers"),
      //create the page for the person search
      Pair((userToken) => PersonSearchScreen(userToken: userToken),
          "Person search"),
      //create the search results page
      Pair((userToken) => SearchResultsScreen(userToken: userToken),
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
    //PersonSearchResponseRepository().clearRepository();
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
    IAbstractModel convertedObject = MessageParsingService().parseModelFromJson(message);

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
                  //the application settings menu entry
                  BottomNavigationBarItem(
                      icon: Icon(Icons.settings, size: 28),
                      label: 'App settings')
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
  void _onItemAdded(_){
    //if we are displaying the notification screen
    if(_currentMenuItemIdx == 2){
      //do nothing
      return;
    }

    //set the value of the notification badge text
    setState(() {
      ++_newNotifications;
    });
  }
}
