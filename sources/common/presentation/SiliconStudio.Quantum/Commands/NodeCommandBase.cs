﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiliconStudio.ActionStack;
using SiliconStudio.Core.Reflection;
using SiliconStudio.Quantum.Contents;

namespace SiliconStudio.Quantum.Commands
{
    /// <summary>
    /// Base class for node commands.
    /// </summary>
    public abstract class NodeCommandBase : INodeCommand
    {
        /// <inheritdoc/>
        public abstract string Name { get; }

        /// <inheritdoc/>
        public abstract CombineMode CombineMode { get; }

        /// <inheritdoc/>
        public abstract bool CanAttach(ITypeDescriptor typeDescriptor, MemberDescriptorBase memberDescriptor);

        public Task<IActionItem> Execute(IContent content, object index, object parameter)
        {
            return Execute(content, index, parameter, Enumerable.Empty<IDirtiable>());
        }

        public abstract Task<IActionItem> Execute(IContent content, object index, object parameter, IEnumerable<IDirtiable> dirtiables);

        /// <inheritdoc/>
        public virtual void StartCombinedInvoke()
        {
            // Intentionally do nothing
        }

        /// <inheritdoc/>
        public virtual void EndCombinedInvoke()
        {
            // Intentionally do nothing
        }
    }
}