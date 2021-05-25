// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin
using System;

namespace PaintDotNet.Effects.ML.StyleTransfer.Color
{
    /// <summary>
    /// Attribute that marks a class as a colour transfer method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TransferMethodAttribute : Attribute
    {
        /// <summary>
        /// Name of the method. 
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Short description of the method.
        /// </summary>
        public string Description
        {
            get; set;
        }

        /// <summary>
        /// Initialise from name and description.
        /// </summary>
        /// <param name="name">Method name</param>
        /// <param name="description">Method description</param>
        public TransferMethodAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
