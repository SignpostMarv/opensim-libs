using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using HttpServer.Language;
using HttpServer.Language.Memory;

namespace HttpServer.Helpers
{
    /// <summary>
    /// General usage validator class.
    /// </summary>
    public class Validator
    {
        /// <summary>
        /// '{0}' is not a valid email address.
        /// </summary>
        /// <seealso cref="Email"/>
        /// <seealso cref="EmailRegEx"/>
        public const string FieldEmail = "Email";

        /// <summary>
        /// '{0}' can only contain letter or digits.
        /// </summary>
        /// <seealso cref="LettersOrDigits"/>
        /// <seealso cref="String"/>
        public const string FieldLetterOrDigit = "LetterOrDigit";

        /// <summary>
        /// '{0}' can only contain letter or digits or one of the following chars: {1}
        /// </summary>
        /// <seealso cref="PasswordChars"/>
        /// <seealso cref="Password"/>
        public const string FieldPassword = "Password";

        /// <summary>
        /// '{0}' is required.
        /// </summary>
        /// <seealso cref="Required"/>
        public const string FieldRequired = "Required";

        /// <summary>
        /// '{0}' must be {1} or {2}.
        /// </summary>
        /// <seealso cref="Boolean"/>
        public const string FieldBoolean = "Required";

        /// <summary>
        /// Used to check if a field is true.
        /// </summary>
        /// <seealso cref="Boolean"/>
        public const string FieldValueTrue = "True";

        /// <summary>
        /// Used to check if a field is false.
        /// </summary>
        /// <seealso cref="Boolean"/>
        public const string FieldValueFalse = "False";
        
        /// <summary>
        /// Regex used to validate emails.
        /// </summary>
        /// <seealso cref="Email"/>
        public static readonly string EmailRegEx = @"^[a-zA-Z0-9_\.\-]+@[a-zA-Z0-9_\.\-]+\.[a-zA-Z]{2,4}$";

        /// <summary>
        /// Language manager used if no one else have been specified.
        /// </summary>
        public static LanguageCategory Language;

        /// <summary>
        /// Extra characters that are allowed in passwords.
        /// </summary>
        /// <seealso cref="Password"/>
        public static string PasswordChars = "!#%&@$£";

        protected readonly NameValueCollection _errors;
        protected readonly LanguageCategory _modelLang = null;
        protected LanguageCategory _validatorLang;

        public Validator(NameValueCollection errors)
        {
            if (errors == null)
                throw new ArgumentNullException("errors");
            SetDefaultMgr();
            _errors = errors;
        }

        public Validator(NameValueCollection errors, LanguageCategory modelLanguage)
        {
            if (errors == null)
                throw new ArgumentNullException("errors");
            if (modelLanguage == null)
                throw new ArgumentNullException("modelLanguage");
            _errors = errors;
            _modelLang = modelLanguage;
        }

        public Validator(LanguageCategory modelLanguage)
        {
            if (modelLanguage == null)
                throw new ArgumentNullException("modelLanguage");
            _modelLang = modelLanguage;
            _errors = new NameValueCollection();
        }

        public Validator()
        {
            _errors = new NameValueCollection();
            SetDefaultMgr();
        }

        /// <summary>
        /// Validate that a string only contains letters or digits.
        /// </summary>
        /// <param name="name">Form parameter name.</param>
        /// <param name="value">value to validate</param>
        /// <param name="required">may not be null or empty if true.</param>
        /// <returns>value if valid; otherwise string.Empty</returns>
        /// <seealso cref="String"/>
        public string LettersOrDigits(string name, string value, bool required)
        {
            if (required && !Required(name, value))
                return string.Empty;
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            foreach (char ch in value)
            {
                if (!char.IsLetterOrDigit(ch))
                {
                    Errors.Add(name, Format(_validatorLang[FieldLetterOrDigit], name, PasswordChars));
                    return string.Empty;
                }
            }

            return value;
        }

        /// <summary>
        /// Validate that a string only contains letters or digits or any of the <see cref="PasswordChars"/>.
        /// </summary>
        /// <param name="name">Form parameter name.</param>
        /// <param name="value">value to validate</param>
        /// <param name="required">field may not be empty or null if true</param>
        /// <returns>vaue if valid; otherwise string.Empty</returns>
        /// <seealso cref="String"/>
        /// <seealso cref="LettersOrDigits"/>
        public string Password(string name, string value, bool required)
        {
            if (required && !Required(name, value))
                return string.Empty;
            if (name == null)
                return string.Empty;

            foreach (char ch in value)
            {
                if (!char.IsLetterOrDigit(ch) && !Contains(PasswordChars, ch))
                {
                    Errors.Add(name, Format(_validatorLang[FieldPassword], name, PasswordChars));
                    return string.Empty;
                }
            }
            return value ?? string.Empty;
        }

