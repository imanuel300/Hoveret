using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEBAPIODATAV3.Utilities
{
    internal enum eMarketingMethod
    {
        MechirLeMishtaken = 18,
        RemiKarkha = 7,
        DiurMugan = 2

    }

    internal static class MarketingMethod
    {
        internal static eMarketingMethod Convert(string val)
        {
            int iVal = (int)eMarketingMethod.MechirLeMishtaken;
            Int32.TryParse(val, out iVal);
            switch (iVal)
            {
                case (int)eMarketingMethod.MechirLeMishtaken:
                    return eMarketingMethod.MechirLeMishtaken;

                case (int)eMarketingMethod.RemiKarkha:
                    return eMarketingMethod.RemiKarkha;

                case (int)eMarketingMethod.DiurMugan:
                    return eMarketingMethod.DiurMugan;

                default:
                    return eMarketingMethod.MechirLeMishtaken;
            }


        }




    }
}