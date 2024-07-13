using System;
using System.Collections;
using System.Collections.Generic;
using HAWStudio.Common;
using UnityEngine;

/// <summary>
/// This feedback allows you to chain any number of target FeedbackPlayers and play them in sequence, with optional delays before and after
/// </summary>
public class FeedbackChain : Feedback
{
    /// <summary>
    /// A class used to store and define items in a chain of MMF Players
    /// </summary>
    [Serializable]
    public class PlayerChainItem
    {
        [Tooltip("the target FeedbackPlayer")] 
        public FeedbackPlayer TargetPlayer;
        
        [Tooltip("a delay in seconds to wait for before playing this MMF Player (x) and after (y)")]
        [Vector("Before", "After")]
        public Vector2 Delay;
        
        [Tooltip("whether this player is active in the list or not. Inactive players will be skipped when playing the chain of players")]
        public bool Inactive = false;
    }
    
    [Tooltip("the list of MMF Player that make up the chain. The chain's items will be played from index 0 to the last in the list")]
    public List<PlayerChainItem> Players;

    /// the duration of this feedback is the duration of the chain
    public override float FeedbackDuration
    {
        get
        {
            if (Players == null || this.Players.Count == 0)
            {
                return 0f;
            }

            float totalDuration = 0f;
            foreach (PlayerChainItem item in this.Players)
            {
                if (item == null || item.TargetPlayer == null || item.Inactive)
                {
                    continue;
                }

                totalDuration += item.Delay.x;
                totalDuration += item.TargetPlayer.TotalDuration;
                totalDuration += item.Delay.y;
            }

            return totalDuration;
        }
    }

    protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1)
    {
        if (this.Players == null || this.Players.Count == 0)
        {
            return;
        }

        if (!this.Active)
        {
            return;
        }

        this.Owner.StartCoroutine(this.PlayChain());
    }

    /// <summary>
    /// Plays all players in the chain in sequence
    /// </summary>
    protected virtual IEnumerator PlayChain()
    {
        foreach (PlayerChainItem item in this.Players)
        {
            if (item == null || item.TargetPlayer == null || item.Inactive)
            {
                continue;
            }

            if (item.Delay.x > 0)
            {
                yield return WaitFor(item.Delay.x);
            }
            
            item.TargetPlayer.PlayFeedbacks();

            yield return WaitFor(item.TargetPlayer.TotalDuration);

            if (item.Delay.y > 0)
            {
                yield return WaitFor(item.Delay.y);
            }
        }
    }
}