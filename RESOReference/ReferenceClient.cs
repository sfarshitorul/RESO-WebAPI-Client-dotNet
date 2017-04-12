using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RESOClientLibrary;
using RESOClientLibrary.Transactions;
using ReferenceLibrary;
using System.Collections;
using System.IO;
using ODataValidator.RuleEngine;

using System.Net;
using ODataValidator.Rule;
//using ODataValidator.Rule;

namespace RESOReference
{
    public partial class ReferenceClient : Form
    {
        RESOLogging debuglog = null;
        public string executepath { get; set; }
        ReferencePropertiesFile clientproperties = new ReferencePropertiesFile();
        public string oauth_bearertoken { get; set; }
        public string metadataresponse { get; set; }
        public string serviceresponse { get; set; }
        public string openidcode { get; set; }
        public string responseheaders { get; set; }

        public Hashtable commandlinefunctions = new Hashtable();
        public ReferenceClient()
        {
            InitializeComponent();

            executepath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            debuglog = new RESOLogging("debug", System.IO.Path.Combine(executepath, @"Logs\\debug.resolog"), false);
            clientproperties.executablepath = executepath;
            CheckArguments();
        }



        void CheckArguments()
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int n = 0; n < args.Length; n++)
            {
                string line = args[n];
                string[] argdata = line.Split('=');
                if (argdata.Length == 2)
                {
                    if (argdata[0].ToUpper() == "SCRIPT")
                    {
                        loadscriptfile(argdata[1].Trim(','));
                    }
                    else if (argdata[0].ToUpper() == "SETTINGS")
                    {
                        loadclientpropertiesfile(argdata[1].Trim(','));
                    }
                }
            }
        }


        private void SelectLogDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog SelectLogDirectoryDialog = new FolderBrowserDialog();
            string currentpath = this.LogDirectory.Text;
            if (string.IsNullOrEmpty(currentpath))
            {
                SelectLogDirectoryDialog.SelectedPath = System.IO.Path.Combine(executepath, @"Logs");
            }
            else
            {
                SelectLogDirectoryDialog.SelectedPath = currentpath;
            }


            if (SelectLogDirectoryDialog.ShowDialog() == DialogResult.OK)
            {
                this.LogDirectory.Text = SelectLogDirectoryDialog.SelectedPath.ToString();
            }
        }

        private void SelectResultsDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog SelectDirectoryDialog = new FolderBrowserDialog();
            string currentpath = this.ResultsDirectory.Text;
            if (string.IsNullOrEmpty(currentpath))
            {
                //ResultsDirectory.Text = System.IO.Path.Combine(executepath, @"Results");
                SelectDirectoryDialog.SelectedPath = System.IO.Path.Combine(executepath, @"Results");
            }
            else
            {
                SelectDirectoryDialog.SelectedPath = currentpath;
            }


            if (SelectDirectoryDialog.ShowDialog() == DialogResult.OK)
            {
                this.ResultsDirectory.Text = SelectDirectoryDialog.SelectedPath.ToString();
            }
        }


        private void SelectClientSettings_Click(object sender, EventArgs e)
        {
            loadclientpropertiesfile();
        }


        private void loadclientpropertiesfile()
        {
            OpenFileDialog testfile = new OpenFileDialog();
            testfile.Filter = "Client Settings Files (*.resocs)|*.resocs|Property Files (*.properties)|*.properties|All files (*.*)|*.*";
            testfile.FilterIndex = 1;
            testfile.RestoreDirectory = true;
            testfile.InitialDirectory = System.IO.Path.Combine(executepath, @"Properties");

            if (testfile.ShowDialog() == DialogResult.OK)
            {
                loadclientpropertiesfile(testfile.FileName);

            }
        }

        private void clearclientsettings()
        {
            try
            {
                DebugLogLabel("ReferenceClient:clearclientsettings()");
                webapicurrentrule.Text = string.Empty;

                this.webapiprogressBar.Value = 0;
                this.ServerVersion.Text = string.Empty;
                this.AuthorizationURI.Text = string.Empty;
                this.TokenURI.Text = string.Empty;
                this.TokenURI.Text = string.Empty;
                this.textOAuthClientIdentification.Text = string.Empty;
                this.textOAuthRedirectURI.Text = string.Empty;
                this.textOAuthClientSecret.Text = string.Empty;
                this.textOAuthClientScope.Text = string.Empty;
                this.textWebAPIURI.Text = string.Empty;
                this.UserName.Text = string.Empty;
                this.bearertokenedit.Text = string.Empty;
                this.Password.Text = string.Empty;
                this.LogDirectory.Text = string.Empty;
                this.ResultsDirectory.Text = string.Empty;
                this.openid_code.Text = string.Empty;
                this.webapi_token.Text = string.Empty;
                this.webapi_metadata.Text = string.Empty;
                this.serviceresponsedata.Text = string.Empty;
                this.preauthenticate.Checked = false;
                resetsessionvalues();
                this.Update();

            }
            catch (Exception ex)
            {
                DebugLogLabel("ReferenceClient:clearclientsettings():ERROR:" + ex.Message);
            }
        }

        private void resetsessionvalues()
        {
            try
            {
                DebugLogLabel("ReferenceClient:resetsessionvalues()");
                RESOClientSettings clientsettings = GetSettings();
                this.openid_code.Text = string.Empty;
                this.webapi_token.Text = string.Empty;
                this.webapi_metadata.Text = string.Empty;
                this.serviceresponsedata.Text = string.Empty;
                openid_code.Text = string.Empty;

                oauth_bearertoken = string.Empty;
                metadataresponse = string.Empty;
                serviceresponse = string.Empty;
                webapi_token.Text = oauth_bearertoken;
                webapi_metadata.Text = metadataresponse;
                serviceresponsedata.Text = serviceresponse;

                clientsettings.SetSetting(settings.openid_code, string.Empty);
            }
            catch (Exception ex)
            {
                DebugLogLabel("ReferenceClient:resetsessionvalues():ERROR:" + ex.Message);
            }
        }

        private RESOClientSettings GetSettings()
        {
            try
            {
                DebugLogLabel("ReferenceClient:GetSettings()");
                RESOClientSettings clientsettings = new RESOClientSettings();

                clientsettings.SetSetting(settings.rulecontroloutput, System.IO.Path.Combine(executepath, @"config"));
                clientsettings.SetSetting(settings.rulecontrolinput, System.IO.Path.Combine(executepath, @"config"));
                clientsettings.SetSetting(settings.rulescontrolfile, System.IO.Path.Combine(executepath, @"config") + "\\rulecontrol.xml");
                clientsettings.SetSetting(settings.certificatepath, System.IO.Path.Combine(executepath, @"Certificate") + "\\FiddlerRoot.cer");

                clientsettings.SetSetting(settings.log_directory, System.IO.Path.Combine(executepath, @"Logs"));


                if (this.ServerVersion.Text == "RETS 1.8")
                {
                    clientsettings.SetSetting(settings.version, "1.8");
                    clientsettings.SetSetting(settings.standard, "RETS");

                }
                else
                {
                    string dataversion = ServerVersion.SelectedItem.ToString();
                    if (!string.IsNullOrEmpty(dataversion))
                    {
                        if (dataversion.IndexOf("/") > 0)
                        {
                            string[] version = dataversion.Split('/');
                            clientsettings.SetSetting(settings.version, version[0]);
                            clientsettings.SetSetting(settings.standard, version[1]);
                        }
                    }
                }
                if (string.IsNullOrEmpty(scriptfile.Text))
                {
                    scriptfile.Text = System.IO.Path.Combine(executepath, @"webapitestscript") + "\\TestScript.xml";
                }

                clientsettings.SetSetting(settings.testscript, scriptfile.Text);
                clientsettings.SetSetting(settings.oauth_authorizationuri, AuthorizationURI.Text);
                clientsettings.SetSetting(settings.oauth_clientidentification, textOAuthClientIdentification.Text);
                clientsettings.SetSetting(settings.oauth_redirecturi, textOAuthRedirectURI.Text);
                clientsettings.SetSetting(settings.oauth_clientscope, textOAuthClientScope.Text);
                clientsettings.SetSetting(settings.oauth_clientsecret, textOAuthClientSecret.Text);
                clientsettings.SetSetting(settings.oauth_tokenuri, TokenURI.Text);
                clientsettings.SetSetting(settings.webapi_uri, textWebAPIURI.Text);
                clientsettings.SetSetting(settings.oauth_granttype, oauth_granttype.Text);
                clientsettings.SetSetting(settings.username, UserName.Text);
                clientsettings.SetSetting(settings.bearertoken, bearertokenedit.Text);
                
                clientsettings.SetSetting(settings.password, Password.Text);
                clientsettings.SetSetting(settings.useragent, "webapiclient/1.0");

                string resultsdirectory = ResultsDirectory.Text;
                if (string.IsNullOrWhiteSpace(resultsdirectory))
                {
                    resultsdirectory = System.IO.Path.Combine(executepath, @"Results");
                    ResultsDirectory.Text = resultsdirectory;

                }
                clientsettings.SetSetting(settings.results_directory, resultsdirectory);
                string logdirectory = LogDirectory.Text;
                if (string.IsNullOrWhiteSpace(logdirectory))
                {
                    logdirectory = System.IO.Path.Combine(executepath, @"Logs");
                    LogDirectory.Text = logdirectory;

                }
                clientsettings.SetSetting(settings.log_directory, logdirectory);

                //LogDirectory.Text = System.IO.Path.Combine(executepath, @"Logs");
                //ResultsDirectory.Text = System.IO.Path.Combine(executepath, @"Results");
                clientsettings.SetSetting(settings.outputdirectory, resultsdirectory);

                return clientsettings;
            }
            catch (Exception ex)
            {
                DebugLogLabel("ReferenceClient:GetSettings():ERROR:" + ex.Message);
            }
            return null;
        }

        private void SaveClientSettings_Click(object sender, EventArgs e)
        {
            saveclientpropertiesfile();
        }

        private void saveclientpropertiesfile()
        {
            try
            {

                DebugLogLabel("ReferenceClient:saveclientpropertiesfile()");
                //if (this.textOAuthClientScope.Text.ToUpper().IndexOf("OPENID") < 0)
                //{
                //    MessageOutput("Scope requires openid as a parameter for the RESO Web API OpenID Authentcation.  Please correct before saving.");
                //    return;
                //}
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Client Settings Files (*.resocs)|*.resocs|Property Files (*.properties)|*.properties|All files (*.*)|*.*";
                saveFileDialog1.Title = "Save Client Settings";
                saveFileDialog1.ShowDialog();

                // If the file name is not an empty string open it for saving.
                if (saveFileDialog1.FileName != "")
                {

                    clientproperties.setProperty("AuthenticationType", this.oauth_granttype.Text);
                    clientproperties.setProperty("Preauthenticate", ((this.preauthenticate.Checked == true) ? ("TRUE") : ("FALSE")));
                    clientproperties.setProperty("ServerVersion", this.ServerVersion.Text);
                    //OData
                    clientproperties.setProperty("textScriptFile", this.scriptfile.Text);
                    clientproperties.setProperty("AuthorizationURI", this.AuthorizationURI.Text);
                    clientproperties.setProperty("TokenURI", this.TokenURI.Text);
                    clientproperties.setProperty("TokenURI", this.TokenURI.Text);
                    clientproperties.setProperty("textWebAPIURI", this.textWebAPIURI.Text);
                    clientproperties.setProperty("textOAuthClientIdentification", this.textOAuthClientIdentification.Text);
                    clientproperties.setProperty("textOAuthRedirectURI", this.textOAuthRedirectURI.Text);
                    clientproperties.setProperty("textOAuthClientSecret", this.textOAuthClientSecret.Text);
                    clientproperties.setProperty("textOAuthClientScope", this.textOAuthClientScope.Text);
                    //clientproperties.setProperty("textWebAPIHost", this.textWebAPIURI.Text);
                    clientproperties.setProperty("UserName", this.UserName.Text);
                    clientproperties.setProperty("BearerToken", bearertokenedit.Text);
                    clientproperties.setProperty("Password", this.Password.Text);
                    clientproperties.setProperty("transactionlogdirectory", this.LogDirectory.Text);
                    clientproperties.setProperty("resultsdirectory", this.ResultsDirectory.Text);
                    clientproperties.saveFile(saveFileDialog1.FileName);
                }
            }
            catch (Exception ex)
            {
                DebugLogLabel("ReferenceClient:saveclientpropertiesfile():ERROR:" + ex.Message);
            }
        }

        private void btnTestOpenIDLogin_Click_1(object sender, EventArgs e)
        {
            RESOClientSettings clientsettings = GetSettings();
            if (string.IsNullOrWhiteSpace(clientsettings.GetSetting(settings.webapi_uri)))
            {
                loadclientpropertiesfile();
                clientsettings = GetSettings();
            }
            if (clientsettings.GetSetting(settings.oauth_granttype) != "Bearer Token")
            {
                if (string.IsNullOrEmpty(clientsettings.GetSetting(settings.oauth_authorizationuri)))
                {
                    MessageOutput("Please Enter login information or load select a saved client properties file");
                    return;
                }
                try
                {
                    Uri test = new Uri(clientsettings.GetSetting(settings.oauth_authorizationuri));
                }
                catch
                {
                    MessageOutput("Please Enter correct authorization URI");

                    return;
                }
            }
            Login();
        }

        private void MessageOutput(string message)
        {
            MessageBox.Show(message);
        }

        private void OutputLog(string logentry)
        {
            //OutputWindow.Text += logentry;
            //OutputWindow.Update();
        }

        private void ViewNavigateURL(Uri uri)
        {
            if (uri.AbsoluteUri.IndexOf("code=") > 0)
            {
                string[] urlparts = uri.AbsoluteUri.Split('&');
                foreach (string part in urlparts)
                {
                    if (part.IndexOf("code=") >= 0)
                    {
                        string[] codeurlparts = part.Split('=');
                        if (codeurlparts.Length == 2)
                        {
                            openidcode = codeurlparts[1];
                        }

                    }
                }

            }
        }

        private bool BrowserLogin(ref RESOClientSettings clientsettings)
        {
            if (clientsettings.GetSetting(settings.oauth_granttype) == "authorization_code")
            {
                if (string.IsNullOrEmpty(clientsettings.GetSetting(settings.openid_code)))
                {
                    LoginBrowser browserform = new LoginBrowser();
                    browserform.SetURL(clientsettings.GetSetting(settings.oauth_authorizationuri) + "?response_type=code&client_id=" + clientsettings.GetSetting(settings.oauth_clientidentification) + "&redirect_uri=" + clientsettings.GetSetting(settings.oauth_redirecturi) + "&scope=" + clientsettings.GetSetting(settings.oauth_clientscope), ViewNavigateURL);
                    //browserform.SetURL(clientsettings.GetSetting(settings.oauth_authorizationuri) + "?response_type=code&client_id=" + clientsettings.GetSetting(settings.oauth_clientidentification) + "&redirect_uri=" + clientsettings.GetSetting(settings.oauth_redirecturi) /*+ "&scope=" + clientsettings.GetSetting(settings.oauth_clientscope)*/, ViewNavigateURL);
                    browserform.ShowDialog();
                    if (string.IsNullOrEmpty(openidcode))
                    {
                        MessageOutput("Open ID Code not retrieved");
                        return false;
                    }
                    clientsettings.SetSetting(settings.openid_code, openidcode);
                }
            }
            return true;
        }
        private bool Login()
        {
            try
            {
                RESOClientSettings clientsettings = GetSettings();
                RESOClient app = new RESOClient(clientsettings, OutputLog);
                return Login(ref app, ref clientsettings);
            }
            catch
            {
                return false;
            }
        }
        private bool Login(ref RESOClient app, ref RESOClientSettings clientsettings)
        {
            if (clientsettings == null)
            {
                clientsettings = GetSettings();
            }
            if (app == null)
            {
                app = new RESOClient(clientsettings, OutputLog);
            }
            
            if (clientsettings.GetSetting(settings.oauth_granttype) == "authorization_code")
            {
                if (!BrowserLogin(ref clientsettings))
                {
                    return false;
                }
                openid_code.Text = "Loading...";
                webapi_token.Text = "Loading...";
                
            }
            if (clientsettings.GetSetting(settings.oauth_granttype) == "Bearer Token")
            {
                if(app.oauth_token == null)
                {
                    app.oauth_token = new OAuthToken();
                }
                app.oauth_token.token_type = "Bearer";
                app.oauth_token.access_token = this.bearertokenedit.Text;

                openid_code.Text = string.Empty;

                webapi_token.Text = "Loading...";

            }
            webapi_metadata.Text = "Loading...";
            serviceresponsedata.Text = "Loading...";
            this.Update();
            bool preauth = preauthenticate.Checked;
            ODataLoginTransaction login = new ODataLoginTransaction(app.clientsettings);
            try
            {
                if (clientsettings.GetSetting(settings.oauth_granttype) == "authorization_code")
                {
                    if (!login.ExecuteEvent(app, preauth))
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("(401)") > 0)
                {
                    if (!preauth)
                    {
                        string messageerror = "Non-Compliant:  " + ex.Message + " Please select Preauthentication to continue test.  Please review the HTTP Standard for Basic Authentication Challenge Response";
                        webapi_metadata.Text = messageerror;

                    }
                    else
                    {
                        webapi_metadata.Text = ex.Message;
                    }

                }
                else
                {
                    webapi_metadata.Text = ex.Message;
                }

                openid_code.Text = "Error";
                webapi_token.Text = "Error";
                serviceresponsedata.Text = "Error";
                this.Update();
                return false;
            }

            //oauth_bearertoken = app.oauth_token.token_type + " " + app.oauth_token.access_token;
            oauth_bearertoken = "Bearer" + " " + app.oauth_token.access_token;
            openid_code.Text = clientsettings.GetSetting(settings.openid_code);
            webapi_token.Text = oauth_bearertoken;
            this.Update();
            ODataMetadataTransaction metadata = new ODataMetadataTransaction(app.clientsettings);
            if (!metadata.ExecuteEvent(app))
            {
                webapi_metadata.Text = "Error";
                this.Update();
                return false;
            }
            metadataresponse = metadata.responsedata;
            if (string.IsNullOrEmpty(metadataresponse))
            {
                webapi_metadata.Text = "No Data Returned";
            }
            else
            {

                webapi_metadata.Text = metadataresponse;
            }
            this.Update();
            ODataServiceTransaction service = new ODataServiceTransaction(app.clientsettings);
            if (!service.ExecuteEvent(app))
            {
                serviceresponsedata.Text = "Error";
                this.Update();
                return false;
            }
            serviceresponse = service.responsedata;
            if (string.IsNullOrEmpty(serviceresponse))
            {
                serviceresponsedata.Text = "No Data Returned";
            }
            else
            {

                serviceresponsedata.Text = serviceresponse;
            }

            this.Update();
      
            responseheaders = app.responseobject.ResponseHeaders;
            return true;
        }

        private void loadscriptfile(string scriptfiledata)
        {

            RESOClientSettings clientsettings = GetSettings();

            clientsettings.SetSetting(settings.testscript, scriptfiledata);

            scriptfile.Text = scriptfiledata;
            ReferencePropertiesFile testproperties = new ReferencePropertiesFile();
            if (string.IsNullOrEmpty(scriptfiledata))
            {
                MessageOutput("Please load a Script File");
                return;
            }
            testproperties.ReadFile(scriptfiledata);
            if (testproperties.IsLoaded())
            {
                loadclientpropertiesfile(scriptfiledata);
                clientproperties.setProperty("textScriptFile", scriptfiledata);
                scriptfile.Text = scriptfiledata;
            }

            if (string.IsNullOrWhiteSpace(clientsettings.GetSetting(settings.oauth_authorizationuri)))
            {
                loadclientpropertiesfile();
                clientsettings = GetSettings();
            }
            RESOClient app = new RESOClient(clientsettings, OutputLog);
            if (!Login(ref app, ref clientsettings))
            {
                return;
            }
            Update();
       
            if (app.oauth_token == null)
            {
                MessageOutput("Error Authenticating.  Token is null");
                return;
            }
            oauth_bearertoken = app.oauth_token.token_type + " " + app.oauth_token.access_token;

            RESOClientLibrary.LoadTestScript testscript = new LoadTestScript();
            testscript.LoadData(clientsettings.GetSetting(settings.testscript));
            
            ODataTestScriptTransaction testscriptobject = new ODataTestScriptTransaction(app.clientsettings);
            string outputdirectory = clientsettings.GetSetting(settings.outputdirectory);
            if (testscript.parameters["Parameter_OutputDirectory"] != null)
            {
                outputdirectory = testscript.parameters["Parameter_OutputDirectory"] as string;
            }
            outputdirectory = outputdirectory.Trim('\\');
            if(string.IsNullOrEmpty(outputdirectory) || !Directory.Exists(outputdirectory))
            {
                outputdirectory = testscript.parameters["config_defaultresultsdirectory"] as string;
                if (!Directory.Exists(outputdirectory))
                {
                    Directory.CreateDirectory(outputdirectory);
                }
            }
            ResultsDirectory.Text = outputdirectory;
            clientsettings.SetSetting(settings.outputdirectory, outputdirectory);
            DebugLogData(outputdirectory);
            
            int currcount = 0;
            foreach (RESOClientLibrary.Request item in testscript.requests)
            {
                StringBuilder sbresults = new StringBuilder();
                RESOClientLibrary.Transactions.EventRequest eventitem = new RESOClientLibrary.Transactions.EventRequest();
                eventitem.url = item.url;
                eventitem.outputfile = item.outputfile;
                eventitem.validationid = item.validationid;
                eventitem.method = item.method;
                eventitem.payload = item.payload;

                if (!testscriptobject.ExecuteEvent(app, eventitem, outputdirectory, ref debuglog))
                {
                    return;
                }
                currcount++;
                
                sbresults.Append(Convert.ToString(currcount));
                sbresults.Append("\t");
                sbresults.Append(outputdirectory + item.outputfile);
                sbresults.Append("\t");
                sbresults.Append(item.method);
                sbresults.Append("\t");
                sbresults.Append(item.url);
                sbresults.Append("\t");
                sbresults.Append(item.payload);
                sbresults.Append("\t");
                sbresults.Append(item.validationid);
                sbresults.Append("\r\n");
                DebugLogData(sbresults.ToString());
                sbresults.Clear();
                webapitestcomplete(Convert.ToString(currcount) + "/" + Convert.ToString(testscript.requests.Count), testscript.requests.Count, currcount);
            }
            DebugLogLabel("CLIENT LOG");
            DebugLogData(app.ClientLog.ToString());
            OutputDebugLog();
            webapitestcomplete("Complete", testscript.requests.Count, currcount);
        }

        private void OutputDebugLog()
        {
            if(debuglog != null)
            {
                debuglog.OutputLogFile();
            }
        }

        private void DebugLogLabel(string label)
        {
            if (debuglog != null)
            {
                debuglog.LogLabel(label);
            }
        }
        private void DebugLogData(string data)
        {
            if (debuglog != null)
            {
                debuglog.LogData(data);
            }
        }
        void webapitestcomplete(string name, int numberofrules, int count)
        {

            webapicurrentrule.Text = name;
            webapicurrentrule.Update();
            this.webapiprogressBar.Value = (int)((100 * count) / numberofrules);


            this.Update();
        }
        private void executetestscript_Click_1(object sender, EventArgs e)
        {

            loadscriptfile(scriptfile.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog testfile = new OpenFileDialog();
            testfile.Filter = "Client Script Files (*.resoscript)|*.resoscript|XML Script Files (*.xml)|*.xml|All files (*.*)|*.*";
            testfile.FilterIndex = 1;
            testfile.RestoreDirectory = true;
            if (string.IsNullOrEmpty(scriptfile.Text))
            {
                testfile.InitialDirectory = System.IO.Path.Combine(executepath, @"webapitestscript");
            }
            else
            {
                testfile.InitialDirectory = scriptfile.Text;
            }

            if (testfile.ShowDialog() == DialogResult.OK)
            {
                scriptfile.Text = testfile.FileName;
                ReferencePropertiesFile testproperties = new ReferencePropertiesFile();
                testproperties.ReadFile(testfile.FileName);
                if(testproperties.IsLoaded())
                {
                    loadclientpropertiesfile(testfile.FileName);
                    clientproperties.setProperty("textScriptFile", testfile.FileName);
                    scriptfile.Text = testfile.FileName;
                }
            }
        }

        private void loadclientpropertiesfile(string filename)
        {
            try
            {
                clearclientsettings();
                clientproperties.ReadFile(filename);
                string sv = clientproperties.getProperty("ServerVersion");
                if (string.IsNullOrEmpty(sv))
                {
                    sv = "Web API/1.0";
                }
                int index = this.ServerVersion.Items.IndexOf(sv.Trim());

                if (index >= 0)
                {
                    this.ServerVersion.SelectedIndex = index;
                }
                else
                {
                    this.ServerVersion.SelectedIndex = 0;
                }
                this.ServerVersion.Text = this.ServerVersion.SelectedItem.ToString();

                string granttype = clientproperties.getProperty("AuthenticationType");
                if (string.IsNullOrEmpty(granttype))
                {
                    granttype = "authorization_code";
                }
                index = this.oauth_granttype.Items.IndexOf(granttype.Trim());

                if (index >= 0)
                {
                    this.oauth_granttype.SelectedIndex = index;
                }
                else
                {
                    this.oauth_granttype.SelectedIndex = 0;
                }
                this.oauth_granttype.Text = this.oauth_granttype.SelectedItem.ToString();

                
                

                //OData
                this.scriptfile.Text = clientproperties.getProperty("textScriptFile");
                if (string.IsNullOrEmpty(scriptfile.Text))
                {
                    scriptfile.Text = System.IO.Path.Combine(executepath, @"webapitestscript") + "\\TestScript.resoscript";
                }

                //clientproperties.setProperty("Preauthenticate", ((this.preauthenticate.Checked == true) ? ("TRUE") : ("FALSE")));
                this.preauthenticate.Checked = ((clientproperties.getProperty("Preauthenticate") == "TRUE") ? (true):(false));
                this.AuthorizationURI.Text = clientproperties.getProperty("AuthorizationURI");
                this.TokenURI.Text = clientproperties.getProperty("TokenURI");

                this.textWebAPIURI.Text = clientproperties.getProperty("textWebAPIURI");

                this.textOAuthClientIdentification.Text = clientproperties.getProperty("textOAuthClientIdentification");
                this.textOAuthRedirectURI.Text = clientproperties.getProperty("textOAuthRedirectURI");

                this.textOAuthClientSecret.Text = clientproperties.getProperty("textOAuthClientSecret");
                this.textOAuthClientScope.Text = clientproperties.getProperty("textOAuthClientScope");


                this.UserName.Text = clientproperties.getProperty("UserName");
                this.bearertokenedit.Text = clientproperties.getProperty("BearerToken");
                this.Password.Text = clientproperties.getProperty("Password");
                //Global
                this.LogDirectory.Text = clientproperties.getProperty("transactionlogdirectory");
                this.ResultsDirectory.Text = clientproperties.getProperty("resultsdirectory");
            }
            catch (Exception ex)
            {
                MessageOutput("Error: Could not read file from disk. Original error: " + ex.Message);
            }
        }

        private void resetsession_Click(object sender, EventArgs e)
        {
            resetsessionvalues();
        }

        private void clearsettings_Click(object sender, EventArgs e)
        {
            clearclientsettings();
        }

        private void ViewMetadata_Click(object sender, EventArgs e)
        {
            MetadataForm metadataview = new MetadataForm();
            metadataview.LoadData(webapi_metadata.Text);
            metadataview.ShowDialog();
        }

        private void viewscript_Click(object sender, EventArgs e)
        {

        }
  
        private void oauth_granttype_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            SetAuthTypeControls(comboBox.SelectedItem as string);

         



        }

        private void SetAuthTypeControls(string authtype)
        {
            bool bearertokenonly = false;
            if (authtype == "Bearer Token")
            {
                bearertokenonly = true;
            }

            authtypelabelun.Visible = !bearertokenonly;
            authtypebearer.Visible = bearertokenonly;
            UserName.Visible = !bearertokenonly;
            bearertokenedit.Visible = bearertokenonly;
            labelpassword.Visible = !bearertokenonly;
            authorizationurilabel.Visible = !bearertokenonly;
            accesstokenurilabel.Visible = !bearertokenonly;
            redirecturilabel.Visible = !bearertokenonly;
            clientidlabel.Visible = !bearertokenonly;
            clientsecretlabel.Visible = !bearertokenonly;
            scopelabel.Visible = !bearertokenonly;
            Password.Visible = !bearertokenonly;
            AuthorizationURI.Visible = !bearertokenonly;
            TokenURI.Visible = !bearertokenonly;
            textOAuthRedirectURI.Visible = !bearertokenonly;
            textOAuthClientIdentification.Visible = !bearertokenonly;
            textOAuthClientSecret.Visible = !bearertokenonly;
            textOAuthClientScope.Visible = !bearertokenonly;
        }

        private void ValidationTest_Click(object sender, EventArgs e)
        {
            RunWebAPITest();
        }

        private void OutputLogCapture()
        {
            debuglog.OutputLogFile();

        }

        private Uri GetUri(string uri)
        {
            LogDebugEvent("GetUri", 1, string.Empty);
            try
            {
                LogDebugEvent("GetUri", 2, uri);
                return new Uri(uri);
            }
            catch (Exception ex)
            {
                LogDebugEvent("GetUri", 3, ex.Message);
            }
            return null;
        }

        private void LogDebugEvent(string v1, int v2, string empty)
        {
            
        }

        private void RunWebAPITest()
        {
            try
            {
               CommonCore1000_Entry hack = new CommonCore1000_Entry();
               string test = hack.Description;

                //int debugcount = 1;
                OutputLogCapture();
                
                RESOClientSettings clientsettings = GetSettings();
                //var instance = ODataValidator.ODataValidator.RuleEngine.TermDocuments.GetInstance();
                if (string.IsNullOrWhiteSpace(clientsettings.GetSetting(settings.webapi_uri)))
                {
                    
                    loadclientpropertiesfile();
                    if (!Login())
                    {
                        MessageOutput("Login Failed");
                        return;
                    }
                }

                if (string.IsNullOrEmpty(webapi_token.Text.Trim().Replace("Loading...", string.Empty).Replace("Error", string.Empty)))
                {
                    if (!Login())
                    {
                        MessageOutput("Login Failed");
                        return;
                    }
                }
                clientsettings = GetSettings();


                
                var reqHeaders = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("ODataVersion", "4.0") };
                reqHeaders.Add(new KeyValuePair<string, string>("Authorization", oauth_bearertoken));


                System.Guid JobID = System.Guid.NewGuid();
                ResultsProvider resultProvider = new ResultsProvider(JobID);
                ILogger logger = resultProvider as ILogger;


                
                Uri url = GetUri(clientsettings.GetSetting(settings.webapi_uri));

                Uri host = GetUri(url.Scheme.ToString() + "://" + url.Host);
                //Uri uri = new Uri(host, url.AbsoluteUri);
                
                Uri service = GetUri(clientsettings.GetSetting(settings.webapi_uri));
                ServiceContext ctx = new ServiceContext(url, JobID, HttpStatusCode.OK, responseheaders, metadataresponse, string.Empty, service, serviceresponse, metadataresponse, false, reqHeaders, ODataMetadataType.MinOnly);


                

                int count = 0;
                TestControl testcontrol = new TestControl();
                testcontrol.BuildRuleControlList(clientsettings);
                RuleCatalogCollection.Instance.Clear();
                
                StringBuilder sb = new StringBuilder();
                sb.Append("Category\tName\tDescription\tHelpLink\tErrorMessage\tRequirementLevel\tAspect\tSpecificationSection\tV4SpecificationSection\tV4Specification\tPayloadType\t");
                sb.Append("LevelType\tResourceType\tDependencyType\tDependencyInfo\tIsMediaLinkEntry\tProjection\tPayloadFormat\tVersion\tOdataMetadataType\tRequireMetadata\tRequireServiceDocument");
                sb.Append("\r\n");

                //RuleStoreAsXmlFolder ruleStore = new RuleStoreAsXmlFolder("rulestore", logger);
                //foreach (var rule in ruleStore.GetRules())
                //{
                //    count++;
                //    AddRuleData(ref sb, rule);
                //    //if (!CheckRule(rule, count, clientsettings, testcontrol))
                //    //{
                //    //    continue;
                //    //}
                //    RuleCatalogCollection.Instance.Add(rule);
                //}

                ExtensionRuleStore extensionStore = new ExtensionRuleStore("extensions", logger);
                foreach (var rule in extensionStore.GetRules())
                {
                    count++;
                    AddRuleData(ref sb, rule);
                    if (!CheckRule(rule, count, clientsettings, testcontrol))
                    {
                        continue;
                    }
                    RuleCatalogCollection.Instance.Add(rule);
                }
                
                System.IO.File.WriteAllText(clientsettings.GetSetting(settings.log_directory) + "\\rulelist.txt", sb.ToString());


                //var header = ctx.RequestHeaders;
                var ruleArray = RuleCatalogCollection.Instance.ToArray();
                
                //RuleEngineWrapper ruleEngine = new RuleEngineWrapper(ctx, resultProvider, logger);
                //return;
                RuleExecuter rules = new RuleExecuter(resultProvider, logger);
                rules.Execute(ctx, ruleArray, (int)ruleArray.Length, webapitestcomplete);
                ResultsProvider logitems = logger as ResultsProvider;
                Hashtable logitemshash = new Hashtable();
                
                int detailcount = 0;
                foreach (ExtensionRuleResultDetail item in logitems.Details)
                {
                    detailcount++;
                    if (logitemshash[item.RuleName] == null)
                    {
                        Hashtable hsh = new Hashtable();
                        hsh[detailcount] = item;
                        logitemshash[item.RuleName] = hsh;
                    }
                    else
                    {
                        Hashtable hsh = logitemshash[item.RuleName] as Hashtable;
                        hsh[detailcount] = item;
                        logitemshash[item.RuleName] = hsh;
                    }
                }
                StringBuilder sbresults = new StringBuilder();
                StringBuilder sbLogAll = new StringBuilder();
                sbresults.Append("URL");
                sbresults.Append("\t");
                sbresults.Append("ODataLevel");
                sbresults.Append("\t");
                sbresults.Append("RuleName");
                sbresults.Append("\t");
                sbresults.Append("Classification");
                sbresults.Append("\t");
                sbresults.Append("Description");
                sbresults.Append("\t");
                sbresults.Append("SpecificationUri");
                sbresults.Append("\t");
                sbresults.Append("\r\n");
                foreach (ODataValidator.RuleEngine.TestResult result in resultProvider.ResultsToSave)
                {
                    BuildResultsOutput(ref sbresults, result, ref sbLogAll, logitemshash);
                }
                
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(clientsettings.GetSetting(settings.log_directory) + "\\" + "outputlog" + ".txt", false))
                {
                    file.Write(sbLogAll.ToString());
                }
                
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(clientsettings.GetSetting(settings.results_directory) + "\\" + "results" + ".txt", false))
                {
                    file.Write(sbresults.ToString());
                }
                OutputForm form = new OutputForm();
                form.SetOutputText(sbresults.ToString());
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            OutputLogCapture();
        }

        void webapitestcomplete(ODataValidator.RuleEngine.TestResult results, int numberofrules, ref int count)
        {

            webapicurrentrule.Text = results.RuleName;
            webapicurrentrule.Update();
            this.webapiprogressBar.Value = (int)((100 * count) / numberofrules);


            this.Update();
        }
        private bool CheckRule(ODataValidator.RuleEngine.Rule rule, int count, RESOClientSettings settings, ODataValidator.RuleEngine.TestControl testcontrol)
        {


            if (rule == null)
            {
                return false;
            }
            if (!rule.IsValid())
            {
                return false;
            }
            if (testcontrol == null)
            {
                return false;
            }
            //Advanced.Conformance.1004
            //Advanced.Conformance.1007


            //if (rule.Name == "Minimal.Conformance.100501")
            //{
            //    return true;
            //}


            //return false;
            //if (rule.Name == "Common.Core.4070")
            //{
            //    return true;
            //}


            //return false;
            if (rule.Name == "Intermediate.Conformance.1016")
            {
                return false;
            }
            if (rule.Name == "Advanced.Conformance.1007")
            {
                return false;
            }
            if (rule.Name == "Advanced.Conformance.1004")
            {
                return false;
            }
            if (rule.Name == "Intermediate.Conformance.1013")
            {
                return false;
            }
            if (rule.Name == "ServiceImpl_GetNavigationProperty")
            {
                return false;
            }
            if (rule.Name == "ServiceImpl_RequestingChanges_delta")
            {
                return false;
            }
            if (rule.Name == "ServiceImpl_SystemQueryOptionSkip")
            {
                return false;
            }
            if (rule.Name == "ServiceImpl_SystemQueryOptionSkipToken")
            {
                return false;
            }
            //Intermediate.Conformance.1013
            //ServiceImpl_GetNavigationProperty
            //ServiceImpl_RequestingChanges_delta
            //ServiceImpl_SystemQueryOptionSkip
            //ServiceImpl_SystemQueryOptionSkipToken
            //return false;
            RuleControl control = testcontrol.rulecontrol[rule.Name] as RuleControl;
            if (control != null)
            {
                if (string.Compare(control.cert_impact, "Core", true) == 0)
                {
                    return true;
                }
                if (string.Compare(control.cert_impact, "Platinum", true) == 0)
                {
                    return true;
                }

                if (string.Compare(control.cert_impact, "Gold", true) == 0)
                {
                    return true;
                }
                if (string.Compare(control.cert_impact, "Silver", true) == 0)
                {
                    return true;
                }
                if (string.Compare(control.cert_impact, "Bronze", true) == 0)
                {
                    return true;
                }
            }
            //ServiceImpl_SystemQueryOptionSkip

            if (rule.Name == "ServiceImpl_SystemQueryOptionCount")//Takes too long.  Need to make it an option.
            {
                return false;
            }
            if (rule.Name == "ServiceImpl_RequestingChanges_delta")//Takes too long.  Need to make it an option.
            {
                return false;
            }


            //return false;

            //if ((rule.RequirementLevel != RequirementLevel.Must) && (rule.RequirementLevel != RequirementLevel.MustNot))
            //{
            //    return false;
            //}
            //if (rule.Name == "Common.Core.3100") return true;
            //if (rule.Name == "Common.Core.3009") return true;
            //return false;
            //if (rule.Name.IndexOf("Advanced.Conformance") >= 0)
            //{
            //    return false;
            //}
            //if (rule.Name.IndexOf("Intermediate.Conformance") >= 0)
            //{
            //    return false;
            //}
            // if (rule.Name.IndexOf("Entry.Core") >= 0)
            {
                // return false;
            }

            if (string.IsNullOrEmpty(rule.V4Specification) || (rule.Version != ODataValidator.RuleEngine.ODataVersion.UNKNOWN && rule.Version != ODataValidator.RuleEngine.ODataVersion.V_All && rule.Version != ODataValidator.RuleEngine.ODataVersion.V3_V4 && rule.Version != ODataValidator.RuleEngine.ODataVersion.V4))
            {
                return false;
            }
            if (rule.PayloadFormat != null && rule.PayloadFormat == ODataValidator.RuleEngine.PayloadFormat.Atom)
            {
                return false;
            }
            if (rule.PayloadType == ODataValidator.RuleEngine.PayloadType.Entry)
            {
                return false;
            }
            //if (rule.ResourceType != null && rule.ResourceType == ConformanceServiceType.ReadWrite)
            //{
            //    return false;
            ////}
            //if (!AtomTests.Checked)
            //{
            //    if (rule.Description.IndexOf("Atom", StringComparison.CurrentCultureIgnoreCase) >= 0)
            //    {
            //        return false;
            //    }
            //    if (rule.PayloadFormat == ODataValidator.RuleEngine.PayloadFormat.Atom)
            //    {
            //        return false;
            //    }
            //}
            //if(rule.IsMediaLinkEntry != null && (bool)rule.IsMediaLinkEntry)
            //{
            //    return false;
            //}



            if (rule.Name == "ServiceImpl_RequestingChanges_delta")//Takes too long.  Need to make it an option.
            {
                return false;
            }

            //if (rule.Category == "SERVICEIMPL")
            //{
            //    //if (rule.Name != "ServiceImpl_RequestingChanges_delta")
            //    return false;
            //}
            //if ((rule.RequirementLevel == RequirementLevel.Must) || (rule.RequirementLevel == RequirementLevel.MustNot))
            //{
            //    return true;
            //}
            return true;
        }

        private void BuildResultsOutput(ref StringBuilder sbresults, ODataValidator.RuleEngine.TestResult result, ref StringBuilder sbLogAll, Hashtable logitemshash)
        {
            if (result.Classification != "notApplicable")
            {
                string[] descspit = result.Description.Split('^');

                if (descspit.Length == 2)
                {
                    sbresults.Append(descspit[1]);
                    sbresults.Append("\t");

                }
                else
                {
                    sbresults.Append(string.Empty);
                    sbresults.Append("\t");
                }
                sbresults.Append(result.ODataLevel);
                sbresults.Append("\t");
                sbresults.Append(result.RuleName);
                sbresults.Append("\t");
                sbresults.Append(result.Classification);
                sbresults.Append("\t");
                if (descspit.Length == 2)
                {
                    sbresults.Append(descspit[0]);
                    if (!string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        sbresults.Append(" ERROR:  ");
                        sbresults.Append(result.ErrorMessage);
                    }
                    sbresults.Append("\t");

                }
                else
                {
                    sbresults.Append(result.Description);
                    if (!string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        sbresults.Append(" ERROR:  ");
                        sbresults.Append(result.ErrorMessage);
                    }
                    sbresults.Append("\t");
                }

                sbresults.Append(result.SpecificationUri);
                sbresults.Append("\t");



                Hashtable hsh = null;
                if (logitemshash[result.RuleName] != null)
                {
                    hsh = logitemshash[result.RuleName] as Hashtable;
                }

                if (hsh != null)
                {
                    StringBuilder sbLog = new StringBuilder();
                    foreach (DictionaryEntry entry in hsh)
                    {
                        ExtensionRuleResultDetail item = entry.Value as ExtensionRuleResultDetail;
                        if (item != null)
                        {
                            try
                            {


                                if (item.RuleName != null) sbLog.Append("___________Start " + item.RuleName + "___________");
                                sbLog.Append("\r\n");
                                if (!string.IsNullOrWhiteSpace(item.ErrorMessage)) sbLog.Append("ERROR MESSAGE: " + item.ErrorMessage + "\r\n");
                                sbLog.Append("___________REQUEST___________");
                                sbLog.Append("\r\n");
                                if (item.HTTPMethod != null) sbLog.Append(item.HTTPMethod);
                                sbLog.Append("\r\n");
                                if (item.URI != null) sbLog.Append(item.URI.ToString());
                                sbLog.Append("\r\n");

                                if (item.RequestData != null) sbLog.Append(item.RequestData.ToString());
                                sbLog.Append("\r\n");

                                if (item.RequestHeaders != null) sbLog.Append(item.RequestHeaders.ToString());
                                sbLog.Append("\r\n");
                                sbLog.Append("___________RESPONSE___________");
                                sbLog.Append("\r\n");
                                if (item.ResponseStatusCode != null) sbLog.Append(item.ResponseStatusCode);
                                sbLog.Append("\r\n");
                                if (item.ResponseHeaders != null) sbLog.Append(item.ResponseHeaders.ToString());
                                sbLog.Append("\r\n");

                                if (item.ResponsePayload != null) sbLog.Append(item.ResponsePayload.ToString());
                                sbLog.Append("\r\n");




                                if (item.RuleName != null) sbLog.Append("___________End " + item.RuleName + "___________");
                                sbLog.Append("\r\n");

                            }

                            catch (Exception ex)
                            {
                                LogDebugEvent("RunWebAPITest", 98, ex.Message);
                            }
                        }
                    }
                    RESOClientSettings clientsettings = GetSettings();
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(clientsettings.GetSetting(settings.log_directory) + "\\" + result.RuleName + ".txt", false))
                    {
                        file.Write(sbLog.ToString());
                    }
                    sbLogAll.Append(sbLog);

                }
                sbresults.Append("\r\n");
            }
        }
        private void AddRuleData(ref StringBuilder sb, ODataValidator.RuleEngine.Rule rule)
        {
            sb.Append(rule.Category);
            sb.Append("\t");
            sb.Append(rule.Name);
            sb.Append("\t");
            sb.Append(rule.Description);
            sb.Append("\t");
            sb.Append(rule.HelpLink);
            sb.Append("\t");
            sb.Append(rule.ErrorMessage);
            sb.Append("\t");
            sb.Append(rule.RequirementLevel);
            sb.Append("\t");
            sb.Append(rule.Aspect);
            sb.Append("\t");
            sb.Append(rule.SpecificationSection);
            sb.Append("\t");
            sb.Append(rule.V4SpecificationSection);
            sb.Append("\t");
            sb.Append(rule.V4Specification);
            sb.Append("\t");
            sb.Append(rule.PayloadType);
            sb.Append("\t");
            sb.Append(rule.LevelType);
            sb.Append("\t");
            sb.Append(rule.ResourceType);
            sb.Append("\t");
            sb.Append(rule.DependencyType);
            sb.Append("\t");
            sb.Append(rule.DependencyInfo);
            sb.Append("\t");
            sb.Append(rule.IsMediaLinkEntry);
            sb.Append("\t");
            sb.Append(rule.Projection);
            sb.Append("\t");
            sb.Append(rule.PayloadFormat);
            sb.Append("\t");
            sb.Append(rule.Version);
            sb.Append("\t");
            sb.Append(rule.OdataMetadataType);
            sb.Append("\t");
            sb.Append(rule.RequireMetadata);
            sb.Append("\t");
            sb.Append(rule.RequireServiceDocument);

            sb.Append("\r\n");
        }
    }




        
    
}
    
