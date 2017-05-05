using System.Collections;
using UnityEngine;

namespace CustomDebug {
    public class GraphChannel : IEnumerable {
        public Color ChannelColor { get; set; }

        /// <summary>
        /// Channels are active by default
        /// </summary>
        public bool Active { get; set; }

        private float[] _data;
        private int _writeIndex;

        public int Size { get { return _data.Length; } }
        public string Name { get; private set; }

        public float Max { get; private set; }
        public float Min { get; private set; }

        public GraphChannel(string name, int size, Color color) {
            _data = new float[size];
            ChannelColor = color;
            Active = true;
            Name = name;
            Min = 0;
            Max = 0;
            _writeIndex = 0;
        }

        public void Feed(float val) {
            _data[_writeIndex] = val;
            _writeIndex++;
            if (_writeIndex >= _data.Length) {
                _writeIndex = 0;
            }

            if (val < Min) { Min = val; }
            if (val > Max) { Max = val; }
        }

        public float this[int index] {
            get {
                int newIndex = (_writeIndex + index);
                if (newIndex >= _data.Length) {
                    newIndex -= _data.Length;
                } else if (newIndex < 0) {
                    newIndex += _data.Length;
                }
                return _data[newIndex];
            }
        }


        public IEnumerator GetEnumerator() {
            return new GraphChannelEnumerator(this);
        }

        public IEnumerator GetReverseEnumerator() {
            return new GraphChannelReverseEnumerator(this);
        }
    }

    public class GraphChannelEnumerator : IEnumerator {

        private int _readIndex;
        private GraphChannel _channel;

        public GraphChannelEnumerator(GraphChannel channel) {
            _channel = channel;
        }

        public object Current { get { return _channel[_readIndex]; } }

        public bool MoveNext() {
            _readIndex++;
            return _readIndex < _channel.Size;
        }

        public void Reset() {
            // IEnumerators are positioned before the first element
            _readIndex = -1;
        }
    }

    public class GraphChannelReverseEnumerator : IEnumerator {
        private int _readIndex;
        private GraphChannel _channel;

        public GraphChannelReverseEnumerator(GraphChannel channel) {
            _channel = channel;
        }

        public object Current { get { return _channel[-_readIndex]; } }

        public bool MoveNext() {
            _readIndex++;
            return _readIndex < _channel.Size;
        }

        public void Reset() {
            // IEnumerators are positioned before the first element
            _readIndex = -1;
        }
    }

}
