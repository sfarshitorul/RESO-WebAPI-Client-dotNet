using RESORuleEngine;

namespace WebAPITest
{
    public class WebAPITestExecuter
    {
        public RuleEngineWrapper RuleEngine { get; set; }

        public WebAPITestExecuter()
        {
            
        }

        public void Execute()
        {
            //RuleSelector.SelectRules()
            RuleEngine.Validate();
        }
    }
}
