import 'package:gods_eye_app/services/models/person_found/model.dart';
import 'package:gods_eye_app/utils/helpers/objects/observer/object.dart';
import 'package:mutex/mutex.dart';

class PersonSearchResponseRepository extends FunctionalObservable {
  //implement the singleton pattern
  static PersonSearchResponseRepository _singletonInstance =
      PersonSearchResponseRepository._internal();

  //create the sync primitive
  final Mutex _syncPrimitive = Mutex();

  //declare a list of
  final List<PersonFoundMessageModel> _foundPersons = [];

  //declare a private constructor
  PersonSearchResponseRepository._internal();

  //implement the factory method
  factory PersonSearchResponseRepository() {
    return _singletonInstance;
  }

  /// This method it is used for adding a new instance into the list
  /// The instance is the [itemToBeAdded]
  Future addItemAsync(final PersonFoundMessageModel itemToBeAdded) async {
    //critical section (add data into the list)
    await _syncPrimitive.protect(() async => _foundPersons.add(itemToBeAdded));

    //notify all observers that a new item was added
    notifyAllObservers(itemToBeAdded);
  }

  /// This method it is used for getting all the items async
  Future<List<PersonFoundMessageModel>> allItemsAsync() async {
    //critical section (lock the list when get all)
    return await _syncPrimitive.protect(() async => _foundPersons);
  }

  /// This method it is used for getting all the responses grouped by
  /// their responseId
  Future<Map<String, List<PersonFoundMessageModel>>>
      groupItemsBasedOnResponseIdAsync() async {

    //get all the items
    final Map<String, List<PersonFoundMessageModel>> map = {};

    //iterate the items
    for (var item in await allItemsAsync()) {
      //if the item does not exist do nothing
      if (!map.containsKey(item.responseId)) {
        map[item.responseId] = [];
      }

      //add the item in dictionary
      map[item.responseId].add(item);
    }

    //return the map
    return map;
  }

  //clear the repository
  void clearRepository() {
    _foundPersons.clear();
  }
}
