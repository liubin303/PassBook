using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VEBS.Core.Infrastructure.WCF;
using System.Runtime.Serialization;
using System.Data;

namespace Contract.Common.DataContract
{
    public class BCUserDC : EntityBase
    {

        /// <summary>
        /// 分店名
        /// </summary>
        [DataMember]
        public string Name { set; get; }
        /// <summary>
        /// 商户名
        /// </summary>
        [DataMember]
        public string BCName { set; get; }
        /// <summary>
        /// 经度
        /// </summary>
        [DataMember]
        public string Longitude { set; get; }
        //纬度
        [DataMember]
        public string Dimensions { set; get; }

        /// <summary>
        /// 城市
        /// </summary>
        [DataMember]
        public int City { set; get; }

        public static BCUserDC GetEntity(IDataReader reader)
        {
            BCUserDC entity = null;
            var cols = GetReaderColumnsName(reader);

            entity = new BCUserDC();

            if (cols.Contains("Name") && reader["Name"] != DBNull.Value)
                entity.Name = Convert.ToString(reader["Name"]);
            if (cols.Contains("BCName") && reader["BCName"] != DBNull.Value)
                entity.BCName = Convert.ToString(reader["BCName"]);
            if (cols.Contains("Longitude") && reader["Longitude"] != DBNull.Value)
                entity.Longitude = Convert.ToString(reader["Longitude"]);

            if (cols.Contains("Dimensions") && reader["Dimensions"] != DBNull.Value)
                entity.Dimensions = Convert.ToString(reader["Dimensions"]);
            if (cols.Contains("City") && reader["City"] != DBNull.Value)
                entity.City = Convert.ToInt32(reader["City"]);

            entity.SetBaseInfo(entity, reader, cols);

            return entity;
        }
    }
}
