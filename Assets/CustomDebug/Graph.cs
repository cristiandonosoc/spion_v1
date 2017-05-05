using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomDebug {

    public class Graph {
        private static object _lock = new object();
        private static Graph _instance;

        public static Graph Instance {
            get {
                lock (_lock) {
                    if (_instance == null) {
                        _instance = new Graph();
                    }
                    return _instance;
                }
            }
        }

        public static float YMin = -1;
        public static float YMax = 1;

        public const int MAX_HISTORY = 2048;
        public const int MAX_CHANNELS = 3;

        private Dictionary<string, GraphChannel> _channelDict;

        public Graph() {
            _channelDict = new Dictionary<string, GraphChannel>();
        }

        public int Size {
            get { return _channelDict.Values.Count; }
        }

        public GraphChannel AddChannel(string channelName, Color color, int size = MAX_HISTORY) {
            if (_channelDict.ContainsKey(channelName)) {
                return _channelDict[channelName];
            }
            GraphChannel channel = new GraphChannel(channelName, size, color);
            _channelDict[channelName] = channel;
            return channel;
        }

        public bool RemoveChannel(string channelName) {
            return _channelDict.Remove(channelName);
        }

        public GraphChannel GetChannel(string channelName) {
            GraphChannel channel;
            _channelDict.TryGetValue(channelName, out channel);
            return channel;
        }

        public GraphChannelSet GetChannelSet(params string[] channelNames) {
            List<GraphChannel> channelList = new List<GraphChannel>();
            if (channelNames.Length == 0) {
                foreach (GraphChannel channel in _channelDict.Values) {
                    channelList.Add(channel);
                }
            } else {
                foreach (string channelName in channelNames) {
                    GraphChannel channel;
                    _channelDict.TryGetValue(channelName, out channel);
                    if (channel != null) {
                        channelList.Add(channel);
                    }
                }
            }
            return new GraphChannelSet(channelList.ToArray());
        }
    }

    public class GraphChannelSet {
        public GraphChannel[] Channels { get; private set; }
        public float ActiveMin {
            get {
                float min = 0;
                foreach (GraphChannel channel in Channels) {
                    if (!channel.Active) { continue; }
                    if (channel.Min < min) { min = channel.Min; }
                }
                return min;
            }
        }

        public float ActiveMax {
            get {
                float max = 0;
                foreach (GraphChannel channel in Channels) {
                    if (!channel.Active) { continue; }
                    if (channel.Max > max) { max = channel.Max; }
                }
                return max;
            }
        }

        public GraphChannelSet(GraphChannel[] channels) {
            Channels = channels;
        }
    }
}
