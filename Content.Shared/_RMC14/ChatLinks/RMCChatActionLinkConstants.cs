namespace Content.Shared._RMC14.ChatLinks;
/// Shared constants for chat action links
public static class RMCChatActionLinkConstants
{
    /// <summary>
    /// Prefix for hivemind watch links embedded in chat markup.
    /// Full format: "rmc-chat:watchxeno:{NetEntityId}"
    /// </summary>
    public const string WatchXenoPrefix = "rmc-chat:watchxeno:";

    /// <summary>
    /// Opening tag of the speaker name bold block in messages.
    /// Together with <see cref="SpeakerNameClosingTag"/> it delimits the name that is replaced with a textlink.
    /// </summary>
    public const string SpeakerNameOpeningTag = "[bold]";

    /// <summary>
    /// Closing tag of the speaker name bold block in radio messages.
    /// The watch link is inserted immediately after this.
    /// </summary>
    public const string SpeakerNameClosingTag = "[/bold]";
}
