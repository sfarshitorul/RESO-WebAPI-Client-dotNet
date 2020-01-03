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
    public class ServiceImpl_SystemQueryOptionFilter_Fractionalseconds : ServiceImplExtensionRule
    {
        /// <summary>
        /// Gets the service implementation feature name
        /// </summary>
        public override string Name
        {
            get
            {
                return "ServiceImpl_SystemQueryOptionFilter_Fractionalseconds";
            }
        }

        /// <summary>
        /// Gets the service implementation feature description
        /// </summary>
        public override string Description
        {
            get
            {
                return this.CategoryInfo.CategoryFullName + ",$filter(fractionalseconds)";
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
                JObject jObj = JObject.Parse(resp.ResponsePayload);
                JArray jArr = jObj.GetValue(Constants.Value) as JArray;
                var entity = jArr.First as JObject;


                var propVal = Convert.ToDecimal(- 1.000000001);

                for (int i = 0; i < propNames.Count - 1; i++)
                {
                    try
                    {
                        if (entity[propNames[i]] != null)
                        {
                            propVal = Convert.ToDecimal(Convert.ToDateTime(entity[propNames[i].ToString()]).Millisecond) / 100.0m;
                        }
                    }
                    catch
                    {

                    }
                    if (propVal >= 0)
                    {
                        propName = propNames[i].ToString();
                        break;
                    }
                }
                if (propVal < 0)
                {
                    passed = false;
                }
                else
                {
                    url = string.Format("{0}?$filter=fractionalseconds({1}) eq {2}", url, propName, propVal);
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
                        jObj = JObject.Parse(resp.ResponsePayload);
                        jArr = jObj.GetValue(Constants.Value) as JArray;
                        foreach (JObject et in jArr)
                        {
                            if (entity[propName] != null)
                            {
                                passed = Convert.ToDecimal(Convert.ToDateTime(entity[propName]).Millisecond) / 100.0m == propVal;
                                if(passed == false)
                                {
                                    detail.ErrorMessage = "This test failed:  Convert.ToDecimal(Convert.ToDateTime(entity[propName]).Millisecond) / 100.0m == propVal where propval is " + propVal;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        detail.ErrorMessage = "The server returned an error response:  " + detail.ResponseStatusCode;
                        passed = false;
                    }
                }
            }
            
            return passed;
        }
    }
}
