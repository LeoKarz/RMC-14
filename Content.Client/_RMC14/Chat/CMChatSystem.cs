using Content.Client.UserInterface.Systems.Chat.Widgets;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.ChatLinks;
using Content.Shared.Chat;
using Content.Shared.Ghost;
using Robust.Client.UserInterface.Controls;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Player;
using Robust.Shared.Network;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Chat;

public sealed class CMChatSystem : SharedCMChatSystem
{
    [Dependency] private readonly IConfigurationManager _config = default!;
    [Dependency] private readonly IClientNetManager _net = default!;
    [Dependency] private readonly IEntityNetworkManager _entityNet = default!;
    [Dependency] private readonly ILogManager _log = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    private ISawmill _sawmill = default!;
    private int _repeatHistory;

    public override void Initialize()
    {
        base.Initialize();

        _sawmill = _log.GetSawmill("chat");
        Subs.CVar(_config, RMCCVars.RMCChatRepeatHistory, v => _repeatHistory = v, true);

        // RMC14 - register net message for chat action link clicks (e.g. [Follow] hivemind link)
        _net.RegisterNetMessage<RMCChatActionLinkClickMsg>();
    }

    /// <summary>
    /// RMC14 - applies chat name coloring. Checks that the sender has an actor (player) component to
    /// avoid coloring NPC names, then applies squad color if available, otherwise the provided default.
    /// </summary>
    public void ApplyChatNameColor(ChatMessage msg, string defaultNameColor)
    {
        var entityUid = EntityManager.GetEntity(msg.SenderEntity);
        if (!HasComp<ActorComponent>(entityUid))
            return;

        var squadColor = ColorizeSpeakerNameBySquadOrNull(msg);
        msg.WrappedMessage = squadColor ?? SharedChatSystem.InjectTagInsideTag(msg, "Name", "color", defaultNameColor);
    }

    // RMC14 - dispatcher for chat action link clicks; delegates to the appropriate handler.
    public void HandleChatLinkClick(string link)
    {
        if (link.StartsWith(RMCChatActionLinkConstants.WatchXenoPrefix, StringComparison.Ordinal))
            HandleHivemindFollowClick(link);
    }

    private void HandleHivemindFollowClick(string link)
    {
        var idStr = link[RMCChatActionLinkConstants.WatchXenoPrefix.Length..];
        if (!int.TryParse(idStr, out var id))
        {
            _sawmill.Warning($"Invalid chat action link id: '{idStr}'");
            return;
        }

        var target = new NetEntity(id);

        // RMC14 - ghosts warp to the target instead of using the xeno watch action
        var localEntity = _player.LocalEntity;
        if (localEntity != null && HasComp<GhostComponent>(localEntity.Value))
        {
            _entityNet.SendSystemNetworkMessage(new GhostWarpToTargetRequestEvent(target));
            return;
        }

        var msg = new RMCChatActionLinkClickMsg
        {
            ActionId = RMCChatActionLinkConstants.WatchXenoPrefix + idStr,
            Target = target
        };
        _net.ClientSendMessage(msg);
    }

    /// <summary>
    /// Channels that may contain RMC-injected markup tags (e.g. textlink).
    /// Only these channels get the extended tag whitelist; all others fall back
    /// to ChatBox's own FilterProblematicTags.
    /// </summary>
    private static readonly HashSet<ChatChannel> FilterableChannels =
    [
        ChatChannel.Radio,
    ];

    private static readonly HashSet<string> TagWhitelist =
    [
        "mono", "scramble", "bolditalic", "bold", "bullet",
        "color", "font", "head", "italic", "textlink",
    ];

    /// <summary>
    /// Filters markup tags for channels listed in <see cref="FilterableChannels"/> using an
    /// extended whitelist that includes RMC-injected tags such as "textlink".
    /// Returns <c>null</c> for all other channels so the caller can fall back to its own filter.
    /// </summary>
    public FormattedMessage? FilterTagsIfNeeded(ChatChannel channel, FormattedMessage message)
    {
        if (!FilterableChannels.Contains(channel))
            return null;

        var output = new FormattedMessage(message.Count);
        foreach (var tag in message)
        {
            if (tag.Name is not { } name || TagWhitelist.Contains(name))
                output.PushTag(tag);
        }

        return output;
    }

    public bool TryRepetition(ChatBox chat, OutputPanel contents, FormattedMessage message, NetEntity sender, string unwrapped, ChatChannel channel, bool repeatCheckSender)
    {
        var repeated = false;
        foreach (var old in chat.RepeatQueue)
        {
            if (!old.Message.Equals(unwrapped) ||
                old.Channel != channel)
            {
                continue;
            }

            if (repeatCheckSender &&
                !old.SenderEntity.Equals(sender))
            {
                continue;
            }

            var copy = new FormattedMessage(old.FormattedMessage);
            old.Count++;
            copy.AddMarkupPermissive($" [color=red]x{old.Count}[/color]");
            contents.SetMessage(old.Index, copy);
            repeated = true;
            break;
        }

        if (!repeated)
        {
            chat.RepeatQueue.Enqueue(new RepeatedMessage(contents.EntryCount, message, sender, unwrapped, channel));
            if (_repeatHistory > 0)
            {
                while (chat.RepeatQueue.Count > _repeatHistory)
                {
                    chat.RepeatQueue.Dequeue();
                }
            }
        }

        return repeated;
    }
}
