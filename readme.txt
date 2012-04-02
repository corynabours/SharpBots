CSRobots v 0.1

First there was CRobots, followed by PRobots and many others, recently
(well also years ago) Robocode emerged and finally this is CSRobots bringing
all the fun to the CSharp community.

What is he talking about?

CSRobots is a simulation environment for robots, these robots have a scanner
and a gun, can move forward and backwards and are entirely controlled by
CSharp controller dlls. All robots are equal (well at the moment, maybe this will 
change) except for the ai.

A simple robot script may look like this:

----------------------- code -----------------------
using CSBot.Robots;

namespace NervousDuck
{
    public class NervousDuck : Robot
    {
        public override void Tick(Events events)
        {
            if(Time == 0) TurnRadar(1);
            if (Time < 3) TurnGun(30);
            Accelerate(1);
            Turn(2);
            if(RobotsScanned().Count > 0) Fire(3.0m);
        }

        public override string Name
        {
            get
            {
                return "NervousDuck";
            }
        }
    }
}
----------------------- code -----------------------

all you need to implement is the tick method which should accept an events object of events that have occurred during the last tick.

By deriving from Robot you get all this methods to control your bot:

  BattlefieldHeight   #the height of the battlefield
  BattlefieldWidth    #the width of the battlefield
  Energy              #your remaining energy (if this drops below 0 you are dead)
  GunHeading          #the heading of your gun, 0 pointing east, 90 pointing 
                      #north, 180 pointing west, 270 pointing south
  GunHeat             #your gun heat, if this is above 0 you can't shoot
  Heading             #your robots heading, 0 pointing east, 90 pointing north,
                      #180 pointing west, 270 pointing south
  Size                #your robots radius, if x <= size you hit the left wall
  RadarHeading        #the heading of your radar, 0 pointing east, 
                      #90 pointing north, 180 pointing west, 270 pointing south
  Time                #ticks since match start
  Speed               #your speed (-8/8)
  X                   #your x coordinate, 0...battlefield_width
  Y                   #your y coordinate, 0...battlefield_height
  Accelerate(param)   #accelerate (max speed is 8, max accelerate is 1/-1, 
                      #negativ speed means moving backwards)
  Stop                #accelerates negative if moving forward (and vice versa),
                      #may take 8 ticks to stop (and you have to call it every tick)
  Fire(power)         #fires a bullet in the direction of your gun, 
                      #power is 0.1 - 3, this power will heat your gun
  Turn(degrees)       #turns the robot (and the gun and the radar), 
                      #max 10 degrees per tick
  TurnGun(degrees)    #turns the gun (and the radar), max 30 degrees per tick
  TurnRadar(degrees)  #turns the radar, max 60 degrees per tick
  Dead                #true if you are dead
  Say(msg)            #shows msg above the robot on screen
  Broadcast(msg)      #broadcasts msg to all bots (they receive 'broadcasts'
                      #events with the msg and rough direction)

These methods are intentionally of very basic nature, you are free to
unleash the whole power of ruby to create higher level functions.
(e.g. move_to, fire_at and so on)

Some words of explanation: The gun is mounted on the body, if you turn
the body the gun will follow. In a similar way the radar is mounted on
the gun. The radar scans everything it sweeps over in a single tick (100 
degrees if you turn your body, gun and radar in the same direction) but
will report only the distance of scanned robots, not the angle. If you 
want more precision you have to turn your radar slower.

CSRobots is implemented in C# using MPF should run on windows.

To start a match call:
CSRobots.exe [resolution] [#match] [-nogui] [-speed=<N>] [-timeout=<N>] 
      <FirstRobotClassName[.rb]> <SecondRobotClassName[.rb]> <...>
	  
  [resolution] (optional) should be of the form 640 480 or 800 600. 
    default is 800 800
  [match] (optional) to replay a match, put the match# here, including 
    the #sign.
  [-nogui] (optional) run the match without the gui, for highest possible 
    speed.(ignores speed value if present)
  [-speed=<N>] (optional, defaults to 1) updates GUI after every N ticks.  
    The higher the N, the faster the match will play.
  [-timeout=<N>] (optional, default 50000) number of ticks a match will 
    last at most.
    
  the names of the dlls containing robot classes have to match the class names of the robots
  (up to 8 robots)
  e.g. 'CSRobots.exe SittingDuck NervousDuck'
  or 'CSRobots.exe 600 600 #1234567890 SittingDuck NervousDuck'

If you want to run a tournament call:
