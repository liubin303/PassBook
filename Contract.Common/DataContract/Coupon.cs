using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using VEBS.Core.Infrastructure.WCF;

namespace Contract.Common.DataContract
{
    

    /// <summary>
    /// 电子优惠券 视图
    /// </summary>
    public class BaseECouponDC : EntityBase
    {
        #region Model

        /// <summary>
        /// 发布到所有点位（门店等）
        /// </summary>
        [Display(Name = "发布到所有点位")]
        [Required]
        [DataMember]
        public int BVC_PublishAll { set; get; }

        /// <summary>
        /// 旧代理商ID
        /// </summary>
        [Display(Name = "代理商")]
        [DataMember]
        public Guid BAG_Id { set; get; }

        /// <summary>
        /// 使用方式：1.展示使用 2.打印使用 3.handy使用
        /// </summary>
        [Display(Name = "使用方式")]
        [DataMember]
        public int BVC_UseType { set; get; }


        /// <summary>
        /// ECID 标识  
        /// </summary>
        [Display(Name = "ECID 标识  ")]
        [DataMember]
        public int BEC_ECID { set; get; }

        /// <summary>
        /// 优惠ID
        /// </summary>
        [Display(Name = "优惠ID")]
        [DataMember]
        public int BVC_VCID { set; get; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        [Required]
        [DataMember]
        public string BEC_Title { set; get; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        [Required]
        [DataMember]
        public string BEC_Content { set; get; }

        /// <summary>
        /// 是否为特权券
        /// </summary>
        [Display(Name = "是否为特权券")]
        [Required]
        [DataMember]
        public bool BEC_IsSpecial { set; get; }

        /// <summary>
        /// 补充描述
        /// </summary>
        [Display(Name = "补充描述")]
        [DataMember]
        public string BEC_Detail { set; get; }

        /// <summary>
        /// 使用开始时间
        /// </summary>
        [Display(Name = "使用开始时间")]
        [Required]
        [DataMember]
        public DateTime BEC_UseBegindate { set; get; }

        /// <summary>
        /// 使用结束时间
        /// </summary>
        [Display(Name = "使用结束时间")]
        [Required]
        [DataMember]
        public DateTime BEC_UseEndDate { set; get; }

        /// <summary>
        /// 发布开始时间
        /// </summary>
        [Display(Name = "发布开始时间")]
        [Required]
        [DataMember]
        public DateTime BEC_PublishBeginDate { set; get; }

        /// <summary>
        /// 发布结束时间
        /// </summary>
        [Display(Name = "发布结束时间")]
        [Required]
        [DataMember]
        public DateTime BEC_PublishEndDate { set; get; }

        /// <summary>
        /// 展示方式：图片 短信 文字
        /// </summary>
        [Display(Name = "展示方式")]
        [Required]
        [DataMember]
        public int BEC_DisplayType { set; get; }

        /// <summary>
        /// 图片名URL
        /// </summary>
        [Display(Name = "图片")]
        [DataMember]
        public string BEC_ShowImage { set; get; }

        /// <summary>
        /// 图片提取码
        /// </summary>
        [DataMember]
        public string BEC_KeyImage { set; get; }

        /// <summary>
        /// 是否上传图片
        /// </summary>
        [DataMember]
        public bool BEC_IsUploadImage { set; get; }

        /// <summary>
        /// 发布数量
        /// </summary>
        [Display(Name = "发布数量")]
        [Required]
        [DataMember]
        public int BEC_PublishCount { set; get; }

        /// <summary>
        /// 短信内容
        /// </summary>
        [Display(Name = "短信内容")]
        [DataMember]
        public string BEC_Message { set; get; }

        /// <summary>
        /// 是否handy打印凭条
        /// </summary>
        [Display(Name = "是否handy打印凭条")]
        [Required]
        [DataMember]
        public int BVC_PrintOption { set; get; }

        /// <summary>
        /// 品牌ID
        /// </summary>
        [Display(Name = "品牌")]
        [DataMember]
        public Guid BBR_Id { set; get; }

        /// <summary>
        /// 是否有密钥
        /// </summary>
        [DataMember]
        public int BEC_HaveKey { set; get; }
        /// <summary>
        /// 城市
        /// </summary>
        [DataMember]
        public int BVC_City { set; get; }

         /// <summary>
        /// 品牌名
        /// </summary>
        [Display(Name = "品牌名")]
        [DataMember]
        public string Name { set; get; }


        /// <summary>
        /// COID
        /// </summary>
        [Display(Name = "COID")]
        [DataMember]
        public string COID { set; get; }
        

        #endregion Model

        /// <summary>
        /// 根据Reader生成BaseEcouponDC对象
        /// </summary>
        /// <param name="reader">数据集</param>
        public static BaseECouponDC GetEntity(IDataReader reader)
        {
            BaseECouponDC entity = null;
            var cols = GetReaderColumnsName(reader);

            entity = new BaseECouponDC();

            if (cols.Contains("BEC_ECID") && reader["BEC_ECID"] != DBNull.Value)
                entity.BEC_ECID = Convert.ToInt32(reader["BEC_ECID"]);
            if (cols.Contains("BVC_VCID") && reader["BVC_VCID"] != DBNull.Value)
                entity.BVC_VCID = Convert.ToInt32(reader["BVC_VCID"]);
            if (cols.Contains("BEC_Title") && reader["BEC_Title"] != DBNull.Value)
                entity.BEC_Title = Convert.ToString(reader["BEC_Title"]);
            if (cols.Contains("BEC_Content") && reader["BEC_Content"] != DBNull.Value)
                entity.BEC_Content = Convert.ToString(reader["BEC_Content"]);
            if (cols.Contains("BEC_IsSpecial") && reader["BEC_IsSpecial"] != DBNull.Value)
                entity.BEC_IsSpecial = Convert.ToBoolean(reader["BEC_IsSpecial"]);
            if (cols.Contains("BEC_Detail") && reader["BEC_Detail"] != DBNull.Value)
                entity.BEC_Detail = Convert.ToString(reader["BEC_Detail"]);
            if (cols.Contains("BEC_UseBegindate") && reader["BEC_UseBegindate"] != DBNull.Value)
                entity.BEC_UseBegindate = Convert.ToDateTime(reader["BEC_UseBegindate"]);
            if (cols.Contains("BEC_UseEndDate") && reader["BEC_UseEndDate"] != DBNull.Value)
                entity.BEC_UseEndDate = Convert.ToDateTime(reader["BEC_UseEndDate"]);
            if (cols.Contains("BEC_PublishBeginDate") && reader["BEC_PublishBeginDate"] != DBNull.Value)
                entity.BEC_PublishBeginDate = Convert.ToDateTime(reader["BEC_PublishBeginDate"]);
            if (cols.Contains("BEC_PublishEndDate") && reader["BEC_PublishEndDate"] != DBNull.Value)
                entity.BEC_PublishEndDate = Convert.ToDateTime(reader["BEC_PublishEndDate"]);
            if (cols.Contains("BEC_DisplayType") && reader["BEC_DisplayType"] != DBNull.Value)
                entity.BEC_DisplayType = Convert.ToInt32(reader["BEC_DisplayType"]);
            if (cols.Contains("BEC_ShowImage") && reader["BEC_ShowImage"] != DBNull.Value)
                entity.BEC_ShowImage = Convert.ToString(reader["BEC_ShowImage"]);
            if (cols.Contains("BEC_KeyImage") && reader["BEC_KeyImage"] != DBNull.Value)
                entity.BEC_KeyImage = Convert.ToString(reader["BEC_KeyImage"]);
            if (cols.Contains("BEC_PublishCount") && reader["BEC_PublishCount"] != DBNull.Value)
                entity.BEC_PublishCount = Convert.ToInt32(reader["BEC_PublishCount"]);
            if (cols.Contains("BEC_Message") && reader["BEC_Message"] != DBNull.Value)
                entity.BEC_Message = Convert.ToString(reader["BEC_Message"]);
            if (cols.Contains("BVC_PrintOption") && reader["BVC_PrintOption"] != DBNull.Value)
                entity.BVC_PrintOption = Convert.ToInt32(reader["BVC_PrintOption"]);
            if (cols.Contains("BBR_Id") && reader["BBR_Id"] != DBNull.Value)
                entity.BBR_Id = new Guid(reader["BBR_Id"].ToString());
            if (cols.Contains("BAG_Id") && reader["BAG_Id"] != DBNull.Value)
                entity.BAG_Id = new Guid(reader["BAG_Id"].ToString());
            if (cols.Contains("BVC_PublishAll") && reader["BVC_PublishAll"] != DBNull.Value)
                entity.BVC_PublishAll = Convert.ToInt32(reader["BVC_PublishAll"]);
            if (cols.Contains("BVC_UseType") && reader["BVC_UseType"] != DBNull.Value)
                entity.BVC_UseType = Convert.ToInt32(reader["BVC_UseType"]);
            if (cols.Contains("BEC_HaveKey") && reader["BEC_HaveKey"] != DBNull.Value)
                entity.BEC_HaveKey = Convert.ToInt32(reader["BEC_HaveKey"]);
            if (cols.Contains("BVC_City") && reader["BVC_City"] != DBNull.Value)
                entity.BVC_City = Convert.ToInt32(reader["BVC_City"]);
            if (cols.Contains("Name") && reader["Name"] != DBNull.Value)
                entity.Name = reader["Name"].ToString();
            if (cols.Contains("bvc_coid") && reader["bvc_coid"] != DBNull.Value)
                entity.COID = reader["bvc_coid"].ToString();
            entity.SetBaseInfo(entity, reader, cols);

            return entity;
        }

        public override string ToString()
        {
            string str = "BEC_ECID:{0} BVC_VCID:{1} BEC_Title:{2} BEC_Content:{3} BEC_IsSpecial:{4} BEC_Detail:{5} BEC_UseBegindate:{6} BEC_UseEndDate:{7} BEC_PublishBeginDate:{8} BEC_PublishEndDate:{9} BEC_DisplayType:{10} BEC_ShowImage:{11} BEC_PublishCount:{12} BEC_Message:{13}BVC_PrintOption:{14}";
            return string.Format(str, BEC_ECID, BVC_VCID, BEC_Title, BEC_Content, BEC_IsSpecial, BEC_Detail, BEC_UseBegindate, BEC_UseEndDate, BEC_PublishBeginDate, BEC_PublishEndDate, BEC_DisplayType, BEC_ShowImage, BEC_PublishCount, BEC_Message, BVC_PrintOption);
        }
    }
    
}
