import 'package:flutter/material.dart';
import 'package:gods_eye_app/screens/home_screen/components/workers_component/component.dart';
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
    Pair((userToken) => Text("da"), "Inbox Workers"),
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
      child: ClipRRect(
        borderRadius: BorderRadius.only(
            topLeft: Radius.circular(80.0), topRight: Radius.circular(80.0)),
        child: BottomNavigationBar(
            currentIndex: _currentMenuItemIdx,
            //items from navigation bar
            items: [
              BottomNavigationBarItem(
                  icon: ImageIcon(AssetImage("assets/network.png"), size: 30),
                  label: 'Cloud'),
              BottomNavigationBarItem(
                  icon: Icon(Icons.mail, size: 30), label: 'Inbox'),
              BottomNavigationBarItem(
                  icon: Icon(Icons.account_circle, size: 30), label: 'Account'),
            ],
            selectedItemColor: Colors.white,
            elevation: 150,
            unselectedItemColor: Colors.blueGrey[300],
            backgroundColor: Colors.blueGrey[700],
            onTap: (index) => setState(() => _currentMenuItemIdx = index)),
      ),
    );
  }

  /// This function it is used for creating the application bar
  Widget _createAppBar() {
    return AppBar(
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.vertical(
            bottom: Radius.circular(40),
          ),
        ),
        backgroundColor: Colors.blueGrey[700],
        title: Align(
          alignment: Alignment.center,
          child: Text(_menuItems[_currentMenuItemIdx].second,
              style: TextStyle(
                  fontWeight: FontWeight.w300,
                  fontStyle: FontStyle.normal,
                  fontSize: 25,
                  color: Colors.white)),
        ),
        automaticallyImplyLeading: false);
  }
}
