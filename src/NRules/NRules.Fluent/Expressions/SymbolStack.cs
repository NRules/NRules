using System;
using System.Collections.Generic;

namespace NRules.Fluent.Expressions
{
    internal class SymbolStack
    {
        private readonly Stack<SymbolTable> _frames = new Stack<SymbolTable>();

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

        private class StackGuard : IDisposable
        {
            private readonly SymbolStack _symbolStack;

            public StackGuard(SymbolStack symbolStack)
            {
                _symbolStack = symbolStack;
            }

            public void Dispose()
            {
                _symbolStack.PopFrame();
            }
        }
    }
}