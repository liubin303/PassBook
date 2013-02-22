using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VEBS.Core.Infrastructure.WCF;
using Contract.Common.DataContract;
using Microsoft.Practices.EnterpriseLibrary.Data;
using VEBS.Core.Helper;
using System.Data;
using System.Data.Common;

namespace DAL.Common.BCUser
{
    public class BCUserDAL : DALBase
    { 
        /// <summary>
        /// 构造函数
        /// </summary>
        public BCUserDAL()
        {
            APPModule = VEBS.Core.Enumerate.ApplicationModule.Common;
        }
        //in
        string GetBCUserList_ByIDList = "SELECT Name,  [Longitude]  ,[Dimensions]  ,[BCName],[City] FROM  [View_BC_BCUserCache] where ID in (";
        //BYBCEID
        string GetBCUserList_ByBCEID = "SELECT  Name, [Longitude]  ,[Dimensions]  ,[BCName],[City] FROM  [View_BC_BCUserCache] where BCID=@BCID";

        public List<BCUserDC> GetBCUserList(List<Guid> iIDList, Guid? iBCID,int iCity)
        {
            List<BCUserDC> list = new List<BCUserDC>();
            Database db = DBHelper.CreateDataBase(APPModule, VEBS.Core.Enumerate.DataAccessPatterns.Query);
            StringBuilder sql = new StringBuilder();
            DbCommand cmd = null;
            if (iIDList != null && iIDList.Count>0)
            {
                sql.Append(GetBCUserList_ByIDList);
                foreach (var id in iIDList)
                {
                    sql.Append(id + ",");
                }
                sql = sql.Remove(sql.Length - 1, 1);
                sql.Append(")");
                sql.Append(" and @City  & City  = City ");
                cmd = db.GetSqlStringCommand(sql.ToString());
                db.AddInParameter(cmd, "@City", DbType.Int32, iCity);
            }
            else if (iBCID != null && iBCID != Guid.Empty)
            {
                sql.Append(GetBCUserList_ByBCEID);
                sql.Append(" and @City  & City  = City ");
                cmd = db.GetSqlStringCommand(sql.ToString());
                db.AddInParameter(cmd, "@BCID", DbType.Guid, iBCID.Value);
                db.AddInParameter(cmd, "@City", DbType.Int32, iCity);
            }
            else
                return null;
          
            //有效数据
           
            using (IDataReader reader = db.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    list.Add(BCUserDC.GetEntity(reader));
                }
            }
            return list;
        }



        string GetKeyword_sql = "SELECT  C.Category FROM Base_VELOCoupon A inner join Base_BCustomerIDRelation B on BVC_VCID=@BVC_VCID and A.BBR_Id = B.BCID AND B.Status = '1' inner join Base_Brand_View C on B.BCIDNew=C.ID ";
        //string GetKeyword_sql = "SELECT  B.Keyword FROM Base_VELOCoupon A inner join mct_BCSearchKeyword B on BVC_VCID=@BVC_VCID and A.BBR_Id = B.BCID AND B.Type = 1 AND B.Status = '01' ";
        public String GetKeyword(int iVCID)
        {
            string rtn="";
            Database db = DBHelper.CreateDataBase(APPModule, VEBS.Core.Enumerate.DataAccessPatterns.Query);
            StringBuilder sql = new StringBuilder();
            DbCommand cmd = null;
            sql.Append(GetKeyword_sql);
            cmd = db.GetSqlStringCommand(sql.ToString());
            db.AddInParameter(cmd, "@BVC_VCID", DbType.Int32, iVCID);

            //有效数据
            var  rtn1= db.ExecuteScalar(cmd);
            if (rtn1 != null)
                rtn = rtn1.ToString();
            
            return rtn;
        }
    }
}
