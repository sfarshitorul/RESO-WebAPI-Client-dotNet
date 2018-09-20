using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptValidators
{
    public class ValidateServiceData : ValidateBase
    {
        public override bool ReadResultsData(string resultsdata)
        {
            base.ReadResultsData(resultsdata);

            string test = this.results;
            
            return true;
        }
    }
}
