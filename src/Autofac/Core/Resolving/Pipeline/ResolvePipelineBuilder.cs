﻿// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Globalization;
using Autofac.Core.Pipeline;
using Autofac.Diagnostics;

namespace Autofac.Core.Resolving.Pipeline;

/// <summary>
/// 提供构建解析管道的功能。
/// Provides the functionality to construct a resolve pipeline.
/// </summary>
/// <remarks>
/// <para>
/// The pipeline builder is built as a doubly-linked list; each node in that list is a
/// <see cref="MiddlewareDeclaration"/>, that holds the middleware instance, and the reference to the next and previous nodes.
/// </para>
///
/// <para>
/// When you call one of the Use* methods, we find the appropriate node in the linked list based on the phase of the new middleware
/// and insert it into the list.
/// </para>
///
/// <para>
/// When you build a pipeline, we walk back through that set of middleware and generate the concrete call chain so that when you execute the pipeline,
/// we don't iterate over any nodes, but just invoke the built set of methods.
/// </para>
/// </remarks>
internal class ResolvePipelineBuilder : IResolvePipelineBuilder, IEnumerable<IResolveMiddleware>
{
    /// <summary>
    /// Termination action for the end of pipelines.
    /// </summary>
    private static readonly Action<ResolveRequestContext> TerminateAction = context => { };

    private MiddlewareDeclaration? _first;
    private MiddlewareDeclaration? _last;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResolvePipelineBuilder"/> class.
    /// </summary>
    /// <param name="pipelineType">The pipeline type.</param>
    public ResolvePipelineBuilder(PipelineType pipelineType)
    {
        Type = pipelineType;
    }

    /// <inheritdoc/>
    public IEnumerable<IResolveMiddleware> Middleware => this;

    /// <inheritdoc/>
    public PipelineType Type { get; }

    /// <inheritdoc/>
    public IResolvePipelineBuilder Use(IResolveMiddleware stage, MiddlewareInsertionMode insertionMode = MiddlewareInsertionMode.EndOfPhase)
    {
        if (stage is null)
        {
            throw new ArgumentNullException(nameof(stage));
        }

        AddStage(stage, insertionMode);

        return this;
    }

    /// <inheritdoc/>
    public IResolvePipelineBuilder UseRange(IEnumerable<IResolveMiddleware> stages, MiddlewareInsertionMode insertionMode = MiddlewareInsertionMode.EndOfPhase)
    {
        if (stages is null)
        {
            throw new ArgumentNullException(nameof(stages));
        }

        // Use multiple stages.
        // Start at the beginning.
        var currentStage = _first;
        using var enumerator = stages.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return this;
        }

        var nextNewStage = enumerator.Current;
        var lastPhase = nextNewStage.Phase;

        VerifyPhase(nextNewStage.Phase);

        while (currentStage is not null)
        {
            if (insertionMode == MiddlewareInsertionMode.StartOfPhase ?
                    currentStage.Middleware.Phase >= nextNewStage.Phase :
                    currentStage.Middleware.Phase > nextNewStage.Phase)
            {
                var newDecl = new MiddlewareDeclaration(enumerator.Current);

                if (currentStage.Previous is not null)
                {
                    // Insert the node.
                    currentStage.Previous.Next = newDecl;
                    newDecl.Next = currentStage;
                    newDecl.Previous = currentStage.Previous;
                    currentStage.Previous = newDecl;
                }
                else
                {
                    _first!.Previous = newDecl;
                    newDecl.Next = _first;
                    _first = newDecl;
                }

                currentStage = newDecl;

                if (!enumerator.MoveNext())
                {
                    // Done.
                    return this;
                }

                nextNewStage = enumerator.Current;

                VerifyPhase(nextNewStage.Phase);

                if (nextNewStage.Phase < lastPhase)
                {
                    throw new InvalidOperationException(ResolvePipelineBuilderMessages.MiddlewareMustBeInPhaseOrder);
                }

                lastPhase = nextNewStage.Phase;
            }

            currentStage = currentStage.Next;
        }

        do
        {
            nextNewStage = enumerator.Current;

            VerifyPhase(nextNewStage.Phase);

            if (nextNewStage.Phase < lastPhase)
            {
                throw new InvalidOperationException(ResolvePipelineBuilderMessages.MiddlewareMustBeInPhaseOrder);
            }

            lastPhase = nextNewStage.Phase;

            var newStageDecl = new MiddlewareDeclaration(nextNewStage);

            if (_last is null)
            {
                _first = _last = newStageDecl;
            }
            else
            {
                newStageDecl.Previous = _last;
                _last.Next = newStageDecl;
                _last = newStageDecl;
            }
        }
        while (enumerator.MoveNext());

