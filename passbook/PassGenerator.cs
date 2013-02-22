using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using passbook.Model;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Security;
using System.Collections;
using Org.BouncyCastle.Cms;
using System.Diagnostics;
using System.Security.Cryptography;
using Newtonsoft.Json;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.GZip;
using System.IO.Compression;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;


namespace passbook
{
    public class PassGenerator
    {

        string iPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\"; 

        public Pass Generate(PassGeneratorRequest request,int  couponID)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            string pathToPackage = CreatePackage(request, couponID);
            ZipFile1(pathToPackage, pathToPackage.Replace("contents", "") + "\\" + couponID.ToString() + ".pkpass", "");
            string pathToZip = "";//= ZipFile(pathToPackage,1,10000);

            return new Pass(pathToZip);
        }


        /// <summary>
        /// 把一个目录下的所有文件压缩到zip文件
        /// </summary>
        /// <param name="FileToZip">要进行压缩的文件目录</param>
        /// <param name="ZipedFile">压缩后生成的压缩文件名</param>
        /// <returns></returns>
        private static bool ZipFile1(string FolderToZip, string FileToZip, string PassWord)
        {
            bool res = true;
            string[] filenames;
            ZipEntry entry = null;
            FileStream fs = null;
            Crc32 crc = new Crc32();

            //新建一个zip文件
            ZipOutputStream s = new ZipOutputStream(File.Create(FileToZip));
            s.SetLevel(9);
            //s.Password = PassWord;
            try
            {
                filenames = Directory.GetFiles(FolderToZip);
                foreach (string file in filenames)
                {
                    //打开压缩文件
                    fs = File.OpenRead(file);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    entry = new ZipEntry(Path.GetFileName(file));

                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();

                    crc.Reset();
                    crc.Update(buffer);

                    entry.Crc = crc.Value;

                    s.PutNextEntry(entry);

                    s.Write(buffer, 0, buffer.Length);
                }
                
                s.Finish();
                s.Close();
            }
            catch
            {
                res = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
                if (entry != null)
                {
                    entry = null;
                }

                GC.Collect();
                GC.Collect(1);
            }
            return res;
        }

        private string CreatePackage(PassGeneratorRequest request, int couponID)
        {
           
            //Path.GetTempPath(), 
            string tempPath = Path.Combine(System.Configuration.ConfigurationManager.AppSettings["CouponPath"], couponID.ToString(), "contents");
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath,true);
            Directory.CreateDirectory(tempPath);
            var logoFile = iPath + "Image/logo.png";
            var logoRetinaFile = iPath + "Image/logo@2x.png";
            var backgroundFile = "";//"D:/www/background.png";//lvma.png";
            var backgroundRetinaFile ="";// "D:/www/background@2x.png";//Snap12.png";
            var iconFile = iPath + "Image/icon.png";
            var iconRetinaFile = iPath + "Image/icon@2x.png";
            CopyImageFiles(request, tempPath, logoFile, logoRetinaFile, backgroundFile, backgroundRetinaFile, iconFile, iconRetinaFile);
          
