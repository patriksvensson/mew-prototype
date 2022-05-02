using System.Linq.Expressions;
using System.Reflection;

namespace Mew.CodeAnalysis.Interpreting;

internal sealed class ExternalFunctionInvoker : IExternalFunctionInvoker
{
    private readonly Dictionary<string, IntPtr> _libraries;
    private readonly Dictionary<string, Dictionary<string, IntPtr>> _functions;
    private bool _disposed;

    public ExternalFunctionInvoker()
    {
        _libraries = new Dictionary<string, IntPtr>();
        _functions = new Dictionary<string, Dictionary<string, IntPtr>>();
    }

    ~ExternalFunctionInvoker()
    {
        Dispose();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (_disposed)
        {
            return;
        }

        foreach (var (_, handle) in _libraries)
        {
            NativeLibrary.Free(handle);
        }

        _disposed = true;
    }

    public static class DelegateCreator
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
        // Hack to invoke a private
        private static readonly Func<Type[], Type> MakeNewCustomDelegate =
            (Func<Type[], Type>)Delegate.CreateDelegate(
                typeof(Func<Type[], Type>),
                typeof(Expression).Assembly.GetType("System.Linq.Expressions.Compiler.DelegateHelpers")
                    .GetMethod("MakeNewCustomDelegate", BindingFlags.NonPublic | BindingFlags.Static));
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        public static Type NewDelegateType(Type ret, params Type[] parameters)
        {
            var offset = parameters.Length;
            Array.Resize(ref parameters, offset + 1);
            parameters[offset] = ret;
            return MakeNewCustomDelegate(parameters);
        }
    }

    public object? Invoke(ExternalFunctionSymbol symbol, ImmutableArray<object?> arguments)
    {
        // TODO: Validate the arguments
        var returnType = GetReturnType(symbol.ReturnType);
        var parameters = symbol.Parameters.Select(x => GetReturnType(x.Type)).ToArray();

        var function = Marshal.GetDelegateForFunctionPointer(
            GetFunctionAddress("mewstd.dll", symbol.Name),
            DelegateCreator.NewDelegateType(
                returnType,
                parameters));

        return function.DynamicInvoke(arguments.ToArray());
    }

    private IntPtr GetFunctionAddress(string library, string function)
    {
        if (!_libraries.TryGetValue(library, out var handle))
        {
            // Load the library
            if (!NativeLibrary.TryLoad(library, out handle))
            {
                throw new InvalidOperationException($"Could not load {library}");
            }

            _libraries[library] = handle;
        }

        // Library not loaded?
        if (!_functions.TryGetValue(library, out var functions))
        {
            functions = _functions[library] = new Dictionary<string, IntPtr>(StringComparer.Ordinal);
        }

        // Function not loaded?
        if (!functions.TryGetValue(function, out var address))
        {
            // Get the method
            if (!NativeLibrary.TryGetExport(handle, function, out address))
            {
                throw new InvalidOperationException($"Could not get export '{function}'");
            }

            functions[function] = address;
        }

        return address;
    }

    private static Type GetReturnType(TypeSymbol symbol)
    {
        if (symbol == PrimitiveTypeSymbol.Void)
        {
            return typeof(void);
        }
        else if (symbol == PrimitiveTypeSymbol.String)
        {
            return typeof(string);
        }
        else if (symbol == PrimitiveTypeSymbol.Boolean)
        {
            return typeof(bool);
        }
        else if (symbol == PrimitiveTypeSymbol.Integer)
        {
            return typeof(long);
        }

        throw new InvalidOperationException("Could not get marshallable type");
    }
}