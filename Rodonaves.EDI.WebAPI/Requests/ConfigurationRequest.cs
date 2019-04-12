using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rodonaves.EDI.WebAPI.Requests
{
    /// <summary>
    /// </summary>
    public class ConfigurationRequest
    {
        /// <summary>
        /// Identificador único da configuração
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do tipo de frequencia
        /// </summary>
        public int Frequence { get; set; }

        /// <summary>
        /// intervalo de execução da tarefa
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Dia de inicio da tarefa
        /// </summary>
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// Horário em que a tarefa irá iniciar o processo
        /// </summary>
        public TimeSpan BeginTime { get; set; }

        /// <summary>
        /// Data em que a tarefa vai deixar de processar
        /// </summary>
        public DateTime ExpireDate { get; set; }

        /// <summary>
        /// Horário em que a tarefa vai deixar de processar
        /// </summary>
        public TimeSpan ExpireTime { get; set; }

        /// <summary>
        /// Se a tarefa executa no domingo
        /// </summary>
        public bool Sunday { get; set; }

        /// <summary>
        /// Se a tarefa executa na segunda
        /// </summary>
        public bool Monday { get; set; }

        /// <summary>
        /// Se a tarefa executa na terça
        /// </summary>
        public bool Tuesday { get; set; }

        /// <summary>
        /// Se a tarefa executa na quarta
        /// </summary>
        public bool Wednesday { get; set; }

        /// <summary>
        /// Se a tarefa executa na quinta
        /// </summary>
        public bool Thursday { get; set; }

        /// <summary>
        /// Se a tarefa executa na sexta
        /// </summary>
        public bool Friday { get; set; }

        /// <summary>
        /// Se a tarefa executa no sabado
        /// </summary>
        public bool Saturday { get; set; }

        /// <summary>
        /// Se a tarefa está habilitada
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// Dias especificos do mês que a tarefa irá processar
        /// </summary>
        public IEnumerable<int> DaysOfMonth { get; set; }

        /// <summary>
        /// Meses especificos em que a tarefa irá processar
        /// </summary>
        public IEnumerable<int> Months { get; set; }
    }
}
