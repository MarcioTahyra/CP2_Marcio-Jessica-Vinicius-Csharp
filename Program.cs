using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ClinicaApp
{
    class Program
    {
        static List<Paciente> pacientes = new();
        static List<Medico> medicos = new();

        static void Main(string[] args)
        {
            string opcao;
            do
            {
                Console.WriteLine("\n--- Menu ---");
                Console.WriteLine("1. Cadastrar Paciente");
                Console.WriteLine("2. Cadastrar Médico");
                Console.WriteLine("3. Agendar Consulta");
                Console.WriteLine("4. Listar Consultas por Médico");
                Console.WriteLine("5. Alterar Consultas (Adicionar/Remover)");
                Console.WriteLine("6. Relatório Diário");
                Console.WriteLine("0. Sair");
                Console.Write("Escolha uma opção: ");
                opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1": CadastrarPaciente(); break;
                    case "2": CadastrarMedico(); break;
                    case "3": AgendarConsulta(); break;
                    case "4": ListarConsultas(); break;
                    case "5": AlterarConsultas(); break;
                    case "6": RelatorioDiario(); break;
                    case "0": break;
                    default: Console.WriteLine("Opção inválida."); break;
                }
            } while (opcao != "0");
        }

        static void CadastrarPaciente()
        {
            Console.Write("Nome do paciente: ");
            string nome = Console.ReadLine();
            Console.Write("CPF: ");
            string cpf = Console.ReadLine();
            pacientes.Add(new Paciente(nome, cpf));
            Console.WriteLine("Paciente cadastrado.");
        }

        static void CadastrarMedico()
        {
            Console.Write("Nome do médico: ");
            string nome = Console.ReadLine();
            Console.Write("CRM: ");
            string crm = Console.ReadLine();
            medicos.Add(new Medico(nome, crm));
            Console.WriteLine("Médico cadastrado.");
        }

        static void AgendarConsulta()
        {
            if (!ValidarCadastro()) return;

            Console.Write("CPF do paciente: ");
            string cpf = Console.ReadLine();
            var paciente = pacientes.FirstOrDefault(p => p.CPF == cpf);
            if (paciente == null) { Console.WriteLine("Paciente não encontrado."); return; }

            Console.Write("CRM do médico: ");
            string crm = Console.ReadLine();
            var medico = medicos.FirstOrDefault(m => m.CRM == crm);
            if (medico == null) { Console.WriteLine("Médico não encontrado."); return; }

            Console.Write("Data e hora da consulta (dd/MM/yyyy HH:mm): ");
            string input = Console.ReadLine();
            if (!DateTime.TryParseExact(input, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataHora))
            {
                Console.WriteLine("Data e hora inválidas. Use o formato dd/MM/yyyy HH:mm.");
                return;
            }

            var consulta = new Consulta(paciente, dataHora);
            medico.AdicionarConsulta(consulta);

            Console.WriteLine("Consulta agendada com sucesso.");
        }


        static void ListarConsultas()
        {
            Console.Write("CRM do médico: ");
            string crm = Console.ReadLine();
            var medico = medicos.FirstOrDefault(m => m.CRM == crm);
            if (medico == null)
            {
                Console.WriteLine("Médico não encontrado.");
                return;
            }

            ListarConsultas(medico);
        }

        static void ListarConsultas(Medico medico)
        {
            Console.WriteLine($"\nConsultas do Dr. {medico.Nome}:");
            foreach (var c in medico.Consultas.OrderBy(c => c.DataHora))
            {
                Console.WriteLine($"{c.DataHora:dd/MM/yyyy HH:mm} - Paciente: {c.Paciente.Nome}");
            }
        }


        static void AlterarConsultas()
        {
            Console.Write("CRM do médico: ");
            string crm = Console.ReadLine();
            var medico = medicos.FirstOrDefault(m => m.CRM == crm);
            if (medico == null)
            {
                Console.WriteLine("Médico não encontrado.");
                return;
            }

            ListarConsultas(medico);

            Console.WriteLine("Digite a data e hora da consulta que deseja remover (dd/MM/yyyy HH:mm)");
            Console.WriteLine("Ou digite 'cancelar' para voltar ao menu.");
            string input = Console.ReadLine();

            if (input.ToLower() == "cancelar")
            {
                Console.WriteLine("Operação cancelada.");
                return;
            }

            if (!DateTime.TryParseExact(input, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataHora))
            {
                Console.WriteLine("Data inválida.");
                return;
            }

            var consulta = medico.Consultas.FirstOrDefault(c => c.DataHora == dataHora);
            if (consulta != null)
            {
                medico.RemoverConsulta(consulta);
                Console.WriteLine("Consulta removida.");
            }
            else
            {
                Console.WriteLine("Consulta não encontrada.");
            }
        }



        static void RelatorioDiario()
        {
            Console.Write("Data do relatório (dd/MM/yyyy): ");
            string input = Console.ReadLine();
            if (!DateTime.TryParseExact(input, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime data))
            {
                Console.WriteLine("Data inválida. Use o formato dd/MM/yyyy.");
                return;
            }


            var consultasDoDia = medicos.SelectMany(m => m.Consultas)
                .Where(c => c.DataHora.Date == data.Date)
                .OrderBy(c => c.DataHora)
                .ToList();

            if (consultasDoDia.Count == 0)
            {
                Console.WriteLine("Nenhuma consulta nesse dia.");
                return;
            }

            var primeira = consultasDoDia.First().DataHora;
            var ultima = consultasDoDia.Last().DataHora;

            if (consultasDoDia.Count > 1)
            {
                var intervalos = new List<TimeSpan>();
                for (int i = 1; i < consultasDoDia.Count; i++)
                    intervalos.Add(consultasDoDia[i].DataHora - consultasDoDia[i - 1].DataHora);

                var intervaloMedio = TimeSpan.FromMinutes(intervalos.Average(i => i.TotalMinutes));

                Console.WriteLine($"Total de Consultas: {consultasDoDia.Count}");
                Console.WriteLine($"Primeira consulta: {primeira:HH:mm}");
                Console.WriteLine($"Última consulta: {ultima:HH:mm}");
                Console.WriteLine($"Intervalo médio: {intervaloMedio.Hours} horas e {intervaloMedio.Minutes} minutos");

            }
            else
            {
                Console.WriteLine("Intervalo médio: não aplicável (apenas uma consulta no dia).");
            }

        }

        static bool ValidarCadastro()
        {
            if (pacientes.Count == 0 || medicos.Count == 0)
            {
                Console.WriteLine("Cadastre pelo menos um paciente e um médico primeiro.");
                return false;
            }
            return true;
        }
    }
}
