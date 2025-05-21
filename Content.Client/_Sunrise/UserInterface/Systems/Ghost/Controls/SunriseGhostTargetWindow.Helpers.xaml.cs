using System.Linq;
using Content.Shared.Ghost;
using Content.Shared.Roles;
using GhostWarpPlayer = Content.Shared.Ghost.SharedGhostSystem.GhostWarpPlayer;
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

    private string GeneratePlayerLabel(GhostWarpPlayer warp)
    {
        var playerName = TruncateWithEllipsis(warp.Name, MaxLenght);
        var jobIcon = _chatIcons.GetJobIcon(warp.JobId, 3);

        return $"{jobIcon} {playerName}";
    }

    private string GeneratePlayerTooltip(GhostWarpPlayer warp)
    {
        var jobName = _prototype.TryIndex(warp.JobId, out var jobPrototype)
            ? jobPrototype.LocalizedName
            : Loc.GetString("ghost-panel-unknown-job");

        // К сожалению тултипы это очко, я не хочу туда лезть с ричтекстом
        // var jobIcon = _chatIcons.GetJobIcon(warp.JobId, 3);

        return GenerateGenericTooltip(warp.Name, jobName.ToUpperInvariant());
    }

    private static string GenerateGenericTooltip(string fullName, string additionalInfo)
    {
        return $"{fullName}\n{additionalInfo}";
    }

    private Dictionary<DepartmentPrototype, List<GhostWarpPlayer>> GroupPlayersByDepartment(List<GhostWarpPlayer> players)
    {
        var result = new Dictionary<DepartmentPrototype, List<GhostWarpPlayer>>();

        foreach (var player in players)
        {
            if (!_prototype.TryIndex(player.DepartmentId, out var department))
                continue;

            if (!result.TryGetValue(department, out var list))
            {
                list = [];
                result[department] = list;
            }

            list.Add(player);
        }

        return result;
    }
}
