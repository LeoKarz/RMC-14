using Robust.Shared.GameObjects;
using Robust.Shared.Network;

namespace Content.Shared._RMC14.ChatLinks;

/// <summary>
/// Raised on the server when a player clicks a chat action link.
/// </summary>
public sealed class RMCChatActionLinkClickedEvent : EntityEventArgs
{
    public readonly EntityUid User;
    public readonly string ActionId;
    public readonly NetEntity Target;

    public RMCChatActionLinkClickedEvent(EntityUid user, string actionId, NetEntity target)
    {
        User = user;
        ActionId = actionId;
        Target = target;
    }
}
