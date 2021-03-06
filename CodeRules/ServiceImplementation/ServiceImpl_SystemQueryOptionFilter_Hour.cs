﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace ODataValidator.Rule
{
    #region Namespace.
    using System;
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
    public class ServiceImpl_SystemQueryOptionFilter_Hour : ServiceImplExtensionRule
    {
        /// <summary>
        /// Gets the service implementation feature name
        /// </summary>
        public override string Name
        {
            get
            {
                return "ServiceImpl_SystemQueryOptionFilter_Hour";
            }
        }

        /// <summary>
        /// Gets the service implementation feature description
        /// </summary>
        public override string Description
        {
            get
            {
                return this.CategoryInfo.CategoryFullName + ",$filter(hour)";
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
            var propTypes = new string[1] { "Edm.DateTimeOffset" };
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
            if (null != resp && HttpStatusCode.OK == resp.StatusCode)
            {
                var settings = new JsonSerializerSettings();
                settings.DateParseHandling = DateParseHandling.None;
                JObject jObj = JsonConvert.DeserializeObject(resp.ResponsePayload, settings) as JObject;
                JArray jArr = jObj.GetValue(Constants.Value) as JArray;
                var entity = jArr.First as JObject;
                string propVal = string.Empty;
                //Need to find one that has a value.  Previously only looked at the first attribute.
                for(int i=0;i< propNames.Count - 1;i++)
                {
                    try
                    {
                        propVal = entity[propNames[i].ToString()].ToString();
                    }
                    catch
                    {

                    }
                    if (!string.IsNullOrEmpty(propVal))
                    { 
                        propName = propNames[i].ToString();
                        break;
                    }
                }
                int index = propVal.IndexOf('T');
                if (index > 0)
                {
                    propVal = propVal.Substring(index + 1, 2);
                    url = string.Format("{0}?$filter=hour({1}) eq {2}", url, propName, propVal);
                    resp = WebHelper.Get(new Uri(url), string.Empty, RuleEngineSetting.Instance().DefaultMaximumPayloadSize, svcStatus.DefaultHeaders);
                    var detail = new ExtensionRuleResultDetail(this.Name, url, HttpMethod.Get, string.Empty);
                    detail.URI = url;
                    detail.ResponsePayload = resp.ResponsePayload;
                    detail.ResponseHeaders = resp.ResponseHeaders;
                    detail.HTTPMethod = "GET";
                    detail.ResponseStatusCode = resp.StatusCode.ToString();

                    info = new ExtensionRuleViolationInfo(new Uri(url), string.Empty, detail);
                    if (null != resp && HttpStatusCode.OK == resp.StatusCode)
                    {
                        jObj = JsonConvert.DeserializeObject(resp.ResponsePayload, settings) as JObject;
                        jArr = jObj.GetValue(Constants.Value) as JArray;
                        if (null == jArr || !jArr.Any())
                        {
                            return false;
                        }

                        foreach (JObject et in jArr)
                        {
                            passed = et[propName].ToString().Substring(index + 1, 2) == propVal;
                            if(passed == false)
                            {
                                detail.ErrorMessage = et[propName].ToString() + " Substring(index + 1, 2) not equal to " + propVal;
                                break;
                            }
                        }
                    }
                    else
                    {
                        detail.ErrorMessage = "The server returned an error response:  " + detail.ResponseStatusCode;
                        passed = false;
                    }
                }
                else
                {
                    var detail2 = new ExtensionRuleResultDetail(this.Name);
                    detail2.ErrorMessage = "The demiliter T is not in the date response:  " + propVal;
                    info = new ExtensionRuleViolationInfo(context.ServiceBaseUri, string.Empty, detail2);
                    
                    passed = false;
                }
            }

            return passed;
        }
    }
}
