using Content.Shared.Radio;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Radio;

/// <summary>
/// Raised on the server after a radio wrapped message string is built but before it is sent.
/// Subscribe to this to append postfixes (e.g. clickable chat links) in a modular way
/// without touching RadioSystem.
/// </summary>
[ByRefEvent]
public struct RMCWrappedRadioMessageEvent
{
    /// <summary>The entity that spoke the message.</summary>
    public readonly EntityUid MessageSource;

    /// <summary>The radio channel the message was sent on.</summary>
    public readonly RadioChannelPrototype Channel;

    /// <summary>The display name of the sender as it appears in the wrapped message (markup-escaped).</summary>
    public readonly string SenderName;

    /// <summary>The fully formatted wrapped message string. Mutate this to append postfixes.</summary>
    public string WrappedMessage;

    public RMCWrappedRadioMessageEvent(EntityUid messageSource, RadioChannelPrototype channel, string wrappedMessage, string senderName)
    {
        MessageSource = messageSource;
        Channel = channel;
        WrappedMessage = wrappedMessage;
        SenderName = senderName;
    }
}
