<?xml version="1.0" encoding="UTF-8"?>
<tileset version="1.5" tiledversion="1.7.1" name="Master" tilewidth="50" tileheight="50" tilecount="27" columns="0">
 <grid orientation="orthogonal" width="1" height="1"/>
 <tile id="2" type="/Base/Ceiling">
  <properties>
   <property name="Anchor" value=""/>
   <property name="AutoFill" type="bool" value="true"/>
   <property name="DontPropagate-Virtual" type="bool" value="false"/>
   <property name="Virtual" type="bool" value="false"/>
  </properties>
  <image width="50" height="50" source="Master Images/ceiling.png"/>
 </tile>
 <tile id="3" type="/Base/Ground">
  <properties>
   <property name="Anchor" value=""/>
   <property name="AutoFill" type="bool" value="true"/>
   <property name="DontPropagate-Virtual" type="bool" value="false"/>
   <property name="Virtual" type="bool" value="false"/>
  </properties>
  <image width="50" height="50" source="Master Images/ground.png"/>
 </tile>
 <tile id="4" type="/Base/Wall">
  <properties>
   <property name="Anchor" value=""/>
   <property name="AutoFill" type="bool" value="true"/>
   <property name="CheckOccupiedNeighbor" type="bool" value="true"/>
   <property name="FillHoles" type="bool" value="true"/>
   <property name="SkipOccupied" value="/Base/Door"/>
  </properties>
  <image width="50" height="50" source="Master Images/wall.png"/>
 </tile>
 <tile id="5" type="/Base/Corner">
  <properties>
   <property name="FillHoles" type="bool" value="false"/>
  </properties>
  <image width="50" height="50" source="Master Images/corner.png"/>
 </tile>
 <tile id="6" type="/Base/Corridor">
  <properties>
   <property name="AutoFill" type="bool" value="true"/>
   <property name="FillHoles" type="bool" value="true"/>
  </properties>
  <image width="50" height="50" source="Master Images/corridor.png"/>
 </tile>
 <tile id="7" type="/Base/Transition">
  <properties>
   <property name="Anchor" value=""/>
   <property name="MazeTransitions" type="int" value="0"/>
   <property name="Music" value=""/>
   <property name="OneWay" type="bool" value="false"/>
   <property name="RandomMusic" value=""/>
   <property name="Reactivatable" type="bool" value="false"/>
   <property name="Snap" type="bool" value="true"/>
   <property name="Speak" value=""/>
   <property name="TargetZone" value=""/>
   <property name="ZoneExit" type="bool" value="true"/>
  </properties>
  <image width="50" height="50" source="Master Images/transition.png"/>
 </tile>
 <tile id="8" type="Elevator">
  <properties>
   <property name="Anchor" value=""/>
   <property name="Variable" value="[auto]"/>
  </properties>
  <image width="50" height="50" source="Master Images/elevator.png"/>
 </tile>
 <tile id="10" type="/Base/CorridorEnd">
  <image width="50" height="50" source="Master Images/corridor-end.png"/>
 </tile>
 <tile id="12" type="/Base/Door">
  <properties>
   <property name="Anchor" value=""/>
   <property name="Variable" value=""/>
  </properties>
  <image width="50" height="50" source="Master Images/door.png"/>
 </tile>
 <tile id="13" type="/Base/Button">
  <properties>
   <property name="Anchor" value=""/>
   <property name="Snap" type="bool" value="false"/>
   <property name="Variable" value=""/>
  </properties>
  <image width="50" height="50" source="Master Images/button.png"/>
 </tile>
 <tile id="14" type="/Base/Signal">
  <properties>
   <property name="Anchor" value=""/>
   <property name="Y" type="int" value="0"/>
  </properties>
  <image width="50" height="50" source="Master Images/signal.png"/>
 </tile>
 <tile id="16" type="/Base/maze-exit">
  <properties>
   <property name="Anchor" value=""/>
   <property name="Virtual" type="bool" value="true"/>
  </properties>
  <image width="50" height="50" source="Master Images/maze-exit.png"/>
 </tile>
 <tile id="17" type="/Base/maze-obstacle">
  <properties>
   <property name="Virtual" type="bool" value="true"/>
  </properties>
  <image width="50" height="50" source="Master Images/maze-obstacle.png"/>
 </tile>
 <tile id="18" type="/Base/maze">
  <properties>
   <property name="AutoFill" type="bool" value="true"/>
   <property name="FillHoles" type="bool" value="true"/>
   <property name="Virtual" type="bool" value="true"/>
  </properties>
  <image width="50" height="50" source="Master Images/maze.png"/>
 </tile>
 <tile id="19" type="/Base/Transition">
  <properties>
   <property name="Anchor" value=""/>
   <property name="Virtual" type="bool" value="true"/>
   <property name="ZoneEntry" type="bool" value="true"/>
  </properties>
  <image width="50" height="50" source="Master Images/start.png"/>
 </tile>
 <tile id="21" type="/Base/no-spawn">
  <properties>
   <property name="Virtual" type="bool" value="true"/>
  </properties>
  <image width="50" height="50" source="Master Images/no-spawn.png"/>
 </tile>
 <tile id="22">
  <properties>
   <property name="Socket" type="bool" value="true"/>
   <property name="ValidSockets" value=""/>
  </properties>
  <image width="50" height="50" source="Master Images/item.png"/>
 </tile>
 <tile id="23" type="/Base/Switch">
  <properties>
   <property name="Anchor" value=""/>
   <property name="Variable" value=""/>
   <property name="Y" type="int" value="0"/>
  </properties>
  <image width="50" height="50" source="Master Images/switch.png"/>
 </tile>
 <tile id="24" type="/Base/StoneWall">
  <properties>
   <property name="Anchor" value=""/>
   <property name="AutoFill" type="bool" value="true"/>
   <property name="CheckOccupiedNeighbor" type="bool" value="true"/>
   <property name="FillHoles" type="bool" value="true"/>
   <property name="SkipOccupied" value="/Base/Door"/>
  </properties>
  <image width="50" height="50" source="Master Images/stonewall.png"/>
 </tile>
 <tile id="25" type="/Base/StoneCorner">
  <properties>
   <property name="FillHoles" type="bool" value="false"/>
  </properties>
  <image width="50" height="50" source="Master Images/stonecorner.png"/>
 </tile>
 <tile id="26" type="/Base/StoneCorridor">
  <properties>
   <property name="AutoFill" type="bool" value="true"/>
   <property name="FillHoles" type="bool" value="true"/>
  </properties>
  <image width="50" height="50" source="Master Images/stonecorridor.png"/>
 </tile>
 <tile id="27" type="/Base/StoneCorridorEnd">
  <image width="50" height="50" source="Master Images/stonecorridor-end.png"/>
 </tile>
 <tile id="28" type="/Base/Redirection">
  <properties>
   <property name="Anchor" value=""/>
   <property name="Duration" type="float" value="3"/>
   <property name="TargetLocation" value=""/>
  </properties>
  <image width="50" height="50" source="Master Images/redirection.png"/>
 </tile>
 <tile id="29" type="/Base/Location">
  <properties>
   <property name="Anchor" value=""/>
   <property name="LocationId" value=""/>
  </properties>
  <image width="50" height="50" source="Master Images/location.png"/>
 </tile>
 <tile id="30" type="/Base/Trigger">
  <properties>
   <property name="Anchor" value=""/>
   <property name="AudioEffect" value=""/>
   <property name="Speak" value=""/>
   <property name="Variable" value=""/>
  </properties>
  <image width="50" height="50" source="Master Images/trigger.png"/>
 </tile>
 <tile id="31" type="/Base/Explicit-Spawn">
  <properties>
   <property name="Virtual" type="bool" value="true"/>
  </properties>
  <image width="50" height="50" source="Master Images/explicit-spawn.png"/>
 </tile>
 <tile id="32" type="/Base/Waypoint">
  <properties>
   <property name="Anchor" value=""/>
   <property name="Key" value=""/>
  </properties>
  <image width="50" height="50" source="Master Images/waypoint.png"/>
 </tile>
</tileset>
