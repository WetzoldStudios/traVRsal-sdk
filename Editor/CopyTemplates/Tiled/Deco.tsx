<?xml version="1.0" encoding="UTF-8"?>
<tileset version="1.4" tiledversion="1.4.3" name="Deco" tilewidth="50" tileheight="50" tilecount="4" columns="0">
 <grid orientation="orthogonal" width="1" height="1"/>
 <tile id="3" type="/Base/MultiDisplay">
  <properties>
   <property name="ImagePool" value=""/>
   <property name="Materials" value=""/>
   <property name="Snap" type="bool" value="true"/>
   <property name="Variable" value=""/>
   <property name="Y" type="int" value="0"/>
  </properties>
  <image width="50" height="50" source="Master Images/multi-display.png"/>
 </tile>
 <tile id="4" type="/Base/Texture">
  <properties>
   <property name="Materials" value=""/>
   <property name="Scale" type="float" value="100"/>
   <property name="Snap" type="bool" value="true"/>
  </properties>
  <image width="50" height="50" source="Master Images/texture.png"/>
 </tile>
 <tile id="5" type="/Base/Scenery">
  <properties>
   <property name="SceneryName" value=""/>
  </properties>
  <image width="50" height="50" source="Master Images/scenery.png"/>
 </tile>
 <tile id="6">
  <properties>
   <property name="Move" value=""/>
   <property name="Offset" value=""/>
   <property name="Rotate" value=""/>
   <property name="Scale" value=""/>
   <property name="Snap" type="bool" value="false"/>
   <property name="Text" value=""/>
   <property name="Y" type="int" value="0"/>
  </properties>
  <image width="50" height="50" source="Master Images/object.png"/>
 </tile>
</tileset>
