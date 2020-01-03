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
    /// Class of service implemenation feature to request the metadata document.
    /// </summary>
    [Export(typeof(ExtensionRule))]
    public class ServiceImpl_MetadataDocumentRequest : ServiceImplExtensionRule
    {
        /// <summary>
        /// Gets the service implementation feature name
        /// </summary>
        public override string Name
        {
            get
            {
                return "ServiceImpl_MetadataDocumentRequest";
            }
        }

        /// <summary>
        /// Gets the service implementation feature description
        /// </summary>
        public override string Description
        {
            get
            {
                return this.CategoryInfo.CategoryFullName + ",Metadata Document Request";
            }
        }

        /// <summary>
        /// Gets the service implementation feature specification in OData document
        /// </summary>
        public override string V4SpecificationSection
        {
            get
            {
                return "11.1.2";
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
                return new ServiceImplCategory(ServiceImplCategoryName.MetadataRequests, null);
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
            //var svcStatus = ServiceStatus.GetInstance();
            var svcStatus = ServiceStatus.GetInstance(context.ServiceBaseUri.OriginalString, context.ResponseHttpHeaders.Replace("\r\n",";"));

            string url = svcStatus.RootURL.TrimEnd('/') + @"/$metadata";
            var detail = new ExtensionRuleResultDetail(this.Name, url, HttpMethod.Get, string.Empty);
            info = new ExtensionRuleViolationInfo(new Uri(url), string.Empty, detail);
            
            if(!string.IsNullOrEmpty(context.MetadataDocument))
            {
                passed = true;
            }
            else
            {
                detail.ErrorMessage = "The Metadata docuemnt is empty";
                passed = false;
            }

            return passed;
        }
    }
}
