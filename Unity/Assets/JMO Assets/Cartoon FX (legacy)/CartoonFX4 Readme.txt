﻿Cartoon FX Pack 4, version 1.1
2019/03/20
© 2019 - Jean Moreno
==============================


PREFABS
-------
Particle Systems prefabs are located in "CFX4 Prefabs" folder.
Particle Systems optimized for Mobile are located in "CFX4 Prefabs (Mobile)" folder.
They should work out of the box for most needs. If you need/don't need looping effect, (un)check the according "Looping" checkbox for each Particle System (you can use the Cartoon FX Easy Editor for that too).
All Assets have a CFX4_ (Desktop) or CFXM4_ (Mobile) prefix so that you don't mix them with your own Assets.


MOBILE OPTIMIZED PREFABS
------------------------
Mobile prefabs feature the following optimizations:
- Added a particle additive shader that uses only the alpha channel to save up on texture memory usage
- Textures' formats have been set to Compressed
- Textures have all been resized to small resolution sizes through Unity; you can however scale them up if you need better quality

It is recommended to use CFX Spawn System for object spawning on mobile (the system also works on any other GameObject, not just effects!), see below.


CFX BLEND EFFECTS and SHADERS
-----------------------------
The CFX Blend effects are using special shaders aiming at improving the visual quality of additive effects over bright backgrounds.
You can create or convert your own effects by using them, simply duplicate the base material and assign one of the following shaders to it:
- Cartoon FX/Alpha Blended + Additive
- Cartoon FX/Alpha Blended + Additive Soft
You will also find the (BaseColor) versions, which allow you to set a base color tone that will affect the entire effect. These are really useful to make vibrant additive effects (set a dark red base color to get hues shifting from red/orange/yellow in fire effects for example).


CARTOON FX EASY EDITOR
----------------------
You can find the "Cartoon FX Easy Editor" in the menu:
Window -> CartoonFX Easy Editor
It allows you to easily change one or several Particle Systems properties:
"Scale Size" to change the size of your Particle Systems (changing speed, velocity, gravity, etc. values to get an accurate scaled up version of the system; also, if the ParticleSystem uses a Mesh as Shape, it will automatically create a new scaled Mesh).
It will also scale lights' intensity accordingly if any are found.
Tip: If you don't want to scale a particular module, disable it before scaling the system and re-enable it afterwards!
"Set Speed" to change the playback speed of your Particle Systems in percentage according to the base effect speed. 100% = normal speed.
"Tint Colors" to change the hue only of the colors of your Particle Systems (including gradients).

The "Copy Modules" section allows you to copy all values/curves/gradients/etc. from one or several Shuriken modules to one or several other Particle Systems.
Just select which modules you want to copy, choose the source Particle System to copy values from, select the GameObjects you want to change, and click on "Copy properties to selected GameObject(s)".

"Include Children" works for both Properties and Copy Modules sections!


CARTOON FX SPAWN SYSTEM
-----------------------
CFX_SpawnSystem allows you to easily preload your effects at the beginning of a Scene and get them later, avoiding the need to call Instantiate. It is highly recommended for mobile platforms!
Create an empty GameObject and drag the script on it. You can then add GameObjects to it with its custom interface.
To get an object in your code, use CFX_SpawnSystem.GetNextObject(object), where 'object' is the original reference to the GameObject (same as if you used Instantiate).
Use the CFX_SpawnSystem.AllObjectsLoaded boolean value to check when objects have finished loading.


TROUBLESHOOTING
---------------
* Almost all prefabs have auto-destruction scripts for the Demo scene; remove them if you do not want your particle system to destroy itself upon completion.
* If you have problems with z-sorting (transparent objects appearing in front of other when their position is actually behind), try changing the values in the Particle System -> Renderer -> Sorting Fudge; as long as the relative order is respected between the different particle systems of a same prefab, it should work ok.
* Some prefabs work with the Collision module: they are set to World Collision by default but it requires CPU. Disable it or change to "Planes" collision mode to save ressources.
* Sometimes when instantiating a Particle System, it would start to emit before being translated, thus creating particles in between its original and desired positions. Drag and drop the CFX_ShurikenThreadFix script on the parent object to fix this problem.


PLEASE LEAVE A REVIEW OR RATE THE PACKAGE IF YOU FIND IT USEFUL! THANKS!
Enjoy! :)


CONTACT
-------
Questions, suggestions, help needed?

jean.moreno.public+unity@gmail.com
http://jeanmoreno.com/faq/

I'd be happy to see any effects used in your project, so feel free to drop me a line about that! :)



RELEASE NOTES
-------------
1.1.06
- Removed 'JMOAssets.dll', became obsolete with the Asset Store update notification system

1.1.05
- fixed small API deprecation (Unity 2017.4+)

1.1.04
- updated shaders so that they work on PS4
- updated shaders for GPU Instancing and Stereo Rendering
- updated effects with "Horizontal Billboard" particle render mode to "Billboard" with local alignment so that they can be freely rotated

1.1.03
- fixed scaling issue with "Rain" and "Wet Drops" effects (ripples were showing as ellipses instead of circles)

1.1.02
- updated demo scene to use Unity UI system
- added some of the latest effects to the demo scene

1.1.01
- CartoonFX Easy Editor: fixed scaling for Unity 2017.1+

1.1 (Unity 5.6+)
- 15+ new effects:
	- Electric/CFXM4 Sparks Explosion B (+ double variant)
	- Explosions/CFX4 Explosion Quick (+ no smoke variant)
	- Fire/CFX4 Flamethrower (+ CFX Blend variant)
	- Fire/CFX4 Fire Vortex (+ CFX Blend variant)
	- Fire/CFX4_Fire Disintegrate
	- Light/CFX4 Aura Bubble C (+ CFX Blend variant)
	- Light/CFX4 Falling Stars (+ CFX Blend variant)
	- Magic/CFX4 Magic Hit (+ CFX Blend variant)
	- Misc/CFX4 Drill Air Hit (Collide/No Collision)
	- Space/CFX4_Space Disintegrate
	- Water/CFX4 Water Explosion (Small/Medium/Big)
	- Water/CFX4 Bubbles Breath Loop
- fixes for some effects not acting properly
- Cartoon FX Easy Editor: added "Hue Shift" slider
- Cartoon FX Easy Editor: improved UI

1.04.4
- Cartoon FX Easy Editor bugfix when scaling shape module (Unity 2017.1)

1.04.3
- Cartoon FX Easy Editor bugfix when scaling (Unity 5.6)
- fixed duration for editor preview on some effects

1.04.21
- Cartoon FX Easy Editor bugfix

1.04.2
- small fix to "CFX4 Firework B" so that it works as expected in Unity 5.5

1.04.1
- Unity 5.5 compatibility

1.04
- fixed Spawn System property 'hideObjectsInHierarchy' not being saved properly
- added more options to the CFX Spawn System:
	* "Spawn as children of this GameObject": will spawn the instances as children of the Spawn System GameObject
	* "Only retrieve inactive GameObjects": will only retrieve GameObjects that are inactive
	* "Instantiate new instances if needed": will create new instances when no inactive instance is available

1.03
- fixed compilation warnings with Unity 5.3+
- fixed CFX_AutoDestructShuriken not working with some prefabs (Unity 5.3.1)

1.02
- put all shaders in the same folder
- renamed some shaders
- removed unused materials
- fixed naming in mobile rain effects

1.01
- fixed deprecated method warning in Unity 4.3+

1.0
- Initial release
