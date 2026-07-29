using Content.Shared._RMC14.ChatLinks;
using Content.Shared._RMC14.Radio;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Chat;

namespace Content.Server._RMC14.ChatLinks;

/// <summary>
/// Wraps the Hivemind radio message sender's name in a clickable watch-xeno textlink,
/// so players can watch/follow the xeno speaker with a single click on their name.
/// Subscribes to <see cref="RMCWrappedRadioMessageEvent"/> raised by RadioSystem,
/// keeping all xeno-specific chat-link logic out of the upstream radio code.
/// </summary>
public sealed class XenoHivemindWatchLinkSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RMCWrappedRadioMessageEvent>(OnWrappedRadioMessage);
    }

    private void OnWrappedRadioMessage(ref RMCWrappedRadioMessageEvent ev)
    {
        if (ev.Channel.ID != SharedChatSystem.HivemindChannel.Id)
            return;

        if (!HasComp<XenoComponent>(ev.MessageSource))
            return;

        var net = GetNetEntity(ev.MessageSource);
        if (net == NetEntity.Invalid)
            return;

        ChatLinkMarkupHelper.WrapSenderNameInTextlink(
            ref ev.WrappedMessage,
            ev.SenderName,
            RMCChatActionLinkConstants.WatchXenoPrefix,
            net.Id);
    }
}
