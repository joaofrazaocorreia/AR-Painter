using System.Collections.Generic;
using UnityEngine;

public static class ColorLibrary
{
    public static Dictionary<FilteredColors, (float, float, float)> filteredColors =
        new Dictionary<FilteredColors, (float, float, float)>
    {
            { FilteredColors.None,       (-100, -100f, -100f) },
            { FilteredColors.Red,        (210f, 0f, 0f) },
            { FilteredColors.Green,      (0f, 210f, 0f) },
            { FilteredColors.Blue,       (0f, 0f, 210f) },
            { FilteredColors.Yellow,     (240f, 210f, 0f) },
            { FilteredColors.Pink,       (240f, 0f, 210f) },
            { FilteredColors.Cyan,       (0f, 210f, 240f) },
            { FilteredColors.Teal,       (0f, 240f, 210f) },
            { FilteredColors.Violet,     (150f, 0f, 210f) },
            { FilteredColors.White,      (210f, 210f, 210f) },
            { FilteredColors.Black,      (0f, 0f, 0f) },
            { FilteredColors.LightGrey,  (120f, 120f, 120f) },
            { FilteredColors.DarkGrey,   (60f, 60f, 60f) },
            { FilteredColors.Orange,     (210f, 90f, 0f) },
            { FilteredColors.LimeGreen,  (150f, 240f, 0f) },
            { FilteredColors.Purple,     (90f, 0f, 90f) },
            { FilteredColors.Brown,      (120f, 60f, 0f) },
            { FilteredColors.DarkRed,    (90f, 0f, 0f) },
            { FilteredColors.DarkGreen,  (0f, 90f, 0f) },
            { FilteredColors.DarkBlue,   (0f, 0f, 90f) }
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
