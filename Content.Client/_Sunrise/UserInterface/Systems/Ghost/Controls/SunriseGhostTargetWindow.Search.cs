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
    }

    private void UpdateVisibleButtons()
    {
        // TODO: Как-то оптимизировать?
        // Паровозик из циклов ради поиска выглядит как большая потеря производительности при этом самом поиске
        // Но я не вижу вариантов, как сделать это иначе. Если бы можно было получить всех детей рекурсивно более производительно
        // То я бы сделал это, вместо цикла в цикла в цикле
        foreach (var bigGrid in GhostTeleportContainer.Children)
        {
            if (bigGrid is not GridContainer)
                continue;

            foreach (var departmentGrid in bigGrid.Children)
            {
                if (departmentGrid is not GridContainer)
                    continue;

                foreach (var gridChild in departmentGrid.Children)
                {
                    if (gridChild is RichTextButton button)
                        gridChild.Visible = ButtonIsVisible(button);
                }
            }
        }
    }

    private bool ButtonIsVisible(RichTextButton button)
    {
        return string.IsNullOrEmpty(_searchText) || button.ToolTip == null || button.ToolTip.Contains(_searchText, StringComparison.OrdinalIgnoreCase);
    }
}
