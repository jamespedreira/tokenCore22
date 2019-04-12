using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Rodonaves.EDI.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.Helpers.GED
{
    public class S3 : IGed
    {
        static string AWS_ACCESS_KEY = string.Empty;
        static string AWS_SECRET_KEY = string.Empty;
        static string AmazonUrl = "https://s3-sa-east-1.amazonaws.com/ged.rte.com.br/";
        static string AmazonAccessKeyId = "?AWSAccessKeyId";

        protected static class Accessor
        {
            static object objLck = new object();

            static AmazonS3Client amazonS3Client;
            public static AmazonS3Client AmazonS3Client
            {
                get
                {
                    lock (objLck)
                    {
                        if (amazonS3Client == null)
                            amazonS3Client = new AmazonS3Client(AWS_ACCESS_KEY, AWS_SECRET_KEY, Amazon.RegionEndpoint.SAEast1);
                        return amazonS3Client;
                    }
                }
            }
        }

        /// <summary>
        /// Autentica o acesso ao Provider
        /// </summary>
        /// <param name="parms">Parâmetros de acordo com o Provider utilizado</param>
        /// <returns>Retorna <see cref="true"/> caso ocorra autenticação e <see cref="false"/> caso não autentique o acesso</returns>
        bool IGed.Authenticate(object[,] parms)
        {
            AWS_ACCESS_KEY = parms[1, 0].ToString();
            AWS_SECRET_KEY = parms[2, 0].ToString();
            return Accessor.AmazonS3Client != null;
        }

        /// <summary>
        /// Valida criação do caminho caso não exista
        /// </summary>
        /// <returns>Retorna <see cref="true"/> caso ocorra com sucesso e <see cref="false"/> caso ocorra falha</returns>
        bool IGed.CreatPath()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Recupera o arquivo buscando no Provider selecionado
        /// </summary>
        /// <param name="parms">Parâmetros de acordo com o Provider utilizado</param>
        /// <returns>Retorna o caminho onde o arquivo está armazenado</returns>
        async Task<string> IGed.Dowload(object[,] parms)
        {
            try
            {
                var ret = System.IO.Path.GetTempFileName();
                var directory = parms[3, 1].ToString();
                var fileName = parms[0, 0].ToString();
                
                using (var stream = WebRequest.Create(fileName).GetResponse().GetResponseStream())
                {
                    using (var ms = new MemoryStream())
                    {
                        await stream.CopyToAsync(ms);

                        fileName = fileName.Substring(0, fileName.IndexOf("?")).Substring(fileName.LastIndexOf("/") + 1);
                        ret = System.IO.Path.Combine(Environment.GetEnvironmentVariable("TEMP"), fileName);

                        File.WriteAllBytes(ret, ms.ToArray());
                    }
                }

                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Busca o local de armazenamento do arquivo conforme o Provider
        /// </summary>
        /// <param name="provider"></param>
        /// <returns>Retorna o caminho de armazenamento do arquivo conforme o Provider</returns>
        string IGed.GetUrl(string provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Move o arquivo de acordo com o parâmetro GED, atributo URL
        /// </summary>
        /// <param name="parms">Parâmetros de acordo com o Provider utilizado</param>
        /// <returns>Retorna chave (caminho completo onde o arquivo foi armazenado)</returns>
        string IGed.Move(object[,] parms)
        {
            return UploadToS3(parms);
        }

        /// <summary>
        /// Armazena o arquivo de acordo com o parâmetro GED, atributo URL
        /// </summary>
        /// <param name="parms">Parâmetros de acordo com o Provider utilizado</param>
        /// <returns>Retorna chave (caminho completo onde o arquivo foi armazenado)</returns>
        string IGed.Upload(object[,] parms)
        {
            return UploadToS3(parms);
        }

        /// <summary>
        /// Faz upload do arquivo para o servidor S3 da Amazon
        /// </summary>
        /// <param name="parms">Array de objetos que contém o nome do arquivo e o arquivo que será salvo</param>
        /// <returns></returns>
        private string UploadToS3(object[,] parms)
        {
            var strKey = string.Empty;

            try
            {
                var expires = DateTime.Now.AddDays(int.Parse(parms[4, 0].ToString()));

                TransferUtility fileTransferUtility = new TransferUtility(Accessor.AmazonS3Client);

                if (parms[0, 1].GetType() == typeof(FileStream))
                {
                    TransferUtilityUploadRequest fileTransferUtilityRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = parms[3, 1].ToString(),
                        InputStream = (FileStream)parms[0, 1],
                        StorageClass = S3StorageClass.ReducedRedundancy,
                        Key = parms[0, 0].ToString(),
                        CannedACL = S3CannedACL.PublicRead
                    };
                    fileTransferUtilityRequest.Headers.Expires = expires;
                    fileTransferUtility.Upload(fileTransferUtilityRequest);
                }
                else if (parms[0, 1].GetType() == typeof(MemoryStream))
                {
                    TransferUtilityUploadRequest fileTransferUtilityRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = parms[3, 1].ToString(),
                        InputStream = (MemoryStream)parms[0, 1],
                        StorageClass = S3StorageClass.ReducedRedundancy,
                        Key = parms[0, 0].ToString(),
                        CannedACL = S3CannedACL.PublicRead
                    };
                    fileTransferUtilityRequest.Headers.Expires = expires;
                    fileTransferUtility.Upload(fileTransferUtilityRequest);
                }
                else
                {
                    TransferUtilityUploadRequest fileTransferUtilityRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = parms[3, 1].ToString(),
                        FilePath = parms[0, 1].ToString(),
                        StorageClass = S3StorageClass.ReducedRedundancy,
                        Key = parms[0, 0].ToString(),
                        CannedACL = S3CannedACL.PublicRead
                    };
                    fileTransferUtilityRequest.Headers.Expires = expires;
                    fileTransferUtility.Upload(fileTransferUtilityRequest);
                }

                strKey = GeneratePreSignedURL(parms);
            }
            catch (Exception exc)
            {
                throw;
            }

            return strKey;
        }

        static string GeneratePreSignedURL(object[,] parms)
        {
            var urlString = string.Empty;
            var expires = DateTime.Now.AddDays(int.Parse(parms[4, 0].ToString()));

            GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
            {
                BucketName = parms[3, 1].ToString(),
                Key = parms[0, 0].ToString(),
                Expires = expires
            };

            try
            {
                urlString = Accessor.AmazonS3Client.GetPreSignedURL(request);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Verifique as credenciais AWS fornecidas.");
                }
                else
                {
                    throw new Exception(string.Format(" Ocorreu um erro. Mensagem:'{0}' ao listar os objetos", amazonS3Exception.Message));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return urlString;
        }
    }
}
