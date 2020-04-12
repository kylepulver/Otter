#region LICENSE
// This project is licensed under The MIT License (MIT)
//
// Copyright 2013 David Koontz, Logan Barnett, Corey Nolan, Alex Burley
//
//	Permission is hereby granted, free of charge, to any person obtaining a copy
//		of this software and associated documentation files (the "Software"), to deal
//		in the Software without restriction, including without limitation the rights
//		to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//		copies of the Software, and to permit persons to whom the Software is
//		furnished to do so, subject to the following conditions:
//
//		The above copyright notice and this permission notice shall be included in
//		all copies or substantial portions of the Software.
//
//		THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//		IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//		FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//		AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//		LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//		OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//		THE SOFTWARE.
//
// Please direct questions, patches, and suggestions to the project page at
// https://github.com/dkoontz/GoodStuff
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace Otter.Utility.GoodStuff
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns an array of all concrete subclasses of the provided type.
        /// </summary>
        public static Type[] Subclasses(this Type type)
        {
            var typeList = new List<System.Type>();
            AppDomain.CurrentDomain.GetAssemblies().Each(a => typeList.AddRange(a.GetTypes()));
            return typeList.Where(t => t.IsSubclassOf(type) && !t.IsAbstract).ToArray();
        }

        /// <summary>
        /// Returns an array of the provided type and all concrete subclasses of that type.
        /// </summary>
        public static Type[] TypeAndSubclasses(this Type type)
        {
            var typeList = new List<System.Type>();
            AppDomain.CurrentDomain.GetAssemblies().Each(a => typeList.AddRange(a.GetTypes()));
            return typeList.Where(t => (t == type || t.IsSubclassOf(type)) && !t.IsAbstract).ToArray();
        }
    }
}
