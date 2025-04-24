using System;
using System.Collections.Generic;
using System.Linq;

namespace ClinicaApp
{
    public class Paciente
    {
        public string Nome { get; set; }
        public string CPF { get; set; }

        public Paciente(string nome, string cpf)
        {
            Nome = nome;
            CPF = cpf;
        }
    }

    public class Medico
    {
        public string Nome { get; set; }
        public string CRM { get; set; }
        public List<Consulta> Consultas { get; private set; } = new();

        public Medico(string nome, string crm)
        {
            Nome = nome;
            CRM = crm;
        }

        public void AdicionarConsulta(Consulta consulta)
        {
            Consultas.Add(consulta);
        }

        public void RemoverConsulta(Consulta consulta)
        {
            Consultas.Remove(consulta);
        }
    }

    public class Consulta
    {
        public Paciente Paciente { get; set; }
        public DateTime DataHora { get; set; }
        public DateTime CriadoEm { get; private set; }

        public Consulta(Paciente paciente, DateTime dataHora)
        {
            Paciente = paciente;
            DataHora = dataHora;
            CriadoEm = DateTime.Now;
        }
    }
}
