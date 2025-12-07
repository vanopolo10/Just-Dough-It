using System.Collections.Generic;
using JustDough;

public static class DoughCraftTree
{
    private static readonly Dictionary<DoughState, Dictionary<DoughCraftAction, DoughState>> s_graph = new();

    static DoughCraftTree()
    {
        void Add(DoughState from, DoughCraftAction action, DoughState to)
        {
            if (s_graph.TryGetValue(from, out var byAction) == false)
            {
                byAction = new Dictionary<DoughCraftAction, DoughState>();
                s_graph[from] = byAction;
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
        
        // Ветка норм теста
        // Пирожок
        Add(DoughState.FlatFolded, DoughCraftAction.Click, DoughState.SimplePie);
        
        // Сосиска в тесте
        Add(DoughState.FlatFolded, DoughCraftAction.Drag, DoughState.HotDogBase);
        Add(DoughState.HotDogBase, DoughCraftAction.ComboClick, DoughState.HotDog);
        
        // Крутой пирожок
        Add(DoughState.FlatFolded, DoughCraftAction.DragAgainst, DoughState.CoolPieBase);
        Add(DoughState.CoolPieBase, DoughCraftAction.ComboClick, DoughState.CoolPie);
        
        // Ветка долгого теста (синабонов)
        Add(DoughState.LongFlat, DoughCraftAction.Drag, DoughState.LongFlatFolded);
        Add(DoughState.LongFlatFolded, DoughCraftAction.Drag, DoughState.CinnabonBase);
        Add(DoughState.LongFlatFolded, DoughCraftAction.DragAgainst, DoughState.DoubleCinnabonBase);
        
        Add(DoughState.CinnabonBase, DoughCraftAction.ComboClick, DoughState.Cinnabon);
        Add(DoughState.DoubleCinnabonBase, DoughCraftAction.ComboClick, DoughState.DoubleCinnabon);
    }

    public static bool TryGetNext(DoughState current, DoughCraftAction action, out DoughState next)
    {
        next = current;

        if (s_graph.TryGetValue(current, out var byAction) == false ||
            byAction.TryGetValue(action, out var to) == false) 
            return false;
        
        next = to;
        return true;
    }
}
