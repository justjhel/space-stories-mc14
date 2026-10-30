using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Construction;

/// <summary>
/// A builder's planned resin structure. The owner and selected structure are retained
/// so a designation can be completed later without depending on the builder's current selection.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class XenoResinBuildingNodeComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid? Owner;

    [DataField, AutoNetworkedField]
    public EntProtoId StructureId = "WallXenoResin";

    [DataField, AutoNetworkedField]
    public bool QueenQueue;
}