        return this;
    }

    /// <summary>
    /// MiddlewareInsertionMode ：StartOfPhase，EndOfPhase
    /// </summary>
    /// <param name="stage"></param>
    /// <param name="insertionLocation"></param>
    private void AddStage(IResolveMiddleware stage, MiddlewareInsertionMode insertionLocation)
    {
        VerifyPhase(stage.Phase);

        // Start at the beginning.
        var currentStage = _first;

        var newStageDecl = new MiddlewareDeclaration(stage);

        if (_first is null)
        {
            _first = _last = newStageDecl;
            return;
        }

        while (currentStage is not null)
        {            
            if (insertionLocation == MiddlewareInsertionMode.StartOfPhase ? currentStage.Middleware.Phase >= stage.Phase : currentStage.Middleware.Phase > stage.Phase)
            {
                if (currentStage.Previous is not null)
                {
                    // Insert the node.
                    currentStage.Previous.Next = newStageDecl;
                    newStageDecl.Next = currentStage;
                    newStageDecl.Previous = currentStage.Previous;
                    currentStage.Previous = newStageDecl;
                }
                else
                {
                    _first.Previous = newStageDecl;
                    newStageDecl.Next = _first;
                    _first = newStageDecl;
                }

                return;
            }

            currentStage = currentStage.Next;
        }

        // Add at the end.
        newStageDecl.Previous = _last;
        _last!.Next = newStageDecl;
        _last = newStageDecl;
    }

    private void AppendStage(IResolveMiddleware stage)
    {
        var newDecl = new MiddlewareDeclaration(stage);

        if (_last is null)
        {
            _first = _last = newDecl;
        }
        else
        {
            newDecl.Previous = _last;
            _last.Next = newDecl;
            _last = newDecl;
        }
    }

    /// <inheritdoc />
    public IResolvePipeline Build()
    {
        return BuildPipeline(_last);
    }

    private static ResolvePipeline BuildPipeline(MiddlewareDeclaration? lastDecl)
    {
        // When we build, we go through the set and construct a single call stack, starting from the end.
        // 当我们构建时，我们遍历集合并从末尾开始构建一个调用堆栈。
        var current = lastDecl;
        Action<ResolveRequestContext>? currentInvoke = TerminateAction;

        Action<ResolveRequestContext> Chain(Action<ResolveRequestContext> next, IResolveMiddleware stage)
        {
            var stagePhase = stage.Phase;
            return (context) =>
            {
                // if/else中的基本流程相同，但对诊断进行一次性检查，并选择“启用诊断”版本与更常见的“未启用诊断”路径：热路径优化。
                // Same basic flow in if/else, but doing a one-time check for diagnostics and choosing the "diagnostics enabled" version vs.
                // the more common "no diagnostics enabled" path: hot-path optimization.
                if (context.DiagnosticSource.IsEnabled())
                {
                    context.DiagnosticSource.MiddlewareStart(context, stage);
                    var succeeded = false;
                    try
                    {
                        context.PhaseReached = stagePhase;
                        stage.Execute(context, next);
                        succeeded = true;
                    }
                    finally
                    {
                        if (succeeded)
                        {
                            context.DiagnosticSource.MiddlewareSuccess(context, stage);
                        }
                        else
                        {
                            context.DiagnosticSource.MiddlewareFailure(context, stage);
                        }
                    }
                }
                else
                {
                    context.PhaseReached = stagePhase;
                    stage.Execute(context, next);
                }
            };
        }

        while (current is not null)
        {
            var stage = current.Middleware;
            currentInvoke = Chain(currentInvoke, stage);
            current = current.Previous;
        }

        return new ResolvePipeline(currentInvoke);
    }

    /// <inheritdoc/>
    public IResolvePipelineBuilder Clone()
    {
        // To clone a pipeline, we create a new instance, then insert the same stage
        // objects in the same order.
        var newPipeline = new ResolvePipelineBuilder(Type);
        var currentStage = _first;

        while (currentStage is not null)
        {
            newPipeline.AppendStage(currentStage.Middleware);
            currentStage = currentStage.Next;
        }

        return newPipeline;
    }

    /// <inheritdoc/>
    public IEnumerator<IResolveMiddleware> GetEnumerator()
    {
        return new PipelineBuilderEnumerator(_first);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void VerifyPhase(PipelinePhase middlewarePhase)
    {
        if (Type == PipelineType.Service)
        {
            if (middlewarePhase > PipelinePhase.ServicePipelineEnd)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        ResolvePipelineBuilderMessages.CannotAddRegistrationMiddlewareToServicePipeline,
                        middlewarePhase,
                        DescribeValidEnumRange(PipelinePhase.ResolveRequestStart, PipelinePhase.ServicePipelineEnd)));
            }
        }
        else if (middlewarePhase < PipelinePhase.RegistrationPipelineStart)
        {
            throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        ResolvePipelineBuilderMessages.CannotAddServiceMiddlewareToRegistrationPipeline,
                        middlewarePhase,
                        DescribeValidEnumRange(PipelinePhase.RegistrationPipelineStart, PipelinePhase.Activation)));
        }
    }

    private static string DescribeValidEnumRange(PipelinePhase start, PipelinePhase end)
    {
        var enumValues = Enum.GetValues(typeof(PipelinePhase))
                             .Cast<PipelinePhase>()
                             .Where(value => value >= start && value <= end);

        return string.Join(", ", enumValues);
    }
}
