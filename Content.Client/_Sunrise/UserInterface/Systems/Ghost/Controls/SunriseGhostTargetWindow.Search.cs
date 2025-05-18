using System.Numerics;
using Content.Client._Sunrise.UserInterface.Controls;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Sunrise.UserInterface.Systems.Ghost.Controls;

public sealed partial class SunriseGhostTargetWindow
{
    private string _searchText = string.Empty;

    private void OnSearchTextChanged(LineEdit.LineEditEventArgs args)
    {
        _searchText = args.Text;

        UpdateVisibleButtons();
        Scroll.SetScrollValue(Vector2.Zero);
    }

    private void UpdateVisibleButtons()
    {
        foreach (var bigGridCandidate in GhostTeleportContainer.Children)
        {
            if (bigGridCandidate is not GridContainer bigGrid)
                continue;

            var anyDepartmentVisible = false;

            foreach (var departmentCandidate in bigGrid.Children)
            {
                if (departmentCandidate is not GridContainer departmentGrid)
                    continue;

                var anyButtonVisible = UpdateButtonsVisibility(departmentGrid);
                departmentGrid.Visible = anyButtonVisible;

                if (anyButtonVisible)
                    anyDepartmentVisible = true;
            }

            bigGrid.Visible = anyDepartmentVisible;
        }
    }

    private bool UpdateButtonsVisibility(GridContainer departmentGrid)
    {
        var foundVisible = false;

        foreach (var child in departmentGrid.Children)
        {
            if (child is not RichTextButton button)
                continue;

            var isVisible = ButtonIsVisible(button);
            button.Visible = isVisible;

            if (isVisible)
                foundVisible = true;
        }

        return foundVisible;
    }

    private bool ButtonIsVisible(RichTextButton button)
    {
        return string.IsNullOrEmpty(_searchText)
               || button.ToolTip == null
               || button.ToolTip.Contains(_searchText, StringComparison.OrdinalIgnoreCase);
    }
}
