using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LabelEx
{
    public class Item
    {
        public string name { get; set; }
        public double price { get; set; }
        public Image image { get; set; }
        public int stock { get; set; } = 6;

        public Item(string _name, double _price, Image _image)
        {
            name = _name;
            price = _price;

            // Found this on stackoverflow
            Bitmap result = new Bitmap(71, 71);

            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(_image, 0, 0, 71, 71);
            }


            image = result;
        }

        public void restock()
        {
            stock = 6;
        }
    }
    public class Entry : Form
    {
        public List<Item> inventory;
        public List<Item> cart = new List<Item>();
        public double cartPrice = 0.00;
        public Entry()
        {

            ClientSize = new Size(400, 650);
            BackColor = Color.FromArgb(227, 30, 14);

            string cwd = Directory.GetCurrentDirectory();

            string drink_coca_cola_path = Path.Combine(cwd, "../../../assets/drink-coca-cola.png");
            string peach_monster_path = Path.Combine(cwd, "../../../assets/drink-coca-cola.png");
            string coca_cola_path = Path.Combine(cwd, "../../../assets/coca-cola.png");
            string dr_pepper_path = Path.Combine(cwd, "../../../assets/dr-pepper.png");
            string red_bull_path = Path.Combine(cwd, "../../../assets/red-bull.png");
            string orange_fanta_path = Path.Combine(cwd, "../../../assets/orange-fanta.jpg");
            string peace_tea_path = Path.Combine(cwd, "../../../assets/peace-tea.png");
            string arizona_tea_path = Path.Combine(cwd, "../../../assets/arizona-tea.png");
            string mtn_dew_path = Path.Combine(cwd, "../../../assets/mtn-dew.jpg");
            string mug_root_beer_path = Path.Combine(cwd, "../../../assets/mug-root-beer.jpg");
            string brisk_tea_path = Path.Combine(cwd, "../../../assets/brisk-tea.jpeg");

            PictureBox picBox = new PictureBox
            {
                Width = ClientSize.Width,
                Height = 100,
                Image = Image.FromFile(drink_coca_cola_path),
                SizeMode = PictureBoxSizeMode.Zoom,
                Anchor = AnchorStyles.Top
            };
            Controls.Add(picBox);

            inventory = [
                new Item("Coca Cola", 2.99, Image.FromFile(coca_cola_path)),
                new Item("Redbull", 3.99, Image.FromFile(red_bull_path)),
                new Item("Dr. Pepper", 1.99, Image.FromFile(dr_pepper_path)),
                new Item("Orange Fanta", 2.99, Image.FromFile(orange_fanta_path)),
                new Item("Peace Tea", 0.99, Image.FromFile(peace_tea_path)),
                new Item("Arizona Tea", 0.99, Image.FromFile(arizona_tea_path)),
                new Item("Mountain Dew", 2.99, Image.FromFile(mtn_dew_path)),
                new Item("MUG Root Beer", 2.99, Image.FromFile(mug_root_beer_path)),
                new Item("Brisk Iced Tea", 1.99, Image.FromFile(brisk_tea_path))
            ];

            int buttonWidth = 71;
            int buttonHeight = 71;

            ToolTip tooltip = new ToolTip();

            for (int i = 0; i < inventory.Count; i++)
            {
                var item = inventory[i];
                var buyButton = new Button
                {
                    //Text = item.name,
                    Size = new Size(buttonWidth, buttonHeight),
                    Image = item.image,
                    TextImageRelation = TextImageRelation.Overlay
                };

                int row = i / 3; // div and mod by 3 cuz i want 3 columns
                int col = i % 3;

                // should pull out a padding variable. this is messy.
                // perhaps make a box and center it on that
                buyButton.Location = new Point(88 + col * (buttonWidth + 5), 125 + row * (buttonHeight + 5));
                buyButton.Click += (_, _) => buyClick(item);

                tooltip.SetToolTip(buyButton, $"{item.name} ${item.price}");
                Controls.Add(buyButton);

            }

            Button openButton = new Button
            {
                Text = "OPEN",
                ForeColor = Color.White,
                BackColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(200, 60),
                Location = new Point((ClientSize.Width - 200) / 2, 350 + 20),
                Anchor = AnchorStyles.Top,
            };
            Controls.Add(openButton);

        }

        private void buyClick(Item item)
        {
            if (--item.stock <= 0)
            {
                Console.WriteLine("Out of stock!");
                // Pop a notification bubble thing up maybe?
                foreach (Control control in Controls)
                {
                    if (control is Button button && button.Image == item.image)
                    {
                        button.Enabled = false;
                        break;
                    }
                }
                return;
            }
            Console.WriteLine($"Clicked {item.name} with price ${item.price}", "Item Clicked");
            cart.Add(item);
            cartPrice += item.price;
            Console.WriteLine($"Cart Price: ${cartPrice}");
        }

        private void checkOut()
        {
            double tax = 0.01;
            double withTax = cartPrice * (1 + tax);

            Console.WriteLine($"Buying {cart.Count} items for ${cartPrice}");
        }

        private void reset()
        {
            cart = new List<Item>();
            cartPrice = 0.00;

            for (int i = 0; i < inventory.Count; i++)
            {
                inventory.ElementAt(i).restock();
            }
        }

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.Run(new Entry());
        }
    }
}
