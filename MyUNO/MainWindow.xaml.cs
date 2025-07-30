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

namespace MyUNO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UnoDeck Deck = new UnoDeck();
        public MainWindow()
        {
            InitializeComponent();
            

        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            string[] upperFiveCards = new string[5];
            for (int i = 0; i <= 4; i++)
            {
                upperFiveCards[i] = Deck.cards[i].ToString();
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
            Deck.Shuffle();
            Button1_Click(sender, e); // 重新显示上五张牌
        }
        public void Tester()
        {
            UnoHandCard hand = new UnoHandCard();
            if (Deck.tryDraw(out UnoCard drawnCard))
            {
                hand.AddCard(drawnCard);
                String Result = hand.ShowCard();
                MessageBox.Show(Result);
            }
            else
            {
                //Console.WriteLine("No cards left to draw.");
                //Maybe show a message box or log
                MessageBox.Show("No cards left to draw.");
            }
        }

        private void Test(object sender, RoutedEventArgs e)
        {
            Tester();
        }
    }
    public enum UnoColor 
    {
        Red, Green, Blue, Yellow ,Wild
    }
    public enum UnoValue    
    {
        Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine,
        Skip = 50 , Reverse = 51, DrawTwo = 52 , 
        WildCard = 100 , WildDrawFour = 101
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
            setDeck();
            Shuffle();
        }
        public void setDeck()
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

        public bool tryDraw(out UnoCard drawnCard)
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

        public List<UnoCard> Cards { get; set; }
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
        public String ShowCard()
        {
            String result = "";
            foreach (UnoCard card in Cards)
            {
                result += card.ToString()+ "\n";
            }
            return result;
        }
    }

    
    }
