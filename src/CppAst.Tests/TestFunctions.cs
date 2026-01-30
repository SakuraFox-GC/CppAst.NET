using NUnit.Framework;
using System;
using System.IO;

namespace CppAst.Tests
{
    public class TestFunctions : InlineTestBase
    {
        [Test]
        public void TestSimple()
        {
            ParseAssert(@"
void function0();
int function1(int a, float b);
float function2(int);
",
                compilation =>
                {
                    Assert.False(compilation.HasErrors);

                    Assert.AreEqual(3, compilation.Functions.Count);

                    {
                        var cppFunction = compilation.Functions[0];
                        Assert.AreEqual("function0", cppFunction.Name);
                        Assert.AreEqual(0, cppFunction.Parameters.Count);
                        Assert.AreEqual("void", cppFunction.ReturnType.ToString());

                        var cppFunction1 = compilation.FindByName<CppFunction>("function0");
                        Assert.AreEqual(cppFunction, cppFunction1);
                    }

                    {
                        var cppFunction = compilation.Functions[1];
                        Assert.AreEqual("function1", cppFunction.Name);
                        Assert.AreEqual(2, cppFunction.Parameters.Count);
                        Assert.AreEqual("a", cppFunction.Parameters[0].Name);
                        Assert.AreEqual(CppTypeKind.Primitive, cppFunction.Parameters[0].Type.TypeKind);
                        Assert.AreEqual(CppPrimitiveKind.Int, ((CppPrimitiveType)cppFunction.Parameters[0].Type).Kind);
                        Assert.AreEqual("b", cppFunction.Parameters[1].Name);
                        Assert.AreEqual(CppTypeKind.Primitive, cppFunction.Parameters[1].Type.TypeKind);
                        Assert.AreEqual(CppPrimitiveKind.Float, ((CppPrimitiveType)cppFunction.Parameters[1].Type).Kind);
                        Assert.AreEqual("int", cppFunction.ReturnType.ToString());

                        var cppFunction1 = compilation.FindByName<CppFunction>("function1");
                        Assert.AreEqual(cppFunction, cppFunction1);
                    }
                    {
                        var cppFunction = compilation.Functions[2];
                        Assert.AreEqual("function2", cppFunction.Name);
                        Assert.AreEqual(1, cppFunction.Parameters.Count);
                        Assert.AreEqual(string.Empty, cppFunction.Parameters[0].Name);
                        Assert.AreEqual(CppTypeKind.Primitive, cppFunction.Parameters[0].Type.TypeKind);
                        Assert.AreEqual(CppPrimitiveKind.Int, ((CppPrimitiveType)cppFunction.Parameters[0].Type).Kind);
                        Assert.AreEqual("float", cppFunction.ReturnType.ToString());

                        var cppFunction1 = compilation.FindByName<CppFunction>("function2");
                        Assert.AreEqual(cppFunction, cppFunction1);
                    }
                    {
                    }
                }
            );
        }


        [Test]
        public void TestFunctionPrototype()
        {
            ParseAssert(@"
typedef void (*function0)(int a, float b);
typedef void (*function1)(int, float);
",
                compilation =>
                {
                    Assert.False(compilation.HasErrors);

                    Assert.AreEqual(2, compilation.Typedefs.Count);

                    {
                        var cppType = compilation.Typedefs[0].ElementType;
                        Assert.AreEqual(CppTypeKind.Pointer, cppType.TypeKind);
                        var cppPointerType = (CppPointerType)cppType;
                        Assert.AreEqual(CppTypeKind.Function, cppPointerType.ElementType.TypeKind);
                        var cppFunctionType = (CppFunctionType)cppPointerType.ElementType;
                        Assert.AreEqual(2, cppFunctionType.Parameters.Count);

                        Assert.AreEqual("a", cppFunctionType.Parameters[0].Name);
                        Assert.AreEqual(CppPrimitiveType.Int, cppFunctionType.Parameters[0].Type);

                        Assert.AreEqual("b", cppFunctionType.Parameters[1].Name);
                        Assert.AreEqual(CppPrimitiveType.Float, cppFunctionType.Parameters[1].Type);
                    }

                    {
                        var cppType = compilation.Typedefs[1].ElementType;
                        Assert.AreEqual(CppTypeKind.Pointer, cppType.TypeKind);
                        var cppPointerType = (CppPointerType)cppType;
                        Assert.AreEqual(CppTypeKind.Function, cppPointerType.ElementType.TypeKind);
                        var cppFunctionType = (CppFunctionType)cppPointerType.ElementType;
                        Assert.AreEqual(2, cppFunctionType.Parameters.Count);

                        Assert.AreEqual(string.Empty, cppFunctionType.Parameters[0].Name);
                        Assert.AreEqual(CppPrimitiveType.Int, cppFunctionType.Parameters[0].Type);

                        Assert.AreEqual(string.Empty, cppFunctionType.Parameters[1].Name);
                        Assert.AreEqual(CppPrimitiveType.Float, cppFunctionType.Parameters[1].Type);
                    }

                }
            );
        }

