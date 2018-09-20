using System;

namespace ScriptValidators
{
    public class ValidateBase
    {
        public string results = string.Empty;
        virtual public bool ReadResultsFile(string filename)
        {

            return false;
        }
        virtual public bool ReadResultsData(string resultsdata)
        {

            results = resultsdata;
            return true;
        }

    }
}
