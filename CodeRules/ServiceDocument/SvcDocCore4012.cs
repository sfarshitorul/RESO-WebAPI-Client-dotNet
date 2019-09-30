﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace ODataValidator.Rule
{
    #region Namespace
    using System;
    using System.ComponentModel.Composition;
    using Newtonsoft.Json.Linq;
    using ODataValidator.RuleEngine;
    
    #endregion

    /// <summary>
    /// Class of extension rule for SvcDoc.Core.4012
    /// </summary>
    [Export(typeof(ExtensionRule))]
    public class SvcDocCore4012 : ExtensionRule
    {
        /// <summary>
        /// Gets Category property
        /// </summary>
        public override string Category
        {
            get
            {
                return "core";
            }
        }

        /// <summary>
        /// Gets rule name
        /// </summary>
        public override string Name
        {
            get
            {
                return "SvcDoc.Core.4012";
            }
        }

        /// <summary>
        /// Gets rule description
        /// </summary>
        public override string Description
        {
            get
            {
                return @"JSON objects representing a related service document MUST contain the kind name/value pair with a value of ServiceDocument.";
            }
        }

        /// <summary>
        /// Gets rule specification in OData document
        /// </summary>
        public override string V4SpecificationSection
        {
            get
            {
                return "5";
            }
        }

        /// <summary>
        /// Gets location of help information of the rule
        /// </summary>
        public override string HelpLink
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the error message for validation failure
        /// </summary>
        public override string ErrorMessage
        {
            get
            {
                return this.Description;
            }
        }

        /// <summary>
        /// Gets the requirement level.
        /// </summary>
        public override RequirementLevel RequirementLevel
        {
            get
            {
                return RequirementLevel.Must;
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public override ODataVersion? Version
        {
            get
            {
                return ODataVersion.V4;
            }
        }

        /// <summary>
        /// Gets the payload type to which the rule applies.
        /// </summary>
        public override PayloadType? PayloadType
        {
            get
            {
                return RuleEngine.PayloadType.ServiceDoc;
            }
        }

        /// <summary>
        /// Gets the payload format to which the rule applies.
        /// </summary>
        public override PayloadFormat? PayloadFormat
        {
            get
            {
                return RuleEngine.PayloadFormat.JsonLight;
            }
        }

        /// <summary>
        /// Gets the RequireMetadata property to which the rule applies.
        /// </summary>
        public override bool? RequireMetadata
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the flag whether this rule applies to offline context
        /// </summary>
        public override bool? IsOfflineContext
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Verify SvcDoc.Core.4012
        /// </summary>
        /// <param name="context">Service context</param>
        /// <param name="info">out parameter to return violation information when rule fail</param>
        /// <returns>true if rule passes; false otherwise</returns>
        public override bool? Verify(ServiceContext context, out ExtensionRuleViolationInfo info)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            bool? passed = null;
            info = null;
            JObject allobject;
            context.ResponsePayload.TryToJObject(out allobject);
            if (allobject == null)
            {
                return passed;
            }

            foreach (JObject val in (JArray)allobject[Constants.Value])
            {
                if (val[Constants.Kind] != null && val[Constants.Kind].Value<string>().ToString().StripOffDoubleQuotes().Equals("ServiceDocument"))
                {
                    string url = val[Constants.Url].Value<string>().ToString().StripOffDoubleQuotes();
                    Response response = WebHelper.Get(new Uri(url), Constants.AcceptHeaderJson, RuleEngineSetting.Instance().DefaultMaximumPayloadSize, context.RequestHeaders);

                    JObject jo;
                    response.ResponsePayload.TryToJObject(out jo);
                    var payloadFormat = response.ResponsePayload.GetFormatFromPayload();
                    var payloadType = ContextHelper.GetPayloadType(response.ResponsePayload, payloadFormat, response.ResponseHeaders);

                    if (payloadType != RuleEngine.PayloadType.ServiceDoc)
                    {
                        passed = false;
                        info = new ExtensionRuleViolationInfo(this.ErrorMessage, context.Destination, context.ResponsePayload);
                        break;
                    }
                    else
                    {
                        passed = true;
                    }
                }             
            }

            return passed;          
        }
    }
}