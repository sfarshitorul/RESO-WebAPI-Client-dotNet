using System;

using Newtonsoft.Json.Linq;



namespace ScriptValidators
{
    static class Extensions
    {
        public static bool TryToJObject(this string payload, out JObject jo)
        {
            if (!string.IsNullOrEmpty(payload))
            {
                try
                {
                    jo = JObject.Parse(payload);
                    return true;
                }
                catch (Exception ex)
                {
                    // it's alright if JSON parsing complaints
                }
            }

            jo = null;
            return false;
        }
    }
    public class ValidateServiceData : ValidateBase
    {
        
        public override bool ReadResultsData(string resultsdata)
        {
            base.ReadResultsData(resultsdata);
            JObject jo;
            
            string test = this.results;
            test.TryToJObject(out jo);


            
            return true;
        }
        
    }
}
