using System.Linq;
using Content.Shared.Ghost;
using GhostWarpGlobalAntagonist = Content.Shared.Ghost.SharedGhostSystem.GhostWarpGlobalAntagonist;

namespace Content.Client._Sunrise.UserInterface.Systems.Ghost.Controls;

public sealed partial class SunriseGhostTargetWindow
{
    private const string Ellipsis = "...";

    private static List<List<GhostWarpGlobalAntagonist>> SortAntagsByWeight(List<GhostWarpGlobalAntagonist> antagonists)
    {
        return antagonists
            .GroupBy(a => a.Weight)
            .OrderBy(g => g.Key)
            .Select(g => g.ToList())
            .ToList();
    }

    /// <summary>
    /// Сортирует по имени
    /// </summary>
    private static List<T> GetSortedByName<T>(List<T> items) where T : SharedGhostSystem.INamedGhostWarp
    {
        return items
            .OrderBy(i => i.Name)
            .ToList();
    }

    /// <summary>
    /// Обрезает строку до данного значения. Вместо обрезанного текста ставит троеточие
    /// </summary>
    private static string TruncateWithEllipsis(string? input, int maxLength)
    {
        if (string.IsNullOrEmpty(input) || maxLength <= 0)
            return string.Empty;

        if (input.Length <= maxLength)
            return input;

        if (maxLength <= Ellipsis.Length)
            return Ellipsis.AsSpan(0, maxLength).ToString();

        var cutLength = maxLength - Ellipsis.Length;

        return string.Concat(input.AsSpan(0, cutLength), Ellipsis);
    }
}
