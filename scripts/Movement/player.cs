using Godot;
using System;
using UnderwaterGame;

public partial class player : Node3D
{
    const float shiftMultiplier = 2f;
    const float velMultiplier = 4f;
    const float acceleration = 30f;
    const float deceleration = -10f;

    Vector2 mousePosition = Vector2.Zero;
    float totalPitch = 0f;
    float totalYaw = 0f;

    bool movementAllowed = true;
    Vector3 direction = Vector3.Zero;
    Vector3 velocity = Vector3.Zero;

    float previousSlowdown = 0;
    float previousUpSlowdown = 0;
    float previousDownSlowdown = 0;
    float previousRightSlowdown = 0;
    float previousLeftSlowdown = 0;
    float previousRightRollSlowdown = 0;
    float previousLeftRollSlowdown = 0;

    public CharacterBody3D Jet;
    public Camera3D FPCamera;
    public Camera3D TPCamera;

    public SpotLight3D Spotlight;
    public SpotLight3D FPLight;

    public Sprite3D Leveling;
    public Control LevelingBars;

    public ProgressBar FlashlightPercent;
    public Timer FlashlightTimer = new();

    public static ProgressBar HealthPower { get; set; }
    public static bool MenuOpened { get; set; } = true;

    readonly Difficulty difficulty = (Difficulty)GameSettings.DifficultySetting;
    PackedScene gameOverScreen = GD.Load<PackedScene>("res://scenes/GameOver.tscn");

    public override void _Ready()
    {
        Jet = GetParent<CharacterBody3D>();

        FPCamera = Jet.GetNode<Camera3D>("Cameras/FPCamera");
        TPCamera = Jet.GetNode<Camera3D>("Cameras/TPCamera");

        Spotlight = Jet.GetNode<SpotLight3D>("Lights/SpotLight");
        FPLight = Jet.GetNode<SpotLight3D>("Lights/FPLight");

        Leveling = Jet.GetNode<Sprite3D>("Leveling");
        LevelingBars = Jet.GetNode<Control>("Leveling/SubViewport/Control/Bars");
        LevelingBars.PivotOffset = new Vector2(LevelingBars.Size.X / 2, 0);

        (HealthPower = Jet.GetNode<ProgressBar>("HP/SubViewport/Control/HealthPower")).ValueChanged += (value) =>
        {
            if (value > 0)
                return;

            AddChild(gameOverScreen.Instantiate<Control>());
        };
        HealthPower.SetValueNoSignal(HealthPower.MaxValue = difficulty switch
        {
            Difficulty.Easy => 200,
            Difficulty.Hard => 75,
            Difficulty.Nightmare => 50,
            _ => 100
        });

        (FlashlightPercent = Jet.GetNode<ProgressBar>("Power/SubViewport/Control/FlashlightPercent")).ValueChanged += (newValue) =>
        {
            if (newValue <= 0)
            {
                ToggleFlashlight();
                movementAllowed = FPLight.Visible = Leveling.Visible = false;
            }
        };
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
            mousePosition = GameSettings.InvertMouseSetting ? -mouseMotion.Relative : mouseMotion.Relative;

        if (Input.IsActionJustPressed("toggle_camera"))
            ToggleCamera();

        if (Input.IsActionJustPressed("toggle_flashlight"))
            ToggleFlashlight();

        if (Input.IsActionJustPressed("reset_roll") && movementAllowed)
            Jet.Rotation = new Vector3(Jet.Rotation.X, Jet.Rotation.Y, 0);
    }

    public override void _Process(double delta)
    {
        UpdateMovement((float)delta);
        RotateJet();
        UpdateMouseLook();
    }

    public void ToggleCamera()
    {
        bool toggle = FPCamera.Current;
        FPCamera.Current = !toggle;
        TPCamera.Current = toggle;

        Spotlight.LightVolumetricFogEnergy = toggle.ToFloat();
    }

