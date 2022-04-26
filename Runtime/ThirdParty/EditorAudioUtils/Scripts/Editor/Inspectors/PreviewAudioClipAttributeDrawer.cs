// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreviewAudioClipAttributeDrawer.cs">
//   Copyright (c) 2021 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace JD.EditorAudioUtils
{
	/// <summary>
	/// Draws an audio clip field with an EditorAudioButton to preview it
	/// </summary>
	[CustomPropertyDrawer(typeof(PreviewAudioClipAttribute))]
	public class PreviewAudioClipAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.ObjectReference)
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			AudioClip audioObject = property.objectReferenceValue as AudioClip;
			position.width -= EditorAudioButton.Styles.ButtonWidth + EditorAudioButton.Styles.Padding;
			EditorGUI.PropertyField(position, property, label);
			position.x += position.width + EditorAudioButton.Styles.Padding;
			position.width = EditorAudioButton.Styles.ButtonWidth;
			
			EditorGUI.BeginDisabledGroup(audioObject == null);
			EditorAudioButton.DrawAudioButton(position, audioObject);
			EditorGUI.EndDisabledGroup();
		}
	}
}