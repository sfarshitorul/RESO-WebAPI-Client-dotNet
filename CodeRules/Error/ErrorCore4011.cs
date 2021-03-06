﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace ODataValidator.Rule
{
    #region Namespaces
    using System;
    using System.ComponentModel.Composition;
    using Newtonsoft.Json.Linq;
    using ODataValidator.RuleEngine;
    
    #endregion

    /// <summary>
    /// Class of extension rule for Error.Core.4011
    /// </summary>
    [Export(typeof(ExtensionRule))]
    public class ErrorCore4011 : ExtensionRule
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
                return "Error.Core.4011";
            }
        }

        /// <summary>
        /// Gets rule description
        /// </summary>
        public override string Description
        {
            get
            {
                return @"The value for the details name/value pair MUST be an array of JSON objects that MAY contain a name/value pair for target.";
            }
        }

        /// <summary>
        /// Gets rule specification in OData document
        /// </summary>
        public override string SpecificationSection
        {
            get
            {
                return "19";
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public override ODataVersion? Version
        {
            get
            {
                return ODataVersion.V3_V4;
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
                return RequirementLevel.May;
            }
        }

        /// <summary>
        /// Gets the payload type to which the rule applies.
        /// </summary>
        public override PayloadType? PayloadType
        {
            get
            {
                return RuleEngine.PayloadType.Error;
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
        /// Gets the flag whether the rule requires metadata document
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
                return null;
            }
        }

        /// <summary>
        /// Verify Error.Core.4011
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

            JObject jObj;
            context.ResponsePayload.TryToJObject(out jObj);
            string errorBodyName = context.Version == ODataVersion.V4 ? Constants.V4JsonLightErrorResponseIdentity : Constants.V3JsonLightErrorResponseIdentity;

            if (jObj != null && jObj.Type == JTokenType.Object)
            {
                var jProps = jObj.Children();

                foreach (JProperty jProp in jProps)
                {
                    if (jProp.Name == errorBodyName && jProp.Value.Type == JTokenType.Object)
                    {
                        var jPs = jProp.Value.Children();

                        foreach (JProperty jP in jPs)
                        {
                            if (jP.Name == @"details" && jP.Value.Type == JTokenType.Array)
                            {
                                JArray jArr = jP.Value as JArray;

                                foreach (var jo in jArr)
                                {
                                    if (typeof(JObject) == jo.GetType())
                                    {
                                        passed = null;
                                        var contents = jo.Children();

                                        foreach (JProperty c in contents)
                                        {
                                            if (c.Name == @"target")
                                            {
                                                passed = true;
                                                break;
                                            }
                                        }

                                        if (passed == null)
                                        {
                                            passed = false;
                                            info = new ExtensionRuleViolationInfo(this.ErrorMessage, context.Destination, context.ResponsePayload);
                                            break;
                                        }
                                    }
                                }

                                break;
                            }
                        }

                        break;
                    }
                }
            }

            return passed;
        }
    }
}
