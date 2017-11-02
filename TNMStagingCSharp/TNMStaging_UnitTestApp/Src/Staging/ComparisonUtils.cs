using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNMStaging_UnitTestApp.Src.Staging
{
    public class ComparisonUtils
    {
        public static bool CompareStringDictionaries(Dictionary<String, String> d1, Dictionary<String, String> d2)
        {
            bool bRetval = true;

            if (d1.Count != d2.Count)
            {
                bRetval = false;
            }
            else
            {
                foreach (KeyValuePair<String, String> kvp in d1)
                {
                    if (d2[kvp.Key] != kvp.Value)
                    {
                        bRetval = false;
                        break;
                    }
                }
            }
            return bRetval;
        }

    }
}
