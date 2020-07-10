// This software is part of the Autofac IoC container
// Copyright © 2020 Autofac Contributors
// https://autofac.org
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A1 PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using Autofac.Core.Diagnostics;
using Xunit;

namespace Autofac.Test.Core.Diagnostics
{
    public class FullOperationDiagnosticTracerBaseTests
    {
        [InlineData(DiagnosticEventKeys.MiddlewareStart)]
        [InlineData(DiagnosticEventKeys.MiddlewareFailure)]
        [InlineData(DiagnosticEventKeys.MiddlewareSuccess)]
        [InlineData(DiagnosticEventKeys.OperationFailure)]
        [InlineData(DiagnosticEventKeys.OperationStart)]
        [InlineData(DiagnosticEventKeys.OperationSuccess)]
        [InlineData(DiagnosticEventKeys.RequestFailure)]
        [InlineData(DiagnosticEventKeys.RequestStart)]
        [InlineData(DiagnosticEventKeys.RequestSuccess)]
        [Theory]
        public void Ctor_SubscribesToAllEvents(string key)
        {
            var tracer = new TestTracer();
            Assert.True(tracer.IsEnabled(key));
        }

        private class TestTracer : FullOperationDiagnosticTracerBase
        {
            public TestTracer()
                : base()
            {
            }

            public override int OperationsInProgress => throw new System.NotImplementedException();
        }
    }
}
