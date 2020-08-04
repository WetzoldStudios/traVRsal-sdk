#if ODIN_INSPECTOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Reflection;

namespace traVRsal.SDK
{
    public class DamageInflictorAttributeProcessor<T> : OdinAttributeProcessor<T> where T : DamageInflictor
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            if (member.Name == "damage")
            {
                attributes.Add(new HideIfAttribute("instantKill"));
            }
            if (member.Name == "originTag")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
        }
    }

    public class StateChangerAttributeProcessor<T> : OdinAttributeProcessor<T> where T : StateChanger
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            if (member.Name == "key")
            {
                attributes.Add(new RequiredAttribute());
            }
            if (member.Name == "duration")
            {
                attributes.Add(new RequiredAttribute());
            }
        }
    }

    public class EnemyBehaviorAttributeProcessor<T> : OdinAttributeProcessor<T> where T : EnemyBehavior
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            if (member.Name == "tracker")
            {
                attributes.Add(new ShowIfAttribute("trackPlayer"));
            }
            if (member.Name == "trackSpeed")
            {
                attributes.Add(new ShowIfAttribute("trackPlayer"));
            }
            if (member.Name == "proximity")
            {
                attributes.Add(new ShowIfAttribute("proximityDamage"));
            }
        }
    }

    public class VariableAttributeProcessor<T> : OdinAttributeProcessor<T> where T : Variable
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            if (member.Name == "runtimeCreated")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "isComboPart")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "targetOrder")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "currentOrder")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "listeners")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "currentAutoIndex")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
        }
    }

    public class BasicEntityAttributeProcessor<T> : OdinAttributeProcessor<T> where T : BasicEntity
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            if (member.Name == "lowKey")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
        }
    }

    public class FloorAttributeProcessor<T> : OdinAttributeProcessor<T> where T : Floor
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            if (member.Name == "idx")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "positionInfoMarker")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "node")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "bounds")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "center")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
        }
    }

    public class MazeAttributeProcessor<T> : OdinAttributeProcessor<T> where T : Maze
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            if (member.Name == "holeCount")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "autoSeed")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
        }
    }

    public class WorldAttributeProcessor<T> : OdinAttributeProcessor<T> where T : World
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            if (member.Name == "worldData")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "zones")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "zoneTemplates")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
        }
    }

    public class ZoneAttributeProcessor<T> : OdinAttributeProcessor<T> where T : Zone
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            if (member.Name == "curSize")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "scenePath")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "musicPlayed")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "agentCount")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "idx")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "stencilId")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "layerIdx")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "navAgentId")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "node")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "bounds")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
            if (member.Name == "center")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
        }
    }

    public class TextFragmentAttributeProcessor<T> : OdinAttributeProcessor<T> where T : TextFragment
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            if (member.Name == "played")
            {
                attributes.Add(new ReadOnlyAttribute());
            }
        }
    }
}
#endif