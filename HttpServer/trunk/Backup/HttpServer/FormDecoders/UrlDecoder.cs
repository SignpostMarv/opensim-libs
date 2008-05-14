using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using HttpServer.FormDecoders;

namespace HttpServer.FormDecoders
{
    /// <summary>
    /// Can handle application/x-www-form-urlencoded
    /// </summary>
    public class UrlDecoder : FormDecoder
    {
        #region FormDecoder Members

        /// <summary>
        /// </summary>
        /// <param name="stream">Stream containing the content</param>
        /// <param name="contentType">Content type (with any additional info like boundry). Content type is always supplied in lower case</param>
        /// <param name="encoding">Stream enconding</param>
        /// <returns>
        /// A http form, or null if content could not be parsed.
        /// </returns>
        /// <exception cref="InvalidDataException">If contents in the stream is not valid input data.</exception>
        public HttpInput Decode(Stream stream, string contentType, Encoding encoding)
        {
            if (stream == null || stream.Length == 0)
                return null;
            if (!CanParse(contentType))
                return null;
            if (encoding == null)
                encoding = Encoding.UTF8;

            HttpInput form = new HttpInput("noname");

            StreamReader reader = new StreamReader(stream, encoding);
            string s = reader.ReadToEnd();
            string[] pairs = s.Split('&');
            foreach (string pair in pairs)
            {
                string[] item = pair.Split('=');
                if (item.Length != 2)
                    throw new InvalidDataException("Invalid url string, expected an equal sign.");

                form.Add(HttpUtility.UrlDecode(item[0]), HttpUtility.UrlDecode(item[1]));
            }

            return form;
        }


        public bool CanParse(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
                return false;

            return contentType.StartsWith("application/x-www-form-urlencoded", true, CultureInfo.InvariantCulture);
        }

        #endregion
    }
}