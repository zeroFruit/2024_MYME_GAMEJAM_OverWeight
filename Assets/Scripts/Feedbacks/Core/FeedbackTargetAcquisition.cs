using UnityEngine;

namespace HAWStudio.Common
{
    /// <summary>
    /// A class collecting target acquisition settings
    /// </summary>
    [System.Serializable]
    public class FeedbackTargetAcquisition
    {
        public enum Modes { None, Self, AnyChild, ChildAtIndex, Parent, FirstReferenceHolder, PreviousReferenceHolder, ClosestReferenceHolder, NextReferenceHolder, LastReferenceHolder }
        
        [Tooltip("the selected mode for target acquisition\n"+
                 "None : nothing will happen\n"+
                 "Self : the target will be picked on the MMF Player's game object\n"+
                 "AnyChild : the target will be picked on any of the MMF Player's child objects\n"+
                 "ChildAtIndex : the target will be picked on the child at index X of the MMF Player\n"+
                 "Parent : the target will be picked on the first parent where a matching target is found\n"+
                 "Various reference holders : the target will be picked on the specified reference holder in the list " +
                 "(either the first one, previous : first one found before this feedback in the list, closest in any direction from this feedback, the next one found, or the last one in the list)")]
        public Modes Mode = Modes.None;
        
        [FEnumCondition("Mode", (int)Modes.ChildAtIndex)]
        public int ChildIndex = 0;
        
        
    }
}