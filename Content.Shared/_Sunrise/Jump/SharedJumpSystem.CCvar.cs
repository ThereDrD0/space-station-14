namespace Content.Shared._Sunrise.Jump;

public abstract partial class SharedJumpSystem
{
    public bool Enable;
    private static float _deadChance;

    public bool BunnyHopEnable;
    private static TimeSpan _bunnyHopSpeedBoostWindow;
    private static float _bunnyHopSpeedUpPerJump;
    private static float _bunnyHopSpeedLimit;
    private static float _bunnyHopMinSpeedThreshold;

    private async void OnClientOptionJumpSound(ClientOptionDisableJumpSoundEvent ev, EntitySessionEventArgs args)
    {
        if (ev.Disable)
            _ignoredRecipients.Add(args.SenderSession);
        else
            _ignoredRecipients.Remove(args.SenderSession);
    }

    private void OnJumpEnableChanged(bool enable)
    {
        Enable = enable;
    }

    private static void OnJumpDeadChanceChanged(float value)
    {
        _deadChance = value;
    }

    private void OnBunnyHopEnableChanged(bool enable)
    {
        BunnyHopEnable = enable;
    }

    private static  void OnBunnyHopMinSpeedThresholdChanged(float value)
    {
        _bunnyHopMinSpeedThreshold = value;
    }

    private static  void OnBunnyHopSpeedBoostWindowChanged(float value)
    {
        _bunnyHopSpeedBoostWindow = TimeSpan.FromSeconds(value);
    }

    private static  void OnBunnyHopSpeedUpPerJumpChanged(float value)
    {
        _bunnyHopSpeedUpPerJump = value;
    }

    private static  void OnBunnyHopSpeedLimitChanged(float value)
    {
        _bunnyHopSpeedLimit = value;
    }
}
