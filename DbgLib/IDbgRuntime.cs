namespace DbgLib;
public interface IDbgRuntime
{
    void SetGame(GameBase game);
    void ArrangePile(PlayerBase player, Pile pile);
    void PutPileAnywhereToAnotherPile(PlayerBase player, Pile pile, Pile anotherPile);
    int[] ChooseFromPile(PlayerBase player, Pile pile, int choiceCount, Predicate<CardBase> predicate);
    int[] ChooseFromOptions(PlayerBase player, int optionCount, int choiceCount);
    int[] ChooseDistinctFromOptions(PlayerBase player, int optionsCount, int choiceCount);
    int ChooseNumber(PlayerBase player, Predicate<int> predicate);
}
