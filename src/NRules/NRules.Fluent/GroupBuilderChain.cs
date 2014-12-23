using System;
using System.Collections.Generic;
using NRules.RuleModel.Builders;

namespace NRules.Fluent
{
    internal sealed class GroupBuilderChain
    {
        private readonly Stack<GroupBuilder> _groupBuilders = new Stack<GroupBuilder>(); 

        public GroupBuilderChain(GroupBuilder rootGroupBuilder)
        {
            _groupBuilders.Push(rootGroupBuilder);
        }

        public GroupBuilder Current { get { return _groupBuilders.Peek(); } }

        public IDisposable BeginGroup(GroupType groupType)
        {
            var newGroupBuilder = Current.Group(groupType);
            _groupBuilders.Push(newGroupBuilder);
            return new GroupScope(this);
        }

        private void EndGroup()
        {
            _groupBuilders.Pop();
        }

        private class GroupScope : IDisposable
        {
            private readonly GroupBuilderChain _builderChain;

            public GroupScope(GroupBuilderChain builderChain)
            {
                _builderChain = builderChain;
            }

            public void Dispose()
            {
                _builderChain.EndGroup();
            }
        }
    }
}