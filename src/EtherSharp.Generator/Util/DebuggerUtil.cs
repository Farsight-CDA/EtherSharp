using System.Diagnostics;

namespace EtherSharp.Generator.Util;
public static class DebuggerUtils
{
#if DEBUG
    private static bool LaunchedBefore;

    public static void Attach()
    {

        if(!Debugger.IsAttached && !LaunchedBefore)
        {
            LaunchedBefore = true;
            Debugger.Launch();
        }
    }
#endif
}
