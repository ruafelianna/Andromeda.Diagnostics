using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;

namespace Andromeda.Diagnostics
{
    /// <summary>
    /// Base events listener
    /// </summary>
    public class DiagnosticListenerBase : DiagnosticListener
    {
        public DiagnosticListenerBase(string name) : base(name)
        {
            var context = AssemblyLoadContext.GetLoadContext(
                Assembly.GetExecutingAssembly()
            );

            if (context is not null)
            {
                context.Unloading += _ => Dispose();
            }
        }
    }
}
