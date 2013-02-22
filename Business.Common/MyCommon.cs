using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VEBS.Core.Infrastructure.WCF.Service;

namespace Business.Common
{
    public class MyCommon
    {
        readonly DAL.Common.BCUser.BCUserDAL dal_BCUser;
        readonly DAL.Common.Coupon.CouponDAL dal_Coupon;
        static MyCommon _MyCommon;

         public MyCommon()
        {
            if (dal_BCUser == null)
            {
                dal_BCUser = new DAL.Common.BCUser.BCUserDAL();
            }
            if (dal_Coupon == null)
            {
                dal_Coupon = new DAL.Common.Coupon.CouponDAL();
            }
        }

         public static MyCommon Instance
        {
            get
            {
                if (_MyCommon == null)
                {
                    _MyCommon = new MyCommon();
                }
                return _MyCommon;
            }
        }

         public IList<Contract.Common.DataContract.BaseECouponDC> GetECouponListByPage(ref PageInfo pageinfo)
         {
             var model = dal_Coupon.GetECouponListByPage(ref  pageinfo);
             return model;
         }

         public bool UpdateECouponHaveKey(List<int> BEC_ECIDList, int HaveKey)
         {
             var model = dal_Coupon.UpdateECouponHaveKey(BEC_ECIDList, HaveKey);
             return model;
            
         }
         public bool UpdateECouponHaveKey()
         {
             var model = dal_Coupon.UpdateECouponHaveKey();
             return model;

         }
         public List<Guid> GetBCUserIDList(int CouponID)
         {
             var model = dal_Coupon.GetBCUserIDList(CouponID);
             return model;
         }
         public List<Contract.Common.DataContract.BCUserDC> GetBCUserList(List<Guid> iIDList, Guid? iBCID,int iCity)
         {
             var model = dal_BCUser.GetBCUserList(iIDList, iBCID, iCity);
             return model;
             
         }


         public String GetKeyword(int iVCID)
         {
             var model = dal_BCUser.GetKeyword(iVCID);
             return model;

         }
    }
}