    public void ToggleFlashlight()
    {
        bool toggle = Spotlight.Visible;

        if (FlashlightPercent.Value != 100 && !toggle)
            return;

        Spotlight.Visible = !toggle;
        FlashlightTimer.Stop();
        FlashlightTimer.Dispose();
        FlashlightTimer = new() { OneShot = false };
        AddChild(FlashlightTimer);

        if (!toggle)
        {
            FlashlightTimer.Timeout += () =>
            {
                if (FlashlightPercent.Value > 0)
                    FlashlightPercent.Value--;
                else
                    FlashlightTimer.Stop();
            };
            FlashlightTimer.Start(difficulty switch
            {
                Difficulty.Easy => 0.5,
                Difficulty.Hard => 0.2,
                Difficulty.Nightmare => 0.1,
                _ => 0.3
            });
        }
        else
        {
            FlashlightTimer.Timeout += () =>
            {
                if (FlashlightPercent.Value < 100)
                    FlashlightPercent.Value++;
                else
                {
                    movementAllowed = FPLight.Visible = Leveling.Visible = true;
                    FlashlightTimer.Stop();
                }
            };
            FlashlightTimer.Start(difficulty switch
            {
                Difficulty.Easy => 0.1,
                Difficulty.Hard => 0.3,
                Difficulty.Nightmare => 0.5,
                _ => 0.2
            });
        }
    }

    public void RotateJet()
    {
        if (!movementAllowed)
        {
            previousSlowdown = previousUpSlowdown = previousDownSlowdown = previousRightSlowdown = previousLeftSlowdown = previousRightRollSlowdown = previousLeftRollSlowdown = 0;
            Jet.Velocity = Vector3.Zero;
            Jet.MoveAndSlide();
            return;
        }

        previousUpSlowdown = previousUpSlowdown > 0 ? previousUpSlowdown : Input.IsActionJustReleased("up") ? 1 : 0;
        previousDownSlowdown = previousDownSlowdown < 0 ? previousDownSlowdown : Input.IsActionJustReleased("down") ? -1 : 0;
        previousRightSlowdown = previousRightSlowdown > 0 ? previousRightSlowdown : Input.IsActionJustReleased("turn_right") ? 1 : 0;
        previousLeftSlowdown = previousLeftSlowdown < 0 ? previousLeftSlowdown : Input.IsActionJustReleased("turn_left") ? -1 : 0;

        var pitch = Input.IsActionPressed("up").ToFloat() - Input.IsActionPressed("down").ToFloat();
        if (pitch == 0)
        {
            if (previousUpSlowdown > 0)
            {
                previousUpSlowdown -= 0.05f;
                pitch = previousUpSlowdown;
            }
            else if (previousDownSlowdown < 0)
            {
                previousDownSlowdown += 0.05f;
                pitch = previousDownSlowdown;
            }
        }
        else
        {
            previousUpSlowdown = 0;
            previousDownSlowdown = 0;
        }
        Jet.RotateObjectLocal(Vector3.ModelLeft, Mathf.DegToRad(-pitch));

        var roll = Input.IsActionPressed("turn_right").ToFloat() - Input.IsActionPressed("turn_left").ToFloat();
        if (roll == 0)
        {
            if (previousRightSlowdown > 0)
            {
                previousRightSlowdown -= 0.03f;
                roll = previousRightSlowdown;
            }
            else if (previousLeftSlowdown < 0)
            {
                previousLeftSlowdown += 0.03f;
                roll = previousLeftSlowdown;
            }
        }
        else
        {
            previousRightSlowdown = 0;
            previousLeftSlowdown = 0;
        }
        Jet.RotateObjectLocal(Vector3.ModelFront, -(roll * 0.025f));

        float jetZRotation = Jet.RotationDegrees.Z;
        if (jetZRotation > 90)
            jetZRotation = 180 - jetZRotation;
        else if (jetZRotation < -90)
            jetZRotation = -180 - jetZRotation;

        if (Input.IsActionPressed("accelerate"))
        {
            Jet.RotateY(Mathf.DegToRad(jetZRotation * 0.025f * (Input.IsActionPressed("speed_up") ? shiftMultiplier : 1)));
            if (jetZRotation > 0)
            {
                previousRightRollSlowdown = 1;
                previousLeftRollSlowdown = 0;
            }
            else if (jetZRotation < 0)
            {
                previousLeftRollSlowdown = -1;
                previousRightRollSlowdown = 0;
            }
        }
        else if (previousSlowdown > 0 && previousSlowdown < 1)
        {
            if (previousRightRollSlowdown > 0)
            {
                previousRightRollSlowdown -= 0.02f;
                Jet.RotateY(Mathf.DegToRad(previousRightRollSlowdown * 0.025f * jetZRotation * (Input.IsActionPressed("speed_up") ? shiftMultiplier : 1)));
            }
            if (previousLeftRollSlowdown < 0)
            {
                previousLeftRollSlowdown += 0.02f;
                Jet.RotateY(Mathf.DegToRad(-previousLeftRollSlowdown * 0.025f * jetZRotation * (Input.IsActionPressed("speed_up") ? shiftMultiplier : 1)));
            }
        }
        else
        {
            previousLeftRollSlowdown = 0;
            previousRightRollSlowdown = 0;
        }

        LevelingBars.Position = new Vector2(0, 76 + Jet.GlobalRotationDegrees.X);
        LevelingBars.RotationDegrees = Jet.GlobalRotationDegrees.Z;
    }

