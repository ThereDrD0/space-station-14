using Robust.Shared.Serialization;

namespace Content.Shared.Ghost;

// TODO: Перевод комментариев на славянский
public abstract partial class SharedGhostSystem
{
    /// <summary>
    /// An player body a ghost can warp to.
    /// This is used as part of <see cref="GhostWarpsResponseEvent"/>
    /// </summary>
    [Serializable, NetSerializable]
    public record struct GhostWarpPlayer(
        NetEntity Entity,
        string PlayerName,
        string PlayerJobName,
        string PlayerDepartmentId,
        bool IsGhost,
        bool IsLeft,
        bool IsDead,
        bool IsAlive) : INamedGhostWarp
    {
        /// <summary>
        /// The entity representing the warp point.
        /// This is passed back to the server in <see cref="GhostWarpToTargetRequestEvent"/>
        /// </summary>
        public readonly NetEntity Entity = Entity;

        /// <summary>
        /// The display player name to be surfaced in the ghost warps menu
        /// </summary>
        public string Name { get; } = PlayerName;

        /// <summary>
        /// The display player job to be surfaced in the ghost warps menu
        /// </summary>
        public readonly string JobName = PlayerJobName;

        /// <summary>
        /// The display player department to be surfaced in the ghost warps menu
        /// </summary>
        public readonly string DepartmentId = PlayerDepartmentId;

        /// <summary>
        /// Is player is ghost
        /// </summary>
        public readonly bool IsGhost = IsGhost;

        /// <summary>
        /// Is player body alive
        /// </summary>
        public readonly bool IsAlive = IsAlive;

        /// <summary>
        /// Is player body dead
        /// </summary>
        public readonly bool IsDead = IsDead;

        /// <summary>
        /// Is player left from body
        /// </summary>
        public readonly bool IsLeft = IsLeft;
    }

    [Serializable, NetSerializable]
    public record struct GhostWarpGlobalAntagonist(
        NetEntity Entity,
        string PlayerName,
        string AntagonistName,
        string AntagonistDescription,
        int Weight) : INamedGhostWarp
    {

        /// <summary>
        /// The entity representing the warp point.
        /// This is passed back to the server in <see cref="GhostWarpToTargetRequestEvent"/>
        /// </summary>
        public readonly NetEntity Entity = Entity;

        /// <summary>
        /// The display player name to be surfaced in the ghost warps menu
        /// </summary>
        public string Name { get; } = PlayerName;

        /// <summary>
        /// The display antagonist name to be surfaced in the ghost warps menu
        /// </summary>
        public readonly string AntagonistName = AntagonistName;

        /// <summary>
        /// The display antagonist description to be surfaced in the ghost warps menu
        /// </summary>
        public readonly string AntagonistDescription = AntagonistDescription;

        /// <summary>
        /// A antagonist prototype id
        /// </summary>
        /// TODO: Ренейм в Priority
        public readonly int Weight = Weight;

    }

    /// <summary>
    /// An individual place a ghost can warp to.
    /// This is used as part of <see cref="GhostWarpsResponseEvent"/>
    /// </summary>
    [Serializable, NetSerializable]
    public record struct GhostWarpPlace(NetEntity Entity, string Name, string Description) : INamedGhostWarp
    {
        /// <summary>
        /// The entity representing the warp point.
        /// This is passed back to the server in <see cref="GhostWarpToTargetRequestEvent"/>
        /// </summary>
        public readonly NetEntity Entity = Entity;

        /// <summary>
        /// The display name to be surfaced in the ghost warps menu
        /// </summary>
        public string Name { get; } = Name;

        /// <summary>
        /// The display name to be surfaced in the ghost warps menu
        /// </summary>
        public readonly string Description = Description;

    }

    /// <summary>
    /// A server to client response for a <see cref="GhostWarpsRequestEvent"/>.
    /// Contains players, and locations a ghost can warp to
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class GhostWarpsResponseEvent(
        List<GhostWarpPlayer> players,
        List<GhostWarpPlace> places,
        List<GhostWarpGlobalAntagonist> antagonists) : EntityEventArgs
    {

        /// <summary>
        /// A list of players to teleport.
        /// </summary>
        public readonly List<GhostWarpPlayer> Players = players;

        /// <summary>
        /// A list of warp points.
        /// </summary>
        public readonly List<GhostWarpPlace> Places = places;

        /// <summary>
        /// A list of antagonists to teleport.
        /// </summary>
        public readonly List<GhostWarpGlobalAntagonist> Antagonists = antagonists;
    }

    public interface INamedGhostWarp
    {
        public string Name { get; }
    }
}
