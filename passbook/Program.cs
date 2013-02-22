using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using passbook.Model;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Security;
using System.Collections;
using Org.BouncyCastle.Cms;
using System.Diagnostics;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Xml.Linq;
using VEBS.Contract.Common.DataContract.VeloWebManage.ECoupon;
using VEBS.Core.Infrastructure.WCF.Service;

namespace passbook
{
    class Program
    {

        private static System.Timers.Timer timerEvery;
        static void Main(string[] args)
        {

            //var xx="维络城你好吗好不好你说呀";
            //var yy = xx.PadLeft(20, ' ');
            timer_Elapsed(null, null);
            timerEvery = new System.Timers.Timer();
            timerEvery.AutoReset = true;
            timerEvery.Interval = 1000 * 60 * 600;
            timerEvery.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timerEvery.Start();
            Console.ReadLine();

            
        }
        static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            int count = 0;
            Console.WriteLine(DateTime.Now + ":生成开始！");
            List<int> idList = null;
            List<int> ErrorIDList = null;
            List<int> addlist = null;
            PageInfo ipage = new PageInfo(1, 50);
            do
            {

                var list = Business.Common.MyCommon.Instance.GetECouponListByPage(ref ipage);
                if (count == 0)
                    count = ipage.TotalCount;
                Console.WriteLine(DateTime.Now + ":剩余待更新数量：" + ipage.TotalCount);
                if (list != null && list.Count > 0)
                {
                  
                    ErrorIDList = new List<int>();
                    idList = new List<int>();
                    addlist = new List<int>();
                    foreach (var icoupon in list)
                    {
                      
                        #region 赋值
                        PassGenerator pg = new PassGenerator();
                        CouponPassGeneratorRequest cpsgr = new CouponPassGeneratorRequest();
                        //二维码
                        cpsgr.AddBarCode(icoupon.BEC_ECID.ToString(), BarcodeType.PKBarcodeFormatPDF417, "iso-8859-1", "COID:" + icoupon.COID.Trim());
                        //更新服务
                        cpsgr.authenticationToken = "vxwxd7J8AlNNFPS8k0a0FfUFtq0ewzFdc";
                        cpsgr.webServiceURL = "https://www.velo.com.cn";
                        //优惠券内容
                        cpsgr.coupon = new coupons();
                        cpsgr.coupon.primaryFields = new List<Fields>();
                        
                        //正面标题
                        List<Fields> pf = new List<Fields>();

                        string UseType = string.Empty;
                        if ((icoupon.BVC_UseType & 1) == 1)
                        {
                            //cpsgr.backgroundColor = "rgb(255, 108,0 )";
                            UseType = "『出示即可使用』";
                        }
                        else if ((icoupon.BVC_UseType & 4) == 4)
                        {
                            //cpsgr.backgroundColor = "rgb(116, 0, 28)";
                            UseType = "『需刷维络卡/报手机号验证使用』";
                        }
                        else if ((icoupon.BVC_UseType & 2) == 2)
                        {
                            //cpsgr.backgroundColor = "rgb(16, 16, 16)";
                            UseType = "『维络城终端打印』";
                        }

                        cpsgr.foregroundColor = "rgb(255, 255, 255)";
                        pf.Add(new Fields("标题", UseType, icoupon.BEC_Title));
                        cpsgr.coupon.primaryFields.AddRange(pf);

                        //正面内容
                        cpsgr.coupon.auxiliaryFields = new List<Fields>();
                        List<Fields> af = new List<Fields>();
                        af.Add(new Fields("内容", "详情阅读背面说明，有效期：", icoupon.BEC_UseEndDate.ToString("yyyy/MM/dd")));
                        cpsgr.coupon.auxiliaryFields.AddRange(af);
                        if (!string.IsNullOrEmpty(icoupon.Name))
                        {
                            var byteLength = System.Text.Encoding.Default.GetBytes(icoupon.Name).Length;
                            if (byteLength > 19)
                            { cpsgr.logoText = icoupon.Name; }
                            else
                            {
                                cpsgr.logoText = icoupon.Name.PadLeft(30 - byteLength, ' ');
                            }
                        }
                        else
                        {
                            WriteLog.SetTradeLog("VCID=" + icoupon.BVC_VCID + "|品牌不存在，无法生成PassBook");
                            ErrorIDList.Add(icoupon.BEC_ECID);
                            continue;
                        }
                        //背面
                        cpsgr.coupon.backFields = new List<Fields>();
                        List<Fields> bf = new List<Fields>();
                        StringBuilder rmk = new StringBuilder();
                        //rmk.Append("1.该优惠券有效期：").Append(icoupon.BEC_UseBegindate.ToString("yyyy年MM月dd日")).Append("-").Append(icoupon.BEC_UseEndDate.ToString("yyyy年MM月dd日")).AppendLine("；");
                        //rmk.AppendLine("2.凭券可以享受以下优惠:");
                        rmk.AppendLine(icoupon.BEC_Content);
                        //rmk.AppendLine("                           ");
                        //rmk.AppendLine("客户热线：400-880-8356                           ");
                        //rmk.AppendLine("官方网站：velo.com.cn                            ");
                        //rmk.AppendLine("Copyright" + DateTime.Now.Year.ToString() + "           维络城");
                        bf.Add(new Fields("维络城","条款说明", rmk.ToString()));
                        //rmk = new StringBuilder();
                        
                        //bf.Add(new Fields("维络城1","",rmk.ToString()  ));
                        cpsgr.coupon.backFields.AddRange(bf);
                       
                        List<Contract.Common.DataContract.BCUserDC> BClist = null;//new List<Contract.Common.DataContract.BCUserDC>();

                        Guid? iBCEID = null;
                        List<Guid> iIDList = null;
                        //经纬度
                        //是否推送所有门店
                        if (icoupon.BVC_PublishAll == 1)
                        {
                            iBCEID = icoupon.BBR_Id;
                        }
                        else
                        {
                            //发布门店IDlist
                            iIDList = Business.Common.MyCommon.Instance.GetBCUserIDList(icoupon.BVC_VCID);
                        }
                        BClist = Business.Common.MyCommon.Instance.GetBCUserList(iIDList, iBCEID, icoupon.BVC_City);

                        cpsgr.locations = new List<location>();
                        if (BClist != null && BClist.Count > 0)
                        {
                           
                            //cpsgr.logoText = BClist[0].BCName.PadLeft(20, ' ');
                            
                            foreach (var bcuser in BClist)
                            {
                                if (string.IsNullOrEmpty(bcuser.Longitude) || string.IsNullOrEmpty(bcuser.Dimensions))
                                { }
                                else
                                    cpsgr.locations.Add(new location(Convert.ToDouble(bcuser.Longitude), Convert.ToDouble(bcuser.Dimensions), 0, bcuser.Name));
                            }
                        }
                       
                        cpsgr.backgroundColor = GetBackgroundColor(icoupon.BVC_VCID, cpsgr.logoText.Trim());
                        //有效期
                        //cpsgr.relevantDate = "2013-10-11T10:03+08:00";
                        cpsgr.relevantDate = icoupon.BEC_UseEndDate.ToString("yyyy-MM-dd'T'HH:mmzzz");
                        cpsgr.serialNumber = icoupon.BVC_VCID.ToString();
                        //cpsgr.teamIdentifier = icoupon.BBR_Id.ToString().Replace("-","");
                        pg.Generate(cpsgr,icoupon.BVC_VCID);
                        idList.Add(icoupon.BEC_ECID);
                        addlist.Add(icoupon.BEC_ECID);
                        #endregion
                    }
                    //成功的更新
                    Business.Common.MyCommon.Instance.UpdateECouponHaveKey(idList, 1);
                    //错误的更新
                    Business.Common.MyCommon.Instance.UpdateECouponHaveKey(ErrorIDList, 2);
                }
                if(addlist!=null)
                    Console.WriteLine(DateTime.Now + ":生成" + addlist.Count + "条Passbook");
                else
                    Console.WriteLine(DateTime.Now + ":生成0条Passbook");
            } while (ipage.TotalCount >0);


