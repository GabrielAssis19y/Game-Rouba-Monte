using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Trabalho_AEDS_RoubaMonte
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StreamWriter Log = new StreamWriter("C:\\Users\\Admin\\Documents\\RoubaMonte\\log.txt", false, Encoding.UTF8);

            int numJogadores = 0;
            Console.WriteLine("Bem-vindo ao Rouba Monte. Escolha a quantidade de jogadores: \n2 - 4 jogadores");

            do
            {
                Console.Write("-> ");
                numJogadores = int.Parse(Console.ReadLine());

                if (numJogadores < 2 || numJogadores > 4)
                {
                    Console.WriteLine("Opção inválida! Escolha um número entre 2 e 4.");
                }
            } while (numJogadores < 2 || numJogadores > 4);

            Log.WriteLine("Criando os jogadores");
            Jogador[] jogadores = new Jogador[numJogadores];

            for (int i = 0; i < numJogadores; i++)
            {
                Console.WriteLine($"Nome jogador {i + 1}: ");
                string nome = Console.ReadLine();
                jogadores[i] = new Jogador(nome);
            }
            foreach (Jogador jogador in jogadores)
            {
                Console.WriteLine($"{jogador.nome}, ID: {jogador.IdJogador()}");
            }
            int numbaralho = 0;
            do
            {
                Console.WriteLine("Numero de baralhos: ");
                numbaralho = int.Parse(Console.ReadLine());
                if (numbaralho < 1)
                {
                    Console.WriteLine("Deve haver pelo menos um baralho! ");
                }

            } while (numbaralho < 1);

            Baralho[] baralhos = new Baralho[numbaralho];
            for (int i = 0; i < numbaralho; i++)
            {
                baralhos[i] = new Baralho();
                Log.WriteLine($"Criando o baralho {i + 1}");
                baralhos[i].Embaralhar();
                Log.WriteLine($"Embaralhando baralho {i + 1}");
            }
            Log.WriteLine($"Inicializando os montes de cada jogador em cada baralho");
            foreach (var jogador in jogadores)
            {
                foreach (var baralho in baralhos)
                {
                    MonteJogador monte = new MonteJogador(jogador);
                    baralho.ListarMontes(monte);
                }
            }  
            Log.WriteLine("Criando o descarte");
            Descarte descarte = new Descarte();


            Console.WriteLine("\nO jogo começou!");
            Log.WriteLine("Jogo iniciado");

            do
            {
                
                foreach (Jogador jogador in jogadores)
                {
                    int id = jogador.IdJogador();
                    int cont = 0;
                    while (cont < baralhos.Count())
                    {
                        if (!BaralhoVazio(baralhos[cont]))
                        {
                            Console.WriteLine($"Turno de {jogador.nome}, em baralho {cont+1}");
                            baralhos[cont].JogarTurno(descarte, id);
                            Console.WriteLine();
                            break;
                        }
                        cont++;
                    }
                }

            } while (!BaralhosVazios(baralhos)); 
            baralhos[0].FinalizarPartida();
            Log.WriteLine("Fim do jogo.");
            Console.WriteLine("\nDeseja visualizar o histórico de posições de um jogador? (S/N)");
            string resposta = Console.ReadLine().ToUpper();

            switch (resposta)
            {
                case "S":

                    Console.WriteLine("Digite o nome do jogador:");
                    string nomeJogador = Console.ReadLine();


                    Jogador jogadorEncontrado = null;
                    foreach (var jogador in jogadores)
                    {
                        if (jogador.nome == nomeJogador)
                        {
                            jogadorEncontrado = jogador;
                            break;
                        }
                    }


                    if (jogadorEncontrado != null)
                    {
                        Console.WriteLine($"\nHistórico de posições de {jogadorEncontrado.nome}:");
                        jogadorEncontrado.VisualizarRanking();
                    }
                    else
                    {
                        Console.WriteLine("Jogador não encontrado.");
                    }
                    break;

                case "N":

                    Console.WriteLine("Fim do jogo!");
                    break;

                default:
                    Console.WriteLine("Opção inválida. O jogo será finalizado.");
                    break;
            }
            Log.Close();



        }



        static bool BaralhosVazios(Baralho[] baralhos)
        {
            foreach (var baralho in baralhos)
            {
                if (baralho.Cartas.Count > 0)
                {
                    return false;
                }
            }
            return true;
        }
        static bool BaralhoVazio(Baralho baralho)
        {
            if (baralho.Cartas.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    class Jogador
    {
        private int id;
        public string nome;
        private int pos;
        private int qtdeMonte;
        private Queue<int> ranking = new Queue<int>();
        private static int contid = 0;
        public Jogador(string nome)
        {
            this.id = contid + 1;
            contid++;
            this.nome = nome;
            this.pos = 0;
            this.qtdeMonte = 0;
        }
        public int IdJogador()
        {
            return id;
        }
        public void AdicionarPosicaoRanking(int posicao)
        {
            if (ranking.Count >= 5)
            {
                ranking.Dequeue();
            }
            ranking.Enqueue(posicao);
        }
        public void VisualizarRanking()
        {
            int cont = 1;
            foreach (int partida in ranking)
            {
                Console.WriteLine($"{cont}ª partida: {partida}");
                cont++;
            }
        }
        public int Pos
        {
            get
            {
                return pos;
            }
            set
            {
                this.pos = value;
            }

        }
        public int QtdeMonte
        {
            get
            {
                return qtdeMonte;
            }
            set
            {
                this.qtdeMonte = value;
            }

        }
        public int Id
        {
            get
            {
                return id;
            }
        }
    }
    class MonteJogador
    {
        Jogador jogador;
        private Stack<Carta> monteCartas = new Stack<Carta>();

        public MonteJogador(Jogador jogador)
        {
            this.jogador = jogador;
            this.monteCartas = new Stack<Carta>();
        }
        public void Inserir(Carta carta)
        {
            if (carta != null)
            {
                monteCartas.Push(carta);
            }
        }
        public Carta remover()
        {
            if (monteCartas.Count > 0)
            {
                return monteCartas.Pop();
            }
            else
            {
                throw new Exception("Monte de cartas do jogador está vazio!");
            }

        }
        public int TamanhoMonte()
        {
            jogador.QtdeMonte = monteCartas.Count;
            return monteCartas.Count;
        }
        public Stack<Carta> MonteCartas
        {
            get
            {
                return monteCartas;
            }
            set
            {
                monteCartas = value;
            }
        }
        public Jogador Jogador
        {
            get
            {
                return jogador;
            }
        }
    }


    class Carta
    {
        public int valor;
        public string naipe;

        public Carta(int valor, string naipe)
        {
            this.valor = valor;
            this.naipe = naipe;
        }

    }
    class Baralho
    {
        private Stack<Carta> cartas = new Stack<Carta>();
        Lista cartastmp = new Lista(52);
        List<MonteJogador> monteJogadores = new List<MonteJogador>();

        public Baralho()
        {
            int[] valores = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            string[] naipe = { "espada", "copas", "ouro", "paus" };
            for (int i = 0; i < valores.Length; i++)
            {
                for (int j = 0; j < naipe.Length; j++)
                {
                    cartastmp.inserirFim(new Carta(valores[i], naipe[j]));
                }
            }

        }
        public Stack<Carta> Cartas
        {
            get
            {
                return cartas;
            }
            set
            {
                cartas = value;
            }
        }

        public void Embaralhar()
        {
            Random rnd = new Random();
            while (cartastmp.n > 0)
            {
                int pos = rnd.Next(0, cartastmp.n);
                cartas.Push(cartastmp.remover(pos));

            }

        }
        public void ListarMontes(MonteJogador monte)
        {
            monteJogadores.Add(monte);
        }

        public void JogarTurno(Descarte descarte, int id)
        {
            bool semjogada = false;
            while (!semjogada && cartas.Count > 0)
            {
                Carta cartadavez = cartas.Pop();
                Console.WriteLine($"Carta retirada: {cartadavez.valor} de {cartadavez.naipe}");
                Verificar(cartadavez, descarte, id, ref semjogada);
            }
        }
        public void Verificar(Carta cartadavez, Descarte descarte, int id, ref bool semjogada)
        {
            bool encontrou = false;
            id--;
            for (int i = 0; i < monteJogadores.Count; i++)
            {
                if (i == id)
                {
                    continue;
                }
                MonteJogador monteAdversario = monteJogadores[i];
                Console.WriteLine($"Verificando monte do jogador {i + 1} ({monteAdversario.TamanhoMonte()} cartas)");
                if (monteAdversario.TamanhoMonte() > 0)
                {
                    Carta cartaTopo = monteAdversario.MonteCartas.Peek();
                    Console.WriteLine($"Comparando {cartadavez.valor} de {cartadavez.naipe} com {cartaTopo.valor} de {cartaTopo.naipe}");

                    if (cartadavez.valor == cartaTopo.valor)
                    {
                        encontrou = true;
                        Console.WriteLine($"Roubo bem-sucedido! Jogador {id + 1} rouba o monte do jogador {i + 1}");
                        InverterMonte(monteAdversario);
                        for (int j = 0; j < monteAdversario.TamanhoMonte(); j++)
                        {
                            monteJogadores[id].MonteCartas.Push(monteAdversario.MonteCartas.Pop());
                        }
                        monteJogadores[id].MonteCartas.Push(cartadavez);

                        Console.WriteLine($"Jogador {id + 1} roubou o monte do jogador {i + 1}!");
                        Console.WriteLine();
                        break;
                    }
                }
            }
            if (!encontrou)
            {
                Console.WriteLine($"Jogador {id + 1} não encontrou um monte para roubar.");
                int pos = descarte.cartasDescarte.Verificar(cartadavez);
                if (pos != -1)
                {
                    Console.WriteLine($"Carta encontrada no descarte");
                    Carta carta = descarte.cartasDescarte.Remover(pos);
                    monteJogadores[id].MonteCartas.Push(carta);
                    Console.WriteLine($"Jogador {id + 1} retirou a carta {carta.valor} de {carta.naipe} do descarte e a adicionou ao seu monte");
                    monteJogadores[id].MonteCartas.Push(cartadavez);
                    Console.WriteLine();

                }
                else
                {
                    if (monteJogadores[id].MonteCartas.Count > 0)
                    {
                        Carta carta = monteJogadores[id].MonteCartas.Peek();
                        if (cartadavez.valor == carta.valor)
                        {
                            monteJogadores[id].MonteCartas.Push(cartadavez);
                            Console.WriteLine($"Jogador {id + 1} adicionou a carta {cartadavez.valor} de {cartadavez.naipe} ao seu monte.");
                        }
                        else
                        {

                            descarte.cartasDescarte.Inserir(cartadavez);
                            Console.WriteLine($"Carta {cartadavez.valor} de {cartadavez.naipe} adicionada ao descarte.");
                            semjogada = true;
                        }
                    }

                    else
                    {
                        descarte.cartasDescarte.Inserir(cartadavez);
                        Console.WriteLine($"Carta {cartadavez.valor} de {cartadavez.naipe} adicionada ao descarte.");
                        semjogada = true;
                    }
                }

            }
        }
        public void InverterMonte(MonteJogador monte)
        {
            Queue<Carta> tmp = new Queue<Carta>();
            foreach (Carta carta in monte.MonteCartas)
            {
                tmp.Enqueue(carta);
            }
            monte.MonteCartas.Clear();
            while (tmp.Count > 0)
            {
                monte.MonteCartas.Push(tmp.Dequeue());
            }

        }
        public void FinalizarPartida()
        {
            Console.WriteLine("Partida finalizada.");
            Console.WriteLine("--------- --------- --------- Resultado --------- --------- ---------");


            int maiorMonte = 0;
            List<Jogador> ganhadores = new List<Jogador>();


            foreach (MonteJogador monteJogador in monteJogadores)
            {
                int qtdeAtual = monteJogador.TamanhoMonte();
                if (qtdeAtual > maiorMonte)
                {
                    maiorMonte = qtdeAtual;
                }
            }


            foreach (MonteJogador monteJogador in monteJogadores)
            {
                if (monteJogador.TamanhoMonte() == maiorMonte)
                {
                    ganhadores.Add(monteJogador.Jogador);
                }
            }


            Console.WriteLine($"A maior quantidade de cartas em um monte foi: {maiorMonte}");

            if (ganhadores.Count == 1)
            {
                Console.WriteLine($"O vencedor é: {ganhadores[0].nome} id: {ganhadores[0].Id} com {maiorMonte} cartas!");
                ganhadores[0].AdicionarPosicaoRanking(1);

            }
            else
            {
                Console.WriteLine("Houve um empate! Vencedores:");
                foreach (var ganhador in ganhadores)
                {
                    Console.WriteLine($"- {ganhador.nome} id: {ganhador.Id} com {maiorMonte} cartas.");
                    ganhador.AdicionarPosicaoRanking(1);

                }
            }
        }


    }
    class Descarte
    {
        public Listaflex cartasDescarte = new Listaflex();

        public void inserir(Carta carta)
        {
            cartasDescarte.Inserir(carta);
        }

    }

    class Lista
    {
        private Carta[] array;
        public int n;
        public Lista() { inicializar(0); }
        public Lista(int tamanho)
        {
            inicializar(tamanho);
        }
        private void inicializar(int tamanho)
        {
            this.array = new Carta[tamanho];
            this.n = 0;
        }
        public void inserirFim(Carta carta)
        {
            if (n >= array.Length)
                throw new Exception("Erro!");
            array[n] = carta;
            n++;
        }
        public Carta remover(int pos)
        {
            if (n == 0 || pos < 0 || pos >= n)
                throw new Exception("Erro!");
            Carta cartaremovida = array[pos];
            n--;
            for (int i = pos; i < n; i++)
            {
                array[i] = array[i + 1];
            }
            return cartaremovida;
        }
    }
    class Celula
    {
        private Carta carta;
        private Celula prox;
        public Celula(Carta carta)
        {
            this.carta = carta;
            this.prox = null;
        }
        public Celula()
        {
            this.carta = null;
            this.prox = null;
        }
        public Celula Prox
        {
            get { return prox; }
            set { prox = value; }
        }
        public Carta Elemento
        {
            get { return carta; }
            set { carta = value; }
        }
    }
    class Listaflex
    {
        private Celula primeiro, ultimo;
        public Listaflex()
        {
            primeiro = new Celula();
            ultimo = primeiro;
        }
        public void Inserir(Carta carta)
        {
            ultimo.Prox = new Celula(carta);
            ultimo = ultimo.Prox;
        }
        public int Verificar(Carta carta)
        {
            int indice = 0;
            for (Celula i = primeiro.Prox; i != null; i = i.Prox)
            {
                if (i.Elemento.valor == carta.valor)
                {
                    return indice;
                }
                indice++;
            }
            return -1;
        }
        public Carta Remover(int pos)
        {
            Carta carta;
            int tamanho = Tamanho();
            if (primeiro == ultimo || pos < 0 || pos >= tamanho)
            {
                throw new Exception("Erro!");
            }
            else
            {
                Celula i = primeiro;
                for (int j = 0; j < pos; j++, i = i.Prox) ;
                Celula tmp = i.Prox;
                carta = tmp.Elemento; i.Prox = tmp.Prox;
                tmp.Prox = null; i = tmp = null;
            }
            return carta;
        }
        public int Tamanho()
        {
            int cont = 0;
            for (Celula i = primeiro.Prox; i != null; i = i.Prox)
            {
                cont++;
            }
            return cont;
        }
    }

}
