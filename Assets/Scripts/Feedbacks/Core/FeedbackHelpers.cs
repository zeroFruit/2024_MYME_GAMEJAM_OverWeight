using System;
using System.Collections;
using UnityEngine;

namespace HAWStudio.Common
{
    public class FeedbackHelpers
    {
        /// <summary>
        /// Remaps a value x in interval [A,B], to the proportional value in interval [C,D]
        /// </summary>
        /// <param name="x">The value to remap.</param>
        /// <param name="A">the minimum bound of interval [A,B] that contains the x value</param>
        /// <param name="B">the maximum bound of interval [A,B] that contains the x value</param>
        /// <param name="C">the minimum bound of target interval [C,D]</param>
        /// <param name="D">the maximum bound of target interval [C,D]</param>
        public static float Remap(float x, float A, float B, float C, float D)
        {
            float remappedValue = C + (x - A) / (B - A) * (D - C);
            return remappedValue;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class FEnumConditionAttribute : PropertyAttribute
    {
        public string ConditionEnum = "";
        public bool Hidden = false;

        BitArray _bitArray = new BitArray(32);

        public FEnumConditionAttribute(string conditionBoolean,params int[] enumValues)
        {
            this.ConditionEnum = conditionBoolean;
            this.Hidden = true;

            for (int i = 0; i < enumValues.Length; i++)
            {
                this._bitArray.Set(enumValues[i], true);
            }
        }

        public bool ContainsBitFlag(int enumValue)
        {
            return this._bitArray.Get(enumValue);
        }
    }

    /// <summary>
    /// An attribute used to group inspector fields under common dropdowns
    /// Implementation inspired by Rodrigo Prinheiro's work, available at https://github.com/RodrigoPrinheiro/unityFoldoutAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class InspectorGroupAttribute : PropertyAttribute
    {
        public string GroupName;
        public bool GroupAllFieldsUntilNextGroupAttribute;
        public int GroupColorIndex;
        public bool RequiresSetup;
        public bool ClosedByDefault;

        public InspectorGroupAttribute(string groupName, bool groupAllFieldsUntilNextGroupAttribute = false, int groupColorIndex = 24, bool requiresSetup = false, bool closedByDefault = false)
        {
            if (groupColorIndex > 139) { groupColorIndex = 139; }

            this.GroupName = groupName;
            this.GroupAllFieldsUntilNextGroupAttribute = groupAllFieldsUntilNextGroupAttribute;
            this.GroupColorIndex = groupColorIndex;
            this.RequiresSetup = requiresSetup;
            this.ClosedByDefault = closedByDefault;
        }
    }
}