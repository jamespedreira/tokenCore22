using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rodonaves.EDI.Helpers.Interfaces
{
    public interface IGed
    {
        /// <summary>
        /// Autentica o acesso ao Provider
        /// </summary>
        /// <param name="parms">Parâmetros de acordo com o Provider utilizado</param>
        /// <returns>Retorna <see cref="true"/> caso ocorra autenticação e <see cref="false"/> caso não autentique o acesso</returns>
        bool Authenticate(object[,] parms);

        /// <summary>
        /// Armazena o arquivo de acordo com o Provider escolhido e o que está definido no parâmetro GED, atributo URL
        /// </summary>
        /// <param name="parms">Parâmetros de acordo com o Provider utilizado</param>
        /// <returns>Retorna chave (caminho completo onde o arquivo foi armazenado)</returns>
        string Upload(object[,] parms);

        /// <summary>
        /// Move o arquivo de acordo com o Provider escolhido e o que está definido no parâmetro GED, atributo URL
        /// </summary>
        /// <param name="parms">Parâmetros de acordo com o Provider utilizado</param>
        /// <returns>Retorna chave (caminho completo onde o arquivo foi armazenado)</returns>
        string Move(object[,] parms);

        /// <summary>
        /// Recupera o arquivo buscando no Provider selecionado
        /// </summary>
        /// <param name="fileName">Caminho completo onde o arquivo foi armazenado</param>
        /// <returns>Retorna o caminho onde o arquivo está armazenado</returns>
        Task<string> Dowload(object[,] parms);

        /// <summary>
        /// Valida criação do caminho caso não exista
        /// </summary>
        /// <returns>Retorna <see cref="true"/> caso ocorra com sucesso e <see cref="false"/> caso ocorra falha</returns>
        bool CreatPath();

        /// <summary>
        /// Busca o local de armazenamento do arquivo conforme o Provider
        /// </summary>
        /// <param name="provider"></param>
        /// <returns>Retorna o caminho de armazenamento do arquivo conforme o Provider</returns>
        string GetUrl(string provider);
    }
}
