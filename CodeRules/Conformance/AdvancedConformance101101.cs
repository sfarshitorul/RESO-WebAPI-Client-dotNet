// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace ODataValidator.Rule
{
    #region Namespaces
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using ODataValidator.Rule.Helper;
    using ODataValidator.RuleEngine;
    
    #endregion

    /// <summary>
    /// Class of extension rule for Advanced.Conformance.101101
    /// </summary>
    [Export(typeof(ExtensionRule))]
    public class AdvancedConformance101101 : ConformanceAdvancedExtensionRule
    {
        /// <summary>
        /// Gets rule name
        /// </summary>
        public override string Name
        {
            get
            {
                return "Advanced.Conformance.101101";
            }
        }

        /// <summary>
        /// Gets rule description
        /// </summary>
        public override string Description
        {
            get
            {
                return @"1). Services MUST support all three formats: Absolute URI with schema, host, port, and absolute resource path. Absolute resource path and separate Host header. Resource path relative to the batch request URI. (section 11.7.2)";
            }
        }

        /// <summary>
        /// Gets rule specification in OData document
        /// </summary>
        public override string V4SpecificationSection
        {
            get
            {
                return "11.7.2";
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
            var serviceStatus = ServiceStatus.GetInstance();
            var detail1 = new ExtensionRuleResultDetail(this.Name);
            var detail2 = new ExtensionRuleResultDetail(this.Name);
            var detail3 = new ExtensionRuleResultDetail(this.Name);
            var detail4 = new ExtensionRuleResultDetail(this.Name);
            string feedUrl = string.Empty;
            string entityUrl = string.Empty;

            KeyValuePair<string, IEnumerable<string>> entityUrls;
            try
            {
                if (JsonParserHelper.GetBatchSupportedEntityUrls(out entityUrls))
                {
                    feedUrl = string.Format("{0}/{1}", serviceStatus.RootURL.TrimEnd('/'), entityUrls.Key);
                    entityUrl = entityUrls.Value.First();
                }
                else
                {
                    var infoerror = new ExtensionRuleResultDetail(this.Name, context.ServiceBaseUri.OriginalString + "/$batch", HttpMethod.Post, string.Empty, string.Empty, string.Empty, string.Empty);
                    passed = false;
                    infoerror.ErrorMessage = string.Format("This server does have any entities that support batch request");
                    return passed;

                }

            }
            catch (Exception ex)
            {
                passed = false;
                detail4.ErrorMessage = ex.Message+" Your service document should identify the EntitySet like this:  {\"name\":\"Property\",\"url\":\"Property\",\"kind\":\"EntitySet\"}";
                List<ExtensionRuleResultDetail> details2 = new List<ExtensionRuleResultDetail>() { detail4 };
                info = new ExtensionRuleViolationInfo(new Uri(serviceStatus.RootURL), serviceStatus.ServiceDocument, details2);

                return passed;
            }


            string relativeUrl = string.Empty;
            try
            {
                relativeUrl = new Uri(entityUrl).LocalPath;
            }
            catch
            {
                var details23 = new ExtensionRuleResultDetail(this.Name);
                details23.ErrorMessage = "The Entity URL is not valid:  " + entityUrl;
                if (info == null)
                {
                    info = new ExtensionRuleViolationInfo(new Uri(serviceStatus.RootURL), serviceStatus.ServiceDocument, details23);
                }
                else
                {
                    info.AddDetail(details23);
                }
                
                passed = false;
                return passed;
            }
            string host = entityUrl.Remove(entityUrl.IndexOf(relativeUrl));

            string format1Request = string.Format(@"
--batch_36522ad7-fc75-4b56-8c71-56071383e77b
Content-Type: application/http 
Content-Transfer-Encoding:binary


GET {0} HTTP/1.1

--batch_36522ad7-fc75-4b56-8c71-56071383e77b--", entityUrl);

            string format2Request = string.Format(@"
--batch_36522ad7-fc75-4b56-8c71-56071383e77b
Content-Type: application/http 
Content-Transfer-Encoding:binary


GET {0} HTTP/1.1
Host: {1}

--batch_36522ad7-fc75-4b56-8c71-56071383e77b--", relativeUrl, host);

            string format3Reuqest = string.Format(@"
--batch_36522ad7-fc75-4b56-8c71-56071383e77b
Content-Type: application/http 
Content-Transfer-Encoding:binary


GET {0} HTTP/1.1

--batch_36522ad7-fc75-4b56-8c71-56071383e77b--", relativeUrl);

            string boundary = @"batch_36522ad7-fc75-4b56-8c71-56071383e77b";
            Response format1Response = WebHelper.BatchOperation(serviceStatus.RootURL.TrimEnd('/') + @"/", format1Request, boundary);
            detail1 = new ExtensionRuleResultDetail(this.Name, feedUrl.TrimEnd('/') + "/$batch", HttpMethod.Post, string.Empty, format1Response, string.Empty, format1Request);
            detail1.URI = serviceStatus.RootURL.TrimEnd('/') + @"/";
            detail1.ResponsePayload = format1Response.ResponsePayload;
            detail1.ResponseHeaders = format1Response.ResponseHeaders;
            detail1.HTTPMethod = "POST";
            detail1.ResponseStatusCode = format1Response.StatusCode.ToString();

            Response format2Response = WebHelper.BatchOperation(serviceStatus.RootURL.TrimEnd('/') + @"/", format2Request, boundary);
            detail2 = new ExtensionRuleResultDetail(this.Name, feedUrl.TrimEnd('/') + "/$batch", HttpMethod.Post, string.Empty, format2Response, string.Empty, format2Request);
            detail2.URI = serviceStatus.RootURL.TrimEnd('/') + @"/";
            detail2.ResponsePayload = format1Response.ResponsePayload;
            detail2.ResponseHeaders = format1Response.ResponseHeaders;
            detail2.HTTPMethod = "POST";
            detail2.ResponseStatusCode = format1Response.StatusCode.ToString();

            Response format3Response = WebHelper.BatchOperation(serviceStatus.RootURL.TrimEnd('/') + @"/", format3Reuqest, boundary);
            detail3 = new ExtensionRuleResultDetail(this.Name, feedUrl.TrimEnd('/') + "/$batch", HttpMethod.Post, string.Empty, format3Response, string.Empty, format3Reuqest);
            detail3.URI = serviceStatus.RootURL.TrimEnd('/') + @"/";
            detail3.ResponsePayload = format1Response.ResponsePayload;
            detail3.ResponseHeaders = format1Response.ResponseHeaders;
            detail3.HTTPMethod = "POST";
            detail3.ResponseStatusCode = format1Response.StatusCode.ToString();

            if (format1Response != null && !string.IsNullOrEmpty(format1Response.ResponsePayload))
            {
                if (!format1Response.ResponsePayload.Contains(@"HTTP/1.1 200 OK"))
                {
                    passed = false;
                    detail1.ErrorMessage = "Batch request failed by above URI. ERROR:  "+ format1Response.ResponsePayload;
                }
            }
            else
            {
                passed = false;
                detail1.ErrorMessage = "No response returned from above URI.";
            }

            if (format2Response != null && !string.IsNullOrEmpty(format2Response.ResponsePayload))
            {
                if (!format2Response.ResponsePayload.Contains(@"HTTP/1.1 200 OK"))
                {
                    passed = false;
                    detail2.ErrorMessage = "Batch request failed by above URI and host : " + format2Response.ResponsePayload;
                }
            }
            else
            {
                passed = false;
                detail2.ErrorMessage = "No response returned from above URI and host : " + format2Response.ResponsePayload;
            }

            if (format3Response != null && !string.IsNullOrEmpty(format3Response.ResponsePayload))
            {
                if (!format3Response.ResponsePayload.Contains(@"HTTP/1.1 200 OK"))
                {
                    passed = false;
                    detail3.ErrorMessage = "Batch request failed by above URI and Batch request host : "+ format2Response.ResponsePayload;
                }
            }
            else
            {
                passed = false;
                detail3.ErrorMessage = "No response returned from above URI and Batch request host : "+ format2Response.ResponsePayload;
            }

            if (passed == null)
            {
                passed = true;
            }

            List<ExtensionRuleResultDetail> details = new List<ExtensionRuleResultDetail>() { detail1, detail2, detail3 };
            info = new ExtensionRuleViolationInfo(new Uri(serviceStatus.RootURL), serviceStatus.ServiceDocument, details);

            return passed;
        }
    }
}
