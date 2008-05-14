using System;
using System.Collections.Specialized;
using HttpServer.Language;

namespace HttpServer.Helpers
{
    /// <summary>
    /// Validator is used to validate all input items in a form.
    /// </summary>
    /// <remarks><para>Should refactor this class to break out all general pieces 
    /// (everything except the form handling) to be able 
    /// to provide a general usage validator.</para>
    /// <para>Language phrases that need translation:
    /// <br/>
    /// <list type="">
    /// <item>Required: {0} is required</item>
    /// <item>Integer: {0} must be an integer.</item>
    /// </list>
    /// 
    /// </para></remarks>
    public class FormValidator : Validator
    {
        /// <summary>
        /// '{0}' is not a number.
        /// </summary>
        public const string FieldNumber = "Number";

        private readonly HttpInputBase _form;

        public FormValidator(NameValueCollection errors) : base(errors)
        {
        }

        public FormValidator(NameValueCollection errors, LanguageCategory langMgr) : base(errors, langMgr)
        {
        }

        public FormValidator(LanguageCategory langMgr) : base(langMgr)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form">form that validation should be made on.</param>
        public FormValidator(HttpInputBase form)
            : this(form, new NameValueCollection())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errors">collection that all validation errors are added to.</param>
        /// <param name="form">form that validation should be made on.</param>
        public FormValidator(HttpInputBase form, NameValueCollection errors) : base(errors)
        {
            if (form == null || form == HttpInput.Empty)
                throw new ArgumentNullException("form");

            _form = form;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errors">collection that all validation errors are added to.</param>
        /// <param name="form">form that validation should be made on.</param>
        /// <param name="modelLanguage">Language category used to translate field names.</param>
        public FormValidator(HttpInputBase form, NameValueCollection errors, LanguageCategory modelLanguage)
            : base(errors, modelLanguage)
        {
            if (form == null || form == HttpInput.Empty)
                throw new ArgumentNullException("form");

            _form = form;
        }

        /// <summary>
        /// Check whether the specified form item is an integer.
        /// </summary>
        /// <param name="name">Form parameter to validate</param>
        /// <returns>value if parameter is an int; 0 if not.</returns>
        public int Integer(string name)
        {
            return Integer(name, _form[name].Value, false);
        }

        /// <summary>
        /// Check whether the specified form item is an integer.
        /// </summary>
        /// <param name="name">Form parameter to validate</param>
        /// <param name="required">Paramater is required (adds an error if it's not specified)</param>
        /// <returns>value if parameter is an int; 0 if not.</returns>
        public int Integer(string name, bool required)
        {
            return Integer(name, _form[name].Value, required);
        }

        /// <summary>
        /// Validate that a string only contains letters or digits.
        /// </summary>
        /// <param name="name">Name of form parameter to validate.</param>
        /// <param name="required">Value is required.</param>
        /// <returns>value if valid; otherwise string.Empty.</returns>
        public string LettersOrDigits(string name, bool required)
        {
            return LettersOrDigits(name, _form[name].Value, required);
        }

        /// <summary>
        /// Validate that a string only contains letters or digits.
        /// </summary>
        /// <param name="name">Form parameter name.</param>
        /// <returns>vaue if found; otherwise string.Empty</returns>
        public string LettersOrDigits(string name)
        {
            return LettersOrDigits(name, _form[name].Value, false);
        }


        /// <summary>
        /// Validate that a string only contains letters or digits or any of the <see cref="Validator.PasswordChars"/>.
        /// </summary>
        /// <param name="name">Name of form parameter to validate.</param>
        /// <param name="required">Value is required.</param>
        /// <returns>value if valid; otherwise string.Empty.</returns>
        public string Password(string name, bool required)
        {
            return Password(name, _form[name].Value, required);
        }

        /// <summary>
        /// Validate that a string only contains letters or digits or any of the <see cref="Validator.PasswordChars"/>.
        /// </summary>
        /// <param name="name">Form parameter name.</param>
        /// <returns>vaue if found; otherwise string.Empty</returns>
        public string Password(string name)
        {
            return Password(name, _form[name].Value, false);
        }

        /// <summary>
        /// Check's weather a parameter is null or not.
        /// </summary>
        /// <param name="name">Parameter in form</param>
        /// <returns>true if value is not null; otherwise false.</returns>
        public bool Required(string name)
        {
            return Required(name, _form[name].Value);
        }

        /// <summary>
        /// Validate a string value
        /// </summary>
        /// <param name="name">Name of form parameter to validate.</param>
        /// <param name="required">Value is required.</param>
        /// <returns>value if valid; otherwise string.Empty.</returns>
        public string String(string name, bool required)
        {
            return String(name, _form[name].Value, required);
        }

        /// <summary>
        /// Validate a string parameter in the form
        /// </summary>
        /// <param name="name">Form parameter name.</param>
        /// <returns>vaue if found; otherwise string.Empty</returns>
        public string String(string name)
        {
            return String(name, _form[name].Value, false);
        }

        /// <summary>
        /// validates email address using a regexp.
        /// </summary>
        /// <param name="name">field name</param>
        /// <param name="required">field is required (may not be null or empty).</param>
        /// <returns>value if validation is ok; otherwise string.Empty.</returns>
        public string Email(string name, bool required)
        {
            return Email(name, _form[name].Value, required);
        }

        /// <summary>
        /// Checks whether a field contains true (can also be in native language).
        /// </summary>
        /// <param name="name">field name</param>
        /// <param name="required">field is required (may not be null or empty).</param>
        /// <returns>true if value is true; false if value is false or if validation failed.</returns>
        /// <remarks>Check validation errors to see if error ocurred.</remarks>
        public bool Boolean(string name, bool required)
        {
            return Boolean(name, _form[name].Value, required);
        }
    }

    public enum ValidateReason
    {
        Required,
        InvalidFormat,
        Number,
        Decimal,
        Currency,
    } ;
}