using HttpTokenize;
using HttpTokenize.Substitutions;
using HttpTokenize.Tokenizers;
using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProjectSpecific
{
    public class MoodleResetHelper
    {
        string exampleRequestPath = @"C:\Users\Richa\Documents\RiverFuzzResources\Moodle\Reset\";

        Request GetLoginForm;
        Request SubmitLoginForm;
        Request TestSession;
        Request GetResetForm;
        Request SubmitResetForm;

        RequestSequence ResetSequence;

        List<IResponseTokenizer> ResponseTokenizers;

        public MoodleResetHelper(string host, string adminUser, string adminPass)
        {
            // Load the Requests from a file.
            GetLoginForm = CaptureParse.BurpSavedParse.LoadSingleRequestFromFile(exampleRequestPath + "get_login_form.txt", host).Request;
            SubmitLoginForm = CaptureParse.BurpSavedParse.LoadSingleRequestFromFile(exampleRequestPath + "submit_login_form.txt", host).Request;
            TestSession = CaptureParse.BurpSavedParse.LoadSingleRequestFromFile(exampleRequestPath + "test_session.txt", host).Request;
            GetResetForm = CaptureParse.BurpSavedParse.LoadSingleRequestFromFile(exampleRequestPath + "get_reset_form.txt", host).Request;
            SubmitResetForm = CaptureParse.BurpSavedParse.LoadSingleRequestFromFile(exampleRequestPath + "submit_reset_form.txt", host).Request;

            // Set up the sequences
            ResetSequence = new RequestSequence();

            // The first stage just needs to get us a new pre-login sessiontoken. It can be to any endpoint.
            Stage GetSessionToken = new Stage(GetLoginForm);
            ResetSequence.Add(GetSessionToken);

            // The first stage needs the session token from the first request. We just need the CSRF Token from this form.
            Stage GetLoginStage = new Stage(GetLoginForm);
            CookieToken sessionToken = new CookieToken("MoodleSession", "", Types.String | Types.Integer);
            GetLoginStage.Substitutions.Add(new SubstituteNamedToken(sessionToken, "MoodleSession", 1, Types.String | Types.Integer));
            ResetSequence.Add(GetLoginStage);
            
            // The second stage needs the CSRF token from the first response, and the username + passwords.
            Stage SubmitLoginStage = new Stage(SubmitLoginForm);

            HtmlFormToken formCsrfToken = new HtmlFormToken("logintoken", "", Types.String | Types.Integer);
            SubmitLoginStage.Substitutions.Add(new SubstituteNamedToken(formCsrfToken, "logintoken", 2, Types.String | Types.Integer));

            HtmlFormToken usernameToken = new HtmlFormToken("username", "", Types.String | Types.Integer);
            SubmitLoginStage.Substitutions.Add(new SubstituteConstant(usernameToken, adminUser));

            HtmlFormToken passwordToken = new HtmlFormToken("password", "", Types.String | Types.Integer);
            SubmitLoginStage.Substitutions.Add(new SubstituteConstant(usernameToken, adminUser));
            ResetSequence.Add(SubmitLoginStage);

            // The third stage needs to confirm the login by going to the test session page.
            Stage TestSessionStage = new Stage(TestSession);

            TestSessionStage.Substitutions.Add(new SubstituteNamedToken(sessionToken, "MoodleSession", 3, Types.String | Types.Integer));
            ResetSequence.Add(TestSessionStage);

            // The third stage needs the MoodleSession cookie from the third response.
            Stage GetResetStage = new Stage(GetResetForm);

            // CookieToken sessionToken = new CookieToken("MoodleSession", "", Types.String | Types.Integer);
            GetResetStage.Substitutions.Add(new SubstituteNamedToken(sessionToken, "MoodleSession", 3, Types.String | Types.Integer));
            ResetSequence.Add(GetResetStage);

            // The fourth stage needs the MoodleSession cookie from the third response, and the CSRF token
            // from the fourth response.
            Stage SubmitResetStage = new Stage(SubmitResetForm);

            SubmitResetStage.Substitutions.Add(new SubstituteNamedToken(sessionToken, "MoodleSession", 3, Types.String | Types.Integer));

            HtmlFormToken resetCsrfToken = new HtmlFormToken("sesskey", "", Types.String | Types.Integer);
            SubmitResetStage.Substitutions.Add(new SubstituteNamedToken(resetCsrfToken, "sesskey", 5, Types.String | Types.Integer));
            ResetSequence.Add(SubmitResetStage);

            // Create the tokenizers we will need later.
            CookieTokenizer cookieTokenizer = new CookieTokenizer();
            HtmlFormTokenizer formTokenizer = new HtmlFormTokenizer();
            JsonTokenizer jsonTokenizer = new JsonTokenizer();

            ResponseTokenizers = new List<IResponseTokenizer>();
            ResponseTokenizers.Add(cookieTokenizer);
            ResponseTokenizers.Add(formTokenizer);
            ResponseTokenizers.Add(jsonTokenizer);
        }

        public async Task Reset(HttpClient client)
        {
            await ResetSequence.Execute(client, ResponseTokenizers, new TokenCollection());
        }
    }
}
