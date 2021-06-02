[![MIT License](https://img.shields.io/github/license/Aarif123456/GoalOrientedBehaviour?style=for-the-badge)](https://lbesson.mit-license.org/)
[![BCH compliance](https://bettercodehub.com/edge/badge/Aarif123456/GoalOrientedBehaviour?branch=main)](https://bettercodehub.com/)
![Lines of code](https://img.shields.io/tokei/lines/github/Aarif123456/GoalOrientedBehaviour?style=for-the-badge)
![Top Language](https://img.shields.io/github/languages/top/Aarif123456/GoalOrientedBehaviour?style=for-the-badge)

## What is this? 🤔

This project is about exploring goal oriented behaviour based on chapter 7-10 of Mat Buckland's book

## Given
1. Meta editors in Editor directory, will probably not touch
2. ShowThots - Can use to visualize the thought process of weebles
3. Parameters - Can modify depending on program
4. Enum Utility - Allows us to add new things, like weapons and other item
5. Steering behaviour - might not modify if I want to add steering behaviours or possible inherit 
6. Armory
    - projectiles
    - weapons
7. Atomic goals - seek to position - is stuck - not implemented 


## Implementation ideas
         Weeble got hit indicator - can make it flash red
        ii) Include visual/audio/log debugging aids 

    c) Design and implement an additional goal-oriented behaviour and a corresponding evaluator and relevant features. 
        - MoveToCover 
        - CaptureTheFlag

## More ways to make it awesome!

- a) PathCosts.cs contains a pre-computed path table similar to the ones discussed in the lectures. Modify the PathPlanner to use this table instead of TimeSlicedAStar. That should improve the efficiency of the game.
- b) Implement the ability to pickup the enemy flag at the base and carry it back to your base. If you are killed while carrying the flag, simply return the flag to its base or implement a means for the flag to be dropped where you died so either a team-mate or the enemy can pick it up. This may require implementing a new form or Trigger.
- c) Improve issues with the frame rate so that the game plays smoothly with 3, 4 or 5 weebles per team.
    - Pathfinding
    - garbage collector
    - string concatenation 
    - other inefficiency 
- d) Design and implement more goal-oriented behaviours and a corresponding evaluators and relevant features. Make use of the low walls and elevated areas for cover and good sniping locations.
- e) Add an additional weapon type.
    - landmine 
- f) More things to add
    - Make them aim where the target will be, not where they are now.
    - sound effects
    - explosion
    - custom map from map file
    - loading page - with loading bar while map loads (pre-computation)
    - setting changing possible main menu
    - character that can move
    - Implementing smell 
    


## TODO: :alarm_clock:
    finish up a) add in though and hit indicator
    implement goal: currently adding in the ability to move away from projectiles being fired 

## Done :star2:

    [Bug Fixing]
        - Weeble Alice had the wrong short-name
        - Added in Wall layers so Weeble does get stuck 
        - Weebles sometimes flip-flop between goals and end up getting stuck for a while so added a consistency boast to encourage finishing goals 
        - Weeble keeps moving back to old location - happens if we manually teleport Weeble

    [Implementation ideas]
        - Made biases into a separate class to make it easier to expand 

        a) The goal here is to provide useful information to help you complete the other parts of the assignment.
            i) Add visual/audio indications to clarify what is happening. 
            - Weeble status (health, score, etc)
            - Weeble thoughts (text bubble?)
            - Added in option to forcibly add goal to agent to make debugging easier 

        b) Tune the parameters and evaluators. Some parameters in Parameters.cs are not used or were set based on a larger map. You can remove unused parameters (or make use of them). You should also adjust the values of the parameters to be sensible. For example, what should the sound range for the shotgun be? What should the rate of fire be? How many searches should be allowed per update?
            - Made ideal range of weapons smaller
            - Activated blasters
            - Made the game check for paths less frequently 
            - shrunk max and min distance in Features 

        c) Design and implement an additional goal-oriented behaviour and a corresponding evaluator and relevant features. 
            - evade bot

    4. Find ways to make it awesome!
    f) Add additional cameras such as one to follow weebles in first or third person. Add the ability to switch between cameras. Perhaps add a mini-map/overhead view as a sub-window of the Weeble camera.
