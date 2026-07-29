using Content.Shared._RMC14.ChatLinks;

namespace Content.Server._RMC14.ChatLinks;

/// <summary>
/// Static helpers for injecting textlink markup into chat wrapped-message strings.
/// </summary>
public static class ChatLinkMarkupHelper
{
    /// <summary>
    /// Replaces the content of the first <c>[bold]...[/bold]</c> block in <paramref name="wrappedMessage"/>
    /// with a <c>[textlink]</c> whose label is <paramref name="senderName"/> and whose link is
    /// <c>{prefix}{netEntityId}</c>, making the speaker name itself clickable.
    /// Falls back to appending the link at the end of the message if no bold block is found.
    /// </summary>
    public static void WrapSenderNameInTextlink(ref string wrappedMessage, string senderName, string prefix, long netEntityId)
    {
        var textlink = $"[textlink=\"{senderName}\" link=\"{prefix}{netEntityId}\"/]";

        var boldOpen = wrappedMessage.IndexOf(RMCChatActionLinkConstants.SpeakerNameOpeningTag, StringComparison.Ordinal);
        var boldClose = wrappedMessage.IndexOf(RMCChatActionLinkConstants.SpeakerNameClosingTag, StringComparison.Ordinal);

        if (boldOpen >= 0 && boldClose > boldOpen)
        {
            var afterOpen = boldOpen + RMCChatActionLinkConstants.SpeakerNameOpeningTag.Length;

            // Replace everything between (and including) the bold tags with [bold][textlink.../][/bold]
            wrappedMessage = wrappedMessage[..afterOpen]
                + textlink
                + wrappedMessage[boldClose..]; // includes "[/bold]" and the rest
        }
        else
        {
            // Fallback: append the textlink at the end
            wrappedMessage += $" {textlink}";
        }
    }
}
