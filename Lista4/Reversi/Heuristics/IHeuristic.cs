namespace Reversi.Heuristics {
    interface IHeuristic {
        double EvaluateBoard(GameState state, Piece color);
        
    }
}