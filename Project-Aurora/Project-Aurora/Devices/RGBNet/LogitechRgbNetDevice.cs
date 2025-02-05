﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using RGB.NET.Core;
using RGB.NET.Devices.Logitech;

namespace Aurora.Devices.RGBNet;

public class LogitechRgbNetDevice : RgbNetDevice, IDisposable
{
    private bool _suspended;

    protected override IRGBDeviceProvider Provider => LogitechDeviceProvider.Instance;

    public override string DeviceName => "Logitech (RGB.NET)";
    protected override string DeviceInfo => string.Join(", ", Provider.Devices.Select(d => d.DeviceInfo.DeviceName));

    protected override void OnInitialized()
    {
        SystemEvents.PowerModeChanged += SystemEventsPowerModeChanged;
        SystemEvents.SessionSwitch += SystemEventsOnSessionSwitch;
    }

    protected override void OnShutdown()
    {
        SystemEvents.PowerModeChanged -= SystemEventsPowerModeChanged;
    }
    
    #region Event handlers

    private void SystemEventsOnSessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        if (!IsInitialized)
            return;

        if (e.Reason == SessionSwitchReason.SessionUnlock && _suspended)
            Task.Run(() =>
            {
                // Give LGS a moment to think about its sins
                Thread.Sleep(5000);
                _suspended = false;
                Initialize();
            });
    }

    private void SystemEventsPowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        if (!IsInitialized)
            return;

        if (e.Mode != PowerModes.Suspend || _suspended) return;
        _suspended = true;
        Shutdown();
    }

    #endregion

    public void Dispose()
    {
        SystemEvents.SessionSwitch -= SystemEventsOnSessionSwitch;
    }
}