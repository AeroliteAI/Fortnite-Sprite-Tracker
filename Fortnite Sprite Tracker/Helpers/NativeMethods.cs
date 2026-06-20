using System.Runtime.InteropServices;

namespace FortniteSpriteTracker.Helpers;

internal static class NativeMethods
{
    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(nint hwnd, int attr, ref int attrValue, int attrSize);

    [DllImport("user32.dll")]
    private static extern int SetWindowCompositionAttribute(nint hwnd, ref WindowCompositionAttributeData data);

    [StructLayout(LayoutKind.Sequential)]
    private struct AccentPolicy
    {
        public int AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WindowCompositionAttributeData
    {
        public int  Attribute;
        public nint Data;
        public int  SizeOfData;
    }

    public static void EnableAcrylic(nint hwnd, int tintAbgr = 0x00000000)
    {
        var accent = new AccentPolicy { AccentState = 4, GradientColor = tintAbgr };
        int size   = Marshal.SizeOf<AccentPolicy>();
        var ptr    = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(accent, ptr, false);
            var data = new WindowCompositionAttributeData { Attribute = 19, Data = ptr, SizeOfData = size };
            SetWindowCompositionAttribute(hwnd, ref data);
        }
        finally { Marshal.FreeHGlobal(ptr); }
    }

    public static void EnableRoundedCorners(nint hwnd)
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000)) return;
        int pref = 2; // DWMWCP_ROUND
        DwmSetWindowAttribute(hwnd, 33, ref pref, sizeof(int));
    }
}
