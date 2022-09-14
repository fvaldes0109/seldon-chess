using System.Text;

namespace ChessLogic {

    public static class Position {

        public static int CoordinatesToIndex(string cor) {

            int col = (int)cor[0] - (int)'a';
            int row = int.Parse(cor[1].ToString()) - 1;

            return col + row * 8;
        }

        public static string IndexToCoordinates(int index) {

            char col = (char)(index % 8 + 'a');
            char row = (char)(index / 8 + '1');

            StringBuilder result = new StringBuilder();
            result.Append(col);
            result.Append(row);
            
            return result.ToString();
        }

        public static int Row(int index) {
            return index / 8;
        }

        public static int Col(int index) {
            return index % 8;
        }
    }
}
