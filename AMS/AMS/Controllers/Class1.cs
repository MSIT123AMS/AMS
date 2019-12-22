using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using AMS.Models;
using System.Data.Entity;

namespace AMS.Controllers
{
    class class1
    {

        // Replace <Subscription Key> with your valid subscription key.
        const string subscriptionKey = "96e819cdb80b4a61b90ed5c5a3b54924";

        // replace <myresourcename> with the string found in your endpoint URL
        const string uriBase =
            "https://hyamsface.cognitiveservices.azure.com/face/v1.0/detect";

        //static void Main(string[] args)
        //{

        //    // Get the path and filename to process from the user.
        //    Console.WriteLine("Detect faces:");
        //    Console.Write(
        //        "Enter the path to an image with faces that you wish to analyze: ");
        //    string imageFilePath = Console.ReadLine();

        //    if (File.Exists(imageFilePath))
        //    {
        //        try
        //        {
        //            MakeAnalysisRequest(imageFilePath);
        //            Console.WriteLine("\nWait a moment for the results to appear.\n");
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine("\n" + e.Message + "\nPress Enter to exit...\n");
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("\nInvalid file path.\nPress Enter to exit...\n");
        //    }
        //    Console.ReadLine();
        //}

        // Gets the analysis of the specified image by using the Face REST API.
        public static async Task<string> MakeAnalysisRequest(byte[] byteData, string employeeID)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameters. A third optional parameter is "details".
            string requestParameters = "returnFaceId=true";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            //byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json"
                // and "multipart/form-data".
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                // Display the JSON response.
                Console.WriteLine("\nResponse:\n");
                JavaScriptSerializer js = new JavaScriptSerializer();
               
                //j[0].
                contentString =contentString.Replace("[","");
                contentString = contentString.Replace("]","");
                var j = js.Deserialize<Class2>(contentString);
                //Debug.WriteLine(j.faceId);
               
                if (employeeID=="")
                {
                     return j.faceId;
                }
                 Entities db = new Entities();
                var f = db.Employees.Find(employeeID);
                f.FaceID = j.faceId;
                db.Entry(f).State = EntityState.Modified;
                db.SaveChanges();
                return "";
                //return j.faceId;
     
            }
        }
        
        // Returns the contents of the specified file as a byte array.
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
        // Formats the given JSON string by adding line breaks and indents.
        static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");

            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            int offset = 0;
            int indentLength = 3;

            foreach (char ch in json)
            {
                switch (ch)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\'':
                        if (quote) ignore = !ignore;
                        break;
                }

                if (quote)
                    sb.Append(ch);
                else
                {
                    switch (ch)
                    {
                        case '{':
                        case '[':
                            //sb.Append(ch);
                            //sb.Append(Environment.NewLine);
                            //sb.Append(new string(' ', ++offset * indentLength));
                            break;
                        case '}':
                        case ']':
                            //sb.Append(Environment.NewLine);
                            //sb.Append(new string(' ', --offset * indentLength));
                            //sb.Append(ch);
                            break;
                        case ',':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', offset * indentLength));
                            break;
                        case ':':
                            sb.Append(ch);
                            sb.Append(' ');
                            break;
                        default:
                            if (ch != ' ') sb.Append(ch);
                            break;
                    }
                }
            }

            return sb.ToString().Trim();
        }


        //static void Main()
        //{
        //    MakeRequest();
         
        //}

        public static async Task<bool> MakeRequest(string faceid1,string faceid2)
        {
            var client = new HttpClient();
           // var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            var uri = "https://hyamsface.cognitiveservices.azure.com/face/v1.0/verify?";

            HttpResponseMessage response;

            // Request body
            string a= @"{""faceId1"":"""+ faceid1+@""",""faceId2"":""" + faceid2+@"""}";
            byte[] byteData = Encoding.UTF8.GetBytes(a);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType =
                     new MediaTypeHeaderValue("application/json");

                response = await client.PostAsync(uri, content);
                string contentString = await response.Content.ReadAsStringAsync();
                JavaScriptSerializer js = new JavaScriptSerializer();

                //j[0].
  
                var j = js.Deserialize<Class3>(contentString);
                return j.isIdentical;
            }

        }



    }
}


