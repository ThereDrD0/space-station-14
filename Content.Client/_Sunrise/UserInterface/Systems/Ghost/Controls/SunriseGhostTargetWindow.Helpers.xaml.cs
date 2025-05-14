using System.Linq;
using Content.Shared.Ghost;
using GhostWarpGlobalAntagonist = Content.Shared.Ghost.SharedGhostSystem.GhostWarpGlobalAntagonist;

namespace Content.Client._Sunrise.UserInterface.Systems.Ghost.Controls;

public sealed partial class SunriseGhostTargetWindow
{
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
}
