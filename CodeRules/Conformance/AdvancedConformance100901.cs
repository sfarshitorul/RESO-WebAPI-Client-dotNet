// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace ODataValidator.Rule
{
    #region Namespaces
    using System;
    using System.Net;
    using System.Linq;
    using System.ComponentModel.Composition;
    using Newtonsoft.Json.Linq;
    using ODataValidator.Rule.Helper;
    using ODataValidator.RuleEngine;
    
    #endregion

    /// <summary>
    /// Class of extension rule for Advanced.Conformance.100901
    /// </summary>
    [Export(typeof(ExtensionRule))]
    public class AdvancedConformance100901 : ConformanceAdvancedExtensionRule
    {
        /// <summary>
        /// Gets rule name
        /// </summary>
        public override string Name
        {
            get
            {
                return "Advanced.Conformance.100901";
            }
        }

        /// <summary>
        /// Gets rule description
        /// </summary>
        public override string Description
        {
            get
            {
                return "9.1. MUST support returning references for expanded properties (section 11.2.4.2)";
            }
        }

        /// <summary>
        /// Gets rule specification in OData document
        /// </summary>
        public override string V4SpecificationSection
        {
            get
            {
                return "13.1.3";
            }
        }

        /// <summary>
        /// Verifies the extension rule.
        /// </summary>
        /// <param name="context">The Interop service context</param>
        /// <param name="info">out parameter to return violation information when rule does not pass</param>
        /// <returns>true if rule passes; false otherwise</returns>
        public override bool? Verify(ServiceContext context, out ExtensionRuleViolationInfo info)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            bool? passed = null;
            ExtensionRuleResultDetail detail = new ExtensionRuleResultDetail(this.Name);
            var expandRestrictions = AnnotationsHelper.GetExpandRestrictions(context.MetadataDocument, context.VocCapabilities);

            if (string.IsNullOrEmpty(expandRestrictions.Item1) ||
                null == expandRestrictions.Item3 || !expandRestrictions.Item3.Any())
            {
                detail.ErrorMessage = "Cannot find an appropriate entity-set which supports $expand system query options.";
                info = new ExtensionRuleViolationInfo(context.Destination, context.ResponsePayload, detail);

                return passed;
            }

            string entitySet = expandRestrictions.Item1;
            string navigProp = expandRestrictions.Item3.First().NavigationPropertyName;
            string url = string.Format("{0}/{1}", context.ServiceBaseUri.OriginalString.TrimEnd('/'), entitySet);
            var resp = WebHelper.Get(new Uri(url), Constants.V4AcceptHeaderJsonFullMetadata, RuleEngineSetting.Instance().DefaultMaximumPayloadSize, context.RequestHeaders);
            detail.URI = url;
            detail.ResponsePayload = resp.ResponsePayload;
            detail.ResponseHeaders = resp.ResponseHeaders;
            detail.HTTPMethod = "GET";
            detail.ResponseStatusCode = resp.StatusCode.ToString();


            if (null == resp || HttpStatusCode.OK != resp.StatusCode)
            {
                detail.ErrorMessage = JsonParserHelper.GetErrorMessage(resp.ResponsePayload);
                info = new ExtensionRuleViolationInfo(new Uri(url), resp.ResponsePayload, detail);

                return passed;
            }

            JObject feed;
            resp.ResponsePayload.TryToJObject(out feed);
            var entities = JsonParserHelper.GetEntriesThatHaveAValue(feed);


            if (null == entities || 0 == entities.Count)
            {
                detail.ErrorMessage = string.Format("The entity-set {0} has no entity.", entitySet);
                info = new ExtensionRuleViolationInfo(new Uri(url), resp.ResponsePayload, detail);

                return passed;
            }

            var entity = entities.First;
            string entityURL = null != entity[Constants.V4OdataId] ? entity[Constants.V4OdataId].ToString() : string.Empty;

            if (string.IsNullOrEmpty(entityURL))
            {
                detail.ErrorMessage = "Cannot find the annotation @odata.id in the current entity.";
                info = new ExtensionRuleViolationInfo(new Uri(url), resp.ResponsePayload, detail);

                return passed;
            }

            url = string.Format("{0}?$expand={1}/$ref", entityURL, navigProp);
            if(url.IndexOf(context.ServiceBaseUri.OriginalString) < 0)
            {
                url = context.ServiceBaseUri.OriginalString + url;
            }
            resp = WebHelper.Get(new Uri(url), Constants.AcceptHeaderJson, RuleEngineSetting.Instance().DefaultMaximumPayloadSize, context.RequestHeaders);
            detail = new ExtensionRuleResultDetail(this.Name, url, "GET", StringHelper.MergeHeaders(Constants.AcceptHeaderJson, context.RequestHeaders), resp);
            detail.URI = url;
            detail.ResponsePayload = resp.ResponsePayload;
            detail.ResponseHeaders = resp.ResponseHeaders;
            detail.HTTPMethod = "GET";
            detail.ResponseStatusCode = resp.StatusCode.ToString();
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                JObject entry;
                resp.ResponsePayload.TryToJObject(out entry);

                if (entry != null && JTokenType.Object == entry.Type)
                {
                    bool entity_error = false;
                    try
                    {
                        entity = entry[navigProp].First;
                    }
                    catch
                    {
                        entity_error = true;
                        passed = false;
                        detail.ErrorMessage = "The service does not execute an accurate result, because the value of the Navigation ("+ navigProp+") does not exist in the entity.  Request attempted: " + url +" Response:  "+ resp.ResponsePayload;
                    }
                    if (!entity_error)
                    {
                        try
                        {
                            var test = entity;
                            var test1 = entity[Constants.V4OdataId];
                            url = entity[Constants.V4OdataId].ToString();
                        }
                        catch(Exception ex)
                        {
                            entity_error = true;
                            passed = false;
                            detail.ErrorMessage = ex.Message + " Attempt to access:  " + Constants.V4OdataId + " Response:  " + resp.ResponsePayload;

                        }
                        if (!entity_error)
                        {
                            if (url.IndexOf(context.ServiceBaseUri.OriginalString) < 0)
                            {
                                url = context.ServiceBaseUri.OriginalString + url;
                            }
                            Uri testurl = null;
                            try
                            {
                                testurl = new Uri(url);

                            }
                            catch
                            {

                            }

                            if (testurl != null)
                            {
                                resp = WebHelper.Get(new Uri(url), Constants.AcceptHeaderJson, RuleEngineSetting.Instance().DefaultMaximumPayloadSize, context.RequestHeaders);
                                detail.URI = url;
                                detail.ResponsePayload = resp.ResponsePayload;
                                detail.ResponseHeaders = resp.ResponseHeaders;
                                detail.HTTPMethod = "GET";
                                detail.ResponseStatusCode = resp.StatusCode.ToString();
                                if (resp.StatusCode == HttpStatusCode.OK)
                                {
                                    passed = true;
                                }
                                else
                                {
                                    passed = false;
                                    detail.ErrorMessage = "The service does not execute an accurate result, because the value of the annotation '@odata.id' (" + url + ") is a bad link.";
                                }
                            }
                            else
                            {
                                passed = false;
                                detail.ErrorMessage = "The service does not execute an accurate result, because the value of the annotation '@odata.id' (" + url + ") is a bad link.";
                            }
                        }
                    }
                }
            }
            else
            {
                passed = false;
                detail.ErrorMessage = "The service does not support the '$ref' segment for expanded properties.";
            }

            info = new ExtensionRuleViolationInfo(context.Destination, context.ResponsePayload, detail);
            return passed;
        }
    }
}
