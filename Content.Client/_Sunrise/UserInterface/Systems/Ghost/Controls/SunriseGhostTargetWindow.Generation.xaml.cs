using System.Linq;
using Content.Client._Sunrise.UserInterface.Controls;
using Content.Shared.Roles;
using Robust.Client.UserInterface.Controls;
using GhostWarpPlayer = Content.Shared.Ghost.SharedGhostSystem.GhostWarpPlayer;
using GhostWarpPlace = Content.Shared.Ghost.SharedGhostSystem.GhostWarpPlace;
using GhostWarpGlobalAntagonist = Content.Shared.Ghost.SharedGhostSystem.GhostWarpGlobalAntagonist;

namespace Content.Client._Sunrise.UserInterface.Systems.Ghost.Controls;

public sealed partial class SunriseGhostTargetWindow
{
    private static readonly Color AntagonistButtonColor = Color.FromHex("#7F4141");
    private static readonly Color PlaceButtonColor = Color.FromHex("#969696");

    private const int DefaultButtonWidth = 200;
    private const int DefaultButtonHeight = 35;
    private const float DefaultTooltipDelay = 0.1f;
    private const int MaxLenght = 15;

    // TODO: Дедупликация одинакового кода
    private void AddPlayerButtons(List<GhostWarpPlayer> warps, string text)
    {
        if (warps.Count == 0)
            return;

        var bigGrid = new GridContainer();

        var bigLabel = new Label
        {
            Text = Loc.GetString(text),
            StyleClasses = { "LabelBig" },
        };

        bigGrid.AddChild(bigLabel);

        var sortedWarps = GroupPlayersByDepartment(warps)
            .OrderBy(kvp => kvp.Key.Weight)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        foreach (var (department, players) in sortedWarps)
        {
            var departmentGrid = new GridContainer
            {
                Columns = 5,
            };

            var departmentLabel = new Label
            {
                Text = Loc.GetString(department.Name) + ": " + players.Count,
                StyleClasses = { "LabelSecondaryColor" },
            };

            foreach (var player in players)
            {
                var playerButton = new RichTextButton
                {
                    ModulateSelfOverride = department.Color,
                    Text = GeneratePlayerLabel(player),
                    TextAlign = Label.AlignMode.Right,
                    HorizontalAlignment = HAlignment.Center,
                    VerticalAlignment = VAlignment.Center,
                    SizeFlagsStretchRatio = 1,
                    ToolTip = GeneratePlayerTooltip(player),
                    TooltipDelay = DefaultTooltipDelay,
                    SetWidth = DefaultButtonWidth,
                    SetHeight = DefaultButtonHeight,
                };

                playerButton.OnPressed += _ => WarpClicked?.Invoke(player.Entity);

                departmentGrid.AddChild(playerButton);
            }

            bigGrid.AddChild(departmentLabel);
            bigGrid.AddChild(departmentGrid);
        }

        GhostTeleportContainer.AddChild(bigGrid);
    }

    private void AddPlaceButtons(List<GhostWarpPlace> places, string text)
    {
        if (places.Count == 0)
            return;

        var bigGrid = new GridContainer();

        var bigLabel = new Label
        {
            Text = Loc.GetString(text),
            StyleClasses = { "LabelBig" }
        };
        bigGrid.AddChild(bigLabel);

        var placesGrid = new GridContainer
        {
            Columns = 5,
        };

        var countLabel = new Label
        {
            Text = Loc.GetString("ghost-teleport-menu-count-label") + ": " + places.Count,
            StyleClasses = { "LabelSecondaryColor" },
        };

        foreach (var place in places)
        {
            var placeButton = new RichTextButton
            {
                ModulateSelfOverride = PlaceButtonColor,
                Text = TruncateWithEllipsis(place.Name, MaxLenght),
                TextAlign = Label.AlignMode.Right,
                HorizontalAlignment = HAlignment.Center,
                VerticalAlignment = VAlignment.Center,
                SizeFlagsStretchRatio = 1,
                ToolTip = place.Description,
                TooltipDelay = DefaultTooltipDelay,
                SetWidth = DefaultButtonWidth,
                SetHeight = DefaultButtonHeight,
            };

            placeButton.OnPressed += _ => WarpClicked?.Invoke(place.Entity);

            placesGrid.AddChild(placeButton);
        }

        bigGrid.AddChild(countLabel);
        bigGrid.AddChild(placesGrid);

        GhostTeleportContainer.AddChild(bigGrid);
    }

    private void AddAntagButtons(List<GhostWarpGlobalAntagonist> antags, string text)
    {
        if (antags.Count == 0)
            return;

        var bigGrid = new GridContainer();

        var bigLabel = new Label
        {
            Text = Loc.GetString(text),
            StyleClasses = { "LabelBig" },
        };
        bigGrid.AddChild(bigLabel);

        var sortedAntags = SortAntagsByWeight(antags);

        foreach (var antagSet in sortedAntags)
        {
            var departmentGrid = new GridContainer
            {
                Columns = 5,
            };

            var labelText = string.Empty;

            foreach (var antag in antagSet)
            {
                var playerButton = new RichTextButton
                {
                    ModulateSelfOverride = AntagonistButtonColor,
                    Text = TruncateWithEllipsis(antag.Name, MaxLenght),
                    TextAlign = Label.AlignMode.Right,
                    HorizontalAlignment = HAlignment.Center,
                    VerticalAlignment = VAlignment.Center,
                    SizeFlagsStretchRatio = 1,
                    ToolTip = Loc.GetString(antag.AntagonistDescription),
                    TooltipDelay = DefaultTooltipDelay,
                    SetWidth = DefaultButtonWidth,
                    SetHeight = DefaultButtonHeight,
                };

                playerButton.OnPressed += _ => WarpClicked?.Invoke(antag.Entity);

                departmentGrid.AddChild(playerButton);

                labelText = antag.AntagonistName;
            }

            var departmentLabel = new Label
            {
                Text = Loc.GetString(labelText) + ": " + antagSet.Count,
                StyleClasses = { "LabelSecondaryColor" }
            };

            bigGrid.AddChild(departmentLabel);
            bigGrid.AddChild(departmentGrid);
        }

        GhostTeleportContainer.AddChild(bigGrid);
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

        return $"{warp.Name}\n{jobName.ToUpperInvariant()}";
    }
}
