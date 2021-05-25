// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaintDotNet.Effects.ML.StyleTransfer.Color
{
    /// <summary>
    /// Helpers for accessing all classes marked as transfer methods. 
    /// </summary>
    public static class TransferMethods
    {
        /// <summary>
        /// Return the names of all transfer methods.
        /// </summary>
        public static IEnumerable<string> AllNames => LoadTypes();

        /// <summary>
        /// Return the description of a transfer method given its name.
        /// </summary>
        /// <param name="name">Name of the transfer method</param>
        /// <returns>Description that matches the method name.</returns>
        public static string GetDescription(string name)
        {
            if (types.TryGetValue(name, out Type type))
            {
                return ((TransferMethodAttribute)Attribute.GetCustomAttribute(
                    type, typeof(TransferMethodAttribute))).Description;
            }
            return null;
        }

        /// <summary>
        /// Return an instance of a transfer method given its name.
        /// </summary>
        /// <param name="name">Name ofthe method.</param>
        /// <returns>Instance of the transfer method; <c>null</c> if no matching method could be found.</returns>
        public static IColorTransfer GetInstance(string name)
        {
            if (types.TryGetValue(name, out Type type))
            {
                var ctor = type.GetConstructor(new Type[0]);
                return ctor.Invoke(new object[0]) as IColorTransfer;
            }
            return null;
        }

        private static IEnumerable<string> LoadTypes()
        {
            if (types.Count == 0)
            {
                (Type, Attribute) TypeAttributePair(Type type)
                {
                    return (type, Attribute.GetCustomAttribute(type, typeof(TransferMethodAttribute)));
                }

                var typeList = typeof(TransferMethods).Assembly.GetExportedTypes();
                var transforms = typeList.Select(TypeAttributePair)
                                         .Where(t => t.Item2 != null);

                foreach (var (type, attr) in transforms)
                {
                    types.Add(((TransferMethodAttribute)attr).Name, type);
                }
            }

            return types.Keys;
        }

        private static readonly Dictionary<string, Type> types
            = new Dictionary<string, Type>();
    }
}
