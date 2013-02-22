using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VEBS.Core.Infrastructure.WCF;
using Contract.Common;
using System.Data;
using VEBS.Core.Infrastructure.WCF.Service;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using VEBS.Core.Helper;
using Contract.Common.DataContract;

namespace DAL.Common.Coupon
{
    public class CouponDAL : DALBase
    {
         /// <summary>
        /// 构造函数
        /// </summary>
        public CouponDAL()
        {
            APPModule = VEBS.Core.Enumerate.ApplicationModule.Common;
        }

        public IList<BaseECouponDC> GetECouponListByPage(ref PageInfo pageinfo)
        {
            IList<BaseECouponDC> list = new List<BaseECouponDC>();
            string tables = " View_ECouponByPassBook";
            string PrimaryKey = " BEC_ECID ";
            string Fields = "*";
            string Group = null;

            #region 查询条件
            string Filter = "BEC_HaveKey=0 and Obj_Status=1 and BEC_PublishEndDate >'" + DateTime.Now.Date.ToString() + "'"; 
          

            #endregion

            using (IDataReader reader = ExecuteReaderPaginationByPageIndex(tables, PrimaryKey, Fields, Filter, pageinfo.OrderByCol, Group, pageinfo.PageIndex, pageinfo.PageSize, VEBS.Core.Enumerate.ApplicationModule.Common))
            {
                while (reader.Read())
                {
                    list.Add(BaseECouponDC.GetEntity(reader));
                }
            }
            //总数量
            pageinfo.TotalCount = ExecuteReaderRecordCount(tables, PrimaryKey, Fields, Filter, pageinfo.OrderByCol, Group, pageinfo.PageIndex, pageinfo.PageSize, VEBS.Core.Enumerate.ApplicationModule.Common);
            return list;
        }
        public bool UpdateECouponHaveKey(List<int> BEC_ECIDList, int HaveKey)
        {
            StringBuilder sql = new StringBuilder();
            Database db = DBHelper.CreateDataBase(APPModule, VEBS.Core.Enumerate.DataAccessPatterns.Write);
            sql.Append("UPDATE Base_Ecoupon SET BEC_HaveKey= " + HaveKey);
            sql.Append(" WHERE BEC_ECID in (");
            if (BEC_ECIDList != null && BEC_ECIDList.Count > 0)
            {

                foreach (var id in BEC_ECIDList)
                {
                    sql.Append(id + ",");
                }
                sql = sql.Remove(sql.Length - 1, 1);
                sql.Append(")");
            }
            else
                return false;
            DbCommand cmd = db.GetSqlStringCommand(sql.ToString());
            return db.ExecuteNonQuery(cmd) > 0 ? true : false;
        }

        public bool UpdateECouponHaveKey()
        {
            StringBuilder sql = new StringBuilder();
            Database db = DBHelper.CreateDataBase(APPModule, VEBS.Core.Enumerate.DataAccessPatterns.Write);
            sql.Append("UPDATE Base_Ecoupon SET BEC_HaveKey=0 ");
            sql.Append(" WHERE BEC_HaveKey=2");
           
            DbCommand cmd = db.GetSqlStringCommand(sql.ToString());
            return db.ExecuteNonQuery(cmd) > 0 ? true : false;
        }

        public List<Guid> GetBCUserIDList(int CouponID)
        {
            List<Guid> list = new List<Guid>();
            Database db = DBHelper.CreateDataBase(APPModule, VEBS.Core.Enumerate.DataAccessPatterns.Query);
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT BVCUA_ShopId FROM Base_VELOCouponUseAddress WHERE BVC_VCID=@BVC_VCID and BVCUA_Type=1 and   Obj_Status =1");
            DbCommand cmd = db.GetSqlStringCommand(sql.ToString());
            //有效数据
            db.AddInParameter(cmd, "@BVC_VCID", DbType.Int32, CouponID);
            using (IDataReader reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    try
                    {
                        list.Add(new Guid(reader.ToString()));
                    }
                    catch
                    { }
                }
            }
            return list;
        }

        
        
    }
}
