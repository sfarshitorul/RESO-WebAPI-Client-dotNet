﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace ODataValidator.Rule
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Net;
    using Newtonsoft.Json.Linq;
    using ODataValidator.Rule.Helper;
    using ODataValidator.RuleEngine;
    
    #endregion

    /// <summary>
    /// Class of extension rule for Minimal.Conformance.1014
    /// </summary>
    [Export(typeof(ExtensionRule))]
    public class MinimalConformance1014 : ConformanceMinimalExtensionRule
    {
        /// <summary>
        /// Gets rule name
        /// </summary>
        public override string Name
        {
            get
            {
                return "Minimal.Conformance.1014";
            }
        }

        /// <summary>
        /// Gets rule description
        /// </summary>
        public override string Description
        {
            get
            {
                return "14. MUST include edit links (explicitly or implicitly) for all updatable or deletable resources according to [OData-Atom] and [OData-JSON]";
            }
        }

        /// <summary>
        /// Gets rule specification in OData document
        /// </summary>
        public override string V4SpecificationSection
        {
            get
            {
                return "13.1.1";
            }
        }

        /// <summary>
        /// Gets the resource type to which the rule applies.
        /// </summary>
        public override ConformanceServiceType? ResourceType
        {
            get
            {
                return ConformanceServiceType.ReadWrite;
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
            info = null;
            ServiceStatus serviceStatus = ServiceStatus.GetInstance();
            TermDocuments termDocs = TermDocuments.GetInstance();
            DataFactory dFactory = DataFactory.Instance();
            ExtensionRuleResultDetail detail1 = new ExtensionRuleResultDetail(this.Name);
            ExtensionRuleResultDetail detail2 = new ExtensionRuleResultDetail(this.Name);
            List<string> keyPropertyTypes = new List<string>() { "Edm.Int32", "Edm.Int16", "Edm.Int64", "Edm.Guid", "Edm.String" };
            List<EntityTypeElement> entityTypeElements = MetadataHelper.GetEntityTypes(serviceStatus.MetadataDocument, 1, keyPropertyTypes, null, NavigationRoughType.None).ToList();

            if (null == entityTypeElements || 0 == entityTypeElements.Count())
            {
                detail1.ErrorMessage = "To verify this rule it expects an entity type with Int32/Int64/Int16/Guid/String key property, but there is no this entity type in metadata so can not verify this rule.";
                info = new ExtensionRuleViolationInfo(new Uri(serviceStatus.RootURL), serviceStatus.ServiceDocument, detail1);

                return passed;
            }

            // To get test entity which is deleteable and insertable
            EntityTypeElement entityType = null;
            foreach (var entityEle in entityTypeElements)
            {
                var matchEntity = entityEle.EntitySetName.GetRestrictions(serviceStatus.MetadataDocument, termDocs.VocCapabilitiesDoc,
                    new List<Func<string, string, string, List<NormalProperty>, List<NavigProperty>, bool>>()
                    {
                        AnnotationsHelper.GetInsertRestrictions, AnnotationsHelper.GetDeleteRestrictions
                    });

                if (!string.IsNullOrEmpty(matchEntity.Item1)
                     && matchEntity.Item2 != null && matchEntity.Item2.Any()
                     && matchEntity.Item3 != null && matchEntity.Item3.Any())
                {
                    entityType = entityEle;
                    break;
                }
            }

            if (null == entityType)
            {
                detail1.ErrorMessage = "To verify this rule it expects an entity type with deleteable and insertable restrictions, but there is no entity type in metadata which is insertable and deleteable.";
                info = new ExtensionRuleViolationInfo(new Uri(serviceStatus.RootURL), serviceStatus.ServiceDocument, detail1);

                return passed;
            }

            string entitySetUrl = entityType.EntitySetName.MapEntitySetNameToEntitySetURL();
            if (string.IsNullOrEmpty(entitySetUrl))
            {
                detail1.ErrorMessage = string.Format("Cannot find the entity-set URL which is matched with {0}.", entityType.EntityTypeShortName);
                info = new ExtensionRuleViolationInfo(new Uri(serviceStatus.RootURL), serviceStatus.ServiceDocument, detail1);

                return passed;
            }

            string url = serviceStatus.RootURL.TrimEnd('/') + @"/" + entitySetUrl;
            var additionalInfos = new List<AdditionalInfo>();
            var reqData = dFactory.ConstructInsertedEntityData(entityType.EntitySetName, entityType.EntityTypeShortName, null, out additionalInfos);
            string reqDataStr = reqData.ToString();
            bool isMediaType = !string.IsNullOrEmpty(additionalInfos.Last().ODataMediaEtag);
            var resp = WebHelper.CreateEntity(url, context.RequestHeaders, reqData, isMediaType, ref additionalInfos);
            detail1 = new ExtensionRuleResultDetail(this.Name, url, HttpMethod.Post, string.Empty, resp, string.Empty, reqDataStr);

            // If create successfully, the resource is updatable and deletable.
            if (resp.StatusCode.HasValue && resp.StatusCode == HttpStatusCode.Created)
            {
                // Get feed response.
                resp = WebHelper.Get(new Uri(url), Constants.V4AcceptHeaderJsonFullMetadata, RuleEngineSetting.Instance().DefaultMaximumPayloadSize, serviceStatus.DefaultHeaders);
                detail2 = new ExtensionRuleResultDetail(this.Name, url, HttpMethod.Get, StringHelper.MergeHeaders(Constants.V4AcceptHeaderJsonFullMetadata, serviceStatus.DefaultHeaders), resp);
                detail2.URI = url;
                detail2.ResponsePayload = resp.ResponsePayload;
                detail2.ResponseHeaders = resp.ResponseHeaders;
                detail2.HTTPMethod = "GET";
                detail2.ResponseStatusCode = resp.StatusCode.ToString();

                if (resp.StatusCode.HasValue && resp.StatusCode == HttpStatusCode.OK)
                {
                    JObject jo;
                    resp.ResponsePayload.TryToJObject(out jo);
                    var entries = JsonParserHelper.GetEntries(jo);
                    foreach (JObject entry in entries)
                    {
                        if (entry[Constants.V4OdataEditLink] == null)
                        {
                            passed = false;
                            detail2.ErrorMessage = "Not all entities from above URI contain edit links.";
                            break;
                        }
                    }

                    if (passed == null)
                    {
                        passed = true;
                    }
                }
                else
                {
                    passed = false;
                    detail2.ErrorMessage = "Get feed resource failed from above URI.";
                }

                // Restore the service.
                var resps = WebHelper.DeleteEntities(context.RequestHeaders, additionalInfos);
            }
            else
            {
                passed = null;
                detail1.ErrorMessage = "Created new entity failed, it is not an updatable resource and can not verify this rule.";
            }

            var details = new List<ExtensionRuleResultDetail>() { detail1, detail2 }.RemoveNullableDetails();
            info = new ExtensionRuleViolationInfo(new Uri(serviceStatus.RootURL), serviceStatus.ServiceDocument, details);

            return passed;
        }
    }
}
