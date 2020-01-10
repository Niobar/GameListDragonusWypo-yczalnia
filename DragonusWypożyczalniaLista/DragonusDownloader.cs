using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;

namespace DragonusWypożyczalniaLista
{
    public class DragonusDownloader
    {
        readonly List<Game> gameList = new List<Game>();

        public DragonusDownloader()
        {

        }
        public void Start()
        {
            //Downloader();
            GameGetter();
            GameDataFromBGG();
            GameDataSaver();
        }

        private void GameDataSaver()
        {
            string tempToFile = "";
            File.WriteAllText("C:\\Users\\biuro\\Desktop\\Apps\\dragonusGameList.txt", tempToFile);
            foreach (var game in gameList)
            {
                tempToFile += game.ToString() + System.Environment.NewLine;
            }
            File.AppendAllText("C:\\Users\\biuro\\Desktop\\Apps\\dragonusGameList.txt", tempToFile);
        }

        private void Downloader()
        {
            {
                WebClient dragonusWeb = new WebClient();
                dragonusWeb.DownloadFile("https://dragonus.pl/pl/i/Wypozyczalnia-gier/18", "C:\\Users\\biuro\\Desktop\\Apps\\DragonusWypozyczalnia.txt");
            }
        }

        private void GameGetter()
        {
            Regex category = new Regex(@"(?<kategoria>Kategoria) \w - (?<lendPrice>\d*)");
            Regex endOfGames = new Regex("Regulamin Wypożyczalni");
            Regex game = new Regex(@"(<div>|<br /|>)(?<game>.*?)(</div>|<br /)");
            var wypozyczalniaWebPage = File.ReadAllText("C:\\Users\\biuro\\Desktop\\Apps\\DragonusWypozyczalnia.txt");
            var kategoriaIndexList = new List<WypozyczalniaIndeksICena>();
            MatchCollection mKategoria = category.Matches(wypozyczalniaWebPage);
            foreach (Match match in mKategoria)
            {
                kategoriaIndexList.Add(new WypozyczalniaIndeksICena { Index = match.Groups["kategoria"].Index, LendPrice = Int32.Parse(match.Groups["lendPrice"].Value) });
            }
            kategoriaIndexList.Add(new WypozyczalniaIndeksICena { Index = endOfGames.Match(wypozyczalniaWebPage).Index, LendPrice = 0 });
            for (int i = 0; i < kategoriaIndexList.Count - 1; i++)
            {
                MatchCollection mGames = game.Matches(wypozyczalniaWebPage.Substring(kategoriaIndexList[i].Index, kategoriaIndexList[i + 1].Index - kategoriaIndexList[i].Index));
                foreach (Match match in mGames)
                {
                    if (String.IsNullOrEmpty(match.Groups["game"].Value) || (match.Groups["game"].Value == " ")) ;
                    else
                    {
                        gameList.Add(new Game { Title = match.Groups["game"].Value, LendPrice = kategoriaIndexList[i].LendPrice });
                    }
                }
            }

        }
        private void GameDataFromBGG()
        {
            var searchLink = "https://www.boardgamegeek.com/geeksearch.php?action=search&objecttype=boardgame&q=";
            //Timer timerSlower = new Timer(2000);
            for (int i = 0; i < gameList.Count; i++)
            {
                //timerSlower.Elapsed += goOn;
                //timerSlower.AutoReset = true;
                //timerSlower.Enabled = true;
                System.Threading.Thread.Sleep(10000);
                {
                    Console.WriteLine(gameList[i].Title);
                    var bggClient = new WebClient();
                    var normalizedTitle = gameList[i].Title.Replace("&amp;", "%26");
                    string temp = bggClient.DownloadString(searchLink + normalizedTitle);
                    Regex boardGameNumber = new Regex(@"href=""/boardgame/(?<gameNumber>\d*)/");
                    Regex boardGameRank = new Regex(@"'collection_rank' *align='center' *>\s*<a name=""(?<gameRank>\d*)""></a>");
                    //Regex boardWeight = new Regex();
                    Regex boardGameScore = new Regex(@"class='collection_bggrating' align='center'>\s*(?<gameScore>\d.\d*)");

                    var tempMatchedNumber = boardGameNumber.Match(temp);
                    var tempNumber = tempMatchedNumber.Groups["gameNumber"].Value;
                    if (String.IsNullOrEmpty(tempNumber))
                    {
                        var googleClient = new WebClient();
                        var tempGoogle = googleClient.DownloadString("https://www.google.pl/search?ei=dFXdXYeYIpKFmwXHl5lY&q=" + gameList[i].Title + " BGG");
                        //File.WriteAllText("C:\\Users\\biuro\\Desktop\\Apps\\googleSearchNumber" + i + ".txt", tempGoogle);
                        Regex googleSearch = new Regex(@"(boardgamegeek.com/boardgame/\d*/(?<gameName>(\w|\-)*)&amp)|(>(?<gameName>(\w|\-|\s)*)\.\s*Name\.)");
                        var googleName = googleSearch.Match(tempGoogle);
                        var tempGoogleMatchedName = googleName.Groups["gameName"].Value;
                        temp = bggClient.DownloadString(searchLink + tempGoogleMatchedName.Replace('-', ' '));
                        tempMatchedNumber = boardGameNumber.Match(temp);
                        tempNumber = tempMatchedNumber.Groups["gameNumber"].Value;
                    }
                    int gameNumber;
                    Int32.TryParse(tempNumber, out gameNumber);

                    var tempMatchedRank = boardGameRank.Match(temp);
                    var tempRank = tempMatchedRank.Groups["gameRank"].Value;
                    int gameRank;
                    Int32.TryParse(tempRank, out gameRank);

                    //var tempMatchedWeight = boardWeight.Match(temp);
                    //var tempWeight = tempMatchedWeight.Groups["gameWeight"].Value;
                    //double gameWeight;
                    //Double.TryParse(tempWeight, out gameWeight);

                    var tempMatchedScore = boardGameScore.Match(temp);
                    var tempScore = tempMatchedScore.Groups["gameScore"].Value;
                    double gameScore;
                    Double.TryParse(tempScore.Replace('.', ','), out gameScore);

                    gameList[i].NumberBGG = gameNumber;
                    gameList[i].RankBGG = gameRank;
                    //gameList[i].WeightBGG = gameWeight;
                    gameList[i].ScoreBGG = gameScore;
                    //File.WriteAllText("C:\\Users\\biuro\\Desktop\\Apps\\gameNumber" + i + ".txt", temp);
                };
            }
        }

        //private void goOn(object sender, ElapsedEventArgs e)
        //{

        //}
    }
}
