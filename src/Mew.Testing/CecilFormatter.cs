// Copyright(c) 2008 - 2015 Jb Evain
// Copyright (c) 2008 - 2011 Novell, Inc.
// Licensed under MIT: https://github.com/jbevain/cecil/blob/master/LICENSE.txt
// https://github.com/jbevain/cecil/blob/master/Test/Mono.Cecil.Tests/Formatter.cs

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Mew.Testing;

internal static class CecilFormatter
{
    public static string FormatInstruction(Instruction instruction)
    {
        var writer = new StringWriter();
        WriteInstruction(writer, instruction);
        return writer.ToString();
    }

    public static string FormatMethodBody(MethodDefinition method)
    {
        var writer = new StringWriter();
        WriteMethodBody(writer, method);
        return writer.ToString();
    }

    public static void WriteMethodBody(TextWriter writer, MethodDefinition method)
    {
        var body = method.Body;

        WriteVariables(writer, body);

        foreach (var instruction in body.Instructions)
        {
            var sequence_point = body.Method.DebugInformation.GetSequencePoint(instruction);
            if (sequence_point != null)
            {
                WriteSequencePoint(writer, sequence_point);
                writer.WriteLine();
            }

            WriteInstruction(writer, instruction);
            writer.WriteLine();
        }

        WriteExceptionHandlers(writer, body);
    }

    private static void WriteVariables(TextWriter writer, MethodBody body)
    {
        var variables = body.Variables;

        writer.Write(".locals {0}(", body.InitLocals ? "init " : string.Empty);

        for (var i = 0; i < variables.Count; i++)
        {
            if (i > 0)
            {
                writer.Write(", ");
            }

            var variable = variables[i];

            writer.Write("{0} {1}", variable.VariableType, GetVariableName(variable, body));
        }

        writer.WriteLine(")");
    }

    private static string GetVariableName(VariableDefinition variable, MethodBody body)
    {
        string name;
        if (body.Method.DebugInformation.TryGetName(variable, out name))
        {
            return name;
        }

        return variable.ToString();
    }

    private static void WriteInstruction(TextWriter writer, Instruction instruction)
    {
        writer.Write(FormatLabel(instruction.Offset));
        writer.Write(": ");
        writer.Write(instruction.OpCode.Name);
        if (instruction.Operand != null)
        {
            writer.Write(' ');
            WriteOperand(writer, instruction.Operand);
        }
    }

    private static void WriteSequencePoint(TextWriter writer, SequencePoint sequence_point)
    {
        if (sequence_point.IsHidden)
        {
            writer.Write(".line hidden '{0}'", sequence_point.Document.Url);
            return;
        }

        writer.Write(
            ".line {0},{1}:{2},{3} '{4}'",
            sequence_point.StartLine,
            sequence_point.EndLine,
            sequence_point.StartColumn,
            sequence_point.EndColumn,
            sequence_point.Document.Url);
    }

    private static string FormatLabel(int offset)
    {
        var label = "000" + offset.ToString("x");
        return "IL_" + label.Substring(label.Length - 4);
    }

    private static string FormatLabel(Instruction instruction)
    {
        return FormatLabel(instruction.Offset);
    }

    private static void WriteOperand(TextWriter writer, object operand)
    {
        if (operand == null)
        {
            throw new ArgumentNullException("operand");
        }

        var target = operand as Instruction;
        if (target != null)
        {
            writer.Write(FormatLabel(target.Offset));
            return;
        }

        var targets = operand as Instruction[];
        if (targets != null)
        {
            WriteLabelList(writer, targets);
            return;
        }

        var s = operand as string;
        if (s != null)
        {
            writer.Write("\"" + s + "\"");
            return;
        }

        var parameter = operand as ParameterDefinition;
        if (parameter != null)
        {
            writer.Write(ToInvariantCultureString(parameter.Sequence));
            return;
        }

        s = ToInvariantCultureString(operand);
        writer.Write(s);
    }

    private static void WriteLabelList(TextWriter writer, Instruction[] instructions)
    {
        writer.Write("(");

        for (var i = 0; i < instructions.Length; i++)
        {
            if (i != 0)
            {
                writer.Write(", ");
            }

            writer.Write(FormatLabel(instructions[i].Offset));
        }

        writer.Write(")");
    }

    private static void WriteExceptionHandlers(TextWriter writer, MethodBody body)
    {
        if (!body.HasExceptionHandlers)
        {
            return;
        }

        foreach (var handler in body.ExceptionHandlers)
        {
            writer.Write("\t");
            writer.WriteLine(
                ".try {0} to {1} {2} handler {3} to {4}",
                FormatLabel(handler.TryStart),
                FormatLabel(handler.TryEnd),
                FormatHandlerType(handler),
                FormatLabel(handler.HandlerStart),
                FormatLabel(handler.HandlerEnd));
        }
    }

    private static string FormatHandlerType(ExceptionHandler handler)
    {
        var handler_type = handler.HandlerType;
        var type = handler_type.ToString().ToLowerInvariant();

        switch (handler_type)
        {
            case ExceptionHandlerType.Catch:
                return string.Format("{0} {1}", type, handler.CatchType.FullName);
            case ExceptionHandlerType.Filter:
                throw new NotSupportedException();
            default:
                return type;
        }
    }

    public static string? ToInvariantCultureString(object value)
    {
        var convertible = value as IConvertible;
        return convertible != null
            ? convertible.ToString(CultureInfo.InvariantCulture)
            : value.ToString();
    }
}
