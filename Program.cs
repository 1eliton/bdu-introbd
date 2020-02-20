using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bigdataintro
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var tempo = DateTime.Now;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Title = "Aula 1 - intro a big data";

            try
            {
                await new Gerar("https://www.gutenberg.org/files/", 10)
                    .runLeituraLivrosAsync();

                //await Task.Run(async () => await new Gerar("https://www.gutenberg.org/files/", 10).runLeituraLivrosAsync());
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}. Pressione qualquer tecla para sair!");
                //await Task.Delay(5000);
            }
            finally
            {

            }

            //await Task.Delay(5000);
            Console.WriteLine($"\n\nFinalizado!! Tempo: {DateTime.Now - tempo}. Pressione qualquer tecla para sair!");
            //Console.ReadKey();
        }
    }

    public class Gerar
    {
        private int TaskTimeout { get; set; }
        private string BaseUrl { get; set; }

        public Gerar(string baseUrl, int timeout = 0)
        {
            BaseUrl = baseUrl;
            TaskTimeout = timeout;
        }

        private Task<Stream> getStreamAsync(string filename)
        {
            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.130 Safari/537.36");

                var task = client.OpenReadTaskAsync($"{BaseUrl}{filename}");
                return task;
            }
            catch (StackOverflowException soe)
            {
                throw;
            }
        }

        private Stream getStream(string filename)
        {
            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.130 Safari/537.36");

                return client.OpenRead($"{BaseUrl}{filename}");
            }
            catch (StackOverflowException se)
            {
                return null;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private async Task<string> leituraAsync(Stream stream)
        {
            var dado = await new StreamReader(stream).ReadToEndAsync();
            lock (dado)
            {
                return dado;
            }
        }

        private string leitura(Stream stream)
            => stream != null ? new StreamReader(stream).ReadToEnd() : "";

        private async Task gerarIteracaoAsync(string fileName)
        {
            try
            {
                var stream = await getStreamAsync(fileName);
                var dir = "C:\\TempDBClass\\" + $"{fileName}".Split("/").ToList().LastOrDefault();
                // Create a FileStream with mode CreateNew  
                var local = new FileStream(dir, FileMode.OpenOrCreate);
                // Create a StreamWriter from FileStream  
                using (StreamWriter writer = new StreamWriter(local, Encoding.UTF8))
                {
                    var readed = await leituraAsync(stream);
                    Console.Write($"\nSucesso ao baixar arquivo {fileName}");
                    await writer.WriteAsync(readed);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("404"))
                {
                    string newFileName;
                    var lastFileName = fileName;

                    if (fileName.Contains("-0.txt"))
                        newFileName = fileName.Replace("-0.txt", "-5.txt");
                    else if (fileName.Contains("-5.txt"))
                        newFileName = fileName.Replace("-5.txt", "-8.txt");
                    else
                        newFileName = fileName.Replace(".txt", "-0.txt");

                    await gerarIteracaoAsync(newFileName);

                    Console.Write($"\nNão achou arquivo {lastFileName}. Tentando achar {newFileName}");
                }
                else
                {
                    if (e.Message.Contains("SSL"))
                    {
                        Thread.Sleep(20000);
                    }
                    if (e.Message.Contains("did not properly respond"))
                    {
                        Thread.Sleep(30000);
                    }

                    Console.Write($"\nErro em: {fileName}: {e.Message}");
                    Thread.Sleep(1000);
                }
            }
        }

        private void gerarIteracao(string fileName, ushort iteracao = 0)
        {
            try
            {
                var stream = getStream(fileName);
                var dir = "C:\\TempDBClass\\" + $"{fileName}".Split("/").ToList().LastOrDefault();
                // Create a FileStream with mode CreateNew  
                var local = new FileStream(dir, FileMode.OpenOrCreate);
                // Create a StreamWriter from FileStream 
                if (stream != null)
                {
                    using (StreamWriter writer = new StreamWriter(local, Encoding.UTF8))
                    {
                        var readed = leitura(stream);
                        Console.Write($"\nSucesso ao baixar arquivo {fileName}");
                        writer.Write(readed);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("404"))
                {
                    string newFileName;
                    var lastFileName = fileName;


                    if (fileName.Contains("-0.txt"))
                    {
                        iteracao++;
                        newFileName = fileName.Replace("-0.txt", "-5.txt");
                    }
                    else if (fileName.Contains("-5.txt"))
                    {
                        iteracao++;
                        newFileName = fileName.Replace("-5.txt", "-8.txt");
                    }
                    else
                    {
                        iteracao++;
                        newFileName = fileName.Replace(".txt", "-0.txt");
                    }

                    if (iteracao > 3)
                        return;

                    gerarIteracao(newFileName, iteracao);

                    Console.Write($"\nNão achou arquivo {lastFileName}. Tentando achar {newFileName}");
                }
                else
                {
                    //if (e.Message.Contains("SSL"))
                    //{
                    //    Thread.Sleep(20000);
                    //}
                    //if (e.Message.Contains("did not properly respond"))
                    //{
                    //    Thread.Sleep(30000);
                    //}

                    Console.Write($"\nErro em: {fileName}: {e.Message}");
                }

            }
        }

        public Task[] ObterTasksLeituraLivrosAsync(int qtdTasks)
        {
            Task[] tasks = new Task[qtdTasks];

            for (int i = 0; i < qtdTasks; i++)
            {
                tasks[i] = gerarIteracaoAsync($"{i}/{i}.txt");
            }

            return tasks;
        }

        internal async Task runLeituraLivrosAsync()
        {
            var tasks = ObterTasksLeituraLivrosAsync(100);
            Task.WaitAll(tasks, TaskTimeout);
        }

        public void RunLeituraLivros()
        {
            try
            {
                for (int i = 0; i < 20; i++)
                {
                    int inicio = (10000 / 20) * i;
                    int fim = inicio + 500;
                    Console.WriteLine($"\n\nPercorrendo pagina {i} ({inicio}-{fim})\n");

                    Parallel.For(inicio, fim,
                        (indice) =>
                        {
                            try
                            {
                                if (indice > 0 && (indice % 1 == 0 && indice % indice == 0) || (indice % 50 == 0))
                                {
                                    Console.WriteLine("Aguardando 5s p continuar");
                                    Thread.Sleep(5000);
                                }
                                gerarIteracao($"{indice}/{indice}.txt");
                            }
                            catch (Exception e)
                            {
                                throw;
                            }
                        });

                    Thread.Sleep(60000);
                }

                Console.WriteLine("Finalizado!");
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
