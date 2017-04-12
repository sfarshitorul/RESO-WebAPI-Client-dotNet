using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RESOClientLibrary;
using Newtonsoft.Json;

namespace WebAPITest
{
    public class RuleControl
    {
        public string notes;
        public string cert_tr;
        public string cert_impact;
        public string ttt_testing_results;
        public string parameters;
        public string category;
        public string rulename;
    }

    public class RuleControlImport : TabDelimitedFile
    {
        string outputpath = string.Empty;
        string inputpath = string.Empty;

        RESOClientSettings clientsettings;

        Hashtable rulecontrollist = new Hashtable();

        Hashtable columndata = new Hashtable();

        
        public RuleControlImport(RESOClientSettings settings)
        {//System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), @"\config");
            clientsettings = settings;
            if (clientsettings != null)
            {
                inputpath = clientsettings.GetSetting(RESOClientLibrary.settings.rulecontrolinput);
                outputpath = clientsettings.GetSetting(RESOClientLibrary.settings.rulecontroloutput);
            }
        }

        public void ReadConfigFile()
        {
            ReadFile(inputpath + "\\serviceimpl.txt");
        }
        public override void ReadFile(string filename)
        {
            base.ReadFile(filename);
        }


        protected override void AddData(string filename, string[] columns, int row)
        {
            if (row > 0)
            {
                if (rulecontrollist[columns[6] as string] == null)
                {
                    RuleControl rule = new RuleControl();
                    rule.notes= columns[0] as string;
                    rule.cert_tr= columns[1] as string;
                    rule.cert_impact= columns[2] as string;
                    rule.ttt_testing_results= columns[3] as string;
                    rule.parameters= columns[4] as string;
                    rule.category= columns[5] as string;
                    rule.rulename= columns[6] as string;
                    rulecontrollist[columns[6]] = rule;
                }
            }

        }
        protected override void FinalizeData()
        {

         

            StringBuilder sb = new StringBuilder();
            sb.Append("<rulecontrols>\r\n");
            foreach (DictionaryEntry item in rulecontrollist)
            {
                string key = item.Key as string;
                //key = key.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                RuleControl value = item.Value as RuleControl;
                StringBuilder record = new StringBuilder();
                record.Append("<rulecontrol>\r\n");
                record.Append("<rulename>" + value.rulename + "</rulename >\r\n");
                record.Append("<notes>" + value.notes + "</notes >\r\n");
                record.Append("<cert_tr>" + value.cert_tr + "</cert_tr >\r\n");
                record.Append("<cert_impact>" + value.cert_impact + "</cert_impact >\r\n");
                record.Append("<ttt_testing_results>" + value.ttt_testing_results + "</ttt_testing_results >\r\n");
                record.Append("<category>" + value.category + "</category >\r\n");

                record.Append("</rulecontrol>\r\n");
                sb.Append(record.ToString());
            }
            
            sb.Append("</rulecontrols>\r\n");

            if (File.Exists(outputpath + "\rulecontrol.xml"))
            {
                File.Delete(outputpath + "\rulecontrol.xml");
            }

            using (System.IO.StreamWriter file =
                                    new System.IO.StreamWriter(outputpath + "\\rulecontrol.xml",false))
            {
                file.WriteLine(sb.ToString());
            }
        }
    }
}
