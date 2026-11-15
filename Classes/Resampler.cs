using NAudio.Dsp;

namespace Drums4me.Classes;

/// <summary>
/// Rechnet einen Klang auf eine andere Abtastrate um, vorab und in hoher Güte. Zwei Stellen brauchen das: Dateien, die nicht
/// mit 44,1 kHz vorliegen, und Spuren mit verstellter Tonhöhe, denn höher oder tiefer spielen heißt schneller oder langsamer.
/// </summary>
internal static class Resampler
{
    // Ausgemessen im November 2026 mit Testtönen von 3 und 10 kHz: Mit diesen Werten liegen die Fehler unter der Messgrenze
    // von etwa -105 dB. Die frühere Umrechnung beim Laden (WdlResamplingSampleProvider in der Grundeinstellung) kam auf -27 dB,
    // die geradlinige Interpolation beim Abspielen verstellter Spuren lag in derselben Größenordnung.
    private const int SincSize = 64;
    private const int SincTable = 512;

    /// <summary>
    /// Liefert den Klang so, als wäre er mit <paramref name="targetRate"/> statt <paramref name="sourceRate"/> aufgenommen.
    /// Der Anfang bleibt, wo er ist: Der Einsatz verschiebt sich nicht.
    /// </summary>
    public static float[] Convert(float[] source, double sourceRate, double targetRate)
    {
        if (source.Length == 0 || sourceRate == targetRate)
        {
            return source;
        }

        var resampler = new WdlResampler();
        resampler.SetMode(true, 0, true, SincSize, SincTable);
        resampler.SetFeedMode(false);
        resampler.SetRates(sourceRate, targetRate);

        var output = new List<float>((int)(source.Length * targetRate / sourceRate) + 64);
        var block = new float[4096];
        var position = 0;
        while (true)
        {
            var needed = resampler.ResamplePrepare(block.Length, 1, out var input);
            var available = Math.Min(needed, source.Length - position);
            source.AsSpan(position, available).CopyTo(input);
            position += available;

            // Bekommt der Resampler weniger als verlangt, gibt er den Rest aus und ist danach leer.
            var produced = resampler.ResampleOut(block, available, block.Length, 1);
            output.AddRange(block.AsSpan(0, produced));
            if (produced == 0 && available == 0)
            {
                break;
            }
        }
        return [.. output];
    }
}
