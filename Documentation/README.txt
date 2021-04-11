What is this? 🤔
This project is about exploring goal oriented behaviour based on chapter 7-10 of Mat Buckland's book

Given
1. 

TODO
1. [Documentation] Document your work. Explain what parts you attempted, how your approach is supposed to work, which parts of the code you modified, removed or added. 

2. [Bug Fixing] 
    
3. [Implementation ideas]
    a) The goal here is to provide useful information to help you complete the other parts of the assignment.
        i) Add visual/audio indications to clarify what is happening. 
        - Weeble status (health, score, etc)
        - Weeble thoughts (text bubble?)
        - Weeble got hit indicator - can make it flash red
        ii) Include visual/audio/log debugging aids 

    b) Tune the parameters and evaluators. Some parameters in Parameters.cs are not used or were set based on a larger map. You can remove unused parameters (or make use of them). You should also adjust the values of the parameters to be sensible. For example, what should the sound range for the shotgun be? What should the rate of fire be? How many searches should be allowed per update?

    c) Design and implement an additional goal-oriented behaviour and a corresponding evaluator and relevant features. 
        - Consider atomic vs composite goals
        - MoveToCover 
        - CaptureTheFlag

4. Find ways to make it awesome!
    a) PathCosts.cs contains a pre-computed path table similar to the ones discussed in the lectures. Modify the PathPlanner to use this table instead of TimeSlicedAStar. That should improve the efficiency of the game.

    b) Implement the ability to pickup the enemy flag at the base and carry it back to your base. If you are killed while carrying the flag, simply return the flag to its base or implement a means for the flag to be dropped where you died so either a team-mate or the enemy can pick it up. This may require implementing a new form or Trigger.

    c) Improve issues with the frame rate so that the game plays smoothly with 3, 4 or 5 weebles per team.

    d) Design and implement more goal-oriented behaviours and a corresponding evaluators and relevant features. Make use of the low walls and elevated areas for cover and good sniping locations.

    e) Add an additional weapon type.

    f)Add additional cameras such as one to follow weebles in first or third person. Add the ability to switch between cameras. Perhaps add a mini-map/overhead view as a sub-window of the Weeble camera.

4. Find ways to make it awesome!
    i) Make them aim where the target will be, not where they are now.
    ii) sound effects
    iii) explosion

In progress:


Completed: 
