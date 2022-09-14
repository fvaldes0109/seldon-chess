using System;
using System.Collections;
using System.Collections.Generic;

namespace ChessLogic {

    public class IndexIterator : IEnumerator<int> {

        private int _position = -1;

        public int Current { get {
            
            if (_position >= 0) return _position;
            else throw new IndexOutOfRangeException("Enumerator out of position");
        }}

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext() {

            if (_position == -1) {
                _position = 56;
                return true;
            }

            _position++;
            if ((_position) % 8 == 0) _position -= 16;
            return _position >= 0;
        }

        public void Reset() {
            _position = -1;
        }
    }
}
