// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataNode.cs" company="Slash Games">
//   Copyright (c) Slash Games. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Slash.Unity.DataBind.Core.Data
{
    using System;
    using Slash.Unity.DataBind.Core.Utils;

    /// <summary>
    ///     Interface of a data node in a data tree.
    /// </summary>
    public interface IDataNode
    {
        /// <summary>
        ///     Called when the value of this node changed.
        /// </summary>
        event Action<object> ValueChanged;

        /// <summary>
        ///     Type of data.
        /// </summary>
        Type DataType { get; }

        /// <summary>
        ///     Name of node.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Current value.
        /// </summary>
        object Value { get; }

        /// <summary>
        ///     Deinitializes the object and removes pending callbacks.
        /// </summary>
        void Destroy();

        /// <summary>
        ///     Returns the descendant node at the specified path.
        /// </summary>
        /// <param name="path">Path to return node for.</param>
        /// <returns>Descendant node at the specified path.</returns>
        IDataNode FindDescendant(string path);


        /// <summary>
        ///     Removes the descendant node at the specified path.
        /// </summary>
        /// <param name="path">Path to remove node for.</param>
        /// <returns>True if there was a node which was removed; otherwise, false.</returns>
        bool RemoveDescendant(string path);

        /// <summary>
        ///     Sets a new value for the data node.
        /// </summary>
        /// <param name="newValue">Value to set.</param>
        void SetValue(object newValue);

        /// <summary>
        ///     Checks if the node is still in use, i.e. there are listeners to its value.
        /// </summary>
        bool IsMonitored();
    }

    public static class DataNodeUtils
    {
        public static int GetValueChangedListenerCount(IDataNode node)
        {
            return ReflectionUtils.GetEventListenerCount<DataNode>(node as DataNode, nameof(DataNode.ValueChanged));
        }
    }
}