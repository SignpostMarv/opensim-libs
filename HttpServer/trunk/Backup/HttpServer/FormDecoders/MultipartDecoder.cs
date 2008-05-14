using System;
using System.IO;
using System.Text;
using HttpServer.Exceptions;
using HttpServer.FormDecoders;

namespace HttpServer.FormDecoders
{
    public class MultipartFormDecoder : FormDecoder
    {
        private const string ContentDisposition = "CONTENT-DISPOSITION";
        private const string ContentType = "CONTENT-TYPE";
        private const string FieldName = "NAME";
        private const string FormFileName = "FILENAME";

        /*protected HttpFile CreateFile(string name, string fileName, string contentType)
        {
            return new HttpFile(name, fileName, contentType);
        }*/

        public HttpInput Decode(Stream stream, string contentTypeAndBoundry, Encoding encoding)
        {
            HttpInput form = new HttpInput("multi");

            StreamReader reader = new StreamReader(stream, encoding);
            int contentPos = contentTypeAndBoundry.IndexOf('=');
            if (contentPos == -1)
                throw new ArgumentException("Boundry not found", "contentTypeAndBoundry");
            string boundry = contentTypeAndBoundry.Substring(contentPos + 1);

            string fieldName = string.Empty;
            string disposition = string.Empty;
            string fileName = string.Empty;
            string contentType = string.Empty;
            bool gotEmptyLine = false;

            while (true)
            {
                string line = reader.ReadLine();
                string upperLine = line.ToUpper();


                if (line.Contains(boundry))
                {
                    if (string.Compare(line, boundry + "--") == 0)
                        break; // end boundry
                    disposition = string.Empty;
                    fileName = string.Empty;
                    fieldName = string.Empty;
                    contentType = string.Empty;
                    gotEmptyLine = false;
                    continue;
                }
                    // Content-Disposition: form-data; name="namn"
                    // Content-Disposition: form-data; name="afile"; filename="intype-0.3.1.664.exe"
                    // Content-Type: application/x-msdos-program
                else if (upperLine.Contains(ContentDisposition) || upperLine.Contains(ContentType))
                {
                    //todo: Remove possible bug if ; is in names (which is encapsulated with "")
                    string[] entries = line.Split(';');
                    if (entries.Length < 1)
                        throw new BadRequestException("Invalid line: " + line);

                    for (int i = 0; i < entries.Length; ++i)
                    {
                        // separate the fields
                        int pos = entries[i].IndexOf('=');
                        if (pos == -1)
                        {
                            pos = entries[i].IndexOf(':');
                            if (pos == -1)
                                throw new BadRequestException("Expected \"=\" after field name, line: " + line);
                        }

                        string name = entries[i].Substring(0, pos);
                        ++pos;

                        while (entries[i][pos] == ' ')
                            ++pos;

                        string value;
                        if (entries[i][pos] == '"')
                        {
                            ++pos;
                            value = entries[i].Substring(pos, entries[i].Length - pos - 1);
                        }
                        else
                            value = entries[i].Substring(pos);

                        switch (name.Trim().ToUpper())
                        {
                            case FieldName:
                                fieldName = value;
                                break;
                            case ContentDisposition:
                                disposition = value.ToLower();
                                break;
                            case FormFileName:
                                fileName = value;
                                break;
                            case ContentType:
                                contentType = value;
                                break;
                        }
                    }
                }
                else if (line == string.Empty && !gotEmptyLine)
                {
                    gotEmptyLine = true;
                }
                else if (disposition == "form-data" && contentType == string.Empty)
                {
                    if (fieldName == string.Empty)
                        throw new BadRequestException("Got form data, but no field name");

                    form.Add(fieldName, line);
                }
                else
                {
                    //todo: implement reading of files.
                    //HttpFile file = CreateFile(fieldName, fileName, contentType);

                    long pos = stream.Position;
                    byte[] buffer = new byte[8196];

                    int bytes = stream.Read(buffer, 0, 8196);
                    while (bytes > 0)
                    {
                        bytes = stream.Read(buffer, 0, 8196);
                    }

                    //got data;
                    Console.Write("SHould do data");
                }
            }
            return form;

            //Content-Type: multipart/form-data; boundary=---------------------------230051238959
            /*
             * -----------------------------230051238959
Content-Disposition: form-data; name="namn"

asdffsdfds
-----------------------------230051238959
Content-Disposition: form-data; name="afile"; filename="intype-0.3.1.664.exe"
Content-Type: application/x-msdos-program

MZP??????��??�???????@????????
             * */
        }

        /// <summary>
        /// Checks if the decoder can handle the mime type
        /// </summary>
        /// <param name="contentType">Content type (with any additional info like boundry). Content type is always supplied in lower case.</param>
        /// <returns>True if the decoder can parse the specified content type</returns>
        public bool CanParse(string contentType)
        {
            return contentType.Contains("multipart/form-data");
        }
        
    }
}