        [Test]
        public void TestFunctionFields()
        {
            ParseAssert(@"
typedef struct struct0 {
    void (*function0)(int a, float b);
    void (*function1)(char, int);
} struct0;
",
                compilation =>
                {
                    Assert.False(compilation.HasErrors);

                    var cls = compilation.Classes[0];
                    Assert.AreEqual(2, cls.Fields.Count);

                    {
                        var cppType = cls.Fields[0].Type;
                        Assert.AreEqual(CppTypeKind.Pointer, cppType.TypeKind);
                        var cppPointerType = (CppPointerType)cppType;
                        Assert.AreEqual(CppTypeKind.Function, cppPointerType.ElementType.TypeKind);
                        var cppFunctionType = (CppFunctionType)cppPointerType.ElementType;
                        Assert.AreEqual(2, cppFunctionType.Parameters.Count);

                        Assert.AreEqual("a", cppFunctionType.Parameters[0].Name);
                        Assert.AreEqual(CppPrimitiveType.Int, cppFunctionType.Parameters[0].Type);

                        Assert.AreEqual("b", cppFunctionType.Parameters[1].Name);
                        Assert.AreEqual(CppPrimitiveType.Float, cppFunctionType.Parameters[1].Type);
                    }

                    {
                        var cppType = cls.Fields[1].Type;
                        Assert.AreEqual(CppTypeKind.Pointer, cppType.TypeKind);
                        var cppPointerType = (CppPointerType)cppType;
                        Assert.AreEqual(CppTypeKind.Function, cppPointerType.ElementType.TypeKind);
                        var cppFunctionType = (CppFunctionType)cppPointerType.ElementType;
                        Assert.AreEqual(2, cppFunctionType.Parameters.Count);

                        Assert.AreEqual(string.Empty, cppFunctionType.Parameters[0].Name);
                        Assert.AreEqual(CppPrimitiveType.Char, cppFunctionType.Parameters[0].Type);

                        Assert.AreEqual(string.Empty, cppFunctionType.Parameters[1].Name);
                        Assert.AreEqual(CppPrimitiveType.Int, cppFunctionType.Parameters[1].Type);
                    }

                }
            );
        }


        [Test]
        public void TestFunctionTypedefFields()
        {
            ParseAssert(@"
typedef struct struct0 struct0;
typedef void (*function0_t)(int a, float b);
typedef void (*function1_t)(char, int);
struct struct0
{
    function0_t function0;
    function1_t function1;
};
",
                compilation =>
                {
                    Assert.False(compilation.HasErrors);

                    var cls = compilation.Classes[0];
                    Assert.AreEqual(2, cls.Fields.Count);

                    {
                        var cppType = cls.Fields[0].Type;
                        Assert.AreEqual(CppTypeKind.Typedef, cppType.TypeKind);
                    }

                    {
                        var cppType = cls.Fields[1].Type;
                        Assert.AreEqual(CppTypeKind.Typedef, cppType.TypeKind);
                    }
                }
            );
        }

        [Test]
        public void TestFunctionExport()
        {
            var text = @"
#ifdef WIN32
#define EXPORT_API __declspec(dllexport)
#else
#define EXPORT_API __attribute__((visibility(""default"")))
#endif
EXPORT_API int function0();
int function1();
";

            ParseAssert(text,
                compilation =>
                {
                    Assert.False(compilation.HasErrors);

                    Assert.AreEqual(2, compilation.Functions.Count);

                    {
                        var cppFunction = compilation.Functions[0];
                        Assert.AreEqual(1, cppFunction.Attributes.Count);
                        Assert.True(cppFunction.IsPublicExport());
                    }
                    {
                        var cppFunction = compilation.Functions[1];
                        Assert.AreEqual(0, cppFunction.Attributes.Count);
                        Assert.True(cppFunction.IsPublicExport());
                    }
                },
                new CppParserOptions() {  }
            );

            ParseAssert(text,
                compilation =>
                {
                    Assert.False(compilation.HasErrors);

                    Assert.AreEqual(2, compilation.Functions.Count);

                    {
                        var cppFunction = compilation.Functions[0];
                        Assert.AreEqual(1, cppFunction.Attributes.Count);
                        Assert.True(cppFunction.IsPublicExport());
                    }
                    {
                        var cppFunction = compilation.Functions[1];
                        Assert.AreEqual(0, cppFunction.Attributes.Count);
                        Assert.True(cppFunction.IsPublicExport());
                    }
                }, new CppParserOptions() { }.ConfigureForWindowsMsvc()
            );
        }

