using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using HttpTokenize.Tokens;
using HtmlAgilityPack;
using System.Linq;

namespace HttpTokenize.Tokenizers
{
    public class HtmlFormTokenizer : IRequestTokenizer, IResponseTokenizer
    {
        public TokenCollection ExtractTokens(Request request)
        {
            TokenCollection tokens = new TokenCollection();
            if (request.Headers.ContainsKey("Content-Type") &&
                request.Headers["Content-Type"].Count >= 1 &&
                request.Headers["Content-Type"][0] == "application/x-www-form-urlencoded")
            {
                string[] pairs = request.Content.Split("&");
                foreach (string pair in pairs)
                {
                    if (pair.Length >= 3)
                    {
                        string[] data = pair.Split('=', 2);
                        HtmlFormToken token = new HtmlFormToken(HttpUtility.UrlDecode(data[0]), HttpUtility.UrlDecode(data[1]), TypeGuesser.GuessTypes(HttpUtility.UrlDecode(data[1])));
                        tokens.Add(token);
                    }
                }
            }
            return tokens;
        }

        public TokenCollection ExtractTokens(Response response)
        {
            TokenCollection tokens = new TokenCollection();
            if (response.Headers.ContainsKey("Content-Type") &&
                response.Headers["Content-Type"].Count >= 1 &&
                response.Headers["Content-Type"].Contains("text/html"))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(response.Content);

                var nodes = doc.DocumentNode.SelectNodes("//input[@type=\"hidden\"]");

                if (nodes != null)
                {
                    foreach (HtmlNode node in nodes)
                    {
                        string value = node.GetAttributeValue("value", "");
                        if (value != "")
                        {
                            HtmlFormToken token = new HtmlFormToken(node.GetAttributeValue("name", "likely-csrf"), value, TypeGuesser.GuessTypes(value));
                            tokens.Add(token);
                        }
                    }
                }

            }
            return tokens;
        }
    }
}
