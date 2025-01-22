using Busylight;
using Microsoft.SemanticKernel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace SKIntroduction;
/// <summary>
/// Based (AKA copied :D) and inspired on the Busylight project from Bill Ayers
/// https://github.com/SPDoctor/Busylight
/// Thanks Bill!!
/// 
/// Also taken some ideas from:
/// https://github.com/hoovercj/BusyLight-Demo-CSharp
/// https://github.com/hoovercj/vsto-outlook-busylight
/// </summary>
public class BusyLightController
{
    private static SDK? _sdk = null;

    public BusyLightController()
    {
        InitializeSDK();
    }

    private static void InitializeSDK()
    {
        if (_sdk == null)
        {
            _sdk = new SDK(true); // Initialize with auto-detection of devices
        }
    }

    [KernelFunction, Description("Turn on the light with the specified RGB color")]
    public void LightOn(
        [Description("The percentage red color (0 to 100)"), Required] int red,
        [Description("The percentage green color (0 to 100)"), Required] int green,
        [Description("The percentage blue color (0 to 100)"), Required] int blue)
    {
        InitializeSDK();
        _sdk.Light(red, blue, green); // Note: colors in SDK are in the wrong order
    }

    [KernelFunction, Description("Turn off the light")]
    public void LightOff()
    {
        InitializeSDK();
        _sdk.Light(0, 0, 0);
    }

    [KernelFunction, Description("Make the light flash")]
    public void LightFlash(
        [Description("The percentage red colour (from 0 to 100)"), Required] int red,
        [Description("The percentage green colour (from 0 to 100)"), Required] int green,
        [Description("The percentage blue colour (from 0 to 100)"), Required] int blue,
        [Description("The on time, in number of tenths of a second (from 0 to 255)")] int onTime = 2,
        [Description("The off time, in number of tenths of a second (from 0 to 255)")] int offTime = 2
      )
    {
        if (_sdk == null) _sdk = new Busylight.SDK(true);
        // there is a design error in the SDK, the colours are in the wrong order
        _sdk.Blink(red, blue, green, onTime, offTime);
        Thread.Sleep(4000);
        LightOff();
    }

    //[KernelFunction, Description("Make the light pulse with specified RGB color")]
    //public void LightPulse(
    //    [Description("The percentage red color (0 to 100)"), Required] int red,
    //    [Description("The percentage green color (0 to 100)"), Required] int green,
    //    [Description("The percentage blue color (0 to 100)"), Required] int blue,
    //    [Description("Intensity step values for the pulse (7 values required)")] int[] steps)
    //{
    //    if (steps.Length != 7)
    //    {
    //        throw new ArgumentException("Exactly 7 intensity step values must be provided.");
    //    }

    //    InitializeSDK();

    //    // Create a pulse sequence with specified color and steps
    //    PulseSequence pulseSequence = new PulseSequence
    //    {
    //        Color = new BusylightColor(red, green, blue),
    //        Step1 = steps[0],
    //        Step2 = steps[1],
    //        Step3 = steps[2],
    //        Step4 = steps[3],
    //        Step5 = steps[4],
    //        Step6 = steps[5],
    //        Step7 = steps[6]
    //    };

    //    _sdk.Pulse(pulseSequence);
    //}

    [KernelFunction, Description("Play a jingle with the banana color")]
    public void PlayJingleBanana()
    {
        _sdk.Alert(BusylightColor.Yellow, BusylightSoundClip.KuandoTrain, BusylightVolume.Low);
        Thread.Sleep(3000);
        LightOff();
    }

    [KernelFunction, Description("Success, Green color")]
    public void Success()
    {
        _sdk.Alert(BusylightColor.Green, BusylightSoundClip.FairyTale, BusylightVolume.Low);
        Thread.Sleep(3000);
        LightOff();
    }

    [KernelFunction, Description("Failure, Red color")]
    public void Failure()
    {
        _sdk.Alert(BusylightColor.Red, BusylightSoundClip.Funky, BusylightVolume.Low);
        Thread.Sleep(3000);
        LightOff();
    }

    public void Alert(BusylightColor color, BusylightSoundClip clip, BusylightVolume volume) { 
        _sdk.Alert(color, clip, volume);
        Thread.Sleep(3000);
        LightOff();
    }
}