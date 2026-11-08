using NAudio.CoreAudioApi;
using System.Runtime.InteropServices;

namespace Drums4me.Classes;

/// <summary>Ein Eintrag der Geräteauswahl. Id null steht für den Windows-Standard.</summary>
internal sealed record AudioDeviceChoice(string? Id, string Name)
{
    public override string ToString() => Name;
}

/// <summary>Die Ausgabegeräte von Windows, etwa Lautsprecher oder ein Audio-Interface für den Kopfhörer.</summary>
internal static class AudioDevices
{
    public const string DefaultName = "Windows-Standard";

    /// <summary>Alle angeschlossenen Ausgabegeräte, alphabetisch, vorneweg der Windows-Standard.</summary>
    public static List<AudioDeviceChoice> List()
    {
        var devices = new List<AudioDeviceChoice>();
        try
        {
            using var enumerator = new MMDeviceEnumerator();
            foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                using (device)
                {
                    devices.Add(new AudioDeviceChoice(device.ID, device.FriendlyName));
                }
            }
        }
        catch (COMException)
        {
            // Ohne Liste bleibt der Windows-Standard, und der funktioniert immer, solange es überhaupt ein Gerät gibt.
        }

        devices.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCultureIgnoreCase));
        devices.Insert(0, new AudioDeviceChoice(null, DefaultName));
        return devices;
    }

    /// <summary>Öffnet das gewählte Gerät. Null heißt Windows-Standard, auch wenn das gewählte Gerät gerade nicht angeschlossen ist.</summary>
    public static MMDevice? Open(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }

        try
        {
            using var enumerator = new MMDeviceEnumerator();
            var device = enumerator.GetDevice(id);
            if (device.State == DeviceState.Active)
            {
                return device;
            }
            device.Dispose();
        }
        catch (COMException)
        {
            // Das Gerät ist Windows nicht mehr bekannt.
        }
        return null;
    }
}
