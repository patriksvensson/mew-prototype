namespace Mew.CodeAnalysis;

internal static class CecilExtensions
{
    public static ModuleReference? GetModuleReference(this AssemblyDefinition assembly, string lib)
    {
        foreach (var module in assembly.Modules)
        {
            foreach (var moduleRef in module.ModuleReferences)
            {
                if (moduleRef.Name.Equals(lib, StringComparison.OrdinalIgnoreCase))
                {
                    return moduleRef;
                }
            }
        }

        return null;
    }

    public static void AddAttribute<T>(
        this ICustomAttributeProvider targetMember,
        ModuleDefinition module,
        params object[] arguments)
            where T : Attribute
    {
        var attribType = typeof(T);

        var constructor = attribType.GetConstructors()[0];
        var constructorRef = module.ImportReference(constructor);
        var attribute = new CustomAttribute(constructorRef);

        foreach (var argument in arguments)
        {
            attribute.ConstructorArguments.Add(
                new CustomAttributeArgument(
                    module.ImportReference(argument.GetType()),
                    argument));
        }

        targetMember.CustomAttributes.Add(attribute);
    }
}
