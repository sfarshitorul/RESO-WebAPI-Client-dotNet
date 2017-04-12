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
                            record = new RuleControl();
                            record.rulename = value;
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
                    }
                }
                //string text = node.InnerXml;
                //string json = JsonConvert.SerializeXmlNode(node);
                //RuleControl record = JsonConvert.DeserializeObject<RuleControl>(json);

            }
        }


    }
}
