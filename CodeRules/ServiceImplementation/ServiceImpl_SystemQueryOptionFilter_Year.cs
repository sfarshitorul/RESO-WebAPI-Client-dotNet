﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace ODataValidator.Rule
{
    #region Namespace.
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Net;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using ODataValidator.Rule.Helper;
    using ODataValidator.RuleEngine;
    
    #endregion

    /// <summary>
    /// Class of service implemenation feature to verify .
    /// </summary>
    [Export(typeof(ExtensionRule))]
    public class ServiceImpl_SystemQueryOptionFilter_Year : ServiceImplExtensionRule
    {
        /// <summary>
        /// Gets the service implementation feature name
        /// </summary>
        public override string Name
        {
            get
            {
                return "ServiceImpl_SystemQueryOptionFilter_Year";
            }
        }

        /// <summary>
        /// Gets the service implementation feature description
        /// </summary>
        public override string Description
        {
            get
            {
                return this.CategoryInfo.CategoryFullName + ",$filter(year)";
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
                parent = new ServiceImplCategory(ServiceImplCategoryName.SystemQueryOption, parent);

                return new ServiceImplCategory(ServiceImplCategoryName.ArithmeticOperators, parent);
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
            var propTypes = new string[2] { "Edm.Date", "Edm.DateTimeOffset" };
            var propNames = MetadataHelper.GetPropertyNames(propTypes, out entityTypeShortName);
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
            var detail = new ExtensionRuleResultDetail(this.Name, url, HttpMethod.Get, string.Empty);
            detail.URI = url;
            detail.ResponsePayload = resp.ResponsePayload;
            detail.ResponseHeaders = resp.ResponseHeaders;
            detail.HTTPMethod = "GET";
            detail.ResponseStatusCode = resp.StatusCode.ToString();

            if (null != resp && HttpStatusCode.OK == resp.StatusCode)
            {
                var settings = new JsonSerializerSettings();
                settings.DateParseHandling = DateParseHandling.None;
                JObject jObj = JsonConvert.DeserializeObject(resp.ResponsePayload, settings) as JObject;
                JArray jArr = jObj.GetValue(Constants.Value) as JArray;
                var entity = jArr.First as JObject;

                if(entity[propName] == null)
                {

                    detail.ErrorMessage = "The Attribute (" + propName + ") does not exist in the entity response:  " + entity.ToString();
                    info = new ExtensionRuleViolationInfo(new Uri(url), string.Empty, detail);
                    passed = false;
                    return passed;
                }
                var propVal = entity[propName].ToString();
                int index = propVal.IndexOf('-');
                if (index >= 0)
                {


                    propVal = propVal.Substring(0, index);
                    url = string.Format("{0}?$filter=year({1}) eq {2}", url, propName, propVal);
                    resp = WebHelper.Get(new Uri(url), string.Empty, RuleEngineSetting.Instance().DefaultMaximumPayloadSize, svcStatus.DefaultHeaders);
                    var detail1 = new ExtensionRuleResultDetail(this.Name, url, HttpMethod.Get, string.Empty);
                    detail1.URI = url;
                    detail1.ResponsePayload = resp.ResponsePayload;
                    detail1.ResponseHeaders = resp.ResponseHeaders;
                    detail1.HTTPMethod = "GET";
                    detail1.ResponseStatusCode = resp.StatusCode.ToString();

                    info = new ExtensionRuleViolationInfo(new Uri(url), string.Empty, detail1);
                    if (null != resp && HttpStatusCode.OK == resp.StatusCode)
                    {
                        jObj = JsonConvert.DeserializeObject(resp.ResponsePayload, settings) as JObject;
                        jArr = jObj.GetValue(Constants.Value) as JArray;
                        foreach (JObject et in jArr)
                        {
                            passed = et[propName].ToString().Substring(0, index) == propVal;
                        }
                    }
                    else
                    {
                        passed = false;
                    }
                }
                else
                {
                    passed = false;
                }
            }
            
            return passed;
        }
    }
}
