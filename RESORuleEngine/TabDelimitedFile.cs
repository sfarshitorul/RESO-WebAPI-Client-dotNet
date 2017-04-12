using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataValidator.RuleEngine
{
    public class TabDelimitedFile
    {
        virtual public void ReadFile(string filename)
        {
            int columncount = 0;
            string[] columns = null;
            string line = string.Empty;
            try
            {
                string[] data = File.ReadAllLines(filename, Encoding.Default);


                for (int n = 0; n < data.Length; n++)
                {
                    line = data[n];
                    columns = line.Split('\t');
                    if (columncount == 0)
                    {
                        columncount = columns.Length;
                    }
                    else if (columns.Length != columncount)
                    {
                        continue;
                    }
                    AddData(filename, columns, n);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                FinalizeData();
            }
        }

        virtual protected void FinalizeData()
        {
            throw new NotImplementedException();
        }

        virtual protected void AddData(string filename, string[] columns, int row)
        {
            throw new NotImplementedException();
        }
    }
}