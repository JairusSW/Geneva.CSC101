/*
I used https://zetcode.com/csharp/windowsforms/ and the docs as main resources for this.
Sorry about that bad code quality here. I last used C# when I was 12.
*/
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LabelEx
{
    public class ReceiptPopup : Form
    {
        public ReceiptPopup(List<Item> cart, decimal subtotal, decimal tax, decimal total)
        {
            Text = "Receipt";
            Size = new Size(200, 300);
            StartPosition = FormStartPosition.CenterScreen;
            AutoScroll = true;

            Label titleLabel = new Label
            {
                Text = "Receipt:\n-------------------",
                AutoSize = true,
                Location = new Point(10, 30)
            };
            Controls.Add(titleLabel);

            int offset = 70;
            for (int i = 0; i < cart.Count; i++)
            {
                Item item = cart.ElementAt(i);
                Label itemLabel = new Label
                {
                    Text = $" {item.name} - ${item.price}",
                    AutoSize = true,
                    Location = new Point(10, offset)
                };
                Controls.Add(itemLabel);
                offset += 20;
            }

            Label subtotalLabel = new Label
            {
                Text = $"-------------------\nSubtotal: ${subtotal}",
                AutoSize = true,
                Location = new Point(10, offset)
            };
            Controls.Add(subtotalLabel);

            Label taxLabel = new Label
            {
                Text = $"Tax: ${tax}",
                AutoSize = true,
                Location = new Point(10, offset + 40)
            };
            Controls.Add(taxLabel);

            Label totalLabel = new Label
            {
                Text = $"Total: ${total}",
                AutoSize = true,
                Location = new Point(10, offset + 60)
            };
            Controls.Add(totalLabel);

            Button okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(100, offset + 80),
                Size = new Size(75, 25)
            };

            okButton.Click += (_, _) => Close();

            Controls.Add(okButton);
        }
    }
    public class Item
    {
        public string name { get; set; }
        public decimal price { get; set; }
        public Image image { get; set; }
        public int stock { get; set; } = 6;
        public Button btn { get; set; } = new Button();

        public Item(string _name, decimal _price, Image _image)
        {
            name = _name;
            price = _price;

            // Found this on stackoverflow
            Bitmap result = new Bitmap(71, 71);

            Graphics graphics = Graphics.FromImage(result);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(_image, 0, 0, 71, 71);

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

        private ToolTip tooltip = new ToolTip();

        public Entry()
        {
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(800, 450);

            Button quit = new Button
            {
                Height = 30,
                Width = 30,
                Text = "✕",
                AutoSize = false
            };
            quit.Click += (_, _) => Close();

            Controls.Add(quit);

            CenterToScreen();

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

            for (int i = 0; i < inventory.Count; i++)
            {
                var item = inventory[i];
                item.btn = new Button
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
                item.btn.Location = new Point(88 + col * (buttonWidth + 5), 125 + row * (buttonHeight + 5));
                item.btn.Click += (_, _) => buyClick(item);

                tooltip.SetToolTip(item.btn, $"{item.name} ${item.price} ({item.stock} left)");
                Controls.Add(item.btn);

            }

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
                Text = $"Tax ({taxPercent * 100}%): $0.00"
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

            Button checkoutButton = new Button
            {
                Text = "Checkout",
                Location = new Point(350, 460),
                Size = new Size(100, 30)
            };
            checkoutButton.Click += (_, _) => checkOut();
            Controls.Add(checkoutButton);

            Button cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(480, 460),
                Size = new Size(100, 30)
            };
            cancelButton.Click += (_, _) => cancelOrder();
            Controls.Add(cancelButton);

            Button restockButton = new Button
            {
                Text = "Restock",
                Location = new Point(210, 500),
                Size = new Size(100, 30)
            };
            restockButton.Click += (_, _) => restock();
            Controls.Add(restockButton);

            Button resetButton = new Button
            {
                Text = "Reset",
                Location = new Point(320, 500),
                Size = new Size(100, 30)
            };
            resetButton.Click += (_, _) => reset();
            Controls.Add(resetButton);
        }

        private void cancelOrder()
        {
            for (int i = 0; i < cart.Count; i++)
            {
                // agh, horribly inefficient
                Item item = cart.ElementAt(i);
                item.stock++;
                item.btn.Enabled = true;
                tooltip.SetToolTip(item.btn, $"{item.name} ${item.price} ({item.stock} left)");
            }

            cart = new List<Item>();
            cartPrice = 0.00m;
            cartItemInfo.Text = "";
            subtotalLabel.Text = "Subtotal: $0.00";
            taxLabel.Text = $"Tax ({taxPercent * 100}%): $0.00";
            totalLabel.Text = $"Total: $0.00";
        }
        private void buyClick(Item item)
        {
            if (item.stock <= 0) return;
            if (item.stock == 1)
            {
                Console.WriteLine("Out of stock!");
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (Controls[i] is Button button && button.Image == item.image)
                    {
                        button.Enabled = false;
                        tooltip.SetToolTip(item.btn, "Out of stock!");
                        break;
                    }
                }
            }
            item.stock--;
            cart.Add(item);
            cartPrice += item.price;
            cartItemInfo.Text += $"{item.name} - ${item.price}\n";

            subtotalLabel.Text = $"Subtotal: ${cartPrice}";
            // does it have like a .toFixed method or something
            decimal totalPrice = Math.Round(cartPrice * (1 + taxPercent) * 100) / 100;
            taxLabel.Text = $"Tax ({taxPercent * 100}%): ${totalPrice - cartPrice}";
            totalLabel.Text = $"Total: ${totalPrice}";
            tooltip.SetToolTip(item.btn, $"{item.name} ${item.price} ({item.stock} left)");
        }

        private void checkOut()
        {
            cartItemInfo.Text = "";
            subtotalLabel.Text = "Subtotal: $0.00";
            taxLabel.Text = $"Tax ({taxPercent * 100}%): $0.00";
            Console.WriteLine($"Buying {cart.Count} items for ${cartPrice}");
            totalLabel.Text = $"Total: $0.00";
            decimal totalPrice = Math.Round(cartPrice * (1 + taxPercent) * 100) / 100;
            new ReceiptPopup(cart, cartPrice, totalPrice - cartPrice, totalPrice).Show();

            cart = new List<Item>();
            cartPrice = 0.00m;
        }

        private void restock()
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                Item item = inventory.ElementAt(i);
                item.restock();
                item.btn.Enabled = true;
                tooltip.SetToolTip(item.btn, $"{item.name} ${item.price} ({item.stock} left)");
            }
        }
        private void reset()
        {
            cart = new List<Item>();
            cartPrice = 0.00m;
            restock();
            cartItemInfo.Text = "";
            subtotalLabel.Text = $"Subtotal: $0.00";
            taxLabel.Text = $"Tax ({taxPercent * 100}%): $0.00";
            totalLabel.Text = $"Total: $0.00";
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
