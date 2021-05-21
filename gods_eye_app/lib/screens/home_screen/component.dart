import 'package:flutter/material.dart';
import 'package:gods_eye_app/screens/home_screen/components/workers_component/component.dart';
import 'package:gods_eye_app/utils/components/loader/component.dart';
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
  final List<Pair<Function, String>> _menuItems = [
    //define the function that creates the element and the title of the page
    Pair((userToken) => RemoteWorkersScreen(userToken), "Remote Workers"),
    Pair((userToken) => Center(child: CircularSpinningLoader()),
        "Inbox Workers"),
    Pair((userToken) => Center(child: CircularSpinningLoader()),
        "Account Workers"),
    Pair((userToken) => Center(child: CircularSpinningLoader()), "Settings")
  ];

  //the menu index
  int _currentMenuItemIdx = 0;

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
    return Container(
      margin: EdgeInsets.only(left: 1, right: 1),
      decoration: BoxDecoration(
          borderRadius: BorderRadius.only(
              topLeft: Radius.circular(80.0), topRight: Radius.circular(80.0)),
          color: Colors.white,
          boxShadow: [
            BoxShadow(
              color: Colors.blueGrey[700],
              blurRadius: .5,
              spreadRadius: 2.0,
              offset: Offset(-1.0, -1.0), // shadow direction: bottom right
            )
          ]),
      child: ClipRRect(
        borderRadius: BorderRadius.only(
            topLeft: Radius.circular(80.0), topRight: Radius.circular(80.0)),
        child: BottomNavigationBar(
            type: BottomNavigationBarType.fixed,
            currentIndex: _currentMenuItemIdx,
            //items from navigation bar
            items: [
              BottomNavigationBarItem(
                  icon: ImageIcon(AssetImage("assets/network.png"), size: 28),
                  label: 'Network'),
              BottomNavigationBarItem(
                  icon: ImageIcon(AssetImage("assets/search_results.png"),
                      size: 28),
                  label: 'Search results'),
              BottomNavigationBarItem(
                  icon: ImageIcon(AssetImage("assets/person_search.png"),
                      size: 28),
                  label: 'Person search'),
              BottomNavigationBarItem(
                  icon: Icon(Icons.settings, size: 28), label: 'App settings')
            ],
            selectedItemColor: Colors.white,
            elevation: 10.0,
            unselectedItemColor: Colors.blueGrey[300],
            backgroundColor: Colors.blueGrey[700],
            onTap: (index) => setState(() => _currentMenuItemIdx = index)),
      ),
    );
  }

  /// This function it is used for creating the application bar
  Widget _createAppBar() {
    return AppBar(
        elevation: 20,
        shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.vertical(
          bottom: Radius.circular(40),
        )),
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
