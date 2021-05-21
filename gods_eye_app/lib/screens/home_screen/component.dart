import 'package:flutter/material.dart';
import 'package:gods_eye_app/screens/home_screen/components/workers_component/component.dart';
import 'package:gods_eye_app/utils/components/loader/component.dart';
import 'package:gods_eye_app/utils/components/notification_badge/component.dart';
import 'package:gods_eye_app/utils/helpers/objects/pair/object.dart';

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

  int _newNotifications = 0;

  //the menu index
  int _currentMenuItemIdx = 1;

  @override
  void initState() {
    //call the initialisation logic
    super.initState();

    //se the menu items
    _menuItems = [
      //define the function that creates the element and the title of the page
      Pair(
          (userToken) => RemoteWorkersScreen(
              userToken, (x) => setState(() => _newNotifications = x)),
          "Remote Workers"),
      Pair((userToken) => Center(child: CircularSpinningLoader()),
          "Person search"),
      Pair((userToken) => Center(child: CircularSpinningLoader()),
          "Search results"),
      Pair((userToken) => Center(child: CircularSpinningLoader()),
          "App settings")
    ];
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
                onTap: (index) => setState(() {
                      //reset the value of the notification badge
                      _newNotifications = index == 2 ? 0 : _newNotifications;
                      //set the current menu index
                      _currentMenuItemIdx = index;
                    }))));
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
}
