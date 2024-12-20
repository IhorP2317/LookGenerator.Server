namespace LookGenerator.Domain.Entities ;

    [Flags]
    public enum Colour : ushort
    {
        None = 0,
        White = 1 << 0,  
        Red = 1 << 1,    
        Black = 1 << 2,   
        Blue = 1 << 3,   
        Grey = 1 << 4,    
        Beige = 1 << 5,   
        Pink = 1 << 6,    
        Violet = 1 << 7,  
        Yellow = 1 << 8,  
        Brown = 1 << 9,   
        Green = 1 << 10,  
        Orange = 1 << 11, 
        Maroon = 1 << 12,
        Multicolor = White | Red | Black | Blue | Grey | Beige | Pink | Violet | Yellow | Brown | Green | Orange | Maroon
    }