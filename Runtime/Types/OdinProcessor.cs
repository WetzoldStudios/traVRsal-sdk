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
}
#endif