        /// <summary>
        /// Check's weather a parameter is null or not.
        /// </summary>
        /// <param name="name">Parameter in form</param>
        /// <returns>true if value is not null/empty; otherwise false.</returns>
        /// <param name="value">value that cannot be null or empty.</param>
        public bool Required(string name, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Errors.Add(name, Format(_validatorLang[FieldRequired], name));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate a string parameter in the form
        /// </summary>
        /// <param name="name">Form parameter name.</param>
        /// <param name="value">value to validate as string.</param>
        /// <param name="required">field may not be empty or null if true.</param>
        /// <returns>vaue if valid; otherwise string.Empty</returns>
        /// <seealso cref="LettersOrDigits"/>
        public string String(string name, string value, bool required)
        {
            if (required && !Required(name, value))
                return string.Empty;

            return value ?? string.Empty;
        }

        /// <summary>
        /// true of validation generated errors.
        /// </summary>
        public bool ContainsErrors
        {
            get { return _errors.Count > 0; }
        }

        /// <summary>
        /// Collection of validation errors.
        /// </summary>
        public NameValueCollection Errors
        {
            get { return _errors; }
        }

        /// <summary>
        /// add default language phrases (only english)
        /// </summary>
        public static void AddDefaultPhrases()
        {
            Language.Add(FieldRequired, 1033, "'{0}' is required.");
            Language.Add(FormValidator.FieldNumber, 1033, "'{0}' is not a number.");
            Language.Add(FieldLetterOrDigit, 1033, "'{0}' can only contain letter or digits.");
            Language.Add(FieldPassword, 1033,
                         "'{0}' can only contain letter or digits or one of the following chars: {1}");
            Language.Add(FieldEmail, 1033, "'{0}' is not a valid email address.");
            Language.Add(FieldValueTrue, 1033, "true");
            Language.Add(FieldValueFalse, 1033, "false");
            Language.Add(FieldBoolean, 1033, "'{0}' must be {1} or {2}.");
        }

        /// <summary>
        /// Checks wether a string contains a specific character.
        /// </summary>
        /// <param name="s">source</param>
        /// <param name="ch">character to find.</param>
        /// <returns>true if found; otherwise false.</returns>
        public static bool Contains(string s, char ch)
        {
            foreach (char c in s)
                if (ch == c)
                    return true;
            return false;
        }

        protected virtual string Format(string format, string fieldName)
        {
            if (_modelLang == null)
                return string.Format(format, fieldName);
            else
                return string.Format(format, _modelLang[fieldName]);
        }

        protected virtual string Format(string format, string fieldName, string extra)
        {
            if (_modelLang == null)
                return string.Format(format, fieldName, extra);
            else
                return string.Format(format, _modelLang[fieldName], extra);
        }

        /// <summary>
        /// Check whether the specified value is an integer.
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Parameter value to validate</param>
        /// <param name="required">Paramater is required (adds an error if it's not specified)</param>
        /// <returns>value if parameter is an int; 0 if not.</returns>
        public int Integer(string name, string value, bool required)
        {
            if (string.IsNullOrEmpty(value) && required)
            {
                Errors.Add(name, Format(_validatorLang[FieldRequired], name));
                return 0;
            }

            int ivalue;
            if (!int.TryParse(value, out ivalue))
            {
                if (required || !string.IsNullOrEmpty(value))
                    Errors.Add(name, Format(_validatorLang[FormValidator.FieldNumber], name));
                return 0;
            }

            return ivalue;
        }

        /// <summary>
        /// validates email address using a regexp.
        /// </summary>
        /// <param name="name">field name</param>
        /// <param name="value">value to validate</param>
        /// <param name="required">field is required (may not be null or empty).</param>
        /// <returns>value if validation is ok; otherwise string.Empty.</returns>
        public string Email(string name, string value, bool required)
        {
            if (required && !Required(name, value))
                return string.Empty;
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (Regex.IsMatch(value, EmailRegEx))
                return value;
            else
            {
                Errors.Add(name, Format(FieldEmail, name));
                return string.Empty;
            }
        }

        /// <summary>
        /// Checks whether a field contains true (can also be in native language).
        /// </summary>
        /// <param name="name">field name</param>
        /// <param name="value">value to validate</param>
        /// <param name="required">field is required (may not be null or empty).</param>
        /// <returns>true if value is true; false if value is false or if validation failed.</returns>
        /// <seealso cref="FieldValueTrue"/>
        /// <remarks>Check validation errors to see if error ocurred.</remarks>
        public bool Boolean(string name, string value, bool required)
        {
            if (required && !Required(name, value))
                return false;

            if (string.IsNullOrEmpty(value))
                return false;
            if (value == "1" 
                || string.Compare(value, "true", true) == 0 
                || string.Compare(value, _validatorLang[FieldValueTrue], true) == 0)
                return true;
            else if (value == "0"
                || string.Compare(value, "false", true) == 0
                || string.Compare(value, _validatorLang[FieldValueFalse], true) == 0)
                return false;
            else
            {
                Errors.Add(name, _validatorLang[FieldBoolean]);
                return false;
            }
        }

        protected void SetDefaultMgr()
        {
            _validatorLang = Language;

            //Add all phrases to the manager.
            lock (Language)
            {
                if (Language == null)
                {
                    Language = new MemLanguageCategory(1033, "Validator");
                    AddDefaultPhrases();
                }
            }
        }
    }
}