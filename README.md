# T3DSharp-Sample

## What is this?

This is a very simple implementation of a T3D module in C#. In the `game` folder, you will find a standard T3D project based on the BaseGame template with an added CoinCollection module in the data folder.

It also includes a pre-compiled version of T3D based on SHA 2b86245 (from [this PR](https://github.com/TorqueGameEngines/Torque3D/pull/326)).

The `t3dsharp` folder contains a C# solution and projects, I will explain this folder further down this README.

## Why?

This project was primarily created in order to have something that utilizes the C-Interface. This is necessary to test updates and bug-fixes for this interface. In particular this PR:https://github.com/TorqueGameEngines/Torque3D/pull/326

## The C# projects

In the `t3dsharp` folder you find three C# projects:

* CoinCollection - The CoinCollection game and CoinCollection module
* T3DSharpFramework - An auto-generated C# layer to interface with T3D through the EngineAPI CInterface
* T3DSharpGenerator - A generator that can generate the ConsoleObject class hierarchy and other console object definitions

### What is the console? 

It's kindda vague what the Console in Torque3D actually _is_. It's a type-system but it can also lookup instances of objects and it might do other stuff as well..

The crux of the matter is that the Console is one of the core components that makes TorqueScript work, and the C# implementation taps into this engine to communicate with the engine. Thus anything that is accessible in the console, should be accessible in C#.

### Do I need to figure out how to run the generator?

No, the generator has already been run in this sample. You can find the result under `t3dsharp/T3DSharpFramework/Generated`. Feel free to run it again if you want to mess with it though (you might have to do a little bit of manual editing of the output atm. but it should be manageable).

### So what do I need to know about the C# projects?

The `t3dsharp/CoinCollection` project is the main point of interest here. It contains 3 things:

1. Standard boot-up procedure for a C# project (found in `Program.cs`)
2. An implementation of a typical main.cs file in C# (found in `Main.cs`)
3. The CoinCollection Module (found in the `CoinCollection` folder)

It also contains the `Core` folder which is just some ad-hoc helper code to access the functionality defined in the `Core` module (which is implemented in TorqueScript).

## The CoinCollection module

So first things first, the CoinCollection module is a quick implementation of a CoinCollection game based on [this tutorial](http://wiki.torque3d.org/scripter:coincollection). It's a simple, multiplayer-capable coin collection game with a bit of UI, networking, visual effects and script logic. So it's a good simple all-rounder example.

**Why not the FPSGameplay template?**

I started doing the FPSGameplay template, but it was big, bulky riddled with bugs and I ended up getting real tired of it real quick. The CoinCollection example serves my current purpose just fine and should be easier to grasp for newbies.

### Why is there still a CoinCollection folder in the data directory?

Well, you still need a folder for art etc. This could be embedded in the C# dll, but in order not to complicate things, and stay as compatible as possible with the "Torque3D-practices" I opted to keep it.

#### Alright but why is there still TorqueScript in there?

I opted for keeping datablocks, UI, levels etc. as TorqueScript, because it keeps them compatible with the editors so you can save your changes to these. Without a major overhaul, this is the recommended setup.

### Why did you do _insert stupid thing_ in your C# code?

The C# scripting system is new, best-practices are yet to be determined, so I usually try different ways of doing things to figure out what I like, and sometimes I forget to come back around and align the coding style.

Also some things are not solved properly by the framework or the solutions might be barebones or outdated, having the perfect C# framework design is not the goal for T3DSharp at the time of writing. The primary goal is making it work.

### Why does the code look like this?

I tried to keep it as close to how it would look in TorqueScript, but if you have any questions contact me on Discord and I will answer them. I will insert FAQs here if any arises.

## T3DSharp

A quick introduction to T3DSharp, the macros in the C++ code `DefineEngineFunction` and the like also define a CInterface-based version of that function that can be called by consumers of Torque3D as a `.dll`. A CInterface version of a function is a compiler-independent simple version of a C++ function based on the C-language. It is limited to C's features as opposed to C++, which means no operator overloading, no class-methods, no virtual methods etc. This significantly simplifies the interface between the C++ code and a potential scripting language.

The CInterface version of these functions are typed, so, as opposed to TorqueScript, C# doesn't have to convert everything to string before it can call the function. It can just call it with the actual parameters that function expects. However, this typing is a one-way street. The console is built up around TorqueScript's stringly-typing where everything is a string, so when the engine wants to call the script it converts everything to string before it sends the command.

TL:DR; almost all calls from C# to C++ is statically typed (variadic functions are an exception atm), calls from C++ to C# will be dynamically typed.

In order to compensate for the dynamic typing that happens when the engine calls a C# function, the T3DSharp layer analyses your C# code, and checks that the number of parameters is correct and converts arguments from string to whatever your function expects.

### Annotations

T3DSharp utilizes three annotations to simplify integration with the engine.

1. `[ScriptEntryPoint]` - Used in `Main.cs` to specify the equivalent of the `main.cs` TorqueScript file.
2. `[ConsoleFunction]` - Use on a `public static` function to register the function (similar to a TorqueScript function definition). The class name will be used as the namespace. For example, the `public static void Foo()` in the class `Bar` can be called as both `Bar::Foo()` and `Foo()`
3. `[ConsoleClass]` -  Used to specify that the class represents a class that should be accessible from the console. If you have a class `Bar` you can't write `new Bar()` in TorqueScript, but you can write `new ScriptObject() { Class ="Bar"; };` 

### How-To:

#### Instantiate an object

```c#
// Works mostly like in TorqueScript, except type-safe.
// Also you have to manually "register" the object as you would in C++.
var Foo = new SimObject("Foo") {
    // You can set static properties here
    CanSaveDynamicFields = true,
};
// Atm, you have to set dynamic properties outside of the initializer:
Foo.SetFieldValue("dynamicProperty", "bar");
Foo.RegisterObject();
```

#### Find an object

```c#
new SimObject("Foo").RegisterObject();
// Find by name
Sim.FindObject<SimObject>("Foo");
// Find by ID
Sim.FindObject<SimObject>(1234);
// If the string can be converted to a number, it will be treated as an ID instead of a name
Sim.FindObject<SimObject>("1234");
```

#### Change type of an object

```c#
// Since the C# objects are representations of the underlying C++, 
// you might sometimes have an object instance of type A, but 
// you know it should be of type B but C# won't let you cast from A to B.
// Then you can do an unsafe-cast from A to B.
new SimSet("Foo").RegisterObject();
var Foo = Sim.FindObject<SimObject>("Foo");
// Cast from SimObject to SimSet
var FooAsSimSet = Foo.As<SimSet>();
```

#### Casting string to type X

```c#
// You can use the helper class GenericMarshal to convert from/to string
GenericMarshal.StringTo<int>("123");
GenericMarshal.ToString(123);
GenericMarshal.StringTo<Point3F>("1 2 3");
GenericMarshal.ToString(new Point3F {X = 1.0f, Y = 2.0f, Z = 3.0f});
```

#### Tag/Untag strings

```c#
// Tagged strings is used extensively in TorqueScript for networking purposes.
// In TorqueScript, tagged strings are specified by using single quotes (').
// In C# you can use string extensions instead:
var taggedStr = "myString".Tag();
var str = taggedStr.DeTag();
```

#### Colors in strings

```c#
// Colors in TorqueScript are supported by special escaped characters like \c0.
// In order to implement this similarly in C#, we use string extensions:
"\c0SomeText".ColorEscape();
```

#### Call engine functions

```c#
// All engine functions are bundled by their scope. Most exist in the "Global" scope.
// Except from the explicit scope, this looks quite like TorqueScript. The only 
// caveat is that they are case-sensitive in C#.
Global.Echo("Some log message here");
```

#### Call TorqueScript functions

```c#
// You can't call TorqueScript directly, but you can use the engine functions Call or Eval
string arg1 = "foo";
string arg2 = "bar";
Global.Call("someTorqueScriptFunction", foo, bar);
Global.Eval($"someTorequScriptFunction({foo}, {bar})");
```

### C# code-patterns

#### Dynamic Field Properties

In the `CoinCollection` C# module, you might see properties like this here and there:

```c#
      public CoinCollectionGameConnection Client {
         get => Sim.FindObject<CoinCollectionGameConnection>(GetFieldValue("client"));
         set => SetFieldValue("client", GenericMarshal.ToString(value));
      }
```

The purpose here is to formalize a dynamic property, making it type-safe and well-documented.
This is quite boiler-plate looking, at some point a better solution should be found for this. 

