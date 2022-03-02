using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Schiffe_Versenken
{

    class Game
    {

        private int dimx = 3;
        private string[,] playfield, playerfield;
        private Random random = new Random();
        private bool cheat = false;
        private int shipcount;
        private enum Direction
        {
            rechts, links, oben, unten, none
        }
        public void Initialize()
        {
            Console.Title = "Schiffe Versenken";
            Console.WriteLine("Willkommen zu meinem Schiffeversenken-Spiel\nDu gibts mir die Dimension des Spielfelds,\nmit einer größe zwischen 4x4 und 20x20 und ich platziere Schiffe auf dem Spielfeld entsprechend der größe.");
            do
            {
                try
                {
                    Console.Write("Gebe nun die Dimensionen des Spielfeldes ein: ");
                    dimx = Convert.ToInt32(Console.ReadLine());
                    //Checks for correct playfield size
                    if (dimx >= 4 && dimx <= 20)
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    Console.Clear();
                    Console.WriteLine("Du hast einen Nicht validen Wert eingegeben. Bitte versuche es erneut!");
                }
                Console.WriteLine("Bitte gebe eine korrekte größe des Spielfeldes ein");
            } while (true);
            //Generate a playfield for the ai
            playfield = new string[dimx, dimx];
            //Generate a playfield for the player
            playerfield = new string[dimx, dimx];
            //Fill the playfields with waves
            for (int i = 0; i < dimx; i++)
            {
                for (int j = 0; j < dimx; j++)
                {
                    playfield[i, j] = "~";
                    playerfield[i, j] = "~";
                }
            }
            shipcount = dimx / 2;

            for (int i = 0; i < shipcount; i++)
            {
                PlaceShip(random.Next(1, 4), false); // false for ai and true for player
            }
        }
        public void RunGame()
        {
            Console.WriteLine("Bitte Plaziere nun {0} Schiffe auf deinem Spielfeld", shipcount);
            Console.Write("Willst du die Schiffe manuell oder automatisch platzieren?(Manuell/Auto): ");
            string choice = "";
            do
            {
                try
                {
                    choice = Console.ReadLine().ToLower();
                }
                catch
                {
                    Console.WriteLine("Bitte gebe eine gültige möglichkeit ein");
                }
                if (choice == "manuell" || choice == "auto") break;
            } while (true);
            if (choice == "auto")
            {
                for (int i = 0; i < shipcount; i++)
                {
                    PlaceShip(random.Next(1, 4), true);
                }
                DrawPlayfield(true);
            }
            else
            {

                Console.WriteLine("Hierzu gebe eine X und eine Y Koordinate, sowie die Ausrichtung des Schiffes!");
                int pshipx = 0, pshipy = 0, placecounter = 0, pshipsize = 0;
                Direction direction = Direction.none;
                string directionstr = "";


                do
                {
                    try
                    {
                        //the -1 is for an offset, since players thought 1/1 was the lowest coordinate
                        Console.Write("X: ");
                        pshipx = Convert.ToInt32(Console.ReadLine()) - 1;

                        Console.Write("Y: ");
                        pshipy = Convert.ToInt32(Console.ReadLine()) - 1;
                        bool tryParse;
                        do
                        {
                            Console.Write("Ausrichtung(Oben,Unten,Rechts,Links): ");

                            directionstr = Console.ReadLine().ToLower();

                            tryParse = Enum.TryParse(directionstr, out direction);
                            if (!tryParse)
                            {
                                Console.Clear();
                                Console.WriteLine("Bitte gib eine Korrekte ausrichtung ein!");
                            }
                        } while (!tryParse);

                        Console.Write("Schiffgröße(1-3): ");

                        pshipsize = Convert.ToInt32(Console.ReadLine());

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    //checks for valid ship placement
                    if (InBounds(pshipsize, pshipx, pshipy, direction) && PlayerShip(pshipsize, pshipx, pshipy, direction))
                    {
                        Console.Clear();
                        placecounter++;
                        if (placecounter != 0)
                        {
                            Console.WriteLine("Schiff erfolgreich Plaziert! {0} bleiben noch", (shipcount) - placecounter);
                        }
                        else
                        {
                            Console.WriteLine("Alle Schiffe erfolgreich Plaziert!");
                        }
                        DrawPlayfield(true);
                    }
                    else if (!InBounds(pshipsize, pshipx, pshipy, direction))
                    {
                        Console.WriteLine("Bitte Platziere dein Schiff innerhalb des Spielfeldes!");
                    }
                    else if (!PlayerShip(pshipsize, pshipx, pshipy, direction))
                    {
                        Console.WriteLine("Bitte Platziere dein Schiff nicht über ein anderes Schiff!");
                    }
                    if (placecounter == shipcount)
                    {
                        break;
                    }

                } while (true);
            }


            int gx, gy;
            bool isrunning = true;
            Console.WriteLine("Möge das Schießen beginnen!");
            do
            {
                try
                {
                    //User input for coordinates
                    Console.Write("Bitte geben sie die X-Koordinate ein: ");
                    gx = (Convert.ToInt32(Console.ReadLine())) - 1;
                    Console.Write("Bitte geben sie die Y-Koordinate ein: ");
                    gy = (Convert.ToInt32(Console.ReadLine())) - 1;

                    //Check for cheat code
                    if (gx == 419 && gy == 68)
                    {
                        cheat = true;
                        Console.Clear();
                        Console.WriteLine("Hidden Cheatcode activated!");
                        Console.ReadKey();
                        DrawPlayfield(false);
                        continue;
                    }
                    //check for correct coords
                    else if (gx > dimx || gx < 0 || gy > dimx || gy < 0)
                    {
                        Console.Clear();
                        Console.WriteLine("Bitte geben sie Korrekte Koordinaten ein!");
                        continue;
                    }

                    //check and draw
                    else if (Check(gx, gy))
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        playfield[gx, gy] = "X";
                        Console.ResetColor();
                        Console.WriteLine("Feuere auf {0} , {1}:... Getroffen! Glückwunsch :)", (gx + 1), (gy + 1));
                        DrawPlayfield(false);
                        CheckVictory(ref isrunning);
                        Console.ReadKey();
                        Console.WriteLine("Jetzt Feuere ich mal auf dich! HAHA!!");
                        Console.ReadKey();
                        if (RandomShot())
                        {
                            Console.WriteLine("Getroffen BWAHAHA!! ");
                        }
                        else
                        {
                            Console.WriteLine("Verdammnt, dich krieg ich noch >:D");
                        }
                        DrawPlayfield(true);
                    }
                    else
                    {
                        Console.Clear();
                        playfield[gx, gy] = "O";
                        Console.ResetColor();
                        Console.WriteLine("Feuere auf {0} , {1}:... Daneben :( versuchs nochmal!", (gx + 1), (gy + 1));
                        DrawPlayfield(false);
                        Console.WriteLine("Jetzt Feuere ich mal auf dich! HAHA");
                        Console.ReadKey();
                        if (RandomShot())
                        {
                            Console.WriteLine("Getroffen BWAHAHA!");
                        }
                        else
                        {
                            Console.WriteLine("Verdammnt, dich krieg ich noch >:D");
                        }
                        DrawPlayfield(true);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fehlerhafte eingabe: " + ex.Message);
                }

            } while (isrunning == true);
        }
        private void PlaceShip(int shipsize, bool player)
        {
            int posy;
            int posx;
            int axis;
            bool isrunning = true;
            int trys = 0;
            axis = random.Next(0, 2); // 0 = x ; 1 = y
            //generate random direction and checks for valid directions
            Direction direction = Direction.none;
            do
            {
                posx = random.Next(0, dimx);
                posy = random.Next(0, dimx);

                switch (axis)
                {
                    case 0:
                        if (posx - shipsize < 0)
                        {
                            direction = Direction.rechts;

                        }
                        else
                        {
                            direction = Direction.links;

                        }
                        break;
                    case 1:
                        if (posy - shipsize < 0)
                        {
                            direction = Direction.unten;

                        }
                        else
                        {
                            direction = Direction.oben;

                        }
                        break;
                }
                //actually writes the string into the array if placement is legal
                //this is only used for automatic placement
                bool possible = true;
                switch (direction)
                {
                    case Direction.rechts:
                        for (int i = 0; i < shipsize; i++)
                        {
                            if (player == true)
                            {
                                if (playerfield[posx + i, posy] == "^")
                                {
                                    possible = false;
                                }
                            }
                            else
                            {

                                if (playfield[posx + i, posy] == "^")
                                {
                                    possible = false;
                                }
                            }
                        }
                        if (possible == true)
                        {
                            if (player == true)
                            {
                                for (int i = 0; i < shipsize; i++)
                                {
                                    playerfield[posx + i, posy] = "^";
                                }
                                isrunning = false;
                            }
                            else
                            {

                                for (int i = 0; i < shipsize; i++)
                                {
                                    playfield[posx + i, posy] = "^";
                                }
                                isrunning = false;
                            }
                        }
                        break;
                    case Direction.links:
                        for (int i = 0; i < shipsize; i++)
                        {
                            if (player == true)
                            {
                                if (playerfield[posx - i, posy] == "^")
                                {
                                    possible = false;
                                }
                            }
                            else
                            {

                                if (playfield[posx - i, posy] == "^")
                                {
                                    possible = false;
                                }
                            }
                        }
                        if (possible == true)
                        {
                            if (player == true)
                            {
                                for (int i = 0; i < shipsize; i++)
                                {
                                    playerfield[posx - i, posy] = "^";
                                }
                                isrunning = false;
                            }
                            else
                            {

                                for (int i = 0; i < shipsize; i++)
                                {
                                    playfield[posx - i, posy] = "^";
                                }
                                isrunning = false;
                            }
                        }
                        break;
                    case Direction.unten:
                            for (int i = 0; i < shipsize; i++)
                            {
                                if (player == true)
                                {
                                    if (playerfield[posx , posy + i] == "^")
                                    {
                                        possible = false;
                                    }
                                }
                                else
                                {

                                    if (playfield[posx, posy + i] == "^")
                                    {
                                        possible = false;
                                    }
                                }
                            }
                        if (possible == true)
                        {
                            if (player == true)
                            {
                                for (int i = 0; i < shipsize; i++)
                                {
                                    playerfield[posx, posy + i] = "^";
                                }
                                isrunning = false;
                            }
                            else
                            {

                                for (int i = 0; i < shipsize; i++)
                                {
                                    playfield[posx, posy + i] = "^";
                                }
                                isrunning = false;
                            }
                        }
                        break;
                    case Direction.oben:
                        for (int i = 0; i < shipsize; i++)
                        {
                            if (player == true)
                            {
                                if (playerfield[posx , posy - i] == "^")
                                {
                                    possible = false;
                                }
                            }
                            else
                            {

                                if (playfield[posx , posy - i] == "^")
                                {
                                    possible = false;
                                }
                            }
                        }
                        if (possible == true)
                        {
                            if (player == true)
                            {
                                for (int i = 0; i < shipsize; i++)
                                {
                                    playerfield[posx , posy - i] = "^";
                                }
                                isrunning = false;
                            }
                            else
                            {

                                for (int i = 0; i < shipsize; i++)
                                {
                                    playfield[posx , posy - i] = "^";
                                }
                                isrunning = false;
                            }
                        }
                        break;
                    default:
                        break;

                }
                if (++trys > 1000) //try limit if no ship placement is possible
                {
                    isrunning = false;
                }

            } while (isrunning == true);
        }
        //method overload for multiple ships
        private void PlaceShip(int shipsize, int count)
        {
            for (int i = 0; i < count; i++)
            {
                PlaceShip(shipsize, false);
            }
        }
        //shoots a random spot on the playerfield
        private bool RandomShot()
        {
            int x = random.Next(1, dimx);
            int y = random.Next(1, dimx);
            if (playerfield[x, y] == "^")
            {
                playerfield[x, y] = "X";
                return true;
            }
            else
            {
                playerfield[x, y] = "O";
                return false;
            }
        }
        //this is only used for manual placement of ships by the player
        private bool PlayerShip(int shipsize, int x, int y, Direction direction)
        {
            //checks for legal placement and writes into the playerfield if legal
            switch (direction)
            {
                case Direction.rechts:
                    for (int i = 0; i < shipsize; i++)
                    {
                        if (playerfield[x + i, y] == "^")
                        {
                            return false;
                        }
                    }
                    for (int i = 0; i < shipsize; i++)
                    {
                        playerfield[x + i, y] = "^";
                    }
                    return true;

                case Direction.links:
                    for (int i = 0; i < shipsize; i++)
                    {
                        if (playerfield[x - i, y] == "^")
                        {
                            return false;
                        }
                    }
                    for (int i = 0; i < shipsize; i++)
                    {
                        playerfield[x - i, y] = "^";
                    }
                    return true;


                case Direction.unten:
                    for (int i = 0; i < shipsize; i++)
                    {
                        if (playerfield[x, y + i] == "^")
                        {
                            return false;
                        }

                    }
                    for (int i = 0; i < shipsize; i++)
                    {
                        playerfield[x, y + i] = "^";
                    }
                    return true;


                case Direction.oben:
                    for (int i = 0; i < shipsize; i++)
                    {
                        if (playerfield[x, y - i] == "^")
                        {
                            return false;
                        }
                    }

                    for (int i = 0; i < shipsize; i++)
                    {
                        playerfield[x, y - i] = "^";
                    }
                    return true;
                default:
                    return false;


            }
        }
        //checks if a placed ship is in bounds
        private bool InBounds(int shipsize, int x, int y, Direction direction)
        {
            if (x > dimx - 1 || x < 0)
            {
                return false;
            }
            else if (y > dimx - 1 || y < 0)
            {
                return false;
            }
            switch (direction)
            {
                case Direction.links:
                    if (x - shipsize < 0) return false;
                    return true;
                case Direction.rechts:
                    if (x + shipsize > dimx - 1) return false;
                    return true;
                case Direction.oben:
                    if (y - shipsize < 0) return false;
                    return true;
                case Direction.unten:
                    if (y + shipsize > dimx - 1) return false;
                    return true;
                default:
                    return false;
            }
        }
        //checks if a chosen spot is occupied by a ship
        private bool Check(int x, int y)
        {
            if (playfield[x, y] == "^")
            {
                return true;
            }
            return false;
        }
        private bool CheckVictory(string[,] pf)
        {

            for (int i = 0; i < dimx; i++)
            {
                for (int j = 0; j < dimx; j++)
                {
                    if (pf[i, j] == "^")
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        //this method actually displays the playingfields depending on the bool it gets, if true it displays the players playfield
        private void DrawPlayfield(bool player)
        {
            if (!player)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                for (int i = 0; i < dimx; i++)
                {
                    Console.WriteLine();
                    for (int j = 0; j < dimx; j++)
                    {
                        Console.Write(" ");

                        if (playfield[i, j] == "^")
                        {
                            if (cheat == true) //if cheat enabled will draw ships and highlight them
                            {
                                Console.BackgroundColor = ConsoleColor.Blue;
                                Console.Write("^");
                                Console.ResetColor();
                                Console.BackgroundColor = ConsoleColor.DarkBlue;
                            }
                            else
                            {
                                Console.Write("~");
                            }
                        }
                        else
                        {
                            Console.Write(playfield[i, j]);
                        }
                    }



                }
                Console.WriteLine();
                Console.ResetColor();
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                for (int i = 0; i < dimx; i++)
                {
                    Console.WriteLine();
                    for (int j = 0; j < dimx; j++)
                    {
                        Console.Write(" ");
                        if(playerfield[j,i] == "^")
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.Gray;
                        }
                        Console.Write(playerfield[j, i]);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                    }
                }
                Console.WriteLine();
                Console.ResetColor();
            }
        }
        private void CheckVictory(ref bool isrunning)
        {
            if (CheckVictory(playfield) && cheat)
            {
                Console.WriteLine("Du hast gewonnen... aber nicht ohne zu schummeln... ich bin entäuscht :(");
                Console.ReadKey();
                isrunning = false;
            }
            else if (CheckVictory(playfield))
            {
                Console.WriteLine("Gewonnen YAY!!!");
                Console.ReadKey();
                isrunning = false;
            }
            if (CheckVictory(playerfield) && cheat)
            {
                Console.WriteLine("Mein gott, du hast verloren obwohl du geschummelt hast?... wow, dass muss man erstmal schaffen :'D");
                Console.ReadKey();
                isrunning = false;
            }
            else if (CheckVictory(playerfield))
            {
                Console.WriteLine("Ich habe gewonnen BWAHAHA, DU LOOSER ]:P");
                Console.ReadKey();
                isrunning = false;
            }
        }
    }
}
