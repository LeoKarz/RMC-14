using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.ChatLinks;

public sealed class RMCChatActionLinkClickMsg : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public string ActionId = string.Empty;
    public NetEntity Target = NetEntity.Invalid;

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        ActionId = buffer.ReadString();

        var hasTarget = buffer.ReadBoolean();
        Target = hasTarget ? buffer.ReadNetEntity() : NetEntity.Invalid;
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(ActionId);

        var hasTarget = Target != NetEntity.Invalid;
        buffer.Write(hasTarget);
        if (hasTarget)
            buffer.Write(Target);
    }
}
