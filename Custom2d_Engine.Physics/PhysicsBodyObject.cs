using Custom2d_Engine.Scenes;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;

namespace Custom2d_Engine.Physics;

public class PhysicsBodyObject : HierarchyObject {
    public float Order { get; set; }

    public Body PhysicsBody { get; protected set; }

    private bool _dirty;
    private bool _isUpdating;

    public PhysicsBodyObject(Body physicsBody) {
        PhysicsBody = physicsBody;
        EnableUpdates = true;
        Transform.Changed += () => {
            if (_isUpdating) {
                return;
            }

            _dirty = true;
        };
    }

    protected override void CustomUpdate(GameTime time) {
        if (PhysicsBody.World == null) {
            return;
        }

        if (_dirty) {
            var p = Transform.LocalPosition;
            PhysicsBody.SetTransformIgnoreContacts(ref p, Transform.LocalRotation);
            _dirty = false;
        }

        _isUpdating = true;
        var bodyTransform = PhysicsBody.GetTransform();

        var angle = bodyTransform.q.Phase;
        var pos = bodyTransform.p;
        Transform.LocalRotation = angle;
        Transform.LocalPosition = pos;
        _isUpdating = false;
    }

    public DrawableObject AddDrawableRectFixture(Vector2 size, Vector2 offset, float rotation, out Fixture fixture,
        float density = 1f) {
        var verts = PolygonTools.CreateRectangle(size.X / 2f, size.Y / 2f);
        verts.Rotate(rotation);
        verts.Translate(offset);
        fixture = PhysicsBody.CreatePolygon(verts, density);
        var drawable = new DrawableObject(Color.WhiteSmoke, 0f);
        drawable.Parent = this;
        drawable.Transform.LocalPosition = offset;
        drawable.Transform.LocalRotation = rotation;
        drawable.Transform.LocalScale = size;
        return drawable;
    }

    public Fixture AddRectFixture(Vector2 size, Vector2 offset, float rotation, float density = 1f) {
        var verts = PolygonTools.CreateRectangle(size.X / 2f, size.Y / 2f);
        verts.Rotate(rotation);
        verts.Translate(offset);
        return PhysicsBody.CreatePolygon(verts, density);
    }

    public void RemoveFixture(Fixture fixture) {
        PhysicsBody.Remove(fixture);
    }
}