using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: AllowPartiallyTrustedCallers]

[assembly: InternalsVisibleTo("NRules.Tests")]
[assembly: InternalsVisibleTo("NRules.IntegrationTests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]