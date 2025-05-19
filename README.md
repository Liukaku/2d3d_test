# 2d3d test game

this is a "test" project for experimenting with a 2d sprite in a 3d world game

TODO:
- within PlayerController.cs ChangeTarget() it should be updated to use a single target rather than pass one to every instance of the spell classes
- figure out what i'm actually doing with this, a story or what?
- update the dialog loader to filter out the greeting objects. Nexted lists didn't agree with unity's jsonutility, this will mean we can filter out the greetings into an array and pick one from random. Jsonutility also doesn't like dictionarys
- break out the player controller script into separate files because holy hell is it getting big
- Dialog is generally in place now
	- handle quests?
	- track player vibe

Done:
[x] basic dialog tree
[x] display images depending on the dialog options
[] handle play vibe based on response 
