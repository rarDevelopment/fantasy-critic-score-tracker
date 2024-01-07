# Fantasy Critic Score Tracker Script
A script for generating CSV files with Fantasy Critic scores. 

### Important Note: This will only work for public leagues.

## Running The Script

- If you don't already use Visual Studio, you can download and install Visual Studio Community for free [here](https://visualstudio.microsoft.com/vs/community/).
- Download or Clone this repository in its entirety
- Open the .sln file in Visual Studio
- Navigate to the Program.cs file
    - Set the location where you want the .csv files to be generated [here](/FantasyCriticScoreTracker/Program.cs#L11)
    - Update the `leagueIds` array, starting [here](/FantasyCriticScoreTracker/Program.cs#L13-L17) to include all of your League IDs that you want to generate files for. Note that they must be in "quotes" and must be separated by a comma (it's okay to put them on separate lines). You're allowed to have only one, also.
    - Make sure the [year](/FantasyCriticScoreTracker/Program.cs#L20) is set to the correct year.
- Run the code ![image](https://github.com/rarDevelopment/fantasy-critic-score-tracker/assets/4060573/2a9dfbac-bc0c-4af1-8caa-8cb66feeecf2)

This should generate your Excel files. You can run this whenever you want and it will add lines to those CSV files, provided you don't move or rename them.

## Running with Task Scheduler in Windows

Once you've built and run it once using the above instructions, you'll have a built .exe file you can set to run on a scheduler. Here's how I did mine:

- Open Windows Task Scheduler
- Create a Basic Task

![image](https://github.com/rarDevelopment/fantasy-critic-score-tracker/assets/4060573/235b7325-48f0-4aca-8806-8f01158fbe33)
- Name your Task
- In the Trigger section, select how often you want the script to run. Remember that more runs means more data points, so think about how you want your data to be tracked. For Fantasy Critic, daily is probably far too often, weekly could be good, and monthly is probably fine as well. It's up to you!
- In the Action section, choose "Start a program". The next screen will have you browsse for a program. Browse to the fantasy-critic-score-tracker\FantasyCriticScoreTracker\bin\Debug\net7.0\ and choose **FantasyCriticScoreTracker.exe**.
- Click Finish

This should set you up with a scheduled task that runs the script. Feel free to customize your task further, like adding a message step to show a message when this runs (this just requires adding an additional Start a Program step under actions, where the program is `msg` and the arguments you pass in are `* "Fantasy Critic Score Updater was run."` for example).


