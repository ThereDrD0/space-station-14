using System.Linq;
using Content.Shared.Ghost;
using GhostWarpGlobalAntagonist = Content.Shared.Ghost.SharedGhostSystem.GhostWarpGlobalAntagonist;

namespace Content.Client._Sunrise.UserInterface.Systems.Ghost.Controls;

public sealed partial class SunriseGhostTargetWindow
{
    private static HashSet<HashSet<GhostWarpGlobalAntagonist>> SortAntagsByWeight(HashSet<GhostWarpGlobalAntagonist> antagonists)
    {
        return antagonists
            .GroupBy(a => a.Weight)
            .OrderBy(g => g.Key)
            .Select(g => g.ToHashSet())
            .ToHashSet();
    }

    /// <summary>
    /// Сортирует по имени
    /// </summary>
    private static HashSet<T> GetSortedByName<T>(HashSet<T> items) where T : SharedGhostSystem.INamedGhostWarp
    {
        return items
            .OrderBy(i => i.Name)
            .ToHashSet();
    }

}
