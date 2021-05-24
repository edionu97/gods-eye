import 'package:flutter/material.dart';

class CollapsableApplicationBar extends StatelessWidget {
  final String title;
  final Widget background;
  final List<Widget> children;
  final int appBarSize;

  const CollapsableApplicationBar(
      {Key key,
      this.title,
      this.background,
      this.children,
      this.appBarSize = 120})
      : super(key: key);

  @override
  Widget build(BuildContext context) {
    //define the app bar header radius
    final BorderRadius appBarHeaderRadius =
        BorderRadius.vertical(bottom: Radius.circular(50));

    //create a custom scroll view
    return CustomScrollView(clipBehavior: Clip.antiAlias, slivers: [
      SliverAppBar(
          //remove the back arrow
          automaticallyImplyLeading: false,
          //set the shape of the appbar
          shape: RoundedRectangleBorder(borderRadius: appBarHeaderRadius),
          //set the floating to true
          floating: true,
          //set the pinned to true (appbar does not scroll)
          pinned: true,
          //set the elevation
          elevation: 10,
          //use the background transparent
          backgroundColor: Colors.transparent,
          //this is the space from that will collapse/ expand
          flexibleSpace: LayoutBuilder(
              //on build
              builder: (BuildContext context, BoxConstraints constraints) =>
                  FlexibleSpaceBar(
                      //center the title
                      centerTitle: true,
                      //put one animation for the title
                      title: AnimatedOpacity(
                          //set it's duration
                          duration: Duration(milliseconds: 250),
                          opacity:
                              constraints.biggest.height >= appBarSize ? 0 : 1,
                          child: Container(
                              decoration: BoxDecoration(
                                  color: Colors.blueGrey[700],
                                  borderRadius: BorderRadius.vertical(
                                      bottom: Radius.circular(30))),
                              height: constraints.biggest.height,
                              child: Align(
                                  alignment: Alignment.bottomCenter,
                                  child: Padding(
                                      padding: const EdgeInsets.all(8.0),
                                      child: Opacity(
                                          opacity: .95,
                                          child: Text(title ?? "",
                                              style: TextStyle(
                                                  fontWeight: FontWeight.w300,
                                                  fontStyle: FontStyle.normal,
                                                  fontSize: 25,
                                                  color: Colors.white))))))),
                      background: background ?? Container())),
          expandedHeight: 250),
      //place all the other children
      ...(children ?? [])
    ]);
  }
}
