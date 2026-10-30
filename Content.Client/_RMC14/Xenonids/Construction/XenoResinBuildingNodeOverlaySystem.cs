using Content.Shared._RMC14.Xenonids.Construction;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Xenonids.Construction;

/// <summary>
/// Shows resin designation marks only to the xeno that placed them.
/// The authoritative node itself has no visible sprite; these are client-local overlays.
/// </summary>
public sealed class XenoResinBuildingNodeOverlaySystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    private readonly Dictionary<EntityUid, EntityUid> _overlays = new();
    private readonly HashSet<EntityUid> _visibleNodes = new();
    private readonly List<EntityUid> _staleNodes = new();

    public override void FrameUpdate(float frameTime)
    {
        base.FrameUpdate(frameTime);

        _visibleNodes.Clear();
        var player = _playerManager.LocalEntity;
        if (player != null)
        {
            var query = EntityQueryEnumerator<XenoResinBuildingNodeComponent>();
            while (query.MoveNext(out var nodeUid, out var node))
            {
                if (node.Owner != player.Value)
                    continue;

                _visibleNodes.Add(nodeUid);
                var coordinates = Transform(nodeUid).Coordinates;
                if (!_overlays.TryGetValue(nodeUid, out var overlay) || Deleted(overlay))
                {
                    var mark = node.StructureId.Id.Contains("Door")
                        ? "DesignNodeMarkConstructDoor"
                        : "DesignNodeMarkConstructWall";
                    overlay = Spawn(mark, coordinates);
                    _overlays[nodeUid] = overlay;
                }
                else if (TryComp(overlay, out TransformComponent? overlayTransform))
                {
                    _transform.SetCoordinates(overlay, overlayTransform, coordinates);
                }

                if (TryComp(overlay, out SpriteComponent? sprite))
                    sprite.Color = node.QueenQueue ? new Color(255, 215, 0, 255) : Color.White;
            }
        }

        _staleNodes.Clear();
        foreach (var (nodeUid, overlay) in _overlays)
        {
            if (_visibleNodes.Contains(nodeUid))
                continue;

            if (!Deleted(overlay))
                QueueDel(overlay);

            _staleNodes.Add(nodeUid);
        }

        foreach (var nodeUid in _staleNodes)
        {
            _overlays.Remove(nodeUid);
        }
    }
}
