﻿namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="SA1506ElementDocumentationHeadersMustNotBeFollowedByBlankLine"/> class.
    /// </summary>
    public class SA1506UnitTests : CodeFixVerifier
    {
        public static IEnumerable<object[]> TypeTestData
        {
            get
            {
                yield return new object[] { "class" };
                yield return new object[] { "struct" };
                yield return new object[] { "interface" };
                yield return new object[] { "enum" };
            }
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle an empty source.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that type declarations with valid (or no) documentation will not produce diagnostics.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeTestData))]
        public async Task TestValidTypeDeclarationAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType1
    {{
    }}

    /// <summary>
    /// This is a test type.
    /// </summary>
    public {typeKeyword} TestType2
    {{
    }}
}}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that type declarations with invalid documentation will produce the expected diagnostics.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeTestData))]
        public async Task TestInvalidTypeDeclarationAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    /// <summary>
    /// This is a test type.
    /// </summary>

    public {typeKeyword} TestType
    {{
    }}
}}
";

            var fixedTestCode = $@"namespace TestNamespace
{{
    /// <summary>
    /// This is a test type.
    /// </summary>
    public {typeKeyword} TestType
    {{
    }}
}}
";

            var expectedResult = this.CSharpDiagnostic().WithLocation(6, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedResult, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that method-like declarations with valid (or no) documentation will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidMethodLikeDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        /// <summary>
        /// This is a constructor
        /// </summary>
        public TestClass()
        {
        }

        private TestClass(int value)
        {
        }

        /// <summary>
        /// This is a destructor
        /// </summary>
        ~TestClass()
        {
        }

        /// <summary>
        /// This is a method.
        /// </summary>
        public void TestMethod1()
        {
        }

        public void TestMethod2()
        {
        }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that method-like declarations with invalid documentation will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidMethodLikeDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        /// <summary>
        /// This is a constructor
        /// </summary>

        public TestClass()
        {
        }

        /// <summary>
        /// This is a destructor
        /// </summary>

        ~TestClass()
        {
        }

        /// <summary>
        /// This is a method.
        /// </summary>

        public void TestMethod()
        {
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        /// <summary>
        /// This is a constructor
        /// </summary>
        public TestClass()
        {
        }

        /// <summary>
        /// This is a destructor
        /// </summary>
        ~TestClass()
        {
        }

        /// <summary>
        /// This is a method.
        /// </summary>
        public void TestMethod()
        {
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(8, 1),
                this.CSharpDiagnostic().WithLocation(16, 1),
                this.CSharpDiagnostic().WithLocation(24, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that property-like declarations with valid (or no) documentation will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidPropertyLikeDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        /// <summary>
        /// This is a property.
        /// </summary>
        public int TestProperty1
        {
            get; set;
        }

        public int TestProperty2
        {
            get; set;
        }

        /// <summary>
        /// This is an indexer.
        /// </summary>
        public int this[int index]
        {
            get { return index; }
        }

        public int this[byte index]
        {
            get { return index; }
        }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that property-like declarations with invalid documentation will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidPropertyLikeDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        /// <summary>
        /// This is a property.
        /// </summary>

        public int TestProperty
        {
            get; set;
        }

        /// <summary>
        /// This is an indexer.
        /// </summary>

        public int this[int index]
        {
            get { return index; }
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        /// <summary>
        /// This is a property.
        /// </summary>
        public int TestProperty
        {
            get; set;
        }

        /// <summary>
        /// This is an indexer.
        /// </summary>
        public int this[int index]
        {
            get { return index; }
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(8, 1),
                this.CSharpDiagnostic().WithLocation(17, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that field declarations with valid (or no) documentation will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidFieldDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        /// <summary>
        /// This is a field.
        /// </summary>
        public int testField1;

        public int testField2;
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that field declarations with invalid documentation will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidFieldDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        /// <summary>
        /// This is a field.
        /// </summary>

        public int testField;
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        /// <summary>
        /// This is a field.
        /// </summary>
        public int testField;
    }
}
";

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(8, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that delegate declarations with valid (or no) documentation will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidDelegateDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    /// <summary>
    /// This is a delegate.
    /// </summary>
    public delegate void TestDelegate1();

    public delegate void TestDelegate2();
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that delegate declarations with invalid documentation will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidDelegateDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    /// <summary>
    /// This is a delegate.
    /// </summary>

    public delegate void TestDelegate();
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    /// <summary>
    /// This is a delegate.
    /// </summary>
    public delegate void TestDelegate();
}
";

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(6, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that event declarations with valid (or no) documentation will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidEventDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        /// <summary>
        /// This is an event.
        /// </summary>
        public event EventHandler TestEvent1;

        public event EventHandler TestEvent2;

        /// <summary>
        /// This is an event.
        /// </summary>
        public event EventHandler TestEvent3
        {
            add { }
            remove { }
        }

        public event EventHandler TestEvent4
        {
            add { }
            remove { }
        }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that event declarations with invalid documentation will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidEventDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        /// <summary>
        /// This is an event.
        /// </summary>

        public event EventHandler TestEvent1;

        /// <summary>
        /// This is an event.
        /// </summary>

        public event EventHandler TestEvent2
        {
            add { }
            remove { }
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        /// <summary>
        /// This is an event.
        /// </summary>
        public event EventHandler TestEvent1;

        /// <summary>
        /// This is an event.
        /// </summary>
        public event EventHandler TestEvent2
        {
            add { }
            remove { }
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(10, 1),
                this.CSharpDiagnostic().WithLocation(16, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1506ElementDocumentationHeadersMustNotBeFollowedByBlankLine();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1506CodeFixProvider();
        }
    }
}
