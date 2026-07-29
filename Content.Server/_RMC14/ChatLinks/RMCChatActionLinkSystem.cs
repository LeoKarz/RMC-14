using Content.Shared._RMC14.ChatLinks;
using Robust.Server.Player;
using Robust.Shared.Network;

namespace Content.Server._RMC14.ChatLinks;

public sealed class RMCChatActionLinkSystem : EntitySystem
{
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    public override void Initialize()
    {
        base.Initialize();
        _net.RegisterNetMessage<RMCChatActionLinkClickMsg>(OnClicked);
    }

    private void OnClicked(RMCChatActionLinkClickMsg msg)
    {
        if (!_player.TryGetSessionById(msg.MsgChannel.UserId, out var session))
            return;

        if (session.AttachedEntity is not { } user)
            return;

        RaiseLocalEvent(new RMCChatActionLinkClickedEvent(user, msg.ActionId, msg.Target));
    }
}
