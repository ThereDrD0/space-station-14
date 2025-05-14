using System.Linq;
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
                var playerButton = new Button
                {
                    Text = player.Name,
                    TextAlign = Label.AlignMode.Right,
                    HorizontalAlignment = HAlignment.Center,
                    VerticalAlignment = VAlignment.Center,
                    SizeFlagsStretchRatio = 1,
                    ModulateSelfOverride = department.Color,
                    ToolTip = player.JobName,
                    TooltipDelay = 0.1f,
                    SetWidth = 180,
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
            var placeButton = new Button
            {
                Text = place.Name,
                TextAlign = Label.AlignMode.Right,
                HorizontalAlignment = HAlignment.Center,
                VerticalAlignment = VAlignment.Center,
                SizeFlagsStretchRatio = 1,
                ModulateSelfOverride = PlaceButtonColor,
                ToolTip = place.Description,
                TooltipDelay = 0.1f,
                SetWidth = 180,
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

        foreach (var antagHashSet in sortedAntags)
        {
            var departmentGrid = new GridContainer
            {
                Columns = 5,
            };

            var labelText = string.Empty;

            foreach (var antag in antagHashSet)
            {
                var playerButton = new Button
                {
                    Text = antag.Name,
                    TextAlign = Label.AlignMode.Right,
                    HorizontalAlignment = HAlignment.Center,
                    VerticalAlignment = VAlignment.Center,
                    SizeFlagsStretchRatio = 1,
                    ModulateSelfOverride = AntagonistButtonColor,
                    ToolTip = Loc.GetString(antag.AntagonistDescription),
                    TooltipDelay = 0.1f,
                    SetWidth = 180,
                };

                playerButton.OnPressed += _ => WarpClicked?.Invoke(antag.Entity);

                departmentGrid.AddChild(playerButton);

                labelText = antag.AntagonistName;
            }

            var departmentLabel = new Label
            {
                Text = Loc.GetString(labelText) + ": " + antagHashSet.Count(),
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
            if (!_prototype.TryIndex<DepartmentPrototype>(player.DepartmentId, out var department))
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
