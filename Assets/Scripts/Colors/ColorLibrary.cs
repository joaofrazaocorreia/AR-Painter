using System.Collections.Generic;
using UnityEngine;

public static class ColorLibrary
{
    public static Dictionary<FilteredColors, (float, float, float)> filteredColors =
        new Dictionary<FilteredColors, (float, float, float)>
    {
            { FilteredColors.None,       (-100, -100f, -100f) },
            { FilteredColors.Red,        (200f, 0f, 0f) },
            { FilteredColors.Green,      (0f, 200f, 0f) },
            { FilteredColors.Blue,       (0f, 0f, 200f) },
            { FilteredColors.Yellow,     (200f, 200f, 0f) },
            { FilteredColors.Pink,       (200f, 0f, 200f) },
            { FilteredColors.Cyan,       (0f, 200f, 200f) },
            { FilteredColors.White,      (200f, 200f, 200f) },
            { FilteredColors.Black,      (0f, 0f, 0f) },
            { FilteredColors.Grey,       (100f, 100f, 100f) },
            { FilteredColors.Orange,     (200f, 100f, 0f) },
            { FilteredColors.LimeGreen,  (100f, 200f, 0f) },
            { FilteredColors.Purple,     (100f, 0f, 200f) },
            { FilteredColors.Brown,      (100f, 100f, 0f) },
            { FilteredColors.DarkRed,    (100f, 0f, 0f) },
            { FilteredColors.DarkGreen,  (0f, 100f, 0f) },
            { FilteredColors.DarkBlue,   (0f, 0f, 100f) }
    };

    public static Color BinaryColor(float r, float g, float b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    public static Color BinaryColor((float, float, float) rgb)
    {
        return new Color(rgb.Item1 / 255f, rgb.Item2 / 255f, rgb.Item3 / 255f);
    }

    public static Color RGBColor(float r, float g, float b)
    {
        return new Color(r, g, b);
    }

    public static Color RGBColor((float, float, float) rgb)
    {
        return new Color(rgb.Item1, rgb.Item2, rgb.Item3);
    }
}