            var jsonStr = Tojson(request);
            WriteText(jsonStr, Path.Combine(tempPath, "pass.json"));
            GenerateManifestFile(request, tempPath);
            return tempPath;
        }

        private void CopyImageFiles(PassGeneratorRequest request, string tempPath, string logoFile, string logoRetinaFile, string backgroundFile, string backgroundRetinaFile, string iconFile, string iconRetinaFile)
        {

          //Path.Combine(tempPath,
            if (!string.IsNullOrEmpty(iconFile))
            {
                string targetIconFileAndPath = Path.GetFileName(iconFile);
                string targetIconRetinaFileAndPath =  Path.GetFileName(iconRetinaFile);

                File.Copy(iconFile, tempPath + "\\" + targetIconFileAndPath,true);
                File.Copy(iconRetinaFile, tempPath + "\\" + targetIconRetinaFileAndPath, true);
            }
            if (!string.IsNullOrEmpty(logoFile))
            {
                string targetLogoFileAndPath =Path.GetFileName(logoFile);
                string targetLogoRetinaFileAndPath = Path.GetFileName(logoRetinaFile);

                File.Copy(logoFile, tempPath + "\\" + targetLogoFileAndPath, true);
                File.Copy(logoRetinaFile, tempPath + "\\" + targetLogoRetinaFileAndPath, true);
            }
            if (!string.IsNullOrEmpty(backgroundFile))
            {
                string targetBackgroundFileAndPath =  Path.GetFileName(backgroundFile);
                string targetBackgroundRetinaFileAndPath = Path.GetFileName(backgroundRetinaFile);

                File.Copy(backgroundFile, tempPath + "\\" + targetBackgroundFileAndPath, true);
                File.Copy(backgroundRetinaFile, tempPath + "\\" + targetBackgroundRetinaFileAndPath, true);
            }
        }
       
        private  string Tojson(object item)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(item.GetType());
            using (MemoryStream ms = new MemoryStream())
            {

                serializer.WriteObject(ms, item);
                StringBuilder sb = new StringBuilder();
                sb.Append(Encoding.UTF8.GetString(ms.ToArray()));
                return sb.ToString();
            }
        }
        
        private void WriteText(string str, string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            StreamWriter streamw = File.CreateText(filePath);
            streamw.WriteLine(str);
            streamw.Close();
        }
      
        private void GenerateManifestFile(PassGeneratorRequest request, string tempPath)
        {

            string manifestFileAndPath = Path.Combine(tempPath, "manifest.json");
            if (File.Exists(manifestFileAndPath))
                File.Delete(manifestFileAndPath);
            var fileHashDic = new Dictionary<string, string>();
            foreach (var fi in Directory.GetFiles(tempPath))
            {
                fileHashDic.Add(Path.GetFileName(fi), GetHashForFile(fi).ToLower());
            }

            var json = JsonConvert.SerializeObject(fileHashDic);

            using (StreamWriter sw = File.CreateText(manifestFileAndPath))
            {
                sw.WriteLine(json);
            }


           
            SignManigestFile(request, manifestFileAndPath);
        }

        private void SignManigestFile(PassGeneratorRequest request, string manifestFileAndPath)
        {
            byte[] dataToSign = File.ReadAllBytes(manifestFileAndPath);

            var certificatePath = iPath + @"cer/pass.velo.coupon.p12";
            X509Certificate2 card = new X509Certificate2(certificatePath, "web@123",X509KeyStorageFlags.Exportable);
            //X509Certificate2 card = GetCertificate();
             Org.BouncyCastle.X509.X509Certificate cert = DotNetUtilities.FromX509Certificate(card);
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter privateKey = DotNetUtilities.GetKeyPair(card.PrivateKey).Private;

            var appleCAPath = iPath + @"cer/AppleWWDRCA.cer";
            X509Certificate2 appleCA = new X509Certificate2(appleCAPath);
            Org.BouncyCastle.X509.X509Certificate appleCert = DotNetUtilities.FromX509Certificate(appleCA);
            ArrayList intermediateCerts = new ArrayList();

            intermediateCerts.Add(appleCert);
            intermediateCerts.Add(cert);

            Org.BouncyCastle.X509.Store.X509CollectionStoreParameters PP = new Org.BouncyCastle.X509.Store.X509CollectionStoreParameters(intermediateCerts);
            Org.BouncyCastle.X509.Store.IX509Store st1 = Org.BouncyCastle.X509.Store.X509StoreFactory.Create("CERTIFICATE/COLLECTION", PP);

            CmsSignedDataGenerator generator = new CmsSignedDataGenerator();

            generator.AddSigner(privateKey, cert, CmsSignedDataGenerator.DigestSha1);
            generator.AddCertificates(st1);

            CmsProcessable content = new CmsProcessableByteArray(dataToSign);
            CmsSignedData signedData = generator.Generate(content, false);

            string outputDirectory = Path.GetDirectoryName(manifestFileAndPath);
            string signatureFileAndPath = Path.Combine(outputDirectory, "signature");
            if (File.Exists(signatureFileAndPath))
                File.Delete(signatureFileAndPath);
            File.WriteAllBytes(signatureFileAndPath, signedData.GetEncoded());
        }

        private static X509Certificate2 GetSpecifiedCertificate(string thumbPrint, StoreName storeName, StoreLocation storeLocation)
        {
            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certs = store.Certificates;

            if (certs.Count > 0)
            {
                for (int i = 0; i < certs.Count; i++)
                {
                    X509Certificate2 cert = certs[i];

                    Debug.WriteLine(cert.Thumbprint);

                    if (string.Compare(cert.Thumbprint, thumbPrint, true) == 0)
                    {
                        return certs[i];
                    }
                }
            }

            return null;
        }

        private string GetHashForFile(string fileAndPath)
        {

            using (FileStream fs = new FileStream(fileAndPath, FileMode.Open))
            {
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    byte[] hash = sha1.ComputeHash(fs);
                    StringBuilder formatted = new StringBuilder(hash.Length);
                    foreach (byte b in hash)
                    {
                        formatted.AppendFormat("{0:X2}", b);
                    }
                    return formatted.ToString();
                }
            }
            //SHA1CryptoServiceProvider oSHA1Hasher = new SHA1CryptoServiceProvider();
            //byte[] hashBytes;
            //using (FileStream fs = File.Open(fileAndPath, FileMode.Open))
            //{
            //    hashBytes = oSHA1Hasher.ComputeHash(fs);
            //}

            //string hash = System.BitConverter.ToString(hashBytes);
            //hash = hash.Replace("-", "");
            //return hash;
        }

       
    }


   
}
