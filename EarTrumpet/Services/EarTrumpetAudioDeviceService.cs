﻿using EarTrumpet.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EarTrumpet.Services
{
    public class EarTrumpetAudioDeviceService
    {
        static class Interop
        {
            [DllImport("EarTrumpet.Interop.dll")]
            public static extern int RefreshAudioDevices();

            [DllImport("EarTrumpet.Interop.dll")]
            public static extern int GetAudioDevices(ref IntPtr devices);

            [DllImport("EarTrumpet.Interop.dll")]
            public static extern int GetAudioDeviceCount();

            [DllImport("EarTrumpet.Interop.dll")]
            public static extern int SetDefaultAudioDevice(string deviceId);
        }

        public IEnumerable<EarTrumpetAudioDeviceModel> GetAudioDevices()
        {
            Interop.RefreshAudioDevices();
            
            var deviceCount = Interop.GetAudioDeviceCount();
            var devices = new List<EarTrumpetAudioDeviceModel>();

            var rawDevicesPtr = IntPtr.Zero;
            Interop.GetAudioDevices(ref rawDevicesPtr);

            var sizeOfAudioDeviceStruct = Marshal.SizeOf(typeof(EarTrumpetAudioDeviceModel));
            for(var i = 0; i < deviceCount; i++)
            {
                var window = new IntPtr(rawDevicesPtr.ToInt64() + (sizeOfAudioDeviceStruct * i));
                
                var device = (EarTrumpetAudioDeviceModel)Marshal.PtrToStructure(window, typeof(EarTrumpetAudioDeviceModel));
                devices.Add(device);
            }
            return devices;
        }

        public void SetDefaultAudioDevice(EarTrumpetAudioDeviceModel device)
        {
            Interop.SetDefaultAudioDevice(device.Id);
        }
    }
}