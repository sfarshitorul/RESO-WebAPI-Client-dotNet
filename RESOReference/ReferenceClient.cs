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
using ODataValidator.Rule.Helper;

//using ODataValidator.Rule;

namespace RESOReference
{
    public partial class ReferenceClient : Form
    {
        StringBuilder sbLogAll = new StringBuilder();
        RESOLogging debuglog = null;
        public string executepath { get; set; }
        ReferencePropertiesFile clientproperties = new ReferencePropertiesFile();
        public string oauth_bearertoken { get; set; }
        public string metadataresponse { get; set; }
        public string serviceresponse { get; set; }
        public string openidcode { get; set; }
        public string responseheaders { get; set; }
        public RESOClientSettings clientsettings_global;
        public Hashtable commandlinefunctions = new Hashtable();
        public ReferenceClient()
        {
            InitializeComponent();



            oauth_granttype.Items.Add(new AuthenticationTypeData { Name = "Authorization Code", Value = "authorization_code" });
            oauth_granttype.Items.Add(new AuthenticationTypeData { Name = "Bearer Token", Value = "bearer_token" });
            oauth_granttype.Items.Add(new AuthenticationTypeData { Name = "Client Credentials", Value = "client_credentials" });
            oauth_granttype.DisplayMember = "Name";
            oauth_granttype.ValueMember = "Value";


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
                clientproperties.setProperty("ResultsDirectory",SelectDirectoryDialog.SelectedPath.ToString());
            }
        }


        private void SelectClientSettings_Click(object sender, EventArgs e)
        {
            loadclientpropertiesfile();
        }


