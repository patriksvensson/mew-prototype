# Mew

This is a prototype/research project for building a compiler in C#.

The code is messy, and I have no plans on updating it, but 
I'm open sourcing it so others can take advantage of the work
I've done. 

**It does not represent any "best practices" in any way.**  
A lot of things are suboptimal and/or are missing tests.

## What's included?

This repository contains:

* Lexer
* Parser
* Semantic analysis
  - Binding
  - Lowering (desugaring)
* Interpreter
* IL emitter
* LLVM emitter
  - (Only partially implemented)
* VSCode language server
  - Semantic highlighting
  - Hover handler
  - AST view
* Runtime/Standard library
  - Functions to print to stdout

## Attribution

This repository contains code and ideas from the following projects:

#### **Minsk**

A lot of the code related to binding, interpreting and IL emitting 
is taken from [Immo Landwerth](https://twitter.com/terrajobst)'s 
excellent [Minsk](https://github.com/terrajobst/minsk) compiler.

#### **Bicep**

[Bicep](https://github.com/Azure/bicep) has served as a big 
inspiration for the language server. 
Their parser  synchronization code also exist in some 
form in this repository.

## Building

We're using [Cake](https://github.com/cake-build/cake) as a 
[dotnet tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) 
for building. So make sure that you've restored Cake by running 
the following in the repository root:

```
> dotnet tool restore
```

After that, running the build is as easy as writing:

```
> dotnet cake
```