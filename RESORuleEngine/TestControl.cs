using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using System.Collections;
using RESOClientLibrary;

namespace ODataValidator.RuleEngine
{
    public class TestControl
    {
        public Hashtable rulecontrol = new Hashtable();

        public void BuildRuleControlList(RESOClientSettings settings)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(settings.GetSetting(RESOClientLibrary.settings.rulescontrolfile));

            //XmlNode node = doc.DocumentElement.SelectSingleNode("/rulescontrol");
            RuleControl record = null;
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                if (node.HasChildNodes)
                {
                    foreach (XmlNode childnode in node)
                    {
                        string name = childnode.Name;
                        string value = childnode.InnerText;
                        if (name == "rulename")
                        {
                            if (record != null)
                            {
                                rulecontrol[value] = record;
                            }
                            else
                            {
                                record = new RuleControl();
                                record.rulename = value;
                                rulecontrol[value] = record;
                            }
                        }
                        if (name == "notes")
                        {
                            record.notes = value;
                        }
                        if (name == "cert_tr")
                        {
                            record.cert_tr = value;
                        }
                        if (name == "cert_impact")
                        {
                            record.cert_impact = value;
                        }
                        if (name == "ttt_testing_results")
                        {
                            record.ttt_testing_results = value;
                        }
                        if (name == "category")
                        {
                            record.category = value;
                        }
                        if (name == "RESOVersion")
                        {
                            record.RESOVersion = value;
                        }
                        if (name == "Description")
                        {
                            record.Description = value;
                        }
                        if (name == "ErrorMessage")
                        {
                            record.ErrorMessage = value;
                        }
                        if (name == "ODataSpecification")
                        {
                            record.ODataSpecification = value;
                        }
                        if (name == "V4ODataSpecification")
                        {
                            record.V4ODataSpecification = value;
                        }
                        if (name == "V4Specification")
                        {
                            record.V4Specification = value;
                        }
                        if (name == "ODataVersion")
                        {
                            record.ODataVersion = value;
                        }
                        if (name == "PayloadType")
                        {
                            record.PayloadType = value;
                        }
                        if (name == "PayloadFormat")
                        {
                            record.PayloadFormat = value;
                        }
                        if (name == "HelpLink")
                        {
                            record.HelpLink = value;
                        }

                        /*
                            <rulename>Common.Core.1000</rulename>
                                <notes></notes>
                                <cert_tr></cert_tr>
                                <cert_impact>Should</cert_impact>
                                <ttt_testing_results></ttt_testing_results>
                                <category>CORE</category>
                                <RESOVersion></RESOVersion>
                                <Description>A data service SHOULD expose a conceptual schema definition language (CSDL) based metadata endpoint that describes the structure and organization of all the resources exposed as HTTP endpoints.</Description>
                                <ErrorMessage>A data service SHOULD expose a conceptual schema definition language (CSDL) based metadata endpoint that describes the structure and organization of all the resources exposed as HTTP endpoints.</ErrorMessage>
                                <ODataSpecification>2.2.3.7.2</ODataSpecification>
                                <V4ODataSpecification></V4ODataSpecification>
                                <V4Specification></V4Specification>
                                <ODataVersion></ODataVersion>
                                <PayloadType>Entry</PayloadType>
                                <PayloadFormat></PayloadFormat>
                                <HelpLink></HelpLink>
                         */
                    }
                }
                //string text = node.InnerXml;
                //string json = JsonConvert.SerializeXmlNode(node);
                //RuleControl record = JsonConvert.DeserializeObject<RuleControl>(json);

            }
        }


    }
}
