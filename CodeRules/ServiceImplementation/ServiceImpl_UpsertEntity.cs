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
    using ODataValidator.Rule.Helper;
    using ODataValidator.RuleEngine;
    
    #endregion

    /// <summary>
    /// Class of service implemenation feature to upsert an entity.
    /// </summary>
    [Export(typeof(ExtensionRule))]
    public class ServiceImpl_UpsertEntity : ServiceImplExtensionRule
    {
        /// <summary>
        /// Gets rule name
        /// </summary>
        public override string Name
        {
            get
            {
                return "ServiceImpl_UpsertEntity";
            }
        }

        /// <summary>
        /// Gets the service implementation category.
        /// </summary>
        public override ServiceImplCategory CategoryInfo
        {
            get
            {
                return new ServiceImplCategory(ServiceImplCategoryName.DataModification);
            }
        }

        /// <summary>
        /// Gets rule description
        /// </summary>
        public override string Description
        {
            get
            {
                return this.CategoryInfo.CategoryFullName + ",Upsert an Entity";
            }
        }

        /// <summary>
        /// Gets rule specification in OData document
        /// </summary>
        public override string V4SpecificationSection
        {
            get
            {
                return "11.4.4";
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
            var detail = new ExtensionRuleResultDetail(this.Name, serviceStatus.RootURL, HttpMethod.Post, string.Empty);
            string upsertUrl = serviceStatus.RootURL;
            List<string> keyPropertyTypes = new List<string>() { "Edm.Int32", "Edm.Int16", "Edm.Int64", "Edm.Guid", "Edm.String" };
            List<EntityTypeElement> entityTypeElements = MetadataHelper.GetEntityTypes(serviceStatus.MetadataDocument, 1, keyPropertyTypes, null, NavigationRoughType.None).ToList();
            if (null == entityTypeElements || 0 == entityTypeElements.Count())
            {
                detail.ErrorMessage = "To verify this rule it expects an entity type with Int32/Int64/Int16/Guid/String key property, but there is no this entity type in metadata so can not verify this rule.";
                info = new ExtensionRuleViolationInfo(new Uri(serviceStatus.RootURL), serviceStatus.ServiceDocument, detail);

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

            if (entityType == null)
            {
                detail.ErrorMessage = "To verify this rule it expects an entity type insertable, updatable and deletable, but at least one of these condition is lack for all the entities in metadata. So can not verify this rule.";
                info = new ExtensionRuleViolationInfo(new Uri(serviceStatus.RootURL), serviceStatus.ServiceDocument, detail);

                return passed;
            }

            string entitySetUrl = entityType.EntitySetName.MapEntitySetNameToEntitySetURL();
            upsertUrl = serviceStatus.RootURL.TrimEnd('/') + @"/" + entitySetUrl;
            if (string.IsNullOrEmpty(entitySetUrl))
            {
                detail.ErrorMessage = string.Format("Cannot find the entity-set URL which is matched with {0}", entityType.EntityTypeShortName);
                info = new ExtensionRuleViolationInfo(new Uri(serviceStatus.RootURL), serviceStatus.ServiceDocument, detail);

                return passed;
            }

            // Create a entity
            string url = serviceStatus.RootURL.TrimEnd('/') + @"/" + entitySetUrl;
            var additionalInfos = new List<AdditionalInfo>();
            var reqData = dFactory.ConstructInsertedEntityData(entityType.EntitySetName, entityType.EntityTypeShortName, null, out additionalInfos);
            string reqDataStr = reqData.ToString();
            bool isMediaType = !string.IsNullOrEmpty(additionalInfos.Last().ODataMediaEtag);
            var resp = WebHelper.CreateEntity(url, context.RequestHeaders, reqData, isMediaType, ref additionalInfos);
            detail = new ExtensionRuleResultDetail(this.Name, url, HttpMethod.Post, string.Empty, resp, string.Empty, reqDataStr);

            if (HttpStatusCode.Created == resp.StatusCode)
            {
                var entityId = additionalInfos.Last().EntityId;
                upsertUrl = entityId;
                var hasEtag = additionalInfos.Last().HasEtag;
                resp = WebHelper.DeleteEntity(entityId, context.RequestHeaders, hasEtag);
                detail = new ExtensionRuleResultDetail(this.Name, entityId, HttpMethod.Delete, string.Empty, resp);
                if (HttpStatusCode.NoContent == resp.StatusCode)
                {
                    var header = new KeyValuePair<string, string>("If-None-Match", "*");
                    var headers = new List<KeyValuePair<string, string>>() { header };
                    resp = WebHelper.UpsertEntity(entityId, reqDataStr, HttpMethod.Put, hasEtag ? headers : null);
                    detail = new ExtensionRuleResultDetail(this.Name, url, HttpMethod.Put, string.Empty, resp, string.Empty, reqDataStr);

                    if (HttpStatusCode.Created == resp.StatusCode)
                    {
                        resp = WebHelper.GetEntity(entityId);
                        detail = new ExtensionRuleResultDetail(this.Name, entityId, HttpMethod.Get, string.Empty, resp);
                        if (HttpStatusCode.OK == resp.StatusCode)
                        {
                            passed = true;
                        }
                        else
                        {
                            passed = false;
                            detail.ErrorMessage = "Can not Upsert the entity from above URI.";
                        }

                        // Restore the service.
                        var resps = WebHelper.DeleteEntities(context.RequestHeaders, additionalInfos);
                    }
                    else
                    {
                        passed = false;
                        detail.ErrorMessage = "Upsert the entity failed.";
                    }
                }
                else
                {
                    detail.ErrorMessage = "Delete the entity failed.";
                }
            }
            else
            {
                detail.ErrorMessage = "Created the new entity failed for above URI.";
            }

            var details = new List<ExtensionRuleResultDetail>() { detail };
            info = new ExtensionRuleViolationInfo(new Uri(upsertUrl), serviceStatus.ServiceDocument, details);

            return passed;
        }
    }
}