            //错误的归零
            Business.Common.MyCommon.Instance.UpdateECouponHaveKey();
            Console.WriteLine(DateTime.Now+ ":生成完毕！");
        }
        //获取背景色
        static string GetBackgroundColor(int iBVC_VCID,string iBCName)
        {
            //if (iBVC_VCID == 206)
            //{ }
            string rtn = "";
            //获取大类
            string keyword = "其他";
            int intAsciiCode = 48;
            var keyword1 = Business.Common.MyCommon.Instance.GetKeyword(iBVC_VCID).Trim();
            if (!string.IsNullOrEmpty(keyword1))
                keyword = keyword1;

            if (!string.IsNullOrEmpty(iBCName))
            {
               
                var first = PinyinHelper.GetShortPinyin(iBCName.Substring(0, 1)).ToLower();
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                var gb= asciiEncoding.GetBytes(first);//[0]
                if(gb!=null&&gb.Count()>0)
                {
                    intAsciiCode=(int)gb[0];
                }
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                switch (keyword)
                {
                    #region 餐饮美食
                    case "餐饮美食":
                        //纯数字
                        if (intAsciiCode >= 48 && intAsciiCode <= 57)
                        {
                            rtn = "rgb(255,109,119)";
                        }
                        // a-d
                        else if (intAsciiCode >= 97 && intAsciiCode <= 100)
                        {
                            rtn = "rgb(249,47,61)";
                        }
                        // e-h
                        else if (intAsciiCode >= 101 && intAsciiCode <= 104)
                        {
                            rtn = "rgb(228,23,38)";
                        }
                        // j-k
                        else if (intAsciiCode >= 105 && intAsciiCode <= 108)
                        {
                            rtn = "rgb(194,9,70)";
                        }
                        // l-p
                        else if (intAsciiCode >= 109 && intAsciiCode <= 112)
                        {
                            rtn = "rgb(155,0,51)";
                        }
                        // q-r
                        else if (intAsciiCode >= 113 && intAsciiCode <= 116)
                        {
                            rtn = "rgb(123,1,41)";
                        }
                        // s-x
                        else if (intAsciiCode >= 117 && intAsciiCode <= 120)
                        {
                            rtn = "rgb(79,1,25)";
                        }
                        // y-z
                        else if (intAsciiCode >= 121 && intAsciiCode <= 122)
                        {
                            rtn = "rgb(42,5,17)";
                        }
                        
                        break;
                    #endregion
                    #region 休闲娱乐
                    case "休闲娱乐":
                        //纯数字
                        if (intAsciiCode >= 48 && intAsciiCode <= 57)
                        {
                            rtn = "rgb(254,221,0)";
                        }
                        // a-d
                        else if (intAsciiCode >= 97 && intAsciiCode <= 100)
                        {
                            rtn = "rgb(248,192,0)";
                        }
                        // e-h
                        else if (intAsciiCode >= 101 && intAsciiCode <= 104)
                        {
                            rtn = "rgb(255,204,0)";
                        }
                        // j-k
                        else if (intAsciiCode >= 105 && intAsciiCode <= 108)
                        {
                            rtn = "rgb(255,127,0)";
                        }
                        // l-p
                        else if (intAsciiCode >= 109 && intAsciiCode <= 112)
                        {
                            rtn = "rgb(229,102,0)";
                        }
                        // q-r
                        else if (intAsciiCode >= 113 && intAsciiCode <= 116)
                        {
                            rtn = "rgb(207,77,0)";
                        }
                        // s-x
                        else if (intAsciiCode >= 117 && intAsciiCode <= 120)
                        {
                            rtn = "rgb(163,51,0)";
                        }
                        // y-z
                        else if (intAsciiCode >= 121 && intAsciiCode <= 122)
                        {
                            rtn = "rgb(97,27,0)";
                        }
                        break;
                    #endregion
                    #region 其他
                    case "其他":
                        //纯数字
                        if (intAsciiCode >= 48 && intAsciiCode <= 57)
                        {
                            rtn = "rgb(0,221,205)";
                        }
                        // a-d
                        else if (intAsciiCode >= 97 && intAsciiCode <= 100)
                        {
                            rtn = "rgb(0,207,192)";
                        }
                        // e-h
                        else if (intAsciiCode >= 101 && intAsciiCode <= 104)
                        {
                            rtn = "rgb(0,181,169)";
                        }
                        // j-k
                        else if (intAsciiCode >= 105 && intAsciiCode <= 108)
                        {
                            rtn = "rgb(0,151,141)";
                        }
                        // l-p
                        else if (intAsciiCode >= 109 && intAsciiCode <= 112)
                        {
                            rtn = "rgb(0,119,110)";
                        }
                        // q-r
                        else if (intAsciiCode >= 113 && intAsciiCode <= 116)
                        {
                            rtn = "rgb(0,90,83)";
                        }
                        // s-x
                        else if (intAsciiCode >= 117 && intAsciiCode <= 120)
                        {
                            rtn = "rgb(0,66,62)";
                        }
                        // y-z
                        else if (intAsciiCode >= 121 && intAsciiCode <= 122)
                        {
                            rtn = "rgb(0,50,46)";
                        }

                        break;
                    #endregion
                    #region VELO自用
                    case "VELO自用":
                        //纯数字
                        if (intAsciiCode >= 48 && intAsciiCode <= 57)
                        {
                            rtn = "rgb(208,208,208)";
                        }
                        // a-d
                        else if (intAsciiCode >= 97 && intAsciiCode <= 100)
                        {
                            rtn = "rgb(187,187,187)";
                        }
                        // e-h
                        else if (intAsciiCode >= 101 && intAsciiCode <= 104)
                        {
                            rtn = "rgb(160,160,160)";
                        }
                        // j-k
                        else if (intAsciiCode >= 105 && intAsciiCode <= 108)
                        {
                            rtn = "rgb(128,128,128)";
                        }
                        // l-p
                        else if (intAsciiCode >= 109 && intAsciiCode <= 112)
                        {
                            rtn = "rgb(93,93,93)";
                        }
                        // q-r
                        else if (intAsciiCode >= 113 && intAsciiCode <= 116)
                        {
                            rtn = "rgb(68,68,68)";
                        }
                        // s-x
                        else if (intAsciiCode >= 117 && intAsciiCode <= 120)
                        {
                            rtn = "rgb(40,40,40)";
                        }
                        // y-z
                        else if (intAsciiCode >= 121 && intAsciiCode <= 122)
                        {
                            rtn = "rgb(17,17,17)";
                        }

                        break;
                    #endregion

                    #region 购物逛街/教育培训/生活服务
                    //购物逛街/教育培训/生活服务	
                    default:
                        //纯数字
                        if (intAsciiCode >= 48 && intAsciiCode <= 57)
                        {
                            rtn = "rgb(28,198,255)";
                        }
                        // a-d
                        else if (intAsciiCode >= 97 && intAsciiCode <= 100)
                        {
                            rtn = "rgb(10,180,237)";
                        }
                        // e-h
                        else if (intAsciiCode >= 101 && intAsciiCode <= 104)
                        {
                            rtn = "rgb(10,154,237)";
                        }
                        // j-k
                        else if (intAsciiCode >= 105 && intAsciiCode <= 108)
                        {
                            rtn = "rgb(10,128,237)";
                        }
                        // l-p
                        else if (intAsciiCode >= 109 && intAsciiCode <= 112)
                        {
                            rtn = "rgb(12 112 206)";
                        }
                        // q-r
                        else if (intAsciiCode >= 113 && intAsciiCode <= 116)
                        {
                            rtn = "rgb(24,75,157)";
                        }
                        // s-x
                        else if (intAsciiCode >= 117 && intAsciiCode <= 120)
                        {
                            rtn = "rgb(20,57,112)";
                        }
                        // y-z
                        else if (intAsciiCode >= 121 && intAsciiCode <= 122)
                        {
                            rtn = "rgb(20,37,64)";
                        }

                        break;
                    #endregion

                }
            }
            return rtn;
        }

      
       

    }


}
