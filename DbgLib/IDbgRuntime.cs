namespace DbgLib;
public interface IDbgRuntime
{
    void SetGame(GameBase game);
    void ArrangePile(PlayerBase player, Pile pile);
    int[] ChooseFromPile(PlayerBase player, Pile pile, int number);
    int ChooseNumber(PlayerBase player, Predicate<int> predicate);
    int ChooseFromList(PlayerBase player, int[] list);
}
