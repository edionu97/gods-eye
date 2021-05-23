abstract class FunctionalObservable {
  //the observers list
  final List<Function> _observers = [];

  /// This function it is used for adding the [observer]
  void registerObserver(final Function observer) {
    //do not register an observer multiple times
    if(_observers.contains(observer)) {
      return;
    }

    //register the observer
    _observers.add(observer);
  }

  /// This function it is used for removing the [observer]
  void unregisterObserver(final Function observer) {
    //remove the observer
    _observers.remove(observer);
  }

  /// Notify all the observers
  Future notifyAllObservers<T>(T parameter) async {
    //notify all observers
    for(var observer in _observers){
      observer(parameter);
    }
  }
}