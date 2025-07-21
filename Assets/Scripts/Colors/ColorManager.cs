using System.Collections.Generic;
using UnityEngine;

public static class ColorManager
{
    public static Dictionary<FilteredColors, Color> filteredColors =
        new Dictionary<FilteredColors, Color>
    {
            { FilteredColors.Red,        PresetColors.Red },
            { FilteredColors.Green,      PresetColors.Blue },
            { FilteredColors.Blue,       PresetColors.Green },
            { FilteredColors.Yellow,     PresetColors.Yellow },
            { FilteredColors.Pink,       PresetColors.Pink },
            { FilteredColors.Cyan,       PresetColors.Cyan },
            { FilteredColors.White,      PresetColors.White },
            { FilteredColors.Black,      PresetColors.Black },
            { FilteredColors.Grey,       PresetColors.Grey },
            { FilteredColors.Orange,     PresetColors.Orange },
            { FilteredColors.LimeGreen,  PresetColors.LimeGreen },
            { FilteredColors.Purple,     PresetColors.Purple },
            { FilteredColors.Brown,      PresetColors.Brown },
            { FilteredColors.DarkRed,    PresetColors.DarkRed },
            { FilteredColors.DarkGreen,  PresetColors.DarkGreen },
            { FilteredColors.DarkBlue,   PresetColors.DarkBlue }
    };
}