        [Test]
        public void TestFunctionVariadic()
        {
            ParseAssert(@"
void function0();
void function1(...);
void function2(int, ...);
",
                compilation =>
                {
                    Assert.False(compilation.HasErrors);

                    Assert.AreEqual(3, compilation.Functions.Count);

                    {
                        var cppFunction = compilation.Functions[0];
                        Assert.AreEqual(0, cppFunction.Parameters.Count);
                        Assert.AreEqual("void", cppFunction.ReturnType.ToString());
                        Assert.AreEqual(CppFunctionFlags.None, cppFunction.Flags & CppFunctionFlags.Variadic);
                    }

                    {
                        var cppFunction = compilation.Functions[1];
                        Assert.AreEqual(0, cppFunction.Parameters.Count);
                        Assert.AreEqual("void", cppFunction.ReturnType.ToString());
                        Assert.AreEqual(CppFunctionFlags.Variadic, cppFunction.Flags & CppFunctionFlags.Variadic);
                    }

                    {
                        var cppFunction = compilation.Functions[2];
                        Assert.AreEqual(1, cppFunction.Parameters.Count);
                        Assert.AreEqual(string.Empty, cppFunction.Parameters[0].Name);
                        Assert.AreEqual(CppTypeKind.Primitive, cppFunction.Parameters[0].Type.TypeKind);
                        Assert.AreEqual(CppPrimitiveKind.Int, ((CppPrimitiveType)cppFunction.Parameters[0].Type).Kind);
                        Assert.AreEqual("void", cppFunction.ReturnType.ToString());
                        Assert.AreEqual(CppFunctionFlags.Variadic, cppFunction.Flags & CppFunctionFlags.Variadic);
                    }
                }
            );
        }



        [Test]
        public void TestFunctionTemplate()
        {
            ParseAssert(@"
template<class T>
void function0(T t);
",
                compilation =>
                {
                    Assert.False(compilation.HasErrors);

                    Assert.AreEqual(1, compilation.Functions.Count);

                    {
                        var cppFunction = compilation.Functions[0];
                        Assert.AreEqual(1, cppFunction.Parameters.Count);
                        Assert.AreEqual("void", cppFunction.ReturnType.ToString());
                        Assert.AreEqual(cppFunction.IsFunctionTemplate, true);
                        Assert.AreEqual(cppFunction.TemplateParameters.Count, 1);
                    }

                }
            );
        }


        [Test]
        public void TestFunctionPointersByParam()
        {
            ParseAssert(@"
void function0(int a, int b, float (*callback)(void*, double));
",
                compilation =>
                {
                    Assert.False(compilation.HasErrors);

                    Assert.AreEqual(1, compilation.Functions.Count);

                    {
                        var cppFunction = compilation.Functions[0];
                        Assert.AreEqual("function0", cppFunction.Name);
                        Assert.AreEqual(3, cppFunction.Parameters.Count);

                        Assert.IsInstanceOf<CppPointerType>(cppFunction.Parameters[2].Type);
                        var pointerType = (CppPointerType)cppFunction.Parameters[2].Type;
                        Assert.IsInstanceOf<CppFunctionType>(pointerType.ElementType);
                        var functionType = (CppFunctionType)pointerType.ElementType;
                        Assert.AreEqual(2, functionType.Parameters.Count);
                        Assert.AreEqual("float", functionType.ReturnType.ToString());
                        Assert.AreEqual("void *", functionType.Parameters[0].Type.ToString());
                        Assert.AreEqual("double", functionType.Parameters[1].Type.ToString());


                        Assert.AreEqual("void", cppFunction.ReturnType.ToString());

                        var cppFunction1 = compilation.FindByName<CppFunction>("function0");
                        Assert.AreEqual(cppFunction, cppFunction1);
                    }
                }
            );
        }