    public void UpdateMovement(float delta)
    {
        if (!movementAllowed)
            return;

        direction = GlobalTransform.Basis * new Vector3(0, 0, -Input.IsActionPressed("accelerate").ToFloat());
        var offset = direction * acceleration * velMultiplier * delta + velocity * deceleration * velMultiplier * delta;
        if (direction == Vector3.Zero)
        {
            if (previousSlowdown > 0)
            {
                previousSlowdown -= 0.02f / (Input.IsActionPressed("speed_up") ? shiftMultiplier : 1);

                direction = GlobalTransform.Basis * new Vector3(0, 0, -previousSlowdown);
                offset = direction * acceleration * velMultiplier * delta + velocity * deceleration * velMultiplier * delta;

                velocity.X += offset.X;
                velocity.Y += offset.Y;
                velocity.Z += offset.Z;
            }
            else
                velocity = Vector3.Zero;
        }
        else
        {
            velocity.X = Mathf.Clamp(velocity.X + offset.X, -velMultiplier, velMultiplier);
            velocity.Y = Mathf.Clamp(velocity.Y + offset.Y, -velMultiplier, velMultiplier);
            velocity.Z = Mathf.Clamp(velocity.Z + offset.Z, -velMultiplier, velMultiplier);

            previousSlowdown = 1f;
        }

        Jet.Velocity = velocity * (Input.IsActionPressed("speed_up") ? 7f * shiftMultiplier : 7f);
        Jet.MoveAndSlide();
    }

    public void UpdateMouseLook()
    {
        if (!FPCamera.Current)
            return;

        mousePosition *= GameSettings.MouseSensitivitySetting;
        var yaw = mousePosition.X;
        var pitch = mousePosition.Y;
        mousePosition = Vector2.Zero;

        pitch = Mathf.Clamp(pitch, -90 - totalPitch, 90 - totalPitch);
        totalPitch += pitch;

        yaw = Mathf.Clamp(yaw, -90 - totalYaw, 90 - totalYaw);
        totalYaw += yaw;

        FPCamera.RotateY(Mathf.DegToRad(-yaw));
        FPCamera.RotateObjectLocal(new Vector3(1, 0, 0), Mathf.DegToRad(-pitch));
    }
}
