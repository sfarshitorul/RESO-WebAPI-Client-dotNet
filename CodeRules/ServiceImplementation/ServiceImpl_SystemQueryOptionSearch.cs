﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace ODataValidator.Rule
{
    #region Namespace.
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Net;
    using Newtonsoft.Json.Linq;
    using ODataValidator.Rule.Helper;
    using ODataValidator.RuleEngine;
    
    #endregion

    /// <summary>
    /// Class of service implemenation feature to verify .
    /// </summary>
    [Export(typeof(ExtensionRule))]
    public class ServiceImpl_SystemQueryOptionSearch : ServiceImplExtensionRule
    {
        /// <summary>
        /// Gets the service implementation feature name
        /// </summary>
        public override string Name
        {
            get
            {
                return "ServiceImpl_SystemQueryOptionSearch";
            }
        }

        /// <summary>
        /// Gets the service implementation feature description
        /// </summary>
        public override string Description
        {
            get
            {
                return this.CategoryInfo.CategoryFullName + ",$search";
            }
        }

        /// <summary>
        /// Gets the service implementation feature specification in OData document
        /// </summary>
        public override string V4SpecificationSection
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// Gets the service implementation feature level.
        /// </summary>
        public override RequirementLevel RequirementLevel
        {
            get
            {
                return RequirementLevel.Must;
            }
        }

        /// <summary>
        /// Gets the service implementation category.
        /// </summary>
        public override ServiceImplCategory CategoryInfo
        {
            get
            {
                var parent = new ServiceImplCategory(ServiceImplCategoryName.RequestingData);

                return new ServiceImplCategory(ServiceImplCategoryName.SystemQueryOption, parent);
            }
        }

        /// <summary>
        /// Verifies the service implementation feature.
        /// </summary>
        /// <param name="context">The Interop service context</param>
        /// <param name="info">out parameter to return violation information when rule does not pass</param>
        /// <returns>true if the service implementation feature passes; false otherwise</returns>
        public override bool? Verify(ServiceContext context, out ExtensionRuleViolationInfo info)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            bool? passed = null;
            info = null;
            var svcStatus = ServiceStatus.GetInstance();
            string entityTypeShortName;
            var propNames = MetadataHelper.GetPropertyNames("Edm.String", out entityTypeShortName);
            if (null == propNames || !propNames.Any())
            {
                return passed;
            }

            string propName = propNames[0];
            var entitySetUrl = entityTypeShortName.GetAccessEntitySetURL();
            if (string.IsNullOrEmpty(entitySetUrl))
            {
                return passed;
            }

            string url = svcStatus.RootURL.TrimEnd('/') + "/" + entitySetUrl;
            var resp = WebHelper.Get(new Uri(url), string.Empty, RuleEngineSetting.Instance().DefaultMaximumPayloadSize, svcStatus.DefaultHeaders);
            if (null != resp && HttpStatusCode.OK == resp.StatusCode)
            {
                JObject jObj = JObject.Parse(resp.ResponsePayload);
                JArray jArr = jObj.GetValue("value") as JArray;
                var entity = jArr.First as JObject;
                var propVal = entity[propName].ToString();
                url = string.Format("{0}?$search={1}", url, propVal);
                resp = WebHelper.Get(new Uri(url), string.Empty, RuleEngineSetting.Instance().DefaultMaximumPayloadSize, svcStatus.DefaultHeaders);
                var detail = new ExtensionRuleResultDetail("ServiceImpl_SystemQueryOptionSearch", url, HttpMethod.Get, string.Empty);
                detail.URI = url;
                detail.ResponsePayload = resp.ResponsePayload;
                detail.ResponseHeaders = resp.ResponseHeaders;
                detail.HTTPMethod = "GET";
                detail.ResponseStatusCode = resp.StatusCode.ToString();

                info = new ExtensionRuleViolationInfo(new Uri(url), string.Empty, detail);
                if (null != resp && HttpStatusCode.OK == resp.StatusCode)
                {
                    jObj = JObject.Parse(resp.ResponsePayload);
                    jArr = jObj.GetValue("value") as JArray;
                    if (null == jArr || !jArr.Any())
                    {
                        return false;
                    }

                    entity = jArr.First as JObject;
                    passed = propVal == entity[propName].ToString();
                }
                else
                {
                    passed = false;
                }
            }
            else 
            {
                return passed;
            }

            return passed;
        }
    }
}
