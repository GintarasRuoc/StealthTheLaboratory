# Stealth-the-laboratory

Game main idea:
Player has to navigate generated map to complete his task and return to start of the map, while trying to avoid guards.

Functional requirements
 + Player controlled character:
   + character tries to hide behind closest object;
   + character skills can be used only few times in game and amount of uses differs from skill to skill.
 + Computer controlled character (AI):
   + AI can have additional passive skill ( run faster, can't be stunned);
   + AI walks along a route, until something happens;
   + AI will react to player character used skills;
   + AI won't be able to spot player character trough objects;
   + AI will try to catch player, if caught the game will be lost;
 + Map generation:
   + map is generated randomly, from room prefabs;
   + tasks are placed in a random room;
   + maps can contain extra task, which gives additional points to unlock skills;
   + exit unlocks, when main task of the map is completed.
 + Pause menu:
   + player can change key binds;
   + player can exit the game;
   + player can see current tasks.

Non-Functional requirements
 + Game mechanics:
   + camera position is not fixed; 
   + main character is controlled with computer mouse.
 + Player interface:
   + show players selected skills;
   + show how to activate skill;
   + show mini-map, which allows player to see current room, rooms that was explored and adjacent unexplored rooms;
   + player can see AI vision field, when moused over or when close enough to AI.

![image](https://user-images.githubusercontent.com/72353599/210767648-50ef2bd6-ef1c-4c3b-82f6-1a81c425b468.png)
                   
1. Figure. Class diagram.

Map creation

Every time game session start, new map generates. Map generateds in 2D grid, which is filled with shapes shown in Figure 2.1. One room can take up one to four spaces in 2D grid. Spawned rooms are prebuilt. Most of the objects, textures and icons were not created by me. These rooms needs to have AI with paths, designated room exit spots and tasks spawn locations.

![image](https://user-images.githubusercontent.com/72353599/210769423-61421be4-0d11-4795-b64f-51c0d6006542.png)

  2.1. Figure. Prefab room shapes.

![image](https://user-images.githubusercontent.com/72353599/210769488-1254062e-65d7-45e4-8919-4b250df46bad.png)

  2.2. Figure. Prefab room example.

To start map generation, first starting room is spawned, which is selected randomly from list of created starting rooms. Then random used tile is selected, which can have a room spawned next to. After selected tile it randomly chooses size of room to spawn ( bigger rooms are less likely to spawn). After room size selection, unused tiles are being selected, that could fit the room. Then a random room is being selected that fits in selected tiles. If needed room is rotated, flipped. After room is spawned, used tiles are updated. After whole map is generated, doors are being checked and opened.

![image](https://user-images.githubusercontent.com/72353599/210771904-97206fe3-4638-4011-ba6a-597bfa0a3354.png)

  2.3. Figure. Generated map.

Gameplay guide: 

After game launch, player can see main menu ( 3.1. Figure), which has four buttons:
+	„Start game“ – starts game session;
+	„Skills“ – opens skill menu;
+	„Options“ – opens options menu;
+	„Quit game“ – shutdowns the game.


![image](https://user-images.githubusercontent.com/72353599/210771635-170952eb-21d8-4d61-b552-b75c20dd956c.png)

3.1. Figure. Main menu interface.

When "Skill" button is pressed in main menu, it opens skill menu ( 3.2 Figure). In this menu player can read information about skills. These skills can have four different markings:
•	lock icon - can't be used, but can be unlocked with knowledge point, which can be seen at the top of skill menu;
•	dark green border - this skill is selected to be used in a game session;
•	gray border - this skill is unlocked and can be selected to play in a game session;
•	light green border - this skill is shown as selected, but it needs to be save to use in a game session.
Skill menu has two buttons: "Save" and "Cancel". First button saves skill changes. Second button cancels skill selection.

![image](https://user-images.githubusercontent.com/72353599/210771663-31f8635d-80bf-444b-af11-d1c0f9d45e49.png)

3.2. Figure. Skill menu.

In options ( 3.3 Figure) player can change sound volume, keybinds. Sound volume can be changed by sliding the bar. Keybind change is called, when button with white background is pressed.

![image](https://user-images.githubusercontent.com/72353599/210771675-db6ba4a4-8f40-47f2-8601-e57036a36606.png)

3.3. Figure. Options menu.

To change buttons, user has to press button with white background ( 3.4. Figure), than system starts to listen for user next button press. If user wants to change button, than they need to press "Confirm" button. To cancle user has to press "Cancel" button.

![image](https://user-images.githubusercontent.com/72353599/210771692-5811952d-957f-4ac5-8b85-0b0a9c0fc309.png)

3.4. Figure. Keybind change window.

At the start of the game player is shown tasks ( 3.5. Figure). This window shows what tasks are generated. When "Play game" button is pressed, a game session is started.

![image](https://user-images.githubusercontent.com/72353599/210771701-8dfaf2a1-9017-4d67-8510-245ff76e92d0.png)

3.5. Figure. Interface shows generated tasks.

Game interface has few elements ( 3.6. Figure) Bottom left shows selected skills. Left icon shows passive skill. Right icon shows active skill, which has more information: how many uses left, how to activate a skill. Top left of the interface minimap is shown, which has three meanings of squares:
 + white - player is in this room;
 + gray - player hasn't been in this room. Gray squares only appear next to explored rooms;
 + black - player has explored this room.

![image](https://user-images.githubusercontent.com/72353599/210771718-c7e6ea55-753b-43cc-ae9d-f9de1b40498f.png)

3.6. Figure. Game interface.

When user activates skill it shows how to use or cancel skill at the bottom of game interface ( 4.7. Figure). Game has four different active skills:
+ "Movement speed boost" - character gets speed boost for a duration of time;
+ "Invisibility" - character becomes invisible for a short period of time;
+ "Coins" - yellow circle appears, which indicates how far the noise will be heard and AI will go to the sound;
+ "Rocks" - player can select AI, which they want to stun.

![image](https://user-images.githubusercontent.com/72353599/210771725-ee579718-1bb7-459e-8fcb-cfc8c57ad2c3.png)

3.7. Figure. "Coins" skill is activated.

AI can show their field of vision ( 3.8 Figure). If user is close enough to AI, it will automatically show field of vision. Field of vision can also be checked by mousing over the AI. If player is seen by AI, than exclamation mark appears above enemy. After short amount of time AI will chase player and exclamation mark becomes red.

![image](https://user-images.githubusercontent.com/72353599/210771738-dc0b72ed-6128-4134-9d16-0aab09a6d15a.png)

3.8. Figure. AI shows its field of vision and black exclamation mark above head.

User can pause game ( 3.9 Figure), by pressing "Escape" button. Pause interface shows information about current task and it has three buttons:
+ "Back to the game" - resume game sessions;
+ "Main menu" - game session ends and main menu is being loaded;
+ "Quit game" - turns off the game.

![image](https://user-images.githubusercontent.com/72353599/210771750-f1ce84b4-6b69-4408-9d6a-5ef9e04e2f59.png)

3.9. Figure. Pause interface.

Task completion areas are marked with red circle ( 3.10. Figure). If player enters this area, it will shows interact button at the bottom of the screen.

![image](https://user-images.githubusercontent.com/72353599/210771768-887353bf-4a2c-4729-a761-d47324f06964.png)

3.10. Figure. Task completion area.


In the starting room user can see gray circle, which mean symbolises locked exit area. After player completes main task, the exit circle becomes green ( 3.11. Figure) and player can finish session.

![image](https://user-images.githubusercontent.com/72353599/210771780-918d028b-2ba7-4395-aa11-38abebc23208.png)

3.11. Figure. Unlocked map exit.
