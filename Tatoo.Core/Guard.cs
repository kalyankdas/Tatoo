using System;

namespace Tatoo.Core
{
    /// <summary>
    ///     Contains guard conditions (pre, post parameter check) tests
    /// </summary>
    public class Guard
    {

        /// <summary>
        ///     Throws an ArgumentNullException if the argument is null
        /// </summary>
        /// <param name="argument">The argument to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <exception cref="ArgumentNullException">If the argument is null.</exception>
        public static void IsNotNull(object argument, string argumentName)
        {

            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static T IsNotNull<T>(T argument, string argumentName)
        {

            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
            return argument;
        }


        public static void IsNull(object argument, string argumentName)
        {

            if (argument != null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        ///     Checks that the predicate is true, if it is not then
        /// an <see cref="ArgumentException"/> is thrown
        /// </summary>
        /// <param name="predicate">
        ///     The expression that is expected to be true
        /// </param>
        /// <param name="errorMessage">
        ///    The message that the exception will be set to report.
        /// </param>
        public static void PredicateHolds
                            (bool predicate
                            , string errorMessage)
        {

            if (!predicate)
            {
                throw new ArgumentException(errorMessage);
            }

        }

    }

    //https://github.com/dodyg/practical-aspnetcore

}