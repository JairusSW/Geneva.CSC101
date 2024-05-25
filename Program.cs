using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LabelEx
{
    public class Item
    {
        public string name { get; set; }
        public decimal price { get; set; }
        public Image image { get; set; }
        public int stock { get; set; } = 6;

        public Item(string _name, decimal _price, Image _image)
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

        public decimal taxPercent = 0.09m;
        private List<Item> inventory;
        private List<Item> cart = new List<Item>();
        private decimal cartPrice = 0.00m;

        private Panel cartPanel;
        private Label cartItemInfo = new Label
        {
            Text = "",
            ForeColor = Color.White,
            AutoSize = true
        };
        private Label subtotalLabel;
        private Label taxLabel;
        private Label totalLabel;
        public Entry()
        {

            ClientSize = new Size(630, 550);
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
                Width = 222, // ratio is ~2.22 610x274
                Height = 100,
                Image = Image.FromFile(drink_coca_cola_path),
                SizeMode = PictureBoxSizeMode.Zoom,
                Anchor = AnchorStyles.Left,
                Location = new Point(90, 20)
            };
            Controls.Add(picBox);

            inventory = [
                new Item("Coca Cola", 2.99m, Image.FromFile(coca_cola_path)),
                new Item("Redbull", 3.99m, Image.FromFile(red_bull_path)),
                new Item("Dr. Pepper", 1.99m, Image.FromFile(dr_pepper_path)),
                new Item("Orange Fanta", 2.99m, Image.FromFile(orange_fanta_path)),
                new Item("Peace Tea", 0.99m, Image.FromFile(peace_tea_path)),
                new Item("Arizona Tea", 0.99m, Image.FromFile(arizona_tea_path)),
                new Item("Mountain Dew", 2.99m, Image.FromFile(mtn_dew_path)),
                new Item("MUG Root Beer", 2.99m, Image.FromFile(mug_root_beer_path)),
                new Item("Brisk Iced Tea", 1.99m, Image.FromFile(brisk_tea_path))
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

            /*  Button openButton = new Button
              {
                  Text = "OPEN",
                  ForeColor = Color.White,
                  BackColor = Color.Black,
                  TextAlign = ContentAlignment.MiddleCenter,
                  Size = new Size(200, 60),
                  Location = new Point((ClientSize.Width - 200) / 2, 350 + 20),
                  Anchor = AnchorStyles.Left,
              };
              Controls.Add(openButton);*/

            cartPanel = new Panel
            {
                Location = new Point(350, 20),
                Size = new Size(230, 350),
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };
            Controls.Add(cartPanel);
            cartPanel.Controls.Add(cartItemInfo);

            subtotalLabel = new Label
            {
                Location = new Point(350, 380),
                Size = new Size(230, 20),
                ForeColor = Color.White,
                Text = "Subtotal: $0.00"
            };
            Controls.Add(subtotalLabel);

            taxLabel = new Label
            {
                Location = new Point(350, 400),
                Size = new Size(230, 20),
                ForeColor = Color.White,
                Text = "Tax (1%): $0.00"
            };
            Controls.Add(taxLabel);

            totalLabel = new Label
            {
                Location = new Point(350, 420),
                Size = new Size(230, 20),
                ForeColor = Color.White,
                Text = "Total: $0.00"
            };
            Controls.Add(totalLabel);

        }

        private void buyClick(Item item)
        {
            if (--item.stock <= 0)
            {
                Console.WriteLine("Out of stock!");
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

            cartItemInfo.Text += $"{item.name} - ${item.price}\n";

            subtotalLabel.Text = $"Subtotal: ${cartPrice}";
            // does it have like a .toFixed method or something
            decimal totalPrice = Math.Round(cartPrice * (1 + taxPercent) * 100) / 100; ;
            taxLabel.Text = $"Tax (${taxPercent}%): ${totalPrice - cartPrice}";
            totalLabel.Text = $"Total: ${totalPrice}";
        }

        private void checkOut()
        {
            decimal tax = 0.01m;
            decimal withTax = cartPrice * (1 + tax);

            Console.WriteLine($"Buying {cart.Count} items for ${cartPrice}");
        }

        private void reset()
        {
            cart = new List<Item>();
            cartPrice = 0.00m;

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