        [Test]
        public void TestFunctionBody()
        {
            var options = new CppParserOptions();
            options.ParseFunctionBodies = true;
            var headerFilename = "test_function_body.h";
            
            var text = @"
void function0();
int function1(int a, float b) {
    return a + (int)b;
}
float function2(int x);
";

            var currentDirectory = Environment.CurrentDirectory;
            var headerFile = Path.Combine(currentDirectory, headerFilename);
            File.WriteAllText(headerFile, text);
            
            var compilation = CppParser.ParseFile(headerFile, options);

            Assert.False(compilation.HasErrors);
            Assert.AreEqual(3, compilation.Functions.Count);

            {
                var cppFunction = compilation.Functions[0];
                Assert.AreEqual("function0", cppFunction.Name);
                Assert.IsNull(cppFunction.BodySpan);
            }

            {
                var cppFunction = compilation.Functions[1];
                Assert.AreEqual("function1", cppFunction.Name);
                Assert.IsNotNull(cppFunction.BodySpan);
                Assert.Greater(cppFunction.BodySpan.Value.Start.Line, 0);
                Assert.Greater(cppFunction.BodySpan.Value.End.Line, 0);
                Assert.GreaterOrEqual(cppFunction.BodySpan.Value.End.Offset, cppFunction.BodySpan.Value.Start.Offset);
            }

            {
                var cppFunction = compilation.Functions[2];
                Assert.AreEqual("function2", cppFunction.Name);
                Assert.IsNull(cppFunction.BodySpan);
            }
        }

        [Test]
        public void TestInlineMethodBody()
        {
            var options = new CppParserOptions();
            options.ParseFunctionBodies = true;
            var headerFilename = "test_inline_method_body.h";
            
            var text = @"
typedef unsigned int ImWchar;

class ImFont {
public:
    bool IsGlyphInFont(ImWchar c)
    {
        return false;
    }
    
    bool AnotherMethod(ImWchar c) {
        if (c == 0) return true;
        return false;
    }
};
";

            var currentDirectory = Environment.CurrentDirectory;
            var headerFile = Path.Combine(currentDirectory, headerFilename);
            File.WriteAllText(headerFile, text);
            
            var compilation = CppParser.ParseFile(headerFile, options);

            Assert.False(compilation.HasErrors);
            Assert.AreEqual(1, compilation.Classes.Count);
            
            var cls = compilation.Classes[0];
            Assert.AreEqual("ImFont", cls.Name);
            Assert.AreEqual(2, cls.Functions.Count);

            {
                var cppFunction = cls.Functions[0];
                Assert.AreEqual("IsGlyphInFont", cppFunction.Name);
                Assert.IsNotNull(cppFunction.BodySpan, "IsGlyphInFont should have BodySpan - this is the bug reported");
                Assert.Greater(cppFunction.BodySpan.Value.Start.Line, 0);
                Assert.Greater(cppFunction.BodySpan.Value.End.Line, 0);
                Assert.GreaterOrEqual(cppFunction.BodySpan.Value.End.Offset, cppFunction.BodySpan.Value.Start.Offset);
            }

            {
                var cppFunction = cls.Functions[1];
                Assert.AreEqual("AnotherMethod", cppFunction.Name);
                Assert.IsNotNull(cppFunction.BodySpan, "AnotherMethod should have BodySpan");
                Assert.Greater(cppFunction.BodySpan.Value.Start.Line, 0);
                Assert.Greater(cppFunction.BodySpan.Value.End.Line, 0);
                Assert.GreaterOrEqual(cppFunction.BodySpan.Value.End.Offset, cppFunction.BodySpan.Value.Start.Offset);
            }
        }

        [Test]
        public void TestMethodDefinitionOutsideClass()
        {
            var options = new CppParserOptions();
            options.ParseFunctionBodies = true;
            var headerFilename = "test_method_outside_class.h";
            
            var text = @"
typedef unsigned int ImWchar;

class ImFont {
public:
    bool IsGlyphInFont(ImWchar c);
};

bool ImFont::IsGlyphInFont(ImWchar c)
{
    return false;
}
";

            var currentDirectory = Environment.CurrentDirectory;
            var headerFile = Path.Combine(currentDirectory, headerFilename);
            File.WriteAllText(headerFile, text);
            
            var compilation = CppParser.ParseFile(headerFile, options);

            Assert.False(compilation.HasErrors);
            Assert.AreEqual(1, compilation.Classes.Count);
            
            var cls = compilation.Classes[0];
            Assert.AreEqual("ImFont", cls.Name);
            

            Assert.AreEqual(1, cls.Functions.Count);

            {
                var cppFunction = cls.Functions[0];
                Assert.AreEqual("IsGlyphInFont", cppFunction.Name);
                Assert.IsNotNull(cppFunction.BodySpan, "IsGlyphInFont should have BodySpan - this is the bug reported (method defined outside class)");
                Assert.Greater(cppFunction.BodySpan.Value.Start.Line, 0);
                Assert.Greater(cppFunction.BodySpan.Value.End.Line, 0);
                Assert.GreaterOrEqual(cppFunction.BodySpan.Value.End.Offset, cppFunction.BodySpan.Value.Start.Offset);
            }
        }



    }
}