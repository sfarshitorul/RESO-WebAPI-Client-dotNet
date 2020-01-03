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
    /// Class of extension rule for Minimal.Conformance.102601
    /// </summary>
    [Export(typeof(ExtensionRule))]
    public class MinimalConformance102601 : ConformanceMinimalExtensionRule
    {
        /// <summary>
        /// Gets rule name
        /// </summary>
        public override string Name
        {
            get
            {
                return "Minimal.Conformance.102601";
            }
        }

        /// <summary>
        /// Gets rule description
        /// </summary>
        public override string Description
        {
            get
            {
                return "1). SHOULD support PUT to an individual primitive (section 11.4.9.1)";
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
        /// Gets the requirement level.
        /// </summary>
        public override RequirementLevel RequirementLevel
        {
            get
            {
                return RequirementLevel.Should;
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
            var detail1 = new ExtensionRuleResultDetail(this.Name);
            var detail2 = new ExtensionRuleResultDetail(this.Name);
            var detail3 = new ExtensionRuleResultDetail(this.Name);
            var detail4 = new ExtensionRuleResultDetail(this.Name);
            List<string> keyPropertyTypes = new List<string>() { "Edm.Int32", "Edm.Int16", "Edm.Int64", "Edm.Guid", "Edm.String" };
            List<EntityTypeElement> entityTypeElements = MetadataHelper.GetEntityTypes(serviceStatus.MetadataDocument, 1, keyPropertyTypes, null, NavigationRoughType.SingleValued).ToList();
            if (null == entityTypeElements || 0 == entityTypeElements.Count())
            {
                detail1.ErrorMessage = "To verify this rule it expects an entity type with Int32/Int64/Int16/Guid/String key property, but there is no this entity type in metadata so can not verify this rule.";
                info = new ExtensionRuleViolationInfo(new Uri(serviceStatus.RootURL), serviceStatus.ServiceDocument, detail1);

                return passed;
            }

            EntityTypeElement entityType = null;
            foreach (var en in entityTypeElements)
            {
                var matchEntity = en.EntitySetName.GetRestrictions(serviceStatus.MetadataDocument, termDocs.VocCapabilitiesDoc,
                    new List<Func<string, string, string, List<NormalProperty>, List<NavigProperty>, bool>>()
                    {
                        AnnotationsHelper.GetDeleteRestrictions, AnnotationsHelper.GetInsertRestrictions, AnnotationsHelper.GetUpdateRestrictions
                    });

                if (!string.IsNullOrEmpty(matchEntity.Item1)
                     && matchEntity.Item2 != null && matchEntity.Item2.Any()
                     && matchEntity.Item3 != null && matchEntity.Item3.Any())
                {
                    entityType = en;
                    break;
                }
            }

            string entitySetUrl = entityType.EntitySetName.MapEntitySetNameToEntitySetURL();
            if (string.IsNullOrEmpty(entitySetUrl))
            {
                detail1.ErrorMessage = string.Format("Cannot find the entity-set URL which is matched with {0}", entityType.EntityTypeShortName);
                info = new ExtensionRuleViolationInfo(new Uri(serviceStatus.RootURL), serviceStatus.ServiceDocument, detail1);

                return passed;
            }

            // Create a entity
            string url = serviceStatus.RootURL.TrimEnd('/') + @"/" + entitySetUrl;
            var keyNames = entityType.KeyProperties;
            string keyName = keyNames.First().PropertyName;
            var additionalInfos = new List<AdditionalInfo>();
            var reqData = dFactory.ConstructInsertedEntityData(entityType.EntitySetName, entityType.EntityTypeShortName, null, out additionalInfos);
            string reqDataStr = reqData.ToString();
            bool isMediaType = !string.IsNullOrEmpty(additionalInfos.Last().ODataMediaEtag);
            var resp = WebHelper.CreateEntity(url, context.RequestHeaders, reqData, isMediaType, ref additionalInfos);
            detail1 = new ExtensionRuleResultDetail(this.Name, url, HttpMethod.Post, string.Empty, resp, string.Empty, reqDataStr);
            if (resp.StatusCode == HttpStatusCode.Created)
            {
                var entityId = additionalInfos.Last().EntityId;
                var hasEtag = additionalInfos.Last().HasEtag;

                // Get a individual property except key property
                string toUpdatePropertyName = string.Empty;
                List<string> properties = MetadataHelper.GetSortedPropertiesOfEntity(serviceStatus.MetadataDocument, entityType.EntityTypeShortName);
                foreach (string name in properties)
                {
                    if (!string.Equals(name.Split(',')[0], keyName))
                    {
                        if (name.Split(',')[1].Equals("Edm.String"))
                        {
                            toUpdatePropertyName = name.Split(',')[0];
                            break;
                        }
                    }
                }

                string individualProUrl = entityId + @"/" + toUpdatePropertyName;
                resp = WebHelper.Get(new Uri(individualProUrl), Constants.AcceptHeaderJson, RuleEngineSetting.Instance().DefaultMaximumPayloadSize, serviceStatus.DefaultHeaders);
                detail2 = new ExtensionRuleResultDetail(this.Name, individualProUrl, HttpMethod.Get, StringHelper.MergeHeaders(Constants.AcceptHeaderJson, serviceStatus.DefaultHeaders), resp);
                detail2.URI = url;
                detail2.ResponsePayload = resp.ResponsePayload;
                detail2.ResponseHeaders = resp.ResponseHeaders;
                detail2.HTTPMethod = "GET";
                detail2.ResponseStatusCode = resp.StatusCode.ToString();

                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    JObject jo;
                    resp.ResponsePayload.TryToJObject(out jo);
                    string valueOfIndividualProperty = jo[Constants.Value].ToString();
                    string newValue = valueOfIndividualProperty + "ToUpdate";
                    jo[Constants.Value] = newValue;

                    // Update the individual property
                    resp = WebHelper.UpdateEntity(individualProUrl, jo.ToString(), HttpMethod.Put);
                    detail3 = new ExtensionRuleResultDetail(this.Name, individualProUrl, HttpMethod.Put, string.Empty, resp, string.Empty, jo.ToString());
                    detail3.URI = individualProUrl;
                    detail3.ResponsePayload = resp.ResponsePayload;
                    detail3.ResponseHeaders = resp.ResponseHeaders;
                    detail3.HTTPMethod = "PUT";
                    detail3.RequestData = jo.ToString();
                    detail3.ResponseStatusCode = resp.StatusCode.ToString();

                    if (resp.StatusCode == HttpStatusCode.NoContent)
                    {
                        // Check whether the individual property is updated to new value
                        if (WebHelper.GetContent(individualProUrl, context.RequestHeaders, out resp))
                        {
                            detail4 = new ExtensionRuleResultDetail(this.Name, individualProUrl, HttpMethod.Get, string.Empty, resp);

                            resp.ResponsePayload.TryToJObject(out jo);

                            if (jo != null && jo[Constants.Value] != null && jo[Constants.Value].Value<string>().Equals(newValue))
                            {
                                passed = true;
                            }
                            else if (jo == null)
                            {
                                passed = false;
                                detail4.ErrorMessage = "Can not get individual property after PUT to it.";
                            }
                            else if (jo != null && jo[Constants.Value] == null)
                            {
                                passed = false;
                                detail4.ErrorMessage = "Can not get the value of individual property.";
                            }
                            else if (jo != null && jo[Constants.Value] != null && !jo[Constants.Value].Value<string>().Equals(newValue))
                            {
                                passed = false;
                                detail4.ErrorMessage = "The value of individual property is not updated.";
                            }
                        }
                    }
                    else
                    {
                        passed = false;
                        detail3.ErrorMessage = "Update individual property from the created entity failed.";
                    }
                }
                else
                {
                    passed = false;
                    detail2.ErrorMessage = "Get individual property from the created entity failed.";
                }

                // Delete the entity
                var resps = WebHelper.DeleteEntities(context.RequestHeaders, additionalInfos);
            }
            else
            {
                passed = false;
                detail1.ErrorMessage = "Created the new entity failed for above URI.";
            }

            var details = new List<ExtensionRuleResultDetail>() { detail1, detail2, detail3, detail4 }.RemoveNullableDetails();
            info = new ExtensionRuleViolationInfo(this.ErrorMessage, new Uri(serviceStatus.RootURL), serviceStatus.ServiceDocument, details);

            return passed;
        }
    }
}
