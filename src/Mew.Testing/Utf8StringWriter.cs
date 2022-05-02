using System.Text;

namespace Mew.Testing;

internal class Utf8StringWriter : StringWriter
{
    public override Encoding Encoding => Encoding.UTF8;
}
