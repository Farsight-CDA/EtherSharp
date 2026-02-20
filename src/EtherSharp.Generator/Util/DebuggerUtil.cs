namespace EtherSharp.Generator.Util;

internal static class DebuggerUtils
{
#if DEBUG
    private static bool _launchedBefore;

    public static void Attach()
    {

        if(!Debugger.IsAttached && !_launchedBefore)
        {
            _launchedBefore = true;
            Debugger.Launch();
        }
    }
#endif
}
