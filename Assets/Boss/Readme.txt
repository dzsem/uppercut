Boss

Important:
Do not move the "boss" parent object from code, instead move the "metacarpus" (main body of the hand).
Do not move boss from code, other than it's own movement code(s).
if you want to put the boss onto the level, move the "boss" parent object.

fingerMover script:
Responsible for moving the legs/fingers
Hopefully you won't have to do anything with it.
parameters:
 - max distance--> how far a leg can be left behind. optimal value: 1.5
 - elevation offset (Vector3) --> target positon to move the leg after reaching max distance on x axis (The leg moves to the ground automaticly after that).
 - speed --> speed of leg movement