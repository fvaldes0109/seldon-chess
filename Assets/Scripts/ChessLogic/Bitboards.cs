namespace ChessLogic {

    public class Bitboards {

        public ulong[,] Pieces { get; private set; }
        public ulong[] Occupied { get; private set; }
        public ulong GeneralOccupied { get; set; }

        public Bitboards() {

            Pieces = new ulong[2, 6];
            Occupied = new ulong[2];
        }

        private Bitboards(ulong[,] pieces, ulong[] occupied, ulong generalOccupied) {

            Pieces = (ulong[,])pieces.Clone();
            Occupied = (ulong[])occupied.Clone();
            GeneralOccupied = generalOccupied;
        }

        public object Clone() {
            return new Bitboards(Pieces, Occupied, GeneralOccupied);
        }
    }
}
