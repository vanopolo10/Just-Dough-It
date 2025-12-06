using System.Collections.Generic;
using JustDough;

public static class DoughCraftTree
{
    private static readonly Dictionary<DoughState, Dictionary<DoughCraftAction, DoughState>> _graph = new();

    static DoughCraftTree()
    {
        void Add(DoughState from, DoughCraftAction action, DoughState to)
        {
            if (_graph.TryGetValue(from, out var byAction) == false)
            {
                byAction = new Dictionary<DoughCraftAction, DoughState>();
                _graph[from] = byAction;
            }

            byAction[action] = to;
        }

        // Сырое тесто -> Продолговатое тесто (Прокатка)
        Add(DoughState.Raw, DoughCraftAction.Roll, DoughState.Flat);
        Add(DoughState.Raw, DoughCraftAction.RollSheer, DoughState.Flat);

        // Продолговатое тесто -> Сложенное один раз (ЛКМ)
        Add(DoughState.Flat, DoughCraftAction.Drag, DoughState.FlatFolded);
        
        // Продолговатое тесто -> Продолговатое длинное тесто (Прокатка)
        Add(DoughState.Flat, DoughCraftAction.Roll, DoughState.LongFlat);
        // Продолговатое тесто -> Плоское круглое тесто (ЛКМ)
        Add(DoughState.Flat, DoughCraftAction.RollSheer, DoughState.RoundFlat);
        
        // Сложенное один раз -> ПИРОЖОК (ПКМ)
        Add(DoughState.FlatFolded, DoughCraftAction.Click, DoughState.SimplePie);
        
        // Сложенное один раз -> Основа для сосиски (ЛКМ)
        Add(DoughState.FlatFolded, DoughCraftAction.Drag, DoughState.HotDogBase);
        
        // Основа для сосиски -> СОСИСКА В ТЕСТЕ (Комбо)
        Add(DoughState.HotDogBase, DoughCraftAction.ComboClick, DoughState.HotDog);
        
        // Сложенное один раз -> База для крутого пирожка (ЛКМ против)
        Add(DoughState.FlatFolded, DoughCraftAction.DragAgainst, DoughState.CoolPieBase);
        
        // База для конверта -> КРУТОЙ ПИРОЖОК (ЛКМ против)
        Add(DoughState.CoolPieBase, DoughCraftAction.ComboClick, DoughState.CoolPie);
    }

    public static bool TryGetNext(DoughState current, DoughCraftAction action, out DoughState next)
    {
        next = current;

        if (_graph.TryGetValue(current, out var byAction) &&
            byAction.TryGetValue(action, out var to))
        {
            next = to;
            return true;
        }

        return false;
    }
}
