using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace PholioVisualisation.ServicesWeb.Validations
{
    /// <summary>
    /// IValidation interface
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Check if the content is valid
        /// </summary>
        bool IsValid();
    }

    /// <inheritdoc />
    /// <summary>
    /// Base class for validations
    /// </summary>
    public abstract class AbstractValidator : IValidator
    {
        /// <summary>
        /// List of exceptions found
        /// </summary>
        public IList<Exception> ValidationsExceptionsFound = new List<Exception>();

        /// <summary>
        /// Check validations for inner parameters
        /// </summary>
        public bool IsValid()
        {
            Validate();

            return !ValidationsExceptionsFound.Any();
        }

        /// <summary>
        /// Reset the exception list
        /// </summary>
        public void ClearExceptions()
        {
            ValidationsExceptionsFound.Clear();
        }

        /// <summary>
        /// Get all exception messages into a string content
        /// </summary>
        /// <returns>StringContent with the exception messages</returns>
        public StringContent GetExceptionStringContentMessages()
        {
            var messagesArray = ValidationsExceptionsFound.Select(x => x.Message).ToArray();
            return new StringContent(string.Join(", ", messagesArray));
        }

        /// <summary>
        /// Abstract method for validating logic
        /// </summary>
        protected abstract void Validate();
    }

    /// <summary>
    /// Helper for validations
    /// </summary>
    public class ValidatorHelper
    {
        /// <summary>
        /// Check if string is valid
        /// </summary>
        public static bool StringValid(string stringParam)
        {
            return !string.IsNullOrEmpty(stringParam) && !string.IsNullOrWhiteSpace(stringParam);
        }

        /// <summary>
        /// Check if int is valid
        /// </summary>
        public static bool IntValid(int? intParam)
        {
            return intParam != null;
        }
    }
}