        private void loadclientpropertiesfile()
        {
            OpenFileDialog testfile = new OpenFileDialog();
            testfile.Filter = "RESOS Script (*.resoscript)|*.resoscript|All files (*.*)|*.*";
            testfile.FilterIndex = 1;
            testfile.RestoreDirectory = true;
            testfile.FileName = RESOReference.Properties.Settings.Default.Folder_Path;
            testfile.InitialDirectory = RESOReference.Properties.Settings.Default.Folder_Path;

            if (testfile.ShowDialog() == DialogResult.OK)
            {
                if (!String.IsNullOrEmpty(Properties.Settings.Default.Folder_Path))
                {
                    Properties.Settings.Default.Folder_Path = testfile.FileName;
                }

                Properties.Settings.Default.Folder_Path = testfile.FileName;
                Properties.Settings.Default.Save();

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
                this.edit_AuthorizationURI.Text = string.Empty;
                this.edit_AccessTokenURI.Text = string.Empty;
                this.edit_AccessTokenURI.Text = string.Empty;
                this.edit_ClientID.Text = string.Empty;
                this.edit_RedirectURI.Text = string.Empty;
                this.edit_ClientSecret.Text = string.Empty;
                this.edit_Scope.Text = string.Empty;
                this.edit_WebAPIEndPointURI.Text = string.Empty;
                this.edit_UserName.Text = string.Empty;
                this.edit_BearerToken.Text = string.Empty;
                this.edit_Password.Text = string.Empty;
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
                RESOClientSettings clientsettings = GetSettings(true);
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
            return GetSettings(false);
        }
        private RESOClientSettings GetSettings(bool reset)
        {
            try
            {
                DebugLogLabel("ReferenceClient:GetSettings()");
                if(clientsettings_global == null)
                {
                    clientsettings_global = new RESOClientSettings();
                }
                if(reset)
                {
                    clientsettings_global.are_set = false;
                }

                if(clientsettings_global.are_set)
                {
                    if (string.IsNullOrEmpty(clientsettings_global.GetSetting(settings.webapi_uri)))
                    {
                        clientsettings_global.are_set = false;
                    }
                    else
                    {
                        return clientsettings_global;
                    }
                }


                clientsettings_global.SetSetting(settings.rulecontroloutput, System.IO.Path.Combine(executepath, @"config"));
                clientsettings_global.SetSetting(settings.rulecontrolinput, System.IO.Path.Combine(executepath, @"config"));
                clientsettings_global.SetSetting(settings.rulescontrolfile, System.IO.Path.Combine(executepath, @"config") + "\\rulecontrol.xml");
                clientsettings_global.SetSetting(settings.certificatepath, System.IO.Path.Combine(executepath, @"Certificate") + "\\FiddlerRoot.cer");

                clientsettings_global.SetSetting(settings.log_directory, System.IO.Path.Combine(executepath, @"Logs"));


                if (this.ServerVersion.Text == "RETS 1.8")
                {
                    clientsettings_global.SetSetting(settings.version, "1.8");
                    clientsettings_global.SetSetting(settings.standard, "RETS");

                }
                else
                {
                    string dataversion = ServerVersion.SelectedItem.ToString();
                    if (!string.IsNullOrEmpty(dataversion))
                    {
                        if (dataversion.IndexOf("/") > 0)
                        {
                            string[] version = dataversion.Split('/');
                            clientsettings_global.SetSetting(settings.version, version[0]);
                            clientsettings_global.SetSetting(settings.standard, version[1]);
                        }
                    }
                }
                if (string.IsNullOrEmpty(scriptfile.Text))
                {
                    scriptfile.Text = System.IO.Path.Combine(executepath, @"webapitestscript") + "\\TestScript.xml";
                }

                clientsettings_global.SetSetting(settings.testscript, scriptfile.Text);
                clientsettings_global.SetSetting(settings.oauth_authorizationuri, edit_AuthorizationURI.Text);
                clientsettings_global.SetSetting(settings.oauth_clientidentification, edit_ClientID.Text);
                clientsettings_global.SetSetting(settings.oauth_redirecturi, edit_RedirectURI.Text);
                clientsettings_global.SetSetting(settings.oauth_clientscope, edit_Scope.Text);
                clientsettings_global.SetSetting(settings.oauth_clientsecret, edit_ClientSecret.Text);
                clientsettings_global.SetSetting(settings.oauth_tokenuri, edit_AccessTokenURI.Text);
                clientsettings_global.SetSetting(settings.webapi_uri, edit_WebAPIEndPointURI.Text);
                AuthenticationTypeData combodata = oauth_granttype.SelectedItem as AuthenticationTypeData;
                if (combodata != null)
                {
                    clientsettings_global.SetSetting(settings.oauth_granttype, combodata.Value);
                }
                else
                {
                    oauth_granttype.SelectedValue = "authorization_code";
                    clientsettings_global.SetSetting(settings.oauth_granttype, "authorization_code");
                }
                clientsettings_global.SetSetting(settings.username, edit_UserName.Text);
                clientsettings_global.SetSetting(settings.bearertoken, edit_BearerToken.Text);

                clientsettings_global.SetSetting(settings.password, edit_Password.Text);
                clientsettings_global.SetSetting(settings.useragent, "webapiclient/1.0");

                clientsettings_global.SetSetting(settings.results_directory,ResultsDirectory.Text);
                VerifyResultsDirectory(clientsettings_global);
                string resultsdirectory = clientsettings_global.GetSetting(settings.results_directory);
                ResultsDirectory.Text = resultsdirectory;

                clientsettings_global.SetSetting(settings.results_directory, resultsdirectory);

                clientsettings_global.SetSetting(settings.log_directory, LogDirectory.Text);
                VerifyLogDirectory(clientsettings_global);
                string logdirectory = clientsettings_global.GetSetting(settings.log_directory);
                LogDirectory.Text = logdirectory;
                clientsettings_global.are_set = true;
                return clientsettings_global;
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
                    AuthenticationTypeData combodata = this.oauth_granttype.SelectedItem as AuthenticationTypeData;
                    if (combodata != null)
                    {
                        clientproperties.setProperty("AuthenticationType", combodata.Name);
                    }
                    else
                    {
                        clientproperties.setProperty("AuthenticationType", "Authorization Code");
                    }
                    
                    
                    clientproperties.setProperty("Preauthenticate", (this.preauthenticate.Visible == false) ? "FALSE" : (this.preauthenticate.Checked == true) ? "TRUE" : "FALSE");
                    clientproperties.setProperty("ServerVersion", this.ServerVersion.Text);
                    //OData
                    clientproperties.setProperty("textScriptFile", this.scriptfile.Text);
                    clientproperties.setProperty("AuthorizationURI", this.edit_AuthorizationURI.Text);
                    clientproperties.setProperty("TokenURI", this.edit_AccessTokenURI.Text);
                    clientproperties.setProperty("TokenURI", this.edit_AccessTokenURI.Text);
                    clientproperties.setProperty("textWebAPIURI", this.edit_WebAPIEndPointURI.Text);
                    clientproperties.setProperty("ClientIdentification", this.edit_ClientID.Text);
                    clientproperties.setProperty("RedirectURI", this.edit_RedirectURI.Text);
                    clientproperties.setProperty("ClientSecret", this.edit_ClientSecret.Text);
                    clientproperties.setProperty("ClientScope", this.edit_Scope.Text);
                    //clientproperties.setProperty("textWebAPIHost", this.textWebAPIURI.Text);
                    clientproperties.setProperty("UserName", this.edit_UserName.Text);
                    clientproperties.setProperty("BearerToken", edit_BearerToken.Text);
                    clientproperties.setProperty("Password", this.edit_Password.Text);
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

        private void Login_Click(object sender, EventArgs e)
        {
            RESOClientSettings clientsettings = GetSettings();
            if (string.IsNullOrWhiteSpace(clientsettings.GetSetting(settings.webapi_uri)))
            {
                loadclientpropertiesfile();
                
            }
            clientsettings = GetSettings();
            string test33 = clientsettings.GetSetting(settings.oauth_granttype);
            if (clientsettings.GetSetting(settings.oauth_granttype) == "authorization_code")
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
                    browserform.SetLoginInfo(clientsettings.GetSetting(settings.username), clientsettings.GetSetting(settings.password));
                    
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
            
            if (clientsettings.GetSetting(settings.oauth_granttype) == "client_credentials")
            {

            }
            if (clientsettings.GetSetting(settings.oauth_granttype) == "bearer_token")
            {
                if(app.oauth_token == null)
                {
                    app.oauth_token = new OAuthToken();
                }
                app.oauth_token.token_type = "Bearer";
                app.oauth_token.access_token = this.edit_BearerToken.Text;

                openid_code.Text = string.Empty;

                webapi_token.Text = "Loading...";

            }
            webapi_metadata.Text = "Loading...";
            serviceresponsedata.Text = "Loading...";
            this.Update();
            bool preauth = (this.preauthenticate.Visible == false) ? false:(this.preauthenticate.Checked == true) ? true : false;
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
                else if(clientsettings.GetSetting(settings.oauth_granttype) == "client_credentials")
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

                webapi_metadata.Text = app.ClientLog.ToString() ;
                if(string.IsNullOrEmpty(webapi_metadata.Text))
                {
                    webapi_metadata.Text = "Error:  No log information on metadata.ExecuteEvent";
                }
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
               // ValidateMetadata test = new ValidateMetadata();
               // test.ReadResultsData(metadataresponse);
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
                //ValidateServiceData test = new ValidateServiceData();
                //test.ReadResultsData(serviceresponse);

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
            if (File.Exists(scriptfiledata))
            {
                testproperties.ReadFile(scriptfiledata);
            }
            else
            {
                MessageBox.Show(scriptfiledata + " Does not exist");
                return;
            }
            if (!testproperties.IsLoaded())
            {
                loadclientpropertiesfile(scriptfiledata);
                
                clientproperties.setProperty("textScriptFile", scriptfiledata);
                clientsettings = GetSettings(true);
                scriptfile.Text = scriptfiledata;
            }

            if (string.IsNullOrWhiteSpace(clientsettings.GetSetting(settings.oauth_authorizationuri)))
            {
                if (string.IsNullOrWhiteSpace(clientsettings.GetSetting(settings.bearertoken)))
                {
                    if (string.IsNullOrWhiteSpace(clientsettings.GetSetting(settings.testscript)))
                    {
                        loadclientpropertiesfile();
                        clientsettings = GetSettings();
                    }
                }
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
            clientsettings.SetSetting(settings.outputdirectory, ResultsDirectory.Text);
            
            VerifyResultsDirectory(clientsettings);
            ResultsDirectory.Text = clientsettings.GetSetting(settings.results_directory);

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
            if(numberofrules <= 0)
            {
                return;
            }
            webapicurrentrule.Text = name;
            webapicurrentrule.Update();
            this.webapiprogressBar.Value = (int)((100 * count) / numberofrules);


            this.Update();
        }
        private void executetestscript_Click_1(object sender, EventArgs e)
        {

            loadscriptfile(scriptfile.Text);
        }

        private void LoadTestScript_Click(object sender, EventArgs e)
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
                    granttype = "Authorization Code";
                }
                if(granttype == "authorization_code")
                {
                    granttype = "Authorization Code";
                }
                this.oauth_granttype.SelectedIndex = this.oauth_granttype.FindString(granttype.Trim());
                //this.oauth_granttype.SelectedIndex = this.oauth_granttype.FindString("Client Credentials");

                if (this.oauth_granttype.SelectedIndex == -1)
                {
                    this.oauth_granttype.SelectedIndex = 0;
                }

                AuthenticationTypeData combodata = this.oauth_granttype.SelectedItem as AuthenticationTypeData;
                this.oauth_granttype.Text = combodata.Name;




                //OData

                this.scriptfile.Text = filename;//clientproperties.getProperty("textScriptFile");
                clientproperties.setProperty("textScriptFile",filename);
                if (string.IsNullOrEmpty(scriptfile.Text))
                {
                    scriptfile.Text = System.IO.Path.Combine(executepath, @"webapitestscript") + "\\TestScript.resoscript";
                }

                //clientproperties.setProperty("Preauthenticate", ((this.preauthenticate.Checked == true) ? ("TRUE") : ("FALSE")));
                this.preauthenticate.Checked = ((clientproperties.getProperty("Preauthenticate") == "TRUE") ? (true):(false));
                this.edit_AuthorizationURI.Text = clientproperties.getProperty("AuthorizationURI");
                this.edit_AccessTokenURI.Text = clientproperties.getProperty("TokenURI");

                this.edit_WebAPIEndPointURI.Text = clientproperties.getProperty("textWebAPIURI");

                //this.edit_ClientID.Text = clientproperties.getProperty("textOAuthClientIdentification");
                this.edit_ClientID.Text = clientproperties.getProperty("ClientIdentification");
                this.edit_RedirectURI.Text = clientproperties.getProperty("RedirectURI");

                this.edit_ClientSecret.Text = clientproperties.getProperty("ClientSecret");
                this.edit_Scope.Text = clientproperties.getProperty("ClientScope");


                this.edit_UserName.Text = clientproperties.getProperty("UserName");
                this.edit_BearerToken.Text = clientproperties.getProperty("BearerToken");
                this.edit_Password.Text = clientproperties.getProperty("Password");
                //Global

                this.LogDirectory.Text = VerifyLogDirectory(clientproperties.getProperty("transactionlogdirectory"));
                
                this.ResultsDirectory.Text = VerifyResultsDirectory(clientproperties.getProperty("resultsdirectory"));
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

  
        private void oauth_granttype_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            AuthenticationTypeData combodata = comboBox.SelectedItem as AuthenticationTypeData;
            if (combodata == null)
            {
                SetAuthTypeControls("authorization_code");
            }
            else
            {
                SetAuthTypeControls(combodata.Value);
            }
        }

        private void SetAuthTypeControls(string authtype)
        {
            //lbl_textWebAPIURI/textWebAPIURI
            //lbl_UserName/UserName and lbl_BearerToken/edit_BearerToken
            //lbl_Password/edit_Password
            //lbl_AuthorizationURI/edit_AuthorizationURI
            //lbl_AccessTokenURI/edit_AccessTokenURI
            //lbl_RedirectURI/edit_RedirectURI
            //lbl_ClientID/edit_ClientID
            //lbl_ClientSecret/edit_ClientSecret
            //lbl_Scope/edit_Scope



            bool bearertokenonly = false;
            bool clientcredientals = false;
            if (authtype == "bearer_token")
            {
                bearertokenonly = true;
            }
            else if(authtype == "client_credentials")
            {
                clientcredientals = true;
            }

            preauthenticate.Visible = (bearertokenonly == true) ? false : (clientcredientals == true) ? true : true; 

            lbl_WebAPIEndPointURI.Visible = edit_WebAPIEndPointURI.Visible = (bearertokenonly == true) ? true : (clientcredientals == true) ? true : true;
            lbl_UserName.Visible = edit_UserName.Visible = (bearertokenonly == true) ? false : (clientcredientals == true) ? false : true;
            lbl_BearerToken.Visible = edit_BearerToken.Visible = (bearertokenonly == true) ? true : (clientcredientals == true) ? false : true;
            lbl_Password.Visible = edit_Password.Visible = (bearertokenonly == true) ? false : (clientcredientals == true) ? false : true;
            lbl_AuthorizationURI.Visible = edit_AuthorizationURI.Visible = (bearertokenonly == true) ? false : (clientcredientals == true) ? false : true;
            lbl_AccessTokenURI.Visible = edit_AccessTokenURI.Visible = (bearertokenonly == true) ? false : (clientcredientals == true) ? true : true;
            lbl_RedirectURI.Visible = edit_RedirectURI.Visible = (bearertokenonly == true) ? false : (clientcredientals == true) ? false : true;
            lbl_ClientID.Visible = edit_ClientID.Visible = (bearertokenonly == true) ? false : (clientcredientals == true) ? true : true;
            lbl_ClientSecret.Visible = edit_ClientSecret.Visible = (bearertokenonly == true) ? false : (clientcredientals == true) ? true : true;
            lbl_Scope.Visible = edit_Scope.Visible = (bearertokenonly == true) ? false : (clientcredientals == true) ? true : true;
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
            int finderror = 0;
            System.Guid JobID = System.Guid.NewGuid();
            ResultsProvider resultProvider = new ResultsProvider(JobID);
            ILogger logger = resultProvider as ILogger;
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
                else
                {
                    string testurl = clientsettings.GetSetting(settings.webapi_uri);
                    testurl = testurl.TrimEnd('/');
                    clientsettings.SetSetting(settings.webapi_uri, testurl);

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

                finderror = 1;

                var reqHeaders = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("OData-Version", "4.0") };
                reqHeaders.Add(new KeyValuePair<string, string>("Authorization", oauth_bearertoken));

                string reqHeadersString = ConvertListToString(reqHeaders);

                


                
                Uri url = GetUri(clientsettings.GetSetting(settings.webapi_uri));

                Uri host = GetUri(url.Scheme.ToString() + "://" + url.Host);
                //Uri uri = new Uri(host, url.AbsoluteUri);
                
                Uri service = GetUri(clientsettings.GetSetting(settings.webapi_uri).TrimEnd('/')+"/");
                ServiceStatus intservice = ServiceStatus.GetInstance(clientsettings.GetSetting(settings.webapi_uri).TrimEnd('/') + "/", reqHeadersString);
                ServiceStatus.ReviseMetadata(metadataresponse);
                ServiceContext ctx = new ServiceContext(url, JobID, HttpStatusCode.OK, reqHeadersString, metadataresponse, string.Empty, service, serviceresponse, metadataresponse, false, reqHeaders, ODataMetadataType.MinOnly);


                finderror = 2;
                if(CheckForErrors(ctx))
                {
                    return;
                }


                finderror = 23;
                TestControl testcontrol = new TestControl();
                testcontrol.BuildRuleControlList(TestsToRun.Text, clientsettings);
                RuleCatalogCollection.Instance.Clear();

                StringBuilder rulecontrolallfile = new StringBuilder();
                rulecontrolallfile.Append("<rulecontrols>\r\n");

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
                int count = 0;
                ExtensionRuleStore extensionStore = new ExtensionRuleStore("extensions", logger);
                foreach (var rule in extensionStore.GetRules())
                {
                    count++;
                    try
                    {
                        AddRuleData(ref rulecontrolallfile, ref sb, rule, testcontrol);
                    }
                    catch(Exception ex)
                    {

                    }
                    if (!CheckRule(rule, count, clientsettings, testcontrol))
                    {
                        continue;
                    }
                    RuleCatalogCollection.Instance.Add(rule);
                }

                VerifyLogDirectory(clientsettings);
                VerifyResultsDirectory(clientsettings);
                try
                {
                    System.IO.File.WriteAllText(clientsettings.GetSetting(settings.log_directory) + "\\rulelist.txt", sb.ToString());
                }
                catch
                {
                    try
                    {
                        System.IO.File.WriteAllText(clientsettings.GetSetting(settings.log_directory) + "\\rulelist.txt", sb.ToString());
                    }
                    catch(IOException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                sb.Clear();

                rulecontrolallfile.Append("</rulecontrols>\r\n");
                System.IO.File.WriteAllText(clientsettings.GetSetting(settings.log_directory) + "\\rulecontrolfile.xml", rulecontrolallfile.ToString());
                rulecontrolallfile.Clear();
                finderror = 6;

                var ruleArray = RuleCatalogCollection.Instance.ToArray();
                
                RuleExecuter rules = new RuleExecuter(resultProvider, logger);
                rules.Execute(ctx, ruleArray, (int)ruleArray.Length, webapitestcomplete);
                ResultsProvider logitems = logger as ResultsProvider;
               // Hashtable logitemshash = new Hashtable();
                finderror = 10;
                int detailcount = 0;
                //foreach (ExtensionRuleResultDetail item in logitems.Details)
                //{
                //    detailcount++;
                //    if (logitemshash[item.RuleName] == null)
                //    {
                //        Hashtable hsh = new Hashtable();
                //        hsh[detailcount] = item;
                //        logitemshash[item.RuleName] = hsh;
                //    }
                //    else
                //    {
                //        Hashtable hsh = logitemshash[item.RuleName] as Hashtable;
                //        hsh[detailcount] = item;
                //        logitemshash[item.RuleName] = hsh;
                //    }
                //}
                finderror = 11;
                StringBuilder sbresults = new StringBuilder();
                
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
                finderror = 7;
                foreach (ODataValidator.RuleEngine.TestResult result in resultProvider.ResultsToSave)
                {
                    //BuildResultsOutput(ref sbresults, result, logitemshash);
                    BuildResultsOutput(ref sbresults, result);
                }
                finderror = 8;
                VerifyLogDirectory(clientsettings);
                VerifyResultsDirectory(clientsettings);

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(clientsettings.GetSetting(settings.log_directory) + "\\" + "outputlog" + ".txt", false))
                {
                    file.Write(sbLogAll.ToString());
                }

                finderror = 9;
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
                MessageOutput(ex.Message);
            }

            OutputLogCapture();
        }

        private void VerifyResultsDirectory(RESOClientSettings clientsettings)
        {
            string result = VerifyResultsDirectory(clientsettings.GetSetting(settings.results_directory));
            clientsettings.SetSetting(settings.results_directory,result);
        }

        private string VerifyResultsDirectory(string directory)
        {
            string result = directory;
            if (!Directory.Exists(result))
            {
                result = @"C:\results";
                if (!Directory.Exists(result))
                {
                    Directory.CreateDirectory(@"C:\results");
                }
            }
            return result;

        }

        private void VerifyLogDirectory(RESOClientSettings clientsettings)
        {
            string result = VerifyLogDirectory(clientsettings.GetSetting(settings.log_directory));
            clientsettings.SetSetting(settings.log_directory, result);
        }

        private string VerifyLogDirectory(string directory)
        {
            string result = directory;
            if (!Directory.Exists(result))
            {
                result = @"C:\Logs";
                if (!Directory.Exists(result))
                {
                    Directory.CreateDirectory(@"C:\results");
                }
            }
            return result;

        }

        private bool CheckForErrors(ServiceContext ctx)
        {

            bool metadatagood = true;
            Tuple<string, List<NormalProperty>, List<NavigProperty>> filterRestrictions = null;
            try
            {
                filterRestrictions = AnnotationsHelper.GetFilterRestrictions(ctx.MetadataDocument, ctx.VocCapabilities);
            }
            catch
            {
                metadatagood = false;
            }

            if (!metadatagood)
            {
                DialogResult dialogResult = MessageBox.Show("The Metadata is malformed.  Many of the test will fail.  Would you like to continue?", "Malformed Metadata", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    return true;
                }
            }

            bool servicegood = true;
            List<string> entitySetURLs = null;
            try
            {
                entitySetURLs = MetadataHelper.GetEntitySetURLs();
            }
            catch (Exception ex)
            {
                servicegood = false;
            }

            
            if (!servicegood)
            {
                DialogResult dialogResult = MessageBox.Show("The Service Document is malformed.  There are two attributes required in the document for each EntitySet:  \"url\" and \"kind\".  Many of the test will fail.  Would you like to continue?", "Malformed Service Document", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    return true;
                }
            }
            return false;
        }

        private string EscapeXML(string data)
        {
            if(string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
            
            return data.Replace("&", "&amp;").Replace("\"", "\\\"").Replace("'", "&apos;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\r",string.Empty).Replace("\n",string.Empty);
        }

        private string ConvertListToString(List<KeyValuePair<string, string>> reqHeaders)
        {
            string ret = string.Empty;

            foreach (KeyValuePair<string, string> entry in reqHeaders)
            {
                string name = entry.Key as string;
                string value = entry.Value as string;
                ret += name + ":" + value + "\r\n;";
            }
            ret = ret.TrimEnd(';');
            return ret;


        }

        void webapitestcomplete(ODataValidator.RuleEngine.TestResult results, int numberofrules, ref int count)
        {
            StringBuilder sbLogAll_out = OutputDetail(results);
            sbLogAll.Append(sbLogAll_out);
            sbLogAll_out.Clear();
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
            if (metadatatests.Checked)
            {
                if((rule.Name.IndexOf("Property") >= 0) && (rule.Name.IndexOf("IndividualProperty") < 0))
                    {
                }
                if ((rule.Name.IndexOf("Metadata") >= 0) || (rule.Name.IndexOf("IndividualProperty") >= 0) || (rule.Name.IndexOf("Property.Core") >= 0))
                {
                    if ((rule.Version == ODataValidator.RuleEngine.ODataVersion.V3_V4) ||
                        (rule.Version == ODataValidator.RuleEngine.ODataVersion.V4) ||
                        (rule.Version == ODataValidator.RuleEngine.ODataVersion.V_All))
                    {
                        if (string.IsNullOrEmpty(TestsToRun.Text))
                        {
                            return true;
                        }
                        else if (rule.Name == TestsToRun.Text)
                        {
                            return true;
                        }

                    }
                    if ((rule.Version == ODataValidator.RuleEngine.ODataVersion.UNKNOWN))
                    {
                        if (string.IsNullOrEmpty(TestsToRun.Text))
                        {
                            return true;
                        }
                        else if (rule.Name == TestsToRun.Text)
                        {
                            return true;
                        }

                    }
                }
            }
            if (servicedoctests.Checked)
            {
                if (rule.Name.IndexOf("SvcDoc") >= 0)
                {
                    if ((rule.Version == ODataValidator.RuleEngine.ODataVersion.V3_V4) ||
                        (rule.Version == ODataValidator.RuleEngine.ODataVersion.V4) ||
                        (rule.Version == ODataValidator.RuleEngine.ODataVersion.V_All))
                    {
                        if (string.IsNullOrEmpty(TestsToRun.Text))
                        {
                            return true;
                        }
                        else if (rule.Name == TestsToRun.Text)
                        {
                            return true;
                        }

                    }
                    if ((rule.Version == ODataValidator.RuleEngine.ODataVersion.UNKNOWN))
                    {
                        if (string.IsNullOrEmpty(TestsToRun.Text))
                        {
                            return true;
                        }
                        else if (rule.Name == TestsToRun.Text)
                        {
                            return true;
                        }

                    }

                }
            }

            if(!rulecontrolfile.Checked)
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
            //if (rule.Name == "Intermediate.Conformance.1016")
            //{
            //    return false;
            //}
            //if (rule.Name == "Advanced.Conformance.1007")
            //{
            //    return false;
            //}
            //if (rule.Name == "Advanced.Conformance.1004")
            //{
            //    return false;
            //}
            //if (rule.Name == "Intermediate.Conformance.1013")
            //{
            //    return false;
            //}
            //if (rule.Name == "ServiceImpl_GetNavigationProperty")
            //{
            //    return false;
            //}
            //if (rule.Name == "ServiceImpl_RequestingChanges_delta")
            //{
            //    return false;
            //}
            //if (rule.Name == "ServiceImpl_SystemQueryOptionSkip")
            //{
            //    return false;
            //}
            //if (rule.Name == "ServiceImpl_SystemQueryOptionSkipToken")
            //{
            //    return false;
            //}

            RuleControl control = testcontrol.rulecontrol[rule.Name] as RuleControl;
            if (control != null)
            {
                return true;
                //if (string.Compare(control.cert_impact, "Core", true) == 0)
                //{
                //    return true;
                //}
                //if (string.Compare(control.cert_impact, "Platinum", true) == 0)
                //{
                //    return true;
                //}

                //if (string.Compare(control.cert_impact, "Gold", true) == 0)
                //{
                //    return true;
                //}
                //if (string.Compare(control.cert_impact, "Silver", true) == 0)
                //{
                //    return true;
                //}
                //if (string.Compare(control.cert_impact, "Bronze", true) == 0)
                //{
                //    return true;
                //}
            }
            else
            {
                return false;
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

        private void BuildResultsOutput(ref StringBuilder sbresults, ODataValidator.RuleEngine.TestResult result)
        {

            //if (result.Classification != "notApplicable")
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


                sbresults.Append("\r\n");
            }
        }

        private StringBuilder OutputDetail(ODataValidator.RuleEngine.TestResult results)
        {
            StringBuilder sbLog = new StringBuilder();
            
            
                sbLog.Append("___________RULE RESULTS___________");
                sbLog.Append("\r\n");
                sbLog.Append("Category:"+  results.Category);
                sbLog.Append("\r\n");
                sbLog.Append("Classification:" + results.Classification);
                sbLog.Append("\r\n");
                sbLog.Append("Description:" + results.Description);
                sbLog.Append("\r\n");
                sbLog.Append("ErrorDetail:" + results.ErrorDetail);
                sbLog.Append("\r\n");
                sbLog.Append("ErrorMessage:" + results.ErrorMessage);
                sbLog.Append("\r\n");
                sbLog.Append("HelpLink:" + results.HelpLink);
                sbLog.Append("\r\n");
                sbLog.Append("HelpUri:" + results.HelpUri);
                sbLog.Append("\r\n");
                sbLog.Append("JobId:" + results.JobId);
                sbLog.Append("\r\n");
                sbLog.Append("LineNumberInError:" + Convert.ToString(results.LineNumberInError));
                sbLog.Append("\r\n");
                sbLog.Append("ODataLevel:" + results.ODataLevel);
                sbLog.Append("\r\n");
                sbLog.Append("RequirementLevel:" + results.RequirementLevel);
                sbLog.Append("\r\n");
                sbLog.Append("RuleName:" + results.RuleName);
                sbLog.Append("\r\n");
                sbLog.Append("SpecificationSection:" + results.SpecificationSection);
                sbLog.Append("\r\n");
                sbLog.Append("SpecificationUri:" + results.SpecificationUri);
                sbLog.Append("\r\n");
                sbLog.Append("Target:" + results.Target);
                sbLog.Append("\r\n");
                sbLog.Append("TextInvolved:" + results.TextInvolved);
                sbLog.Append("\r\n");
                sbLog.Append("V4Specification:" + results.V4Specification);
                sbLog.Append("\r\n");
                sbLog.Append("V4SpecificationSection:" + results.V4SpecificationSection);
                sbLog.Append("\r\n");
                sbLog.Append("ValidationJobID:" + results.ValidationJobID);
                sbLog.Append("\r\n");
                sbLog.Append("Version:" + results.Version);
                sbLog.Append("\r\n");

                sbLog.Append("___________DETAILED RESULTS___________\r\n");
                if (results.Details != null)
                {

                    foreach (ExtensionRuleResultDetail item in results.Details)
                    {


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
                                if (string.IsNullOrEmpty(item.ResponsePayload))
                                {

                                }
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
                }
                else
                {
                    sbLog.Append("No detailed results provided\r\n");
                }
            RESOClientSettings clientsettings = GetSettings();
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(clientsettings.GetSetting(settings.log_directory) + "\\" + results.RuleName + ".txt", false))
            {
                file.Write(sbLog.ToString());
                file.Close();
            }
            return sbLog;
        }
    
        private void AddRuleData(ref StringBuilder allrulecontrollist, ref StringBuilder sb, ODataValidator.RuleEngine.Rule rule, TestControl testcontrol)
        {
            if (testcontrol != null)
            {
                RuleControl control = testcontrol.rulecontrol[rule.Name] as RuleControl;

                allrulecontrollist.Append("\t<rulecontrol>\r\n");
                //Original
                allrulecontrollist.Append("\t\t<rulename>" + EscapeXML(rule.Name) + "</rulename>\r\n");
                if (control == null)
                {
                    allrulecontrollist.Append("\t\t<notes></notes>\r\n");
                    allrulecontrollist.Append("\t\t<cert_tr></cert_tr>\r\n");
                    allrulecontrollist.Append("\t\t<cert_impact>" + rule.RequirementLevel + "</cert_impact>\r\n");
                    allrulecontrollist.Append("\t\t<ttt_testing_results></ttt_testing_results>\r\n");
                    allrulecontrollist.Append("\t\t<category>" + rule.Category + "</category>\r\n");
                    allrulecontrollist.Append("\t\t<RESOVersion></RESOVersion>\r\n");

                }
                else
                {
                    allrulecontrollist.Append("\t\t<notes>" + EscapeXML(control.notes) + "</notes>\r\n");
                    allrulecontrollist.Append("\t\t<cert_tr>" + control.cert_tr + "</cert_tr>\r\n");
                    allrulecontrollist.Append("\t\t<cert_impact>" + control.cert_impact + "</cert_impact>\r\n");
                    allrulecontrollist.Append("\t\t<ttt_testing_results>" + control.ttt_testing_results + "</ttt_testing_results>\r\n");
                    allrulecontrollist.Append("\t\t<category>" + control.category + "</category>\r\n");
                    allrulecontrollist.Append("\t\t<RESOVersion>1.02</RESOVersion>\r\n");
                }
                //Added
                allrulecontrollist.Append("\t\t<Description>" + EscapeXML(rule.Description) + "</Description>\r\n");
                allrulecontrollist.Append("\t\t<ErrorMessage>" + EscapeXML(rule.ErrorMessage) + "</ErrorMessage>\r\n");
                allrulecontrollist.Append("\t\t<ODataSpecification>" + (string.IsNullOrEmpty(rule.SpecificationSection) ? "" : rule.SpecificationSection) + "</ODataSpecification>\r\n");
                allrulecontrollist.Append("\t\t<V4ODataSpecification>" + (string.IsNullOrEmpty(rule.V4SpecificationSection) ? "" : rule.V4SpecificationSection) + "</V4ODataSpecification>\r\n");
                allrulecontrollist.Append("\t\t<V4Specification>" + (string.IsNullOrEmpty(rule.V4Specification) ? "" : rule.V4Specification) + "</V4Specification>\r\n");
                allrulecontrollist.Append("\t\t<ODataVersion>" + rule.Version + "</ODataVersion>\r\n");
                allrulecontrollist.Append("\t\t<PayloadType>" + rule.PayloadType + "</PayloadType>\r\n");
                allrulecontrollist.Append("\t\t<PayloadFormat>" + rule.PayloadFormat + "</PayloadFormat>\r\n");
                allrulecontrollist.Append("\t\t<HelpLink>" + rule.HelpLink + "</HelpLink>\r\n");
                


                allrulecontrollist.Append("\t</rulecontrol>\r\n");



      }
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

    public class AuthenticationTypeData
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }




}

