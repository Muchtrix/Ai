namespace Jungle.Heuristics {
    interface IHeuristic {
        double Evaluate(GameState state, Player player);
    }
}