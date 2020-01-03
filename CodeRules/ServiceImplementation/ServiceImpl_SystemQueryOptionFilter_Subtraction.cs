﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace ODataValidator.Rule
{
    #region Namespace.
    using System;
    using System.ComponentModel.Composition;
    using System.Net;
    using Newtonsoft.Json.Linq;
    using ODataValidator.Rule.Helper;
    using ODataValidator.RuleEngine;
    
    #endregion

    /// <summary>
    /// Class of service implemenation feature to verify .
    /// </summary>
    [Export(typeof(ExtensionRule))]
    public class ServiceImpl_SystemQueryOptionFilter_Subtraction : ServiceImplExtensionRule
    {
        /// <summary>
        /// Gets the service implementation feature name
        /// </summary>
        public override string Name
        {
            get
            {
                return "ServiceImpl_SystemQueryOptionFilter_Subtraction";
            }
        }

        /// <summary>
        /// Gets the service implementation feature description
        /// </summary>
        public override string Description
        {
            get
            {
                return this.CategoryInfo.CategoryFullName + ",$filter(Subtraction)";
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
            var propNames = MetadataHelper.GetNumericPropertyNames(out entityTypeShortName);
            string propName = propNames[0];
            if (string.IsNullOrEmpty(entityTypeShortName) || string.IsNullOrEmpty(propName))
            {
                return passed;
            }

            string entitySetUrl = entityTypeShortName.MapEntityTypeShortNameToEntitySetURL();

            // Set the subtrahend as 5.0. It is only a testing number, so it can be any numbers.
            double subtrahend = 5.0;

            // Set the threshold as 1. It is only a testing number, so it can be any numbers.
            double threshold = 1.0;
            string url = string.Format("{0}/{1}?$filter={2} sub {3} gt {4}", svcStatus.RootURL.TrimEnd('/'), entitySetUrl, propName, subtrahend, threshold);
            var resp = WebHelper.Get(new Uri(url), Constants.V4AcceptHeaderJsonFullMetadata, RuleEngineSetting.Instance().DefaultMaximumPayloadSize, context.RequestHeaders);
            var detail = new ExtensionRuleResultDetail("ServiceImpl_SystemQueryOptionFilter_Subtraction", url, HttpMethod.Get, string.Empty);
            detail.URI = url;
            detail.ResponsePayload = resp.ResponsePayload;
            detail.ResponseHeaders = resp.ResponseHeaders;
            detail.HTTPMethod = "GET";
            detail.ResponseStatusCode = resp.StatusCode.ToString();

            info = new ExtensionRuleViolationInfo(new Uri(url), string.Empty, detail);
            if (null != resp && HttpStatusCode.OK == resp.StatusCode)
            {
                var jObj = JObject.Parse(resp.ResponsePayload);
                var jArr = jObj.GetValue(Constants.Value);
                passed = true;
                foreach (JObject entity in jArr)
                {
                    try
                    {
                        double val = Convert.ToDouble(entity[propName]);
                        if (val - subtrahend <= threshold)
                        {
                            detail.ErrorMessage = "Subtraction was done incorrectly.  ";
                            passed = false;
                            break;
                        }
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
            else
            {
                detail.ErrorMessage = "Response return an error message:  " + resp.StatusCode.ToString();
                passed = false;
            }

            
            return passed;
        }
    }
}
