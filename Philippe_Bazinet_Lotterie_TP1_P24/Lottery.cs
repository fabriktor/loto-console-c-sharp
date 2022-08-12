using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Philippe_Bazinet_Lotterie_TP1_P24
{
    internal class Lottery
    {
        //VARIABLES INITIALES
        public static string whiteSpace = "                 ";
        public static Random boule = new Random();
        public static int[] combinaisonGagnante; //on initialise a 0 pour afficher notre billet vierge
        public static ushort complementaireGagnant = 0;
        public static Combinaison[] tableauCombinaisons;
        public static Billet billet;

        //LIST POUR ENTREPOSER NOS NOMBRES GAGNANTS ET AFFICHER LEUR FREQUENCE D'AFFFICHAGE
        public static List<ushort> nbGagnants = new List<ushort>();

        //LIST POUR ENTREPOSER NOS COMPLEMENTAIRES GAGNANTS ET AFFICHER LEUR FREQUENCE D'AFFFICHAGE
        public static List<ushort> compGagnants = new List<ushort>();

        //ENUM SERVANT À AFFICHER NOTRE BILLET DANS CERTAINS "MODES D'AFFICHAGES"
        public enum Billet
        {
            Vierge,
            Gagnant
        }

        //STRUCTS
        public struct StatsTableau
        {
            //VARIABLES POUR STATISTIQUES
            public static ushort nbComb;
            public static ushort nbCombTotal = 0;
            public static int nbTirages = 1;
            //---
            public static ushort gainCount = 0;
            public static bool gainCountC = false;
            public static ushort gain3sur6 = 0;
            public static ushort gain3sur6C = 0;
            public static ushort gain4sur6 = 0;
            public static ushort gain4sur6C = 0;
            public static ushort gain5sur6 = 0;
            public static ushort gain5sur6C = 0;
            public static ushort gain6sur6 = 0;
            public static ushort gain6sur6C = 0;
            //---
            public static float sommeGains = 0F;

            //RESET LES STATISTIQUES UTILES AU COMPTAGE DU NOMBRE DE NUMEROS GAGNANTS POUR CHAQUE COMBINAISON 
            public static void ResetStats()
            {
                gainCount = 0;
                gainCountC = false;
            }

            //AFFICHE LE NOMBRE DE FOIS QUE NOS NOMBRES GAGNANTS SE SONT AFFICHÉS
            public static void DuplicateCounter() 
            {
                var query = //pour grouper
                    from number in nbGagnants
                    group number by number;

                var orderQuery = query.OrderBy(s => s.Count()).Reverse(); //pour trier en ordre d'apparition inverse! utile pour la lecture du tableau final.

                foreach (var group in orderQuery)
                {
                        WriteRow($"{group.First()}", $"{group.Count()} fois");
                }
            }

            //AFFICHE LE NOMBRE DE FOIS QUE NOS COMPLÉMENTAIRES SE SONT AFFICHÉS
            public static void CompDuplicateCounter()
            {
                var query = //pour grouper
                    from number in compGagnants
                    group number by number;

                var orderQuery = query.OrderBy(s => s.Count()).Reverse(); //pour trier en ordre d'apparition inverse! utile pour la lecture du tableau final.

                foreach (var group in orderQuery)
                {
                    WriteRow($"{group.First()}", $"{group.Count()} fois");
                }
            }

            //INCREMENTE NOS FAMILLES DE GAINS
            public static void SwitchNStats(ref ushort gain36, ref ushort gain36C, ref ushort gain46, ref ushort gain46C, ref ushort gain56, ref ushort gain56C, ref ushort gain66, ref ushort gain66C)
            {
                if (gainCountC)
                {
                    switch (gainCount)
                    {
                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            gain36C++;
                            break;
                        case 4:
                            gain46C++;
                            break;
                        case 5:
                            gain56C++;
                            break;
                        case 6:
                            gain66C++;
                            break;
                    }
                } 
                else if (!gainCountC)
                {
                    switch (gainCount)
                    {
                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            gain36++;
                            break;
                        case 4:
                            gain46++;
                            break;
                        case 5:
                            gain56++;
                            break;
                        case 6:
                            gain66++;
                            break;
                    }

                }
            }

            public static void SommerGains(ref float somme)
            {
                somme = (float)(somme + gain3sur6 + gain3sur6C + gain4sur6 + gain4sur6C + gain5sur6 + gain5sur6C + gain6sur6 + gain6sur6C);
            }
            //AFFICHE NOTRE TABLEAU DE STATISTIQUES
            public static void AfficherStats()
            {
                Clear();
                ForegroundColor = ConsoleColor.Yellow;
                WriteLine($"Il est venu le temps de découvrir vos résultats.\nVous avez acheté {Bill.index} billet{(Bill.index > 1 ? "s" : "")}, et avez effectué un total de {nbTirages} tirage{(nbTirages > 1 ? "s" : "")}.\nPesez sur une touche pour vous les statistiques qui en découlent... ");
                ReadKey(true);
                Clear();
                ForegroundColor = ConsoleColor.Cyan;
                WriteLine($@"
  _      ____ _______ ____     _____ _______    _______ _____ 
 | |    / __ \__   __/ __ \   / ____|__   __|/\|__   __/ ____|
 | |   | |  | | | | | |  | | | (___    | |  /  \  | | | (___  
 | |   | |  | | | | | |  | |  \___ \   | | / /\ \ | |  \___ \ 
 | |___| |__| | | | | |__| |  ____) |  | |/ ____ \| |  ____) |
 |______\____/  |_|  \____/  |_____/   |_/_/    \_\_| |_____/ 
                                                              
");
                WriteLine();
                //HEADER TABLEAU
                ForegroundColor = ConsoleColor.Yellow;
                WriteLine($"Pesez sur une touche pour revenir au menu précédent... ");
                for (int i = 0; i <= 80; i++)
                {
                    if (i == 0 || i == 80)
                    {
                        Write(" ");
                    }
                    else
                    {
                        Write("_");
                    }
                }
                WriteLine();

                //STATISTIQUES GÉNÉRALES
                WriteRow("STATISTIQUES GÉNÉRALES", "VALEUR NUMÉRIQUE");
                WriteRow("Nombre de billets achetés", $"{Bill.index}");
                WriteRow("Nombre de tirages effectués", $"{nbTirages}");
                WriteRow($"Combinaisons générées pour {(Bill.index > 1 ? "vos" : "votre")} {(Bill.index > 1 ? $"{Bill.index}" : "")} billet{(Bill.index > 1 ? "s" : "")}", $"{nbCombTotal}");
                for (int i = 0; i <= 80; i++)
                {
                    if (i == 0 || i == 80)
                    {
                        Write("|");
                    }
                    else
                    {
                        Write("_");
                    }
                }
                WriteLine();

                //NOMBRE DE COMBINAISONS GAGNANTES PAR FAMILLE
                WriteRow("STATISTIQUES POUR TOUS LES TIRAGES", "APPARITION");
                WriteRow("Combinaisons gagnantes 3 / 6", $"{gain3sur6}");
                WriteRow("Combinaisons gagnantes 3 / 6 + C", $"{gain3sur6C}");
                WriteRow("Combinaisons gagnantes 4 / 6", $"{gain4sur6}");
                WriteRow("Combinaisons gagnantes 4 / 6 + C", $"{gain4sur6C}");
                WriteRow("Combinaisons gagnantes 5 / 6", $"{gain5sur6}");
                WriteRow("Combinaisons gagnantes 5 / 6 + C", $"{gain5sur6C}");
                WriteRow("Combinaisons gagnantes 6 / 6", $"{gain6sur6}");
                WriteRow("Combinaisons gagnantes 6 / 6 + C", $"{gain6sur6C}");
                for (int i = 0; i <= 80; i++)
                {
                    if (i == 0 || i == 80)
                    {
                        Write("|");
                    }
                    else
                    {
                        Write("_");
                    }
                }
                WriteLine();

                //FREQUENCE D'AFFICHAGE DES FAMILLES DE GAINS
                SommerGains(ref sommeGains);
                WriteRow("GAINS", "FRÉQUENCE DE SORTIE");
                WriteRow("Gain 3 / 6", $"{gain3sur6/sommeGains:P2}");
                WriteRow("Gain 3 / 6 + C", $"{gain3sur6C/sommeGains:P2}");
                WriteRow("Gain 4 / 6", $"{gain4sur6/sommeGains:P2}");
                WriteRow("Gain 4 / 6 + C", $"{gain4sur6C/sommeGains:P2}");
                WriteRow("Gain 5 / 6", $"{gain5sur6/sommeGains:P2}");
                WriteRow("Gain 5 / 6 + C", $"{gain5sur6C/sommeGains:P2}");
                WriteRow("Gain 6 / 6", $"{gain6sur6/sommeGains*100:P2}");
                WriteRow("Gain 6 / 6 + C", $"{gain6sur6C/sommeGains:P2}");

                for (int i = 0; i <= 80; i++)
                {
                    if (i == 0 || i == 80)
                    {
                        Write("|");
                    }
                    else
                    {
                        Write("_");
                    }   
                }
                WriteLine();

                //AFFICHAGE DU NOMBRE D'APPARITION DES NOMBRES AYANT ETE TIRES DANS LES COMBINAISONS SEULEMENT!
                WriteRow("NOMBRES GAGNANTS", "APPARITION");
                DuplicateCounter();

                for (int i = 0; i <= 80; i++)
                {
                    if (i == 0 || i == 80)
                    {
                        Write("|");
                    }
                    else
                    {
                        Write("_");
                    }
                }
                WriteLine();

                //AFFICHAGE DU NOMBRE D'APPARITION DES NOMBRES AYANT ETE TIRES DANS LES COMPLEMENTAIRES!
                WriteRow("COMPLÉMENTAIRES GAGNANTS", "APPARITION");
                CompDuplicateCounter();

                //FOOTER DU BILLET
                for (int i = 0; i <= 80; i++)
                {
                    if (i == 0 || i == 80)
                    {
                        Write("|");
                    }
                    else
                    {
                        Write("_");
                    }
                }
                ForegroundColor = ConsoleColor.White;
                ReadKey(true);
                RunSecondMenu();
            }
        }
        public struct Bill
        {
            public static int index = 1;
            public static ushort billWidth = 49;
            public static void AfficherBillet(ref Combinaison[] tableauCombinaisons, ref ushort billWidth, ref int[] combinaisonGagnante, ref ushort complementaireGagnant)
            {

                //esthétique du billet - ligne du haut
                Clear();
                ForegroundColor = ConsoleColor.Cyan;
                for (int i = 0; i < billWidth; i++)
                {
                    Write("_");
                }
                WriteLine();

                //entête du billet
                Write(" _     _ _ _      _ ".PadRight(billWidth));
                WriteLine("|");
                Write("| |   (_| | |    | |".PadRight(billWidth));
                WriteLine("|");
                Write("| |__  _| | | ___| |_ ".PadRight(billWidth));
                WriteLine("|");
                Write("| '_ \\| | | |/ _ | __|".PadRight(billWidth));
                WriteLine("|");
                Write("| |_) | | | |  __| |_".PadRight(billWidth));
                WriteLine("|");
                Write("|_.__/|_|_|_|\\___|\\__|".PadRight(billWidth));
                WriteLine("|");
                Write("".PadRight(billWidth));
                WriteLine("|");
                Write("Continuez en appuyant sur une touche...".PadRight(billWidth));
                WriteLine("|");

                ForegroundColor = ConsoleColor.Yellow;

                //affichage des combinaisons
                foreach (Combinaison comb in tableauCombinaisons)
                {
                    //initialisation des compteurs de gains
                    StatsTableau.ResetStats(); 

                    string number;
                    //lignes séparatrices
                    for (int i = 0; i < comb.dimension + 1; i++)
                    {
                        ForegroundColor = ConsoleColor.Cyan;
                        Write("-------");
                        ForegroundColor = ConsoleColor.White;
                    }

                    ForegroundColor = ConsoleColor.Cyan;
                    Write("|");
                    ForegroundColor = ConsoleColor.Yellow;
                    WriteLine();

                    //affichage des numéros
                    if (billet == Billet.Vierge) //utilisation de l'enum pour simplifier l'affichage du billet vierge
                    {
                        for (int i = 0; i < comb.dimension; i++)
                        {
                            number = $"{comb.combinaison[i]}".PadRight(2);
                            Write($"{number}  -".PadRight(billWidth / 7));
                        }
                        Write($"C {comb.complementaire}".PadRight(billWidth / 7));
                        ForegroundColor = ConsoleColor.Cyan;
                        Write("|");
                        ForegroundColor = ConsoleColor.Yellow;
                        WriteLine();
                    }
                    else if (billet == Billet.Gagnant) //utilisation de l'enum pour simplifier l'affichage du billet avec les combinaisons gagnantes
                    {
                        for (int i = 0; i < comb.dimension; i++) //boucle de comparaison avec la combinaison gagnante
                        {
                            for (int j = 0; j < comb.dimension; j++)
                            {
                                if (comb.combinaison[i] == combinaisonGagnante[j])
                                {
                                    nbGagnants.Add((ushort)comb.combinaison[i]);
                                    ForegroundColor = ConsoleColor.Black;
                                    BackgroundColor = ConsoleColor.Yellow;
                                    StatsTableau.gainCount++; //on incremente notre compteur pour savoir combien de nombres gagnants aura la combinaison comb du foreach
                                    break;
                                }
                            }
                            number = $"{comb.combinaison[i]}".PadRight(2);
                            Write($"{number}  -".PadRight(billWidth / 7));
                            ForegroundColor = ConsoleColor.Yellow;
                            BackgroundColor = ConsoleColor.Black;
                        }

                        if (comb.complementaire == complementaireGagnant) //boucle de comparaison avec le complementaire gagnant
                        {
                            StatsTableau.gainCountC = true; //bool servant a determiner si notre combinaison comporte un complementaire gagnant
                            compGagnants.Add((ushort)comb.complementaire);
                            ForegroundColor = ConsoleColor.Black;
                            BackgroundColor = ConsoleColor.Yellow;
                            Write($"C {comb.complementaire}".PadRight(billWidth / 7));
                            BackgroundColor = ConsoleColor.Black;
                            ForegroundColor = ConsoleColor.Cyan;
                            Write("|");
                            ForegroundColor = ConsoleColor.Yellow;
                            WriteLine();
                        }
                        else
                        {
                            Write($"C {comb.complementaire}".PadRight(billWidth / 7));
                            ForegroundColor = ConsoleColor.Cyan;
                            Write("|");
                            ForegroundColor = ConsoleColor.Yellow;
                            WriteLine();
                        }
                    }
 
                    StatsTableau.SwitchNStats(ref StatsTableau.gain3sur6, ref StatsTableau.gain3sur6C, ref StatsTableau.gain4sur6, ref StatsTableau.gain4sur6C, ref StatsTableau.gain5sur6, ref StatsTableau.gain5sur6C, ref StatsTableau.gain6sur6, ref StatsTableau.gain6sur6C);
                }
                ForegroundColor = ConsoleColor.White;

                //footer du billet
                for (int i = 0; i < billWidth; i++)
                {
                    if (i == billWidth - 1)
                    {
                        ForegroundColor = ConsoleColor.Cyan;
                        Write("_|");
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.Cyan;
                        Write("_");
                    }
                }
                WriteLine("\n");

                if (billet == Billet.Vierge)
                {
                    ForegroundColor = ConsoleColor.Cyan;
                    WriteLine("Consultez votre billet ci-dessus\net appuyez sur une touche pour continuer... ");
                }
                else if (billet == Billet.Gagnant)
                {
                    ForegroundColor = ConsoleColor.Yellow;
                    WriteLine("Consultez votre billet ci-dessus.\nLes chiffres surlignés en jaune concordent avec la combinaison gagnante.\nAppuyez sur une touche pour continuer... ");
                }
                ForegroundColor = ConsoleColor.White;
                ReadKey(true);
                Clear();

                if (billet == Billet.Gagnant)
                {
                    RunSecondMenu(); //puisqu'on doit ABSOLUMENT revenir au menu 2 (RunSecondMenu()) si le billet == Billet.Gagnant
                }
            }

        }
        public struct Combinaison 
        {
            public int dimension;
            public int[] combinaison; //array 1 dimension de integer, formant 1 combinaison. Plusieurs combinaisons seront formées en appellant le CONSTRUCTOR dans une boucle
            public int complementaire; 
            
            //Constructor pour combinaisons et complémentaire
            public Combinaison(int Dimension)
            {
                dimension = Dimension;
                combinaison = new int[Dimension];
                bool duplicate;
                do
                {
                    duplicate = false;
                    for (int i = 0; i < dimension; i++)
                    {
                        combinaison[i] = boule.Next(1, 50);
                        for (int j = 0; j < i; j++)
                        {
                            if (combinaison[i] == combinaison[j] && (i != j))
                            {
                                duplicate = true;
                            }
                        }
                        if (duplicate) break;
                    }
                } while (duplicate) ;

                Array.Sort(combinaison); //essentiel: affichage en ordre CROISSANT
                complementaire = boule.Next(1, 10);
            }
        }







        //---------------------------------------------START()
        public void Start()
        {
            //INTRODUCTION ET MENU
            PresenterIntro();
            Clear();
            RunMainMenu();

            //DEMANDE À L'UTILISATEUR DU NOMBRE DE COMBINAISONS DÉSIRÉES
            DemanderNbCombinaisons(out StatsTableau.nbComb); //Vérification que l'input de l'utilisateur est valide et mise à jour par référence avec OUT puisque nbComb n'est pas initialisé
            Clear();
            tableauCombinaisons = GenererCombinaisons();

            //AFFICHAGE DU BILLET VIERGE
            billet = Billet.Vierge; //MODE CHANGE
            ForegroundColor = ConsoleColor.Yellow;
            WriteLine($"Votre billet de {StatsTableau.nbComb} combinaisons apparaîtra dans quelques secondes.\nVous pourrez alors appuyer sur une touche pour que l'ordinateur génère une combinaison gagnante... ");
            AfficherCountdown();

            Bill.AfficherBillet(ref tableauCombinaisons, ref Bill.billWidth, ref combinaisonGagnante, ref complementaireGagnant); 
            combinaisonGagnante = GenererCombinaisonGagnante();
            complementaireGagnant = (ushort)GenererComplementaireGagnant();

            //AFFICHAGE DU BILLET AVEC NUMÉROS GAGNANTS
            billet = Billet.Gagnant; //MODE CHANGE
            AfficherCombGagnante();
            Bill.AfficherBillet(ref tableauCombinaisons, ref Bill.billWidth, ref combinaisonGagnante, ref complementaireGagnant);

            //POSSIBILITE DE REFAIRE UN TIRAGE OU CHANGER DE BILLET
            RunSecondMenu(); //Menu permettant de refaire un tirage, acheter un nouveau billet, afficher les statistiques ou quitter le jeu
        }








        //---------------------------------------------FONCTIONS ET PROCEDURES
        //MENUS
        public static void RunMainMenu()
        {
            string prompt = "Veuillez choisir un option:";
            string[] options = { "--Instructions", "--Jouer", "--Quitter" };
            Menu mainMenu = new Menu(prompt, options);
            int selectedIndex = mainMenu.Run();

            switch (selectedIndex)
            {
                case 0:
                    ShowInstructions();
                    break;
                case 1:
                    Clear();
                    System.Threading.Thread.Sleep(800);
                    break;
                case 2:
                    ExitGame();
                    break;
            }
        }
        public static void RunSecondMenu()
        {
            string prompt = "Veuillez choisir un option:";
            string[] options = { "--Refaire un tirage", "--Acheter un billet supplémentaire","--Afficher les statistiques", "--Quitter"};
            Menu mainMenu = new Menu(prompt, options);
            int selectedIndex = mainMenu.Run();

            switch (selectedIndex)
            {
                case 0:
                    Clear();
                    StatsTableau.nbTirages++; //variable utile pour nos statistiques finales
                    combinaisonGagnante = GenererCombinaisonGagnante();
                    complementaireGagnant = (ushort)GenererComplementaireGagnant();
                    AfficherCombGagnante();
                    Bill.AfficherBillet(ref tableauCombinaisons, ref Bill.billWidth, ref combinaisonGagnante, ref complementaireGagnant); //On reaffiche notre billet avec les combinaisons gagnantes en jaune
                    break;
                case 1:
                    Clear();
                    Bill.index++; //variable utile pour les statistiques
                    DemanderNbCombinaisons(out StatsTableau.nbComb);
                    tableauCombinaisons = GenererCombinaisons();
                    billet = Billet.Vierge;
                    Bill.AfficherBillet(ref tableauCombinaisons, ref Bill.billWidth, ref combinaisonGagnante, ref complementaireGagnant); //On reaffiche notre billet avec les combinaisons gagnantes en jaune
                    combinaisonGagnante = GenererCombinaisonGagnante();
                    complementaireGagnant = (ushort)GenererComplementaireGagnant();
                    StatsTableau.nbTirages++; //ajout d'un nouveau tirage
                    AfficherCombGagnante();
                    billet = Billet.Gagnant;
                    Bill.AfficherBillet(ref tableauCombinaisons, ref Bill.billWidth, ref combinaisonGagnante, ref complementaireGagnant); //On reaffiche notre billet avec les combinaisons gagnantes en jaune
                    break;
                case 2:
                    StatsTableau.AfficherStats();
                    break;
                case 3:
                    ExitGame();
                    break;
            }
        } //LOOPING MENU
        //JEU
        public static void TypeLine(string line)
        {
            for (int k = 0; k < line.Length; k++)
            {
                Write(line[k]);
                System.Threading.Thread.Sleep(20);
            }
        }
        public static void PresenterIntro()
        {
            ForegroundColor = ConsoleColor.Cyan;
            Write($"{whiteSpace} _           _           ____     _____  _____\n");
            Write($"{whiteSpace}| |         | |         / ___|   / /   ||  _  |\n");
            System.Threading.Thread.Sleep(500);
            ForegroundColor = ConsoleColor.Yellow;
            Write($"{whiteSpace}| |     ___ | |_ ___   / /___   / / /| || |_| |\n");
            System.Threading.Thread.Sleep(500);
            ForegroundColor = ConsoleColor.Cyan;
            Write($"{whiteSpace}| |    / _ \\| __/ _ \\  | ___ \\ / / /_| |\\____ |\n");
            System.Threading.Thread.Sleep(500);
            ForegroundColor = ConsoleColor.Yellow;
            Write($"{whiteSpace}| |___| (_) | || (_) | | \\_/ |/ /\\___  |.___/ /\n");
            System.Threading.Thread.Sleep(500);
            ForegroundColor = ConsoleColor.Cyan;
            Write($"{whiteSpace}\\_____/\\___/ \\__\\___/  \\_____/_/     |_/\\____/\n");
            WriteLine();
            System.Threading.Thread.Sleep(500);
            ForegroundColor = ConsoleColor.Cyan;
            TypeLine($"{whiteSpace}Jouez avec modération.");
            WriteLine();
            System.Threading.Thread.Sleep(1000);
            ForegroundColor = ConsoleColor.White;
            Write($"{whiteSpace}Pesez sur une touche pour commencer... ");
            ReadKey(true);
        }
        public static void ShowInstructions()
        {
            Clear();
            TypeLine("Ce jeu vous demandera un nombre de combinaisons que vous désirez pour participer à une loterie fictive.");
            System.Threading.Thread.Sleep(600);
            TypeLine("\nL'ordinateur générera alors vos combinaisons aléatoires.");
            System.Threading.Thread.Sleep(500);
            TypeLine("\nIl générera aussi une combinaison gagnante fictive.");
            System.Threading.Thread.Sleep(400);
            TypeLine("\nPar la suite, vous pourrez voir vos combinaisons gagnantes ainsi que les statistiques des résultats.");
            System.Threading.Thread.Sleep(1000);
            Write("\nPesez sur une touche pour revenir au menu précédent... ");
            ReadKey();
            RunMainMenu();
        }
        public static void DemanderNbCombinaisons(out ushort comb)
        {
            string nbCombString;
            bool valid;

            ForegroundColor = ConsoleColor.Yellow;
            Write($"Combien de combinaisons désirez-vous avoir?\nEntrez une valeur entre 10 et 200: ");
            do
            {
                nbCombString = Console.ReadLine();
                valid = ushort.TryParse(nbCombString, out comb);
                if (!valid || comb < 10 || comb > 200)
                {
                    Clear();
                    Write("Entrez un nombre entre 10 et 200 inclusivement: ");
                }
            } while (!valid || comb < 10 || comb > 200);

            StatsTableau.nbCombTotal = (ushort)(StatsTableau.nbCombTotal + comb);
        }
        public static Combinaison[] GenererCombinaisons() 
        {
            //utilisation du constructor pour générer nos combinaisons dans un tableau
            tableauCombinaisons = new Combinaison[StatsTableau.nbComb];
            for (int i = 0; i < StatsTableau.nbComb; i++)
            {
                tableauCombinaisons[i] = new Combinaison(6);          
            }
            ForegroundColor = ConsoleColor.White;

            return tableauCombinaisons;
        }
        public static int[] GenererCombinaisonGagnante()
        {
            Combinaison combinaison = new Combinaison(6); //on utilise notre struct
            int[] combinaisonGagnante = combinaison.combinaison; //on genere une combinaison gagnante
            return combinaisonGagnante;
        }
        public static int GenererComplementaireGagnant()
        {
            Combinaison combinaison = new Combinaison(6); //on utilise notre struct
            int complementaire = combinaison.complementaire; //on genere le complementaire gagnant
            return complementaire;
        }
        public static void AfficherCountdown()
        {
            for(double i = 0D; i <= 100; i++)
            {
                System.Threading.Thread.Sleep(34);
                ForegroundColor = ConsoleColor.Yellow;
                SetCursorPosition(0,3);
                Write(String.Format("{0:P2}", i/100));
                ForegroundColor = ConsoleColor.White;
            }
            System.Threading.Thread.Sleep(500);
        }
        public static void AfficherCombGagnante()
        {
            ForegroundColor = ConsoleColor.Yellow;
            WriteLine($"Voici votre combinaison gagnante: {string.Join(" - ", combinaisonGagnante)}\nVotre numéro complémentaire est le {complementaireGagnant}.");
            Write("Pesez sur une touche pour voir les numéros gagnants sur votre billet... ");
            ReadKey(true);
        }
        public static void WriteRow(string contentLeft, string contentRight)
        {
            Write("");
            Write($"| {contentLeft}".PadRight(60,'.'));
            WriteLine($"{contentRight} |".PadLeft(21,'.'));
        }
        public static void ExitGame()
        {
            Clear();
            Write("Pesez sur une touche pour quitter... ");
            ReadKey(true);
            Environment.Exit(0);
        }
    }
}



