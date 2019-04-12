using Rodonaves.EDI.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rodonaves.EDI.Helpers.GED
{
    public class Path : IGed
    {
        /// <summary>
        /// Autentica o acesso ao Provider
        /// </summary>
        /// <param name="parms">Parâmetros de acordo com o Provider utilizado</param>
        /// <returns>Retorna <see cref="true"/> caso ocorra autenticação e <see cref="false"/> caso não autentique o acesso</returns>
        bool IGed.Authenticate(object[,] parms)
        {
            //No caso do provider Path não é necessário autenticação
            return true;
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
        /// <param name="fileName">Caminho completo onde o arquivo foi armazenado</param>
        /// <returns>Retorna o caminho onde o arquivo está armazenado</returns>
        async Task<string> IGed.Dowload(object[,] parms)
        {
            return parms[0, 1].ToString();
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
        /// Armazena o arquivo de acordo com o parâmetro GED, atributo URL
        /// </summary>
        /// <param name="parms">Parâmetros de acordo com o Provider utilizado</param>
        /// <returns>Retorna chave (caminho completo onde o arquivo foi armazenado)</returns>
        string IGed.Upload(object[,] parms)
        {
            var filePath = string.Empty;

            var directory = string.Format(@"{0}{1}\{2}\{3}\", parms[3, 0].ToString(), DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"));

            filePath = AppendNewFile(directory, parms);

            return filePath;
        }

        /// <summary>
        /// Move o arquivo de acordo com o parâmetro GED, atributo URL
        /// </summary>
        /// <param name="parms">Parâmetros de acordo com o Provider utilizado</param>
        /// <returns>Retorna chave (caminho completo onde o arquivo foi armazenado)</returns>
        string IGed.Move(object[,] parms)
        {
            lock (parms)
            {
                var filePath = string.Empty;

                var directory = string.Format(@"{0}{1}\{2}\{3}\{4}\", parms[3, 0].ToString(), DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"), DateTime.Now.ToString("hh"));

                filePath = MoveFile(directory, parms);

                return filePath;
            }
        }

        /// <summary>
        /// Salva o arquivo no diretório informado no provider
        /// </summary>
        /// <param name="directory">Caminho onde o arquivo será salvo</param>
        /// <param name="parms">Array de objetos que contém o nome do arquivo e o arquivo que será salvo</param>
        /// <returns></returns>
        private static string AppendNewFile(string directory, object[,] parms)
        {
            var filePath = string.Empty;

            if (!Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }

            if (Directory.Exists(directory))
            {
                try
                {
                    var fileName = parms[0, 0].ToString(); //Posição do nome do arquivo
                    var ms = parms[0, 1] as MemoryStream;

                    filePath = string.Concat(directory, fileName);

                    using (FileStream file = new FileStream(filePath, FileMode.Create, System.IO.FileAccess.Write))
                    {
                        byte[] bytes = new byte[ms.Length];
                        ms.Read(bytes, 0, (int)ms.Length);
                        file.Write(bytes, 0, bytes.Length);
                        ms.Close();
                    }
                }
                catch (InvalidCastException)
                {
                    throw new InvalidCastException("O arquivo não está no formato correto");
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return filePath;
        }

        private static string MoveFile(string directory, object[,] parms)
        {
            int NumberOfRetries = 6;
            int DelayOnRetry = 10000;

            var filePath = string.Empty;

            if (!Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }

            if (Directory.Exists(directory))
            {
                var fileName = parms[0, 0].ToString(); //Posição do nome do arquivo
                var fileTemp = parms[0, 1].ToString();
                filePath = string.Concat(directory, fileName);

                for (int i = 1; i <= NumberOfRetries; ++i)
                {
                    try
                    {
                        File.Copy(fileTemp, filePath, true);
                        break;
                    }
                    catch (IOException) when (i <= NumberOfRetries)
                    {
                        Thread.Sleep(DelayOnRetry);
                    }
                    catch (InvalidCastException)
                    {
                        throw new InvalidCastException("O arquivo não está no formato correto");
                    }
                    catch (Exception)
                    {
                        throw new Exception(string.Format("Falha ao mover o arquivo {0} para {1}", fileTemp, filePath));
                    }
                }

                try
                {
                    File.Delete(fileTemp);
                }
                catch { }
            }
            return filePath;
        }
    }
}
