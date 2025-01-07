using Godot;
using System;

public enum FishType
{
    Default,
    QuickBoi,
    Epik,
    NextBot
}

public partial class Fish : RigidBody3D
{
    public FishType Type { get; set; }
    float speed = 15;

    Node3D playerWindscreen;

    double damage = 1;
    bool healthCooldown = true;

    public Vector3 forwardAxis = new(0, 0, -1);
    public int forwardAxisCooldown = 0;

    public override void _Ready()
    {
        playerWindscreen = GetParent().GetNode<Node3D>("Jet/Leveling");

        GravityScale = 0;
        ContactMonitor = true;
        MaxContactsReported = 1;

        switch (Type)
        {
            case FishType.QuickBoi:
                speed = 25;
                break;
            case FishType.Epik:
                damage = 200;
                break;
            case FishType.NextBot:
                speed = 25;
                damage = 200;
                break;
        }
    }

    private void LookFollow(PhysicsDirectBodyState3D state, Transform3D currentTransform, Vector3 targetPosition)
    {
        Vector3 forwardDir = (currentTransform.Basis * forwardAxis).Normalized();
        Vector3 targetDir = (targetPosition - currentTransform.Origin).Normalized();
        float localSpeed = Mathf.Clamp(_speed, 0.0f, Mathf.Acos(forwardDir.Dot(targetDir)));

        AngularVelocity = forwardDir.Cross(targetDir) * localSpeed / state.Step;
    }

    public override void _IntegrateForces(PhysicsDirectBodyState3D state) => LookFollow(state, GlobalTransform, playerWindscreen.GlobalTransform.Origin);

    public override void _Process(double delta)
    {
        if (GetContactCount() > 0 && GetCollidingBodies()[0] is CharacterBody3D)
        {
            forwardAxis = new(0, 0, 1);
            forwardAxisCooldown = 300;
            if (healthCooldown && player.HealthPower is not null)
            {
                player.HealthPower.Value--;
                healthCooldown = false;
            }
            return;
        }

        LinearVelocity = GlobalTransform.Basis * new Vector3(0, 0, -20);

        if (forwardAxisCooldown > 0)
        {
            forwardAxisCooldown--;
            return;
        }

        healthCooldown = true;
        forwardAxis = new(0, 0, -1);
    }
}
