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
    /// Class of service implemenation feature to delete an media entity.
    /// </summary>
    [Export(typeof(ExtensionRule))]
    public class ServiceImpl_DeleteMediaEntity : ServiceImplExtensionRule
    {
        /// <summary>
        /// Gets rule name
        /// </summary>
        public override string Name
        {
            get
            {
                return "ServiceImpl_DeleteMediaEntity";
            }
        }

        /// <summary>
        /// Gets the service implementation category.
        /// </summary>
        public override ServiceImplCategory CategoryInfo
        {
            get
            {
                var parent = new ServiceImplCategory(ServiceImplCategoryName.DataModification);
                return new ServiceImplCategory(ServiceImplCategoryName.ManagingMediaEntities, parent);
            }
        }

        /// <summary>
        /// Gets rule description
        /// </summary>
        public override string Description
        {
            get
            {
                return this.CategoryInfo.CategoryFullName + ",Delete a Media Entity";
            }
        }

        /// <summary>
        /// Gets rule specification in OData document
        /// </summary>
        public override string V4SpecificationSection
        {
            get
            {
                return "11.4.7.3";
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
            DataFactory dFactory = DataFactory.Instance();
            var detail = new ExtensionRuleResultDetail(this.Name, serviceStatus.RootURL, HttpMethod.Post, string.Empty);
            string deleteUrl = serviceStatus.RootURL;
            List<string> keyPropertyTypes = new List<string>() { "Edm.Int32", "Edm.Int16", "Edm.Int64", "Edm.Guid", "Edm.String" };
            List<EntityTypeElement> entityTypeElements = MetadataHelper.GetEntityTypes(serviceStatus.MetadataDocument, 1, keyPropertyTypes, null, NavigationRoughType.None).ToList();
            if (null == entityTypeElements || 0 == entityTypeElements.Count())
            {
                detail.ErrorMessage = "To verify this rule it expects an entity type with Int32/Int64/Int16/Guid/String key property, but there is no this entity type in metadata so can not verify this rule.";
                info = new ExtensionRuleViolationInfo(new Uri(serviceStatus.RootURL), serviceStatus.ServiceDocument, detail);
                return passed;
            }

            EntityTypeElement eTypeElement = null;
            foreach (var en in entityTypeElements)
            {

                if (MetadataHelper.IsMediaEntity(serviceStatus.MetadataDocument, en.EntityTypeShortName, context))
                {
                    eTypeElement = en;
                    break;
                }
            }

            if (null == eTypeElement)
            {
                detail.ErrorMessage = "To verify this rule it expects an entity type with deleteable and insertable restrictions, but there is no entity type in metadata which is insertable and deleteable.";
                info = new ExtensionRuleViolationInfo(new Uri(serviceStatus.RootURL), serviceStatus.ServiceDocument, detail);

                return passed;
            }

            string entitySetUrl = eTypeElement.EntitySetName.MapEntitySetNameToEntitySetURL();
            deleteUrl = serviceStatus.RootURL.TrimEnd('/') + @"/" + entitySetUrl;
            if (string.IsNullOrEmpty(entitySetUrl))
            {
                detail.ErrorMessage = string.Format("Cannot find the entity-set URL which is matched with {0}", eTypeElement.EntityTypeShortName);
                info = new ExtensionRuleViolationInfo(new Uri(serviceStatus.RootURL), serviceStatus.ServiceDocument, detail);

                return passed;
            }

            string url = serviceStatus.RootURL.TrimEnd('/') + @"/" + entitySetUrl;
            var additionalInfos = new List<AdditionalInfo>();
            var reqData = dFactory.ConstructInsertedEntityData(eTypeElement.EntitySetName, eTypeElement.EntityTypeShortName, null, out additionalInfos);
            string reqDataStr = reqData.ToString();
            var resp = WebHelper.CreateEntity(url, context.RequestHeaders, reqData, true, ref additionalInfos);
            detail = new ExtensionRuleResultDetail(this.Name, url, HttpMethod.Post, string.Empty, resp, string.Empty, reqDataStr);
            if (HttpStatusCode.Created == resp.StatusCode || HttpStatusCode.NoContent == resp.StatusCode)
            {
                var entityId = additionalInfos.Last().EntityId;
                deleteUrl = entityId;
                var hasEtag = additionalInfos.Last().HasEtag;
                resp = WebHelper.DeleteEntity(entityId, context.RequestHeaders, hasEtag);
                detail = new ExtensionRuleResultDetail(this.Name, entityId, HttpMethod.Delete, string.Empty, resp);
                detail.URI = entityId;
                detail.ResponsePayload = resp.ResponsePayload;
                detail.ResponseHeaders = resp.ResponseHeaders;
                detail.HTTPMethod = "GET";
                detail.ResponseStatusCode = resp.StatusCode.ToString();

                if (HttpStatusCode.NoContent == resp.StatusCode)
                {
                    resp = WebHelper.GetEntity(entityId);
                    detail = new ExtensionRuleResultDetail(this.Name, entityId, HttpMethod.Get, string.Empty, resp);
                    if (HttpStatusCode.NotFound == resp.StatusCode)
                    {
                        additionalInfos.Remove(additionalInfos.Last());
                        passed = true;
                    }
                    else
                    {
                        passed = false;
                        detail.ErrorMessage = string.Format("Delete media entity failed because it still can get the entity {0}.", entityId);
                    }
                }
                else
                {
                    passed = false;
                    detail.ErrorMessage = string.Format("Delete the created media entity failed.");
                }

                // Restore the service.
                var resps = WebHelper.DeleteEntities(context.RequestHeaders, additionalInfos);
            }
            else
            {
                detail.ErrorMessage = "Created the new entity failed for above URI.";
            }

            var details = new List<ExtensionRuleResultDetail>() { detail };
            info = new ExtensionRuleViolationInfo(new Uri(deleteUrl), serviceStatus.ServiceDocument, details);

            return passed;
        }
    }
}
