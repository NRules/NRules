using System;
using System.Collections.Generic;

namespace NRules.Fluent.Expressions;

internal class SymbolStack
{
    private readonly Stack<SymbolTable> _frames = new();

    public SymbolStack()
    {
        _frames.Push(new SymbolTable());
    }

    public SymbolTable Scope => _frames.Peek();

    public IDisposable Frame()
    {
        PushFrame();
        return new StackGuard(this);
    }

    private void PushFrame()
    {
        var symbolTable = new SymbolTable(Scope);
        _frames.Push(symbolTable);
    }

    private void PopFrame()
    {
        _frames.Pop();
    }

    private sealed class StackGuard(SymbolStack symbolStack) : IDisposable
    {
        public void Dispose()
        {
            symbolStack.PopFrame();
        }
    }
}