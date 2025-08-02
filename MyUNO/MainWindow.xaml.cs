using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//My first C# WPF project     
namespace MyUNO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UnoDeck _deck = new UnoDeck();
        public MainWindow()
        {
            InitializeComponent();
            

        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            string[] upperFiveCards = new string[5];
            for (int i = 0; i <= 4; i++)
            {
                upperFiveCards[i] = _deck.cards[i].ToString();
            }
            foreach (string card in upperFiveCards)
            {
                Console.WriteLine(card);  // 或调试窗口打印
            }
            l1.Content = upperFiveCards[0];
            l2.Content = upperFiveCards[1];
            l3.Content = upperFiveCards[2];
            l4.Content = upperFiveCards[3];
            l5.Content = upperFiveCards[4];
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            _deck.Shuffle();
            Button1_Click(sender, e); // 重新显示上五张牌
        }
        public string TryDrawOneCardAndShow()
        {
            UnoHandCard hand = new UnoHandCard();
            if (_deck.TryDraw(out UnoCard drawnCard))
            {
                hand.AddCard(drawnCard);
                return (string)hand.ShowAllHandCard();
                //MessageBox.Show(Result);
            }
            else
            {
                //Console.WriteLine("No cards left to draw.");
                //Maybe show a message box or log
                return "No cards left to draw.";
            }
        }

        private void OnTestButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(TryDrawOneCardAndShow());
        }
    }
    public enum UnoColor 
    {
        Red, Green, Blue, Yellow, Wild,
        ErrorColor = 9999
    }
    public enum UnoValue    
    {
        Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine,
        Skip = 50, Reverse = 51, DrawTwo = 52,
        WildCard = 100, WildDrawFour = 101,
        ErrorValue = 9999
    }
    public enum UnoValueAddition
    {
        Special1, Special2, Special3, Special4, Special5, Special6, Special7, Special8, Special9,
    }
    public class UnoCard
    {
        public UnoColor Color { get; set; }
        public UnoValue Value { get; set; }

        //public UnoValueAddition ValueAddition { get; set; } 
        //暂时不引入额外牌
        public UnoCard()
        {
            Color = UnoColor.ErrorColor;
            Value = UnoValue.ErrorValue;
            //A Red, WildDrawFour, means there is something wrong.
        }
        public UnoCard(UnoColor color, UnoValue value)
        {
            Color = color;
            Value = value;
        }
        public override string ToString()
        {
            return $"[{Color}] {Value}";
        }
    }
    public class UnoDeck
    {
        public List<UnoCard> cards;
        private Random rng = new Random();
        public UnoDeck()
        {
            cards = new List<UnoCard>();
            SetDeck();
            Shuffle();
        }
        public void SetDeck()
        {
            cards.Clear();
            //Let's add nums cards to the deck
            UnoColor[] colors = { UnoColor.Blue, UnoColor.Green, UnoColor.Red, UnoColor.Yellow };
            foreach (UnoColor color in colors)
            {
                cards.Add(new UnoCard(color, UnoValue.Zero));

                for (int i = 1; i <= 2; i++)
                {
                    cards.Add(new UnoCard(color, UnoValue.DrawTwo));
                    cards.Add(new UnoCard(color, UnoValue.Skip));
                    cards.Add(new UnoCard(color, UnoValue.Reverse));
                }

                for (int i = 1; i <= 9; i++)
                {
                    cards.Add(new UnoCard(color, (UnoValue)i));
                    cards.Add(new UnoCard(color, (UnoValue)i));
                }
            }
            //Add Wild and WildDrawFour cards
            for (int i = 0; i < 4; i++)
            {
                cards.Add(new UnoCard(UnoColor.Wild, UnoValue.WildCard));
                cards.Add(new UnoCard(UnoColor.Wild, UnoValue.WildDrawFour));
            }
        }

        public void Shuffle()
        {
            int n = cards.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = rng.Next(0, i + 1);
                // Swap cards[i] and cards[j]
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }
        }

        public bool TryDraw(out UnoCard drawnCard)
        {
            drawnCard = null;
            if (cards.Count == 0)
                return false;
            drawnCard = cards[0];
            cards.RemoveAt(0);
            return true;
        }
    }
    public class UnoHandCard
    {

        public List<UnoCard> Cards { get; private set; }
        public UnoHandCard()
        {
            Cards = new List<UnoCard>();
        }
        public void AddCard(UnoCard card)
        {
            Cards.Add(card);
        }
        public void RemoveCard(UnoCard card)
        {
            Cards.Remove(card);
        }
        public String ShowAllHandCard()
        {
            String result = "";
            foreach (UnoCard card in Cards)
            {
                result += card.ToString() + "\n";
            }
            return result;
        }
        public int Count()
        {
            return Cards.Count;
        }
    }

    public enum PlayerType
    {
        Human,
        HonestAI,
        NormalAI,
        HardAI,
        CheaterAI
    }
    public class UnoPlayer
    {
        public UnoHandCard hand = new UnoHandCard();
        public String playerName = "Default";
        public PlayerType type { get; set; } = PlayerType.HonestAI;
        // public void AddToHand(UnoCard card)
        //     {
        //     hand.AddCard(card);
        //     }
        //Have defined addcard and removecard in UnoHandCard, no need to redefine again.
        //How to set up a player? How to get data in? Using which method?
        public void SetUpPlayer(string playerName, PlayerType type)//I decide to using static datapair.
        {
            this.playerName = playerName;
            this.type = type;
        }
    }
    public class CardList
    {
        public List<UnoCard> cardsList = new List<UnoCard>();
        public void AddCard(UnoCard card)
        {
            cardsList.Add(card);
        }
    }
    public class AICardStrategy
    {
        public static List<(UnoCard, float)> GetCardSocer(UnoHandCard hand, CardList thisTurn)
        {
            List<(UnoCard, float)> result = new List<(UnoCard, float)>();

            UnoCard Lastcard = thisTurn.cardsList[^1];

            //Check if the card is playable.
            int playable = 0;
            for (int i = 0; i < hand.Count(); i++)
            //((UnoCard,int) card in result)
            {
                UnoCard card = hand.Cards[i];
                if (card.Color == Lastcard.Color ||
                    card.Color == UnoColor.Wild ||
                    card.Value == Lastcard.Value)
                {
                    result.Add((card, 10)); //Playable.
                    playable++;
                }
            }

            //No playable cards, return.
            if (playable == 0)
            {
                return result;
            }

            //Choose the best card to play.
            for (int i = 0; i < result.Count; i++)
            {
                var card = result[i];
                float value = card.Item2;
                //Check if the card is a Wild or WildDrawFour.
                if (card.Item1.Color == UnoColor.Wild)
                {
                    //No need to play wild cards too early.
                    value = value * 1;
                    if (card.Item1.Value == UnoValue.WildDrawFour)
                    {
                        value = value / 3;
                    }

                }

                if (card.Item1.Value == UnoValue.DrawTwo)
                {
                    //No agressive AI, thank you.
                    value = value * 0.5f;

                }

                if (card.Item1.Value == UnoValue.Skip)
                {
                    //No agressive AI, thank you.
                    value = value * 0.67f;
                }

                if (card.Item1.Value == UnoValue.Reverse)
                {
                    value = value * 0.9f;
            }

                result[i] = (card.Item1, value);
            }

            double colorMultiplier = 1.5;
            double valueMultiplier = 1.2;
            Dictionary<UnoColor, int> colorCounts = new Dictionary<UnoColor, int>();
            Dictionary<UnoValue, int> valueCounts = new Dictionary<UnoValue, int>();
            foreach (var card in hand.Cards)
            {
                if (!colorCounts.ContainsKey(card.Color))
                    colorCounts[card.Color] = 0;
                colorCounts[card.Color]++;
                if (!valueCounts.ContainsKey(card.Value))
                    valueCounts[card.Value] = 0;
                valueCounts[card.Value]++;
            }

            for (int i = 0; i < result.Count; i++)
            {
                var card = result[i];
                float value = card.Item2;
                // Check color counts
                if (colorCounts.ContainsKey(card.Item1.Color))
                {
                    value = (float)(value * Math.Pow(colorMultiplier, colorCounts[card.Item1.Color]));
                }
                // Check value counts
                if (valueCounts.ContainsKey(card.Item1.Value))
                {
                    value = (float)(value * Math.Pow(valueMultiplier, valueCounts[card.Item1.Value]));
                }
                result[i] = (card.Item1, value);
            }
            return result;
        }
        //Let's coding the mainGame logic.

    }

    public class MainUnoGame
    {
        public UnoPlayer[] players;
        UnoDeck deck = new UnoDeck();
        CardList cardList = new CardList();
        public MainUnoGame()
        {
            //How many players?
            int playerCount = 4; //For now, hardcode it.
            //Will get playerType outside, todo.
            this.players = new UnoPlayer[playerCount];
            this.players[0] = new UnoPlayer();
            this.players[0].type = PlayerType.Human;//Always the first player is human.
            this.players[0].playerName = "Lucidius";
            for (int i = 1; i < playerCount; i++)
            {
                this.players[i] = new UnoPlayer();
                this.players[i].type = PlayerType.HonestAI;
                this.players[i].playerName = "HonestAI" + i.ToString();
            }
            //No meaningful type now,All AI players are HonestAI.
            InitializeGame(out UnoCard firstCard);
            this.cardList.AddCard(firstCard);
            for(int playerIndex = 0; playerIndex < playerCount; playerIndex++)
            {
                if(players[playerIndex].type == PlayerType.Human)
                {
                    Console.WriteLine($"Player {playerIndex + 1} is a human player: {players[playerIndex].playerName}");
                    //do something humanplayer will do...
                }
                else
                {
                    Console.WriteLine($"Player {playerIndex + 1} is an AI player: {players[playerIndex].playerName}");
                    //call AI logic, now!
                    List<(UnoCard,float)> socredList = AICardStrategy.GetCardSocer(players[playerIndex].hand, cardList);
                    //There will be more strategies in the future.Todo.
                    float sumScore = 0;
                    List<(UnoCard,float)> prefixSumList = new List<(UnoCard, float)>();
                    foreach (var (card,score) in socredList)
                    {
                        sumScore += score;
                        prefixSumList.Add((card, sumScore));
                    }
                    Random rng = new Random();
                    float thisScore = 0;
                    thisScore = (float)rng.NextDouble() * sumScore; //Get a random score.
                    UnoCard chosenCard = new UnoCard(); //Set a default card.
                    foreach (var (card,score) in prefixSumList)
                    {
                        if (thisScore <= score)
                        {
                            chosenCard = card; //Found the card.
                            break;
                        }
    }
                }
            }
        }

    


        public void InitializeGame(out UnoCard drawnCard)
        {
            //Give every player 7 cards.
            for (int i = 0; i < players.Length; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (deck.TryDraw(out UnoCard card))
                    {
                        players[i].hand.AddCard(card);
    }
                    else
                    {
                        Console.WriteLine("No cards left to draw.");
                        break;
                    }
                }
            }
            //Reveal the first card.
            if (deck.TryDraw(out drawnCard))
            {
                String drawnCardString = drawnCard.ToString();
                Console.WriteLine($"First card drawn: {drawnCardString}");
                //MessageBox.Show(Result);
            }
            else
            {
                Console.WriteLine("No cards left to draw.");
                //drawnCard.Color = UnoColor.Wild;
                //drawnCard.Value = UnoValue.WildCard; //Set a default card if no cards left.
                drawnCard = FastUtils.SetDefaultUnoCard();
            }
        }
    }
    public class FastUtils
    {
        //This class is used to store some fast utils.
        //Remember, all methods in this class are static.
        public static UnoCard SetDefaultUnoCard()
        {
            UnoCard card = new UnoCard(UnoColor.Wild, UnoValue.WildCard);
             //Set a default card if no cards left.
            return card;
        }

        //public static 
    }
}


   
