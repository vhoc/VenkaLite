using System;
using System.Runtime.InteropServices;

namespace VenkaLite
{

    public static class OperatingSystem
    {

        public static bool IsWindows()  => RuntimeInformation.IsOSPlatform( OSPlatform.Windows );
        public static bool IsMacOs()    => RuntimeInformation.IsOSPlatform( OSPlatform.OSX );
        public static bool IsLinux()    => RuntimeInformation.IsOSPlatform( OSPlatform.Linux );

    }

}
