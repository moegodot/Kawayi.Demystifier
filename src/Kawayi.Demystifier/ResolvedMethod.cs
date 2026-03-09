// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;
using System.Text;
using Kawayi.Demystifier.Enumerable;

namespace Kawayi.Demystifier;

public class ResolvedMethod
{
    public MethodBase? MethodBase { get; set; }

    public Type? DeclaringType { get; set; }

    public bool IsAsync { get; set; }

    public bool IsLambda { get; set; }

    public ResolvedParameter? ReturnParameter { get; set; }

    public string? Name { get; set; }

    public int? Ordinal { get; set; }

    public string? GenericArguments { get; set; }

    public Type[]? ResolvedGenericArguments { get; set; }

    public MethodBase? SubMethodBase { get; set; }

    public string? SubMethod { get; set; }

    public EnumerableIList<ResolvedParameter> Parameters { get; set; }

    public EnumerableIList<ResolvedParameter> SubMethodParameters { get; set; }
    public int RecurseCount { get; internal set; }

    internal bool IsSequentialEquivalent(ResolvedMethod obj)
    {
        return
            IsAsync == obj.IsAsync &&
            DeclaringType == obj.DeclaringType &&
            Name == obj.Name &&
            IsLambda == obj.IsLambda &&
            Ordinal == obj.Ordinal &&
            GenericArguments == obj.GenericArguments &&
            SubMethod == obj.SubMethod;
    }

    public override string ToString() => Append(new StringBuilder()).ToString();

    public StringBuilder Append(StringBuilder builder)
        => Append(builder, true);

    public StyledStringBuilder Append(
        StyledStringBuilder stringBuilder,
        StyleOptions option)
        => Append(stringBuilder, true, option);

    public StringBuilder Append(StringBuilder builder, bool fullName)
    {
        if (IsAsync)
        {
            builder.Append("async ");
        }

        if (ReturnParameter != null)
        {
            ReturnParameter.Append(builder);
            builder.Append(" ");
        }

        if (DeclaringType != null)
        {

            if (Name == ".ctor")
            {
                if (string.IsNullOrEmpty(SubMethod) && !IsLambda)
                    builder.Append("new ");

                AppendDeclaringTypeName(builder, fullName);
            }
            else if (Name == ".cctor")
            {
                builder.Append("static ");
                AppendDeclaringTypeName(builder, fullName);
            }
            else
            {
                AppendDeclaringTypeName(builder, fullName)
                    .Append(".")
                    .Append(Name);
            }
        }
        else
        {
            builder.Append(Name);
        }
        builder.Append(GenericArguments);

        builder.Append("(");
        if (MethodBase != null)
        {
            var isFirst = true;
            foreach (var param in Parameters)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    builder.Append(", ");
                }
                param.Append(builder);
            }
        }
        else
        {
            builder.Append("?");
        }
        builder.Append(")");

        if (!string.IsNullOrEmpty(SubMethod) || IsLambda)
        {
            builder.Append("+");
            builder.Append(SubMethod);
            builder.Append("(");
            if (SubMethodBase != null)
            {
                var isFirst = true;
                foreach (var param in SubMethodParameters)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        builder.Append(", ");
                    }
                    param.Append(builder);
                }
            }
            else
            {
                builder.Append("?");
            }
            builder.Append(")");
            if (IsLambda)
            {
                builder.Append(" => { }");

                if (Ordinal.HasValue)
                {
                    builder.Append(" [");
                    builder.Append(Ordinal);
                    builder.Append("]");
                }
            }
        }

        if (RecurseCount > 0)
        {
            builder.Append($" x {RecurseCount + 1:0}");
        }

        return builder;
    }


    public StyledStringBuilder Append(StyledStringBuilder stringBuilder, bool fullName, StyleOptions option)
    {
        if (IsAsync)
        {
            stringBuilder.Append(option.KeywordAsyncStyle, "async ");
        }

        if (ReturnParameter != null)
        {
            {
                var sb = new StringBuilder();
                ReturnParameter.Append(sb);
                stringBuilder.Append(option.MethodReturnTypeStyle, sb.ToString());
            }
            stringBuilder.Append(" ");
        }

        if (DeclaringType != null)
        {

            if (Name == ".ctor")
            {
                if (string.IsNullOrEmpty(SubMethod) && !IsLambda)
                    stringBuilder.Append(option.KeywordNewStyle, "new ");

                AppendDeclaringTypeName(stringBuilder, fullName, option);
            }
            else if (Name == ".cctor")
            {
                stringBuilder.Append(option.KeywordStaticStyle, "static ");
                AppendDeclaringTypeName(stringBuilder, fullName, option);
            }
            else
            {
                AppendDeclaringTypeName(stringBuilder, fullName, option)
                    .Append(".")
                    .Append(option.MethodNameStyle, Name ?? "null");
            }
        }
        else
        {
            stringBuilder.Append(option.MethodNameStyle, Name ?? "null");
        }
        stringBuilder.Append(option.GenericArgumentStyle, GenericArguments ?? string.Empty);

        stringBuilder.Append("(");
        if (MethodBase != null)
        {
            var isFirst = true;
            foreach (var param in Parameters)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    stringBuilder.Append(", ");
                }
                param.Append(stringBuilder, option);
            }
        }
        else
        {
            stringBuilder.Append("?");
        }
        stringBuilder.Append(")");

        if (!string.IsNullOrEmpty(SubMethod) || IsLambda)
        {
            stringBuilder.Append("+");
            stringBuilder.Append(option.SubMethodOrLambdaStyle, new StringBuilder().Append(SubMethod).ToString());
            stringBuilder.Append("(");
            if (SubMethodBase != null)
            {
                var isFirst = true;
                foreach (var param in SubMethodParameters)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        stringBuilder.Append(", ");
                    }
                    param.Append(stringBuilder, option);
                }
            }
            else
            {
                stringBuilder.Append("?");
            }
            stringBuilder.Append(")");
            if (IsLambda)
            {
                stringBuilder.Append(option.SubMethodOrLambdaStyle, " => { }");

                if (Ordinal.HasValue)
                {
                    stringBuilder.Append(" [");
                    stringBuilder.Append(Ordinal.ToString() ??
                                         throw new InvalidOperationException());
                    stringBuilder.Append("]");
                }
            }
        }

        if (RecurseCount > 0)
        {
            stringBuilder.Append($" x {RecurseCount + 1:0}");
        }

        return stringBuilder;
    }

    private StringBuilder AppendDeclaringTypeName(StringBuilder builder, bool fullName = true)
    {
        return DeclaringType != null ? builder.AppendTypeDisplayName(DeclaringType, fullName: fullName, includeGenericParameterNames: true) : builder;
    }

    private StyledStringBuilder AppendDeclaringTypeName(StyledStringBuilder stringBuilder, bool fullName, StyleOptions option)
    {
        StringBuilder sb = new();
        AppendDeclaringTypeName(sb, fullName);
        return stringBuilder.Append(option.DeclaringTypeOfMethodStyle, sb.ToString());
    }
}