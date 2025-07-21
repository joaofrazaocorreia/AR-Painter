using System.Collections.Generic;
using UnityEngine;

public static class ColorManager
{
    public static Dictionary<FilteredColors, Color> filteredColors =
        new Dictionary<FilteredColors, Color>
    {
            { FilteredColors.Red,        new Color(200f, 0f, 0f) },
            { FilteredColors.Green,      new Color(0f, 200f, 0f) },
            { FilteredColors.Blue,       new Color(0f, 0f, 200f) },
            { FilteredColors.Yellow,     new Color(200f, 200f, 0f) },
            { FilteredColors.Pink,       new Color(200f, 0f, 200f) },
            { FilteredColors.Cyan,       new Color(0f, 200f, 200f) },
            { FilteredColors.White,      new Color(200f, 200f, 200f) },
            { FilteredColors.Black,      new Color(0f, 0f, 0f) },
            { FilteredColors.Grey,       new Color(100f, 100f, 100f) },
            { FilteredColors.Orange,     new Color(200f, 100f, 0f) },
            { FilteredColors.LimeGreen,  new Color(0f, 200f, 100f) },
            { FilteredColors.Purple,     new Color(100f, 0f, 200f) },
            { FilteredColors.Brown,      new Color(100f, 100f, 0f) },
            { FilteredColors.DarkRed,    new Color(100f, 0f, 0f) },
            { FilteredColors.DarkGreen,  new Color(0f, 100f, 0f) },
            { FilteredColors.DarkBlue,   new Color(0f, 0f, 100f) }
    };
}
