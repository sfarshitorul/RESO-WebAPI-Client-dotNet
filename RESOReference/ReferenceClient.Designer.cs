namespace RESOReference
{
    partial class ReferenceClient
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_SelectResultsDirectory = new System.Windows.Forms.Button();
            this.ResultsDirectory = new System.Windows.Forms.TextBox();
            this.LogDirectory = new System.Windows.Forms.TextBox();
            this.btn_SelectLogDirectory = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.webapicurrentrule = new System.Windows.Forms.Label();
            this.webapiprogressBar = new System.Windows.Forms.ProgressBar();
            this.ServerVersion = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.preauthenticate = new System.Windows.Forms.CheckBox();
            this.btn_ClearSettings = new System.Windows.Forms.Button();
            this.btn_ResetSession = new System.Windows.Forms.Button();
            this.btn_ExecuteTestScript = new System.Windows.Forms.Button();
            this.lbl_Password = new System.Windows.Forms.Label();
            this.lbl_UserName = new System.Windows.Forms.Label();
            this.lbl_RedirectURI = new System.Windows.Forms.Label();
            this.edit_RedirectURI = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.lbl_AuthorizationURI = new System.Windows.Forms.Label();
            this.edit_AuthorizationURI = new System.Windows.Forms.TextBox();
            this.edit_Password = new System.Windows.Forms.TextBox();
            this.edit_UserName = new System.Windows.Forms.TextBox();
            this.oauth_granttype = new System.Windows.Forms.ComboBox();
            this.lbl_WebAPIEndPointURI = new System.Windows.Forms.Label();
            this.edit_WebAPIEndPointURI = new System.Windows.Forms.TextBox();
            this.btn_SaveClientSettings = new System.Windows.Forms.Button();
            this.btn_SelectClientSettings = new System.Windows.Forms.Button();
            this.lbl_AccessTokenURI = new System.Windows.Forms.Label();
            this.lbl_ClientID = new System.Windows.Forms.Label();
            this.lbl_ClientSecret = new System.Windows.Forms.Label();
            this.lbl_Scope = new System.Windows.Forms.Label();
            this.edit_AccessTokenURI = new System.Windows.Forms.TextBox();
            this.edit_ClientID = new System.Windows.Forms.TextBox();
            this.edit_ClientSecret = new System.Windows.Forms.TextBox();
            this.edit_Scope = new System.Windows.Forms.TextBox();
            this.btn_Login = new System.Windows.Forms.Button();
            this.ViewMetadata = new System.Windows.Forms.Button();
            this.label24 = new System.Windows.Forms.Label();
            this.serviceresponsedata = new System.Windows.Forms.TextBox();
            this.webapi_metadata = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.webapi_token = new System.Windows.Forms.TextBox();
            this.openid_code = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.scriptfile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_LoadTestScript = new System.Windows.Forms.Button();
            this.edit_BearerToken = new System.Windows.Forms.TextBox();
            this.lbl_BearerToken = new System.Windows.Forms.Label();
            this.btn_RunValidationTest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_SelectResultsDirectory
            // 
            this.btn_SelectResultsDirectory.Location = new System.Drawing.Point(629, 446);
            this.btn_SelectResultsDirectory.Name = "btn_SelectResultsDirectory";
            this.btn_SelectResultsDirectory.Size = new System.Drawing.Size(143, 23);
            this.btn_SelectResultsDirectory.TabIndex = 150;
            this.btn_SelectResultsDirectory.Tag = "Select Results Directory";
            this.btn_SelectResultsDirectory.Text = "Select Results Directory";
            this.btn_SelectResultsDirectory.UseVisualStyleBackColor = true;
            this.btn_SelectResultsDirectory.Click += new System.EventHandler(this.SelectResultsDirectory_Click);
            // 
            // ResultsDirectory
            // 
            this.ResultsDirectory.Location = new System.Drawing.Point(140, 448);
            this.ResultsDirectory.Name = "ResultsDirectory";
            this.ResultsDirectory.Size = new System.Drawing.Size(470, 20);
            this.ResultsDirectory.TabIndex = 15;
            this.ResultsDirectory.TabStop = false;
            // 
            // LogDirectory
            // 
            this.LogDirectory.Location = new System.Drawing.Point(140, 419);
            this.LogDirectory.Name = "LogDirectory";
            this.LogDirectory.Size = new System.Drawing.Size(470, 20);
            this.LogDirectory.TabIndex = 14;
            this.LogDirectory.TabStop = false;
            // 
            // btn_SelectLogDirectory
            // 
            this.btn_SelectLogDirectory.Location = new System.Drawing.Point(629, 417);
            this.btn_SelectLogDirectory.Name = "btn_SelectLogDirectory";
            this.btn_SelectLogDirectory.Size = new System.Drawing.Size(143, 23);
            this.btn_SelectLogDirectory.TabIndex = 140;
            this.btn_SelectLogDirectory.Tag = "Select Log Directory";
            this.btn_SelectLogDirectory.Text = "Select Log Directory";
            this.btn_SelectLogDirectory.UseVisualStyleBackColor = true;
            this.btn_SelectLogDirectory.Click += new System.EventHandler(this.SelectLogDirectory_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(44, 452);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(90, 13);
            this.label12.TabIndex = 92;
            this.label12.Text = "Results Directory:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(61, 423);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(73, 13);
            this.label13.TabIndex = 91;
            this.label13.Text = "Log Directory:";
            // 
            // webapicurrentrule
            // 
            this.webapicurrentrule.AutoSize = true;
            this.webapicurrentrule.Location = new System.Drawing.Point(368, 40);
            this.webapicurrentrule.Name = "webapicurrentrule";
            this.webapicurrentrule.Size = new System.Drawing.Size(36, 13);
            this.webapicurrentrule.TabIndex = 100;
            this.webapicurrentrule.Text = "00/00";
            // 
            // webapiprogressBar
            // 
            this.webapiprogressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webapiprogressBar.Location = new System.Drawing.Point(605, 40);
            this.webapiprogressBar.Name = "webapiprogressBar";
            this.webapiprogressBar.Size = new System.Drawing.Size(308, 23);
            this.webapiprogressBar.TabIndex = 99;
            // 
            // ServerVersion
            // 
            this.ServerVersion.FormattingEnabled = true;
            this.ServerVersion.Items.AddRange(new object[] {
            "Web API/1.0"});
            this.ServerVersion.Location = new System.Drawing.Point(140, 20);
            this.ServerVersion.Name = "ServerVersion";
            this.ServerVersion.Size = new System.Drawing.Size(212, 21);
            this.ServerVersion.TabIndex = 10;
            this.ServerVersion.Text = "Web API/1.0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(43, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(91, 13);
            this.label7.TabIndex = 97;
            this.label7.Text = "Standard Version:";
            // 
            // preauthenticate
            // 
            this.preauthenticate.AutoSize = true;
            this.preauthenticate.Location = new System.Drawing.Point(140, 48);
            this.preauthenticate.Name = "preauthenticate";
            this.preauthenticate.Size = new System.Drawing.Size(101, 17);
            this.preauthenticate.TabIndex = 20;
            this.preauthenticate.Text = "Preauthenticate";
            this.preauthenticate.UseVisualStyleBackColor = true;
            // 
            // btn_ClearSettings
            // 
            this.btn_ClearSettings.Location = new System.Drawing.Point(140, 359);
            this.btn_ClearSettings.Name = "btn_ClearSettings";
            this.btn_ClearSettings.Size = new System.Drawing.Size(135, 23);
            this.btn_ClearSettings.TabIndex = 129;
            this.btn_ClearSettings.TabStop = false;
            this.btn_ClearSettings.Text = "Clear Settings";
            this.btn_ClearSettings.UseVisualStyleBackColor = true;
            this.btn_ClearSettings.Click += new System.EventHandler(this.clearsettings_Click);
            // 
            // btn_ResetSession
            // 
            this.btn_ResetSession.Location = new System.Drawing.Point(281, 359);
            this.btn_ResetSession.Name = "btn_ResetSession";
            this.btn_ResetSession.Size = new System.Drawing.Size(135, 23);
            this.btn_ResetSession.TabIndex = 128;
            this.btn_ResetSession.TabStop = false;
            this.btn_ResetSession.Text = "Reset Session";
            this.btn_ResetSession.UseVisualStyleBackColor = true;
            this.btn_ResetSession.Click += new System.EventHandler(this.resetsession_Click);
            // 
            // btn_ExecuteTestScript
            // 
            this.btn_ExecuteTestScript.Location = new System.Drawing.Point(778, 388);
            this.btn_ExecuteTestScript.Name = "btn_ExecuteTestScript";
            this.btn_ExecuteTestScript.Size = new System.Drawing.Size(143, 23);
            this.btn_ExecuteTestScript.TabIndex = 126;
            this.btn_ExecuteTestScript.TabStop = false;
            this.btn_ExecuteTestScript.Text = "Execute Test Script";
            this.btn_ExecuteTestScript.UseVisualStyleBackColor = true;
            this.btn_ExecuteTestScript.Click += new System.EventHandler(this.executetestscript_Click_1);
            // 
            // lbl_Password
            // 
            this.lbl_Password.AutoSize = true;
            this.lbl_Password.Location = new System.Drawing.Point(77, 152);
            this.lbl_Password.Name = "lbl_Password";
            this.lbl_Password.Size = new System.Drawing.Size(56, 13);
            this.lbl_Password.TabIndex = 124;
            this.lbl_Password.Text = "Password:";
            // 
            // lbl_UserName
            // 
            this.lbl_UserName.AutoSize = true;
            this.lbl_UserName.Location = new System.Drawing.Point(32, 130);
            this.lbl_UserName.Name = "lbl_UserName";
            this.lbl_UserName.Size = new System.Drawing.Size(99, 13);
            this.lbl_UserName.TabIndex = 123;
            this.lbl_UserName.Text = "            User Name:";
            // 
            // lbl_RedirectURI
            // 
            this.lbl_RedirectURI.AutoSize = true;
            this.lbl_RedirectURI.Location = new System.Drawing.Point(61, 233);
            this.lbl_RedirectURI.Name = "lbl_RedirectURI";
            this.lbl_RedirectURI.Size = new System.Drawing.Size(72, 13);
            this.lbl_RedirectURI.TabIndex = 122;
            this.lbl_RedirectURI.Text = "Redirect URI:";
            // 
            // edit_RedirectURI
            // 
            this.edit_RedirectURI.Location = new System.Drawing.Point(139, 231);
            this.edit_RedirectURI.Name = "edit_RedirectURI";
            this.edit_RedirectURI.Size = new System.Drawing.Size(379, 20);
            this.edit_RedirectURI.TabIndex = 90;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(71, 74);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(63, 13);
            this.label23.TabIndex = 121;
            this.label23.Text = "Grant Type:";
            // 
            // lbl_AuthorizationURI
            // 
            this.lbl_AuthorizationURI.AutoSize = true;
            this.lbl_AuthorizationURI.Location = new System.Drawing.Point(40, 179);
            this.lbl_AuthorizationURI.Name = "lbl_AuthorizationURI";
            this.lbl_AuthorizationURI.Size = new System.Drawing.Size(93, 13);
            this.lbl_AuthorizationURI.TabIndex = 115;
            this.lbl_AuthorizationURI.Text = "Authorization URI:";
            // 
            // edit_AuthorizationURI
            // 
            this.edit_AuthorizationURI.Location = new System.Drawing.Point(139, 177);
            this.edit_AuthorizationURI.Name = "edit_AuthorizationURI";
            this.edit_AuthorizationURI.Size = new System.Drawing.Size(379, 20);
            this.edit_AuthorizationURI.TabIndex = 70;
            // 
            // edit_Password
            // 
            this.edit_Password.Location = new System.Drawing.Point(139, 150);
            this.edit_Password.Name = "edit_Password";
            this.edit_Password.Size = new System.Drawing.Size(379, 20);
            this.edit_Password.TabIndex = 60;
            // 
            // edit_UserName
            // 
            this.edit_UserName.Location = new System.Drawing.Point(139, 125);
            this.edit_UserName.Name = "edit_UserName";
            this.edit_UserName.Size = new System.Drawing.Size(381, 20);
            this.edit_UserName.TabIndex = 50;
            // 
            // oauth_granttype
            // 
            this.oauth_granttype.FormattingEnabled = true;
            this.oauth_granttype.Location = new System.Drawing.Point(140, 71);
            this.oauth_granttype.Name = "oauth_granttype";
            this.oauth_granttype.Size = new System.Drawing.Size(121, 21);
            this.oauth_granttype.TabIndex = 30;
            this.oauth_granttype.SelectedValueChanged += new System.EventHandler(this.oauth_granttype_SelectedValueChanged);
            // 
            // lbl_WebAPIEndPointURI
            // 
            this.lbl_WebAPIEndPointURI.AutoSize = true;
            this.lbl_WebAPIEndPointURI.Location = new System.Drawing.Point(10, 101);
            this.lbl_WebAPIEndPointURI.Name = "lbl_WebAPIEndPointURI";
            this.lbl_WebAPIEndPointURI.Size = new System.Drawing.Size(124, 13);
            this.lbl_WebAPIEndPointURI.TabIndex = 109;
            this.lbl_WebAPIEndPointURI.Text = "Web API End Point URI:";
            // 
            // edit_WebAPIEndPointURI
            // 
            this.edit_WebAPIEndPointURI.Location = new System.Drawing.Point(140, 98);
            this.edit_WebAPIEndPointURI.Name = "edit_WebAPIEndPointURI";
            this.edit_WebAPIEndPointURI.Size = new System.Drawing.Size(379, 20);
            this.edit_WebAPIEndPointURI.TabIndex = 40;
            // 
            // btn_SaveClientSettings
            // 
            this.btn_SaveClientSettings.Location = new System.Drawing.Point(496, 10);
            this.btn_SaveClientSettings.Name = "btn_SaveClientSettings";
            this.btn_SaveClientSettings.Size = new System.Drawing.Size(135, 23);
            this.btn_SaveClientSettings.TabIndex = 107;
            this.btn_SaveClientSettings.TabStop = false;
            this.btn_SaveClientSettings.Tag = "Select Results Directory";
            this.btn_SaveClientSettings.Text = "Save Client Settings";
            this.btn_SaveClientSettings.UseVisualStyleBackColor = true;
            this.btn_SaveClientSettings.Click += new System.EventHandler(this.SaveClientSettings_Click);
            // 
            // btn_SelectClientSettings
            // 
            this.btn_SelectClientSettings.Location = new System.Drawing.Point(637, 10);
            this.btn_SelectClientSettings.Name = "btn_SelectClientSettings";
            this.btn_SelectClientSettings.Size = new System.Drawing.Size(135, 23);
            this.btn_SelectClientSettings.TabIndex = 106;
            this.btn_SelectClientSettings.TabStop = false;
            this.btn_SelectClientSettings.Tag = "Select Results Directory";
            this.btn_SelectClientSettings.Text = "Select Client Settings";
            this.btn_SelectClientSettings.UseVisualStyleBackColor = true;
            this.btn_SelectClientSettings.Click += new System.EventHandler(this.SelectClientSettings_Click);
            // 
            // lbl_AccessTokenURI
            // 
            this.lbl_AccessTokenURI.AutoSize = true;
            this.lbl_AccessTokenURI.Location = new System.Drawing.Point(32, 206);
            this.lbl_AccessTokenURI.Name = "lbl_AccessTokenURI";
            this.lbl_AccessTokenURI.Size = new System.Drawing.Size(101, 13);
            this.lbl_AccessTokenURI.TabIndex = 102;
            this.lbl_AccessTokenURI.Text = "Access Token URI:";
            // 
            // lbl_ClientID
            // 
            this.lbl_ClientID.AutoSize = true;
            this.lbl_ClientID.Location = new System.Drawing.Point(83, 260);
            this.lbl_ClientID.Name = "lbl_ClientID";
            this.lbl_ClientID.Size = new System.Drawing.Size(50, 13);
            this.lbl_ClientID.TabIndex = 103;
            this.lbl_ClientID.Text = "Client ID:";
            // 
            // lbl_ClientSecret
            // 
            this.lbl_ClientSecret.AutoSize = true;
            this.lbl_ClientSecret.Location = new System.Drawing.Point(63, 287);
            this.lbl_ClientSecret.Name = "lbl_ClientSecret";
            this.lbl_ClientSecret.Size = new System.Drawing.Size(70, 13);
            this.lbl_ClientSecret.TabIndex = 104;
            this.lbl_ClientSecret.Text = "Client Secret:";
            // 
            // lbl_Scope
            // 
            this.lbl_Scope.AutoSize = true;
            this.lbl_Scope.Location = new System.Drawing.Point(92, 314);
            this.lbl_Scope.Name = "lbl_Scope";
            this.lbl_Scope.Size = new System.Drawing.Size(41, 13);
            this.lbl_Scope.TabIndex = 105;
            this.lbl_Scope.Text = "Scope:";
            // 
            // edit_AccessTokenURI
            // 
            this.edit_AccessTokenURI.Location = new System.Drawing.Point(139, 204);
            this.edit_AccessTokenURI.Name = "edit_AccessTokenURI";
            this.edit_AccessTokenURI.Size = new System.Drawing.Size(379, 20);
            this.edit_AccessTokenURI.TabIndex = 80;
            // 
            // edit_ClientID
            // 
            this.edit_ClientID.Location = new System.Drawing.Point(139, 258);
            this.edit_ClientID.Name = "edit_ClientID";
            this.edit_ClientID.Size = new System.Drawing.Size(212, 20);
            this.edit_ClientID.TabIndex = 100;
            // 
            // edit_ClientSecret
            // 
            this.edit_ClientSecret.Location = new System.Drawing.Point(139, 285);
            this.edit_ClientSecret.Name = "edit_ClientSecret";
            this.edit_ClientSecret.Size = new System.Drawing.Size(212, 20);
            this.edit_ClientSecret.TabIndex = 110;
            // 
            // edit_Scope
            // 
            this.edit_Scope.Location = new System.Drawing.Point(139, 312);
            this.edit_Scope.Name = "edit_Scope";
            this.edit_Scope.Size = new System.Drawing.Size(212, 20);
            this.edit_Scope.TabIndex = 120;
            // 
            // btn_Login
            // 
            this.btn_Login.Location = new System.Drawing.Point(778, 10);
            this.btn_Login.Name = "btn_Login";
            this.btn_Login.Size = new System.Drawing.Size(135, 23);
            this.btn_Login.TabIndex = 16;
            this.btn_Login.Text = "Login";
            this.btn_Login.UseVisualStyleBackColor = true;
            this.btn_Login.Click += new System.EventHandler(this.Login_Click);
            // 
            // ViewMetadata
            // 
            this.ViewMetadata.Location = new System.Drawing.Point(544, 235);
            this.ViewMetadata.Name = "ViewMetadata";
            this.ViewMetadata.Size = new System.Drawing.Size(41, 23);
            this.ViewMetadata.TabIndex = 140;
            this.ViewMetadata.TabStop = false;
            this.ViewMetadata.Text = "View";
            this.ViewMetadata.UseVisualStyleBackColor = true;
            this.ViewMetadata.Click += new System.EventHandler(this.ViewMetadata_Click);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(541, 304);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(46, 13);
            this.label24.TabIndex = 139;
            this.label24.Text = "Service:";
            // 
            // serviceresponsedata
            // 
            this.serviceresponsedata.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serviceresponsedata.Location = new System.Drawing.Point(605, 301);
            this.serviceresponsedata.Multiline = true;
            this.serviceresponsedata.Name = "serviceresponsedata";
            this.serviceresponsedata.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.serviceresponsedata.Size = new System.Drawing.Size(308, 73);
            this.serviceresponsedata.TabIndex = 138;
            this.serviceresponsedata.TabStop = false;
            // 
            // webapi_metadata
            // 
            this.webapi_metadata.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webapi_metadata.Location = new System.Drawing.Point(605, 198);
            this.webapi_metadata.Multiline = true;
            this.webapi_metadata.Name = "webapi_metadata";
            this.webapi_metadata.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.webapi_metadata.Size = new System.Drawing.Size(308, 97);
            this.webapi_metadata.TabIndex = 137;
            this.webapi_metadata.TabStop = false;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(541, 201);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(55, 13);
            this.label22.TabIndex = 136;
            this.label22.Text = "Metadata:";
            // 
            // webapi_token
            // 
            this.webapi_token.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webapi_token.Location = new System.Drawing.Point(605, 99);
            this.webapi_token.Multiline = true;
            this.webapi_token.Name = "webapi_token";
            this.webapi_token.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.webapi_token.Size = new System.Drawing.Size(308, 91);
            this.webapi_token.TabIndex = 135;
            this.webapi_token.TabStop = false;
            // 
            // openid_code
            // 
            this.openid_code.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.openid_code.Location = new System.Drawing.Point(605, 69);
            this.openid_code.Name = "openid_code";
            this.openid_code.Size = new System.Drawing.Size(308, 20);
            this.openid_code.TabIndex = 134;
            this.openid_code.TabStop = false;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(555, 102);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(41, 13);
            this.label19.TabIndex = 133;
            this.label19.Text = "Token:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(561, 76);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(35, 13);
            this.label18.TabIndex = 132;
            this.label18.Text = "Code:";
            // 
            // scriptfile
            // 
            this.scriptfile.Location = new System.Drawing.Point(140, 388);
            this.scriptfile.Name = "scriptfile";
            this.scriptfile.Size = new System.Drawing.Size(470, 20);
            this.scriptfile.TabIndex = 13;
            this.scriptfile.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(61, 392);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 141;
            this.label2.Text = "Test Script";
            // 
            // btn_LoadTestScript
            // 
            this.btn_LoadTestScript.Location = new System.Drawing.Point(629, 387);
            this.btn_LoadTestScript.Name = "btn_LoadTestScript";
            this.btn_LoadTestScript.Size = new System.Drawing.Size(143, 23);
            this.btn_LoadTestScript.TabIndex = 130;
            this.btn_LoadTestScript.Tag = "Select Log Directory";
            this.btn_LoadTestScript.Text = "Load Test Script";
            this.btn_LoadTestScript.UseVisualStyleBackColor = true;
            this.btn_LoadTestScript.Click += new System.EventHandler(this.LoadTestScript_Click);
            // 
            // edit_BearerToken
            // 
            this.edit_BearerToken.Location = new System.Drawing.Point(140, 125);
            this.edit_BearerToken.Name = "edit_BearerToken";
            this.edit_BearerToken.Size = new System.Drawing.Size(380, 20);
            this.edit_BearerToken.TabIndex = 50;
            this.edit_BearerToken.Visible = false;
            // 
            // lbl_BearerToken
            // 
            this.lbl_BearerToken.AutoSize = true;
            this.lbl_BearerToken.Location = new System.Drawing.Point(56, 130);
            this.lbl_BearerToken.Name = "lbl_BearerToken";
            this.lbl_BearerToken.Size = new System.Drawing.Size(75, 13);
            this.lbl_BearerToken.TabIndex = 151;
            this.lbl_BearerToken.Text = "Bearer Token:";
            this.lbl_BearerToken.Visible = false;
            // 
            // btn_RunValidationTest
            // 
            this.btn_RunValidationTest.Location = new System.Drawing.Point(778, 445);
            this.btn_RunValidationTest.Name = "btn_RunValidationTest";
            this.btn_RunValidationTest.Size = new System.Drawing.Size(143, 23);
            this.btn_RunValidationTest.TabIndex = 152;
            this.btn_RunValidationTest.Tag = "Select Log Directory";
            this.btn_RunValidationTest.Text = "Run Validation Test";
            this.btn_RunValidationTest.UseVisualStyleBackColor = true;
            this.btn_RunValidationTest.Click += new System.EventHandler(this.ValidationTest_Click);
            // 
            // ReferenceClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(946, 490);
            this.Controls.Add(this.btn_RunValidationTest);
            this.Controls.Add(this.btn_LoadTestScript);
            this.Controls.Add(this.scriptfile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ViewMetadata);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.serviceresponsedata);
            this.Controls.Add(this.webapi_metadata);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.webapi_token);
            this.Controls.Add(this.openid_code);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.preauthenticate);
            this.Controls.Add(this.btn_ClearSettings);
            this.Controls.Add(this.btn_ResetSession);
            this.Controls.Add(this.btn_ExecuteTestScript);
            this.Controls.Add(this.lbl_Password);
            this.Controls.Add(this.lbl_RedirectURI);
            this.Controls.Add(this.edit_RedirectURI);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.lbl_AuthorizationURI);
            this.Controls.Add(this.edit_AuthorizationURI);
            this.Controls.Add(this.edit_Password);
            this.Controls.Add(this.oauth_granttype);
            this.Controls.Add(this.lbl_WebAPIEndPointURI);
            this.Controls.Add(this.edit_WebAPIEndPointURI);
            this.Controls.Add(this.btn_SaveClientSettings);
            this.Controls.Add(this.btn_SelectClientSettings);
            this.Controls.Add(this.lbl_AccessTokenURI);
            this.Controls.Add(this.lbl_ClientID);
            this.Controls.Add(this.lbl_ClientSecret);
            this.Controls.Add(this.lbl_Scope);
            this.Controls.Add(this.edit_AccessTokenURI);
            this.Controls.Add(this.edit_ClientID);
            this.Controls.Add(this.edit_ClientSecret);
            this.Controls.Add(this.edit_Scope);
            this.Controls.Add(this.btn_Login);
            this.Controls.Add(this.webapicurrentrule);
            this.Controls.Add(this.webapiprogressBar);
            this.Controls.Add(this.ServerVersion);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btn_SelectResultsDirectory);
            this.Controls.Add(this.ResultsDirectory);
            this.Controls.Add(this.LogDirectory);
            this.Controls.Add(this.btn_SelectLogDirectory);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.lbl_UserName);
            this.Controls.Add(this.edit_UserName);
            this.Controls.Add(this.edit_BearerToken);
            this.Controls.Add(this.lbl_BearerToken);
            this.Name = "ReferenceClient";
            this.Text = "RESO Reference Client v3.1.11";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_SelectResultsDirectory;
        private System.Windows.Forms.TextBox ResultsDirectory;
        private System.Windows.Forms.TextBox LogDirectory;
        private System.Windows.Forms.Button btn_SelectLogDirectory;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label webapicurrentrule;
        private System.Windows.Forms.ProgressBar webapiprogressBar;
        private System.Windows.Forms.ComboBox ServerVersion;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox preauthenticate;
        private System.Windows.Forms.Button btn_ClearSettings;
        private System.Windows.Forms.Button btn_ResetSession;
        private System.Windows.Forms.Button btn_ExecuteTestScript;
        private System.Windows.Forms.Label lbl_Password;
        private System.Windows.Forms.Label lbl_UserName;
        private System.Windows.Forms.Label lbl_RedirectURI;
        private System.Windows.Forms.TextBox edit_RedirectURI;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label lbl_AuthorizationURI;
        private System.Windows.Forms.TextBox edit_AuthorizationURI;
        private System.Windows.Forms.TextBox edit_Password;
        private System.Windows.Forms.TextBox edit_UserName;
        private System.Windows.Forms.ComboBox oauth_granttype;
        private System.Windows.Forms.Label lbl_WebAPIEndPointURI;
        private System.Windows.Forms.TextBox edit_WebAPIEndPointURI;
        private System.Windows.Forms.Button btn_SaveClientSettings;
        private System.Windows.Forms.Button btn_SelectClientSettings;
        private System.Windows.Forms.Label lbl_AccessTokenURI;
        private System.Windows.Forms.Label lbl_ClientID;
        private System.Windows.Forms.Label lbl_ClientSecret;
        private System.Windows.Forms.Label lbl_Scope;
        private System.Windows.Forms.TextBox edit_AccessTokenURI;
        private System.Windows.Forms.TextBox edit_ClientID;
        private System.Windows.Forms.TextBox edit_ClientSecret;
        private System.Windows.Forms.TextBox edit_Scope;
        private System.Windows.Forms.Button btn_Login;
        private System.Windows.Forms.Button ViewMetadata;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox serviceresponsedata;
        private System.Windows.Forms.TextBox webapi_metadata;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox webapi_token;
        private System.Windows.Forms.TextBox openid_code;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox scriptfile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_LoadTestScript;
        private System.Windows.Forms.TextBox edit_BearerToken;
        private System.Windows.Forms.Label lbl_BearerToken;
        private System.Windows.Forms.Button btn_RunValidationTest;
    }
}

