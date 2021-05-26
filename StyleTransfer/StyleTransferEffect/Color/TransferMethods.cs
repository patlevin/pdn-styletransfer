// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaintDotNet.Effects.ML.StyleTransfer.Color
{
    /// <summary>
    /// Descriptor of a color transfer method.
    /// </summary>
    public class ColorTransferDescriptor
    {
        public ColorTransferDescriptor(string name, string description)
        {
            Name = name;
            Description = description;
            DisplayName = name;
        }

        public ColorTransferDescriptor(
            TransferMethodAttribute attr,
            IColorTransfer instance)
        {
            Name = attr.Name;
            DisplayName = Name;
            Description = attr.Description;
            Interface = instance;
        }

        /// <summary>
        /// Name of the method.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Localized display name of the method.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Description of the method.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Instance of the method.
        /// </summary>
        public IColorTransfer Interface { get; private set; }
    }

    /// <summary>
    /// Helpers for accessing all classes marked as transfer methods. 
    /// </summary>
    public static class TransferMethods
    {
        /// <summary>
        /// Descriptors of all available color transfer methods.
        /// </summary>
        public static IReadOnlyList<ColorTransferDescriptor> All => LoadTypes();

        private static IReadOnlyList<ColorTransferDescriptor> LoadTypes()
        {
            if (types.Count == 0)
            {

                var typeList = typeof(TransferMethods).Assembly.GetExportedTypes();
                var transforms = typeList.Select(TypeAttributePair)
                                         .Where(t => t.Item2 != null);

                foreach (var (type, attr) in transforms)
                {
                    var inst = GetInstance(type);
                    types.Add(new ColorTransferDescriptor(attr, inst));
                }

                (Type, TransferMethodAttribute) TypeAttributePair(Type type)
                {
                    return (type,
                        (TransferMethodAttribute)Attribute.GetCustomAttribute(
                            type, typeof(TransferMethodAttribute)));
                }

                IColorTransfer GetInstance(Type type)
                {
                    var ctor = type.GetConstructor(new Type[0]);
                    return ctor.Invoke(new object[0]) as IColorTransfer;
                }
            }

            return types.AsReadOnly();
        }

        private static readonly List<ColorTransferDescriptor> types
            = new List<ColorTransferDescriptor>();
    }
}
