using System.Runtime.InteropServices;

namespace FileShare.App.Icons;

public static class DefaultIcons
{
    private static Icon folderIcon;

    public static Icon FolderLarge => folderIcon ?? (folderIcon = GetStockIcon(SHSIID_FOLDER, SHGSI_LARGEICON));

    private static Icon GetStockIcon(uint type, uint size)
    {
        var info = new SHSTOCKICONINFO();
        info.cbSize = (uint)Marshal.SizeOf(info);

        SHGetStockIconInfo(type, SHGSI_ICON | size, ref info);

        var icon = (Icon)Icon.FromHandle(info.hIcon).Clone();
        DestroyIcon(info.hIcon);

        return icon;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SHSTOCKICONINFO
    {
        public uint cbSize;
        public IntPtr hIcon;
        public int iSysIconIndex;
        public int iIcon;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szPath;
    }

    [DllImport("shell32.dll")]
    public static extern int SHGetStockIconInfo(uint siid, uint uFlags, ref SHSTOCKICONINFO psii);

    [DllImport("user32.dll")]
    public static extern bool DestroyIcon(IntPtr handle);

    private const uint SHSIID_FOLDER = 0x3;
    private const uint SHGSI_ICON = 0x100;
    private const uint SHGSI_LARGEICON = 0x0;
    private const uint SHGSI_SMALLICON = 0x1;
}