// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Precondition.cs" company="ID Business Solutions Ltd.">
//
//    Copyright (C) 2014
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation, either version 3 of the
//    License, or (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Idbs.Ewb.Printing
{
    /// <summary>
    /// Provides a set of tests and raised exceptions based on predefined conditions
    /// </summary>
    public static class Precondition
    {
        /// <summary>
        /// Asserts the given item is not null. Raising an <see cref="ArgumentNullException"/>
        /// when this is found to not be the case.
        /// </summary>
        /// <typeparam name="T">The type of item to test</typeparam>
        /// <param name="item">The item to test</param>
        /// <param name="argumentName">The name of the argument</param>
        /// <returns>True if OK</returns>
        /// <exception cref="ArgumentNullException">Raised when item is null</exception>"
        public static bool NotNull<T>(T item, string argumentName) where T : class
        {
            if (item == null)
                throw new ArgumentNullException(argumentName);

            return true;
        }

        /// <summary>
        /// Tests a string and raises an <see cref="ArgumentException"/> when it is found
        /// to be empty
        /// </summary>
        /// <param name="item">The item to test</param>
        /// <param name="argumentName">The name of the argument</param>
        /// <returns>True if OK</returns>
        /// <exception cref="ArgumentException">Raised when the string is empty</exception>
        public static bool NotEmpty(string item, string argumentName)
        {
            if (item.Length == 0)
                throw new ArgumentException("Input string cannot be empty", argumentName);

            return true;
        }

        /// <summary>
        /// Verifies a string is neither null or empty, raising an exception of the appropriate
        /// type if either case is false.
        /// </summary>
        /// <param name="item">The string to test</param>
        /// <param name="argumentName">The name of the argument</param>
        /// <returns>True if OK</returns>
        /// <exception cref="ArgumentNullException">Raised when the string is null</exception>
        /// <exception cref="ArgumentException">Raised when the string is empty</exception>
        public static bool NotNullOrEmpty(string item, string argumentName)
        {
            return NotNull(item, argumentName) && NotEmpty(item, argumentName);
        }

        /// <summary>
        /// Asserts that the given precondition is true, raising an exception if it is not.
        /// </summary>
        /// <param name="precondition">The precondition to test</param>
        /// <param name="message">The message to report in the exception</param>
        public static void Assert(Func<bool> precondition, string message)
        {
            if (!precondition())
                throw new Exception(message);
        }
    }
}
