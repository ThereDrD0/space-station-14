using Content.Shared._Sunrise.Ghost;
using Content.Shared.Ghost;
using Content.Shared.Humanoid;
using Content.Shared.Mind.Components;
using Content.Shared.Roles;
using Content.Shared.Silicons.Borgs.Components;
using Content.Shared.Silicons.StationAi;
using Content.Shared.SSDIndicator;
using Content.Shared.Warps;
using Robust.Shared.Prototypes;

namespace Content.Server.Ghost;

public sealed partial class GhostSystem
{
    private static readonly ProtoId<DepartmentPrototype> UnknownDepartmentPrototype = "Specific";

    private IEnumerable<GhostWarpPlace> GetLocationWarps()
    {
        var query = AllEntityQuery<WarpPointComponent, MetaDataComponent>();
        while (query.MoveNext(out var uid, out var component, out var meta))
        {
            var warp = new GhostWarpPlace(
                GetNetEntity(uid),
                component.Location ?? meta.EntityName,
                component.Location ?? meta.EntityDescription);

            yield return warp;
        }
    }

    private IEnumerable<GhostWarpPlayer> GetPlayerWarps()
    {
        var query = AllEntityQuery<MindContainerComponent, MetaDataComponent>();
        while (query.MoveNext(out var uid, out var mind, out var meta))
        {
            var isGhost = HasComp<GhostComponent>(uid);

            if (!IsEntityPanelRelevant(uid, isGhost))
                continue;

            // Антагонисты не сюда
            if (HasComp<GhostPanelAntagonistMarkerComponent>(uid))
                continue;

            var playerDepartmentId = _prototypeManager.Index(UnknownDepartmentPrototype).ID;
            var playerJobName = Loc.GetString("ghost-panel-unknown-job");

            if (_jobs.MindTryGetJob(mind.Mind ?? mind.LastMindStored, out var jobPrototype))
            {
                playerJobName = Loc.GetString(jobPrototype.Name);

                if (_jobs.TryGetDepartment(jobPrototype.ID, out var departmentPrototype))
                {
                    playerDepartmentId = departmentPrototype.ID;
                }
            }
            var hasAnyMind = (mind.Mind ?? mind.LastMindStored) != null;
            var isDead = _mobState.IsDead(uid);
            var isLeft = TryComp<SSDIndicatorComponent>(uid, out var indicator) && indicator.IsSSD && !isDead && hasAnyMind;

            var warp = new GhostWarpPlayer(
                GetNetEntity(uid),
                meta.EntityName,
                playerJobName,
                playerDepartmentId,
                isGhost,
                isLeft,
                isDead,
                _mobState.IsAlive(uid) // TODO: Возможно сделать проверку на !IsDead() или как-то объединить это
            );

            yield return warp;
        }
    }

    private IEnumerable<GhostWarpGlobalAntagonist> GetAntagonistWarps()
    {
        var query = AllEntityQuery<GhostPanelAntagonistMarkerComponent, MetaDataComponent>();
        while (query.MoveNext(out var uid, out var component, out var meta))
        {
            var warp = new GhostWarpGlobalAntagonist(
                GetNetEntity(uid),
                meta.EntityName,
                component.Name,
                component.Description,
                component.Weight
            );

            yield return warp;
        }
    }

    /// <summary>
    /// Проверяет, является ли живая сушность подходящей для нахождения в панели телепорта призрака
    /// </summary>
    private bool IsEntityPanelRelevant(EntityUid uid, bool isGhost)
    {
        return HasComp<HumanoidAppearanceComponent>(uid)
               || isGhost
               || HasComp<BorgChassisComponent>(uid)
               || HasComp<StationAiHeldComponent>(uid);
    }
}
