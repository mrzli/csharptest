using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReadExpertTagsGoogleSpreadsheet
{
    class Program
    {
        private const string CLIENT_ID = "16198450227-ltnn0r6fkaf7iirovd2oebh4u2b16f0b.apps.googleusercontent.com";
        private const string CLIENT_SECRET = "d4QMbaQkEvBYRTDCGyJ80WOy";
        private const string SPREADSHEET_SCOPE_1 = "https://spreadsheets.google.com/feeds";
        private const string SPREADSHEET_SCOPE_2 = "https://docs.google.com/feeds";
        private const string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";
        private const string APPLICATION_NAME = "GM Google Drive";

        static void Main(string[] args)
        {
            //ListFiles();

            SpreadsheetsService service = ConnectToSpreasheetService();
            //string spreadsheetLink = "https://docs.google.com/spreadsheets/d/1ZlJNhiwE4XLzSS6qwBjb35-kuvFuVNhYW7EYl9D8_-E/edit#gid=0";
            string spreadsheetLink = "https://spreadsheets.google.com/feeds/1ZlJNhiwE4XLzSS6qwBjb35-kuvFuVNhYW7EYl9D8_-E";
            var query = new SpreadsheetQuery(spreadsheetLink);
            var feed = service.Query(query);
            var sheet = (SpreadsheetEntry)feed.Entries[0];
            Console.WriteLine(sheet.Title.Text);
        }

        private static void ListFiles()
        {
            DriveService service = ConnectToDriveService();

            var response = service.Files.List().Execute();
            foreach (var item in response.Files)
            {
                Console.WriteLine(item.Name);
            }
        }

        private static DriveService ConnectToDriveService()
        {
            UserCredential credential = GetCredential(new[] { DriveService.Scope.Drive }).Result;
            BaseClientService.Initializer initializer = new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = APPLICATION_NAME
            };
            DriveService service = new DriveService(initializer);

            return service;
        }

        private static SpreadsheetsService ConnectToSpreasheetService()
        {
            IEnumerable<string> scopes = new[] { SPREADSHEET_SCOPE_1, SPREADSHEET_SCOPE_2 };
            UserCredential credential = GetCredential(scopes).Result;

            OAuth2Parameters parameters = new OAuth2Parameters();
            parameters.ClientId = CLIENT_ID;
            parameters.ClientSecret = CLIENT_SECRET;
            parameters.RedirectUri = REDIRECT_URI;
            parameters.Scope = scopes.ToString();
            parameters.AccessToken = credential.Token.AccessToken;
            parameters.RefreshToken = credential.Token.RefreshToken;

            //string authorizationUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);
            //OAuthUtil.GetAccessToken(parameters);

            GOAuth2RequestFactory requestFactory = new GOAuth2RequestFactory(null, APPLICATION_NAME, parameters);
            SpreadsheetsService service = new SpreadsheetsService(APPLICATION_NAME);
            service.RequestFactory = requestFactory;

            return service;
        }

        private static async Task<UserCredential> GetCredential(IEnumerable<string> scopes)
        {
            ClientSecrets clientSecrets = new ClientSecrets
            {
                ClientId = CLIENT_ID,
                ClientSecret = CLIENT_SECRET
            };
            UserCredential credentialTask = await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets, scopes, "goran.mrzljak@gmail.com", CancellationToken.None);
            return credentialTask;
        }
    }
}
