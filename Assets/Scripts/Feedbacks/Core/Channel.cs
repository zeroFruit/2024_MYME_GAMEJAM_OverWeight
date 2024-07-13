using UnityEngine;

namespace HAWStudio.Common
{
    /// <summary>
    /// The possible modes used to identify a channel, either via an int or a Channel scriptable object
    /// </summary>
    public enum ChannelModes
    {
        Int,
        Channel,
    }

    /// <summary>
    /// A data structure used to pass channel information
    /// </summary>
    public class ChannelData
    {
        public ChannelModes ChannelMode;
        public int Channel;
        public Channel ChannelDefinition;

        public ChannelData(ChannelModes mode, int channel, Channel channelDefinition)
        {
            this.ChannelMode = mode;
            this.Channel = channel;
            this.ChannelDefinition = channelDefinition;
        }
    }

    public static class ChannelDataExtensions
    {
        public static ChannelData Set(this ChannelData data, ChannelModes mode, int channel, Channel channelDefinition)
        {
            data.ChannelMode = mode;
            data.Channel = channel;
            data.ChannelDefinition = channelDefinition;
            return data;
        }
    }

    public class Channel : ScriptableObject
    {
        public static bool Match(ChannelData dataA, ChannelModes modeB, int channelB, Channel channelDefinitionB)
        {
            if (dataA == null)
            {
                return true;
            }

            if (dataA.ChannelMode != modeB)
            {
                return false;
            }

            if (dataA.ChannelMode == ChannelModes.Int)
            {
                return dataA.Channel == channelB;
            }
            
            return dataA.ChannelDefinition == channelDefinitionB;
        }
    }
}