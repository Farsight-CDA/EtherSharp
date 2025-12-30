namespace EtherSharp.Generator.Util;

internal static class DebuggerUtils
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
