public enum DoughState
{
    // Верхний уровень
    Raw,                // "Сырое тесто"

    // Ветки с формами теста
    Flat,
    RoundFlat,
    LongFlat,
    
    // Ветка Flat
    FlatFolded,
    
    SimplePie,
    
    HotDogBase,
    HotDog,
    
    CoolPieBase,
    CoolPie,
    
    // Long flat branch
    
    LongFlatFolded, 
    
    CinnabonBase,
    Cinnabon,
    
    DoubleCinnabonBase,
    DoubleCinnabon,

    // Round flat branch
    RoundFlatCutting,
    RoundFlatCut,

    // Rose branch
    Rose_3,
    Rose_2,
    Rose_1,
    RoseBase,
    Rose,

    // Square flat branch
    SquareDoughCutting,
    SquareDoughCut,

    // Boat branch
    Boat_2,
    Boat_1,
    BoatBase,
    Boat
}