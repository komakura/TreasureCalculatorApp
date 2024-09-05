using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Forms.Integration;
using System.Windows.Forms;
using System.Linq;

namespace TreasureCalculator
{
    public partial class MainWindow : Window
    {
        private System.Windows.Controls.TextBox resultTextBox; // TextBox to display the result
        private System.Windows.Controls.TextBox detailTextBox; // TP強化の内訳を表示
        private decimal sum = 0;
        private int[,] eachTP = new int[8, 10];//newだと0が初期で入っている。
        
        private List<TreasureEnhancementItem> items;
        public MainWindow()
        {
            InitializeComponent();

            // 項目名を定義
            items = new List<TreasureEnhancementItem>
            {
                new TreasureEnhancementItem("弱点値上昇", new[] { "+1", "+2", "-", "+3", "-", "+4", "-", "+5", "-", "+6" }),
                new TreasureEnhancementItem("先制値上昇", new[] { "+1", "+2", "-", "+3", "-", "+4", "-", "+5", "-", "+6" }),
                new TreasureEnhancementItem("瞬間打撃点", new[] { "+2", "+4", "+6", "+8", "-", "+10", "-", "+12", "-", "+14" }),
                new TreasureEnhancementItem("瞬間防護点", new[] { "+2", "+4", "+6", "+8", "-", "+10", "-", "+12", "-", "+14" }),
                new TreasureEnhancementItem("瞬間達成値", new[] { "+1", "+2", "-", "+3", "-", "+4", "-", "+5", "-", "+6" }),
                new TreasureEnhancementItem("追加攻撃出目", new[] { "⑥/1", "⑤⑥/1", "-", "⑤⑥/2", "-", "-", "④⑤⑥/2", "-", "-", "④⑤⑥/3" }),
                new TreasureEnhancementItem("呪いの波動", new[] { "1点", "-", "2点", "-", "3点", "-", "4点", "-", "-", "5点" }),
                new TreasureEnhancementItem("世界の汚染", new[] { "-", "威力10", "-", "威力20", "-", "威力30", "-", "威力40", "-", "威力50" })
            };

            CreateTreasure(items);
        }

        private void CreateTreasure(List<TreasureEnhancementItem> items)
        {
            // メインのGridを作成
            Grid mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // resultTextBox用
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // ScrollViewer用

            // 0行目のGridを作成
            Grid rowGrid = new Grid();
            for (int i = 0; i < 10; i++)
            {
                rowGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 0; i < 11; i++)
            {
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            // 項目ヘッダーを追加
            string[] headers = { "項目", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
            for (int i = 0; i < headers.Length; i++)
            {
                AddCellWithBorder(rowGrid, new TextBlock
                {
                    Text = headers[i],
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center
                }, 0, i);
            }

            // 項目以降の行を作成
            for (int i = 0; i < items.Count; i++)
            {
                CreateRow(rowGrid, items[i].Name, i + 1, items[i].Values);
            }

            // メインのGridに追加
            Grid.SetRow(rowGrid, 0);
            mainGrid.Children.Add(rowGrid);


            // TextBoxを作成
            resultTextBox = new System.Windows.Controls.TextBox
            {
                IsReadOnly = true
            };
            resultTextBox.Text = "合計TP: 0";
            Grid.SetRow(resultTextBox, 2);
            mainGrid.Children.Add(resultTextBox);

            // リセットボタンを作成
            System.Windows.Controls.Button resetButton = new System.Windows.Controls.Button
            {
                Content = "全てリセット",
                Margin = new Thickness(0, 10, 0, 10)
            };
            resetButton.Click += ResetButton_Click;
            Grid.SetRow(resetButton, 1);  // resultTextBoxの上に配置
            mainGrid.Children.Add(resetButton);

            // ScrollViewerを作成し、その中にdetailTextBoxを配置
            ScrollViewer scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            detailTextBox = new System.Windows.Controls.TextBox
            {
                IsReadOnly = true,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                AcceptsReturn = true
            };

            scrollViewer.Content = detailTextBox;
            Grid.SetRow(scrollViewer, 3);
            mainGrid.Children.Add(scrollViewer);

            this.Content = mainGrid;
        }

        private void CreateRow(Grid grid, string labelText, int rowIndex, string[] values)
        {
            // 項目ラベル
            AddCellWithBorder(grid, new TextBlock
            {
                Text = labelText,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center
            }, rowIndex, 0);

            // NumericUpDownとTextBlockを配置
            for (int columnIndex = 1; columnIndex <= 10; columnIndex++)
            {
                if (values[columnIndex - 1] == "-")
                {
                    AddCellWithBorder(grid, new TextBlock
                    {
                        Text = values[columnIndex - 1],
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center
                    }, rowIndex, columnIndex);
                }
                else
                {
                    Grid innerGrid = new Grid();
                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    TextBlock textBlock = new TextBlock
                    {
                        Text = values[columnIndex - 1],
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center
                    };
                    Grid.SetColumn(textBlock, 0);
                    innerGrid.Children.Add(textBlock);

                    WindowsFormsHost host = new WindowsFormsHost();
                    NumericUpDown numericUpDown = new NumericUpDown
                    {
                        Minimum = 0,
                        Maximum = 50,
                        Value = 0,
                        Tag = new System.Tuple<int, int>(rowIndex - 1, columnIndex - 1), // 行と列の情報をTagに保存
                        Width = 60,  // 幅を大きくする
                        Height = 25  // 高さを大きくする
                    };
                    numericUpDown.ValueChanged += (s, e) => NumericUpDown_ValueChanged(s, e);

                    Grid.SetColumn(host, 1);
                    host.Child = numericUpDown;
                    innerGrid.Children.Add(host);

                    AddCellWithBorder(grid, innerGrid, rowIndex, columnIndex);
                }
            }
        }

        private void NumericUpDown_ValueChanged(object sender, System.EventArgs e)
        {
            NumericUpDown numericUpDown = sender as NumericUpDown;
            if (numericUpDown != null)
            {
                var tag = numericUpDown.Tag as System.Tuple<int, int>;
                if (tag != null)
                {
                    int row = tag.Item1;
                    int column = tag.Item2;
                    eachTP[row, column] = (int)numericUpDown.Value;
                    // 必要に応じて追加の計算や更新を行う
                    UpdateCalculation();
                    UpdateDetailTextBox();
                }
            }
        }

        private void UpdateCalculation()
        {
            // ここで全体の計算を更新する
            //内容が追加されるなら＋を入れる    
            bool isPlus = false;
            sum = 0;
            string str_sum ="";
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    //TPの係数を加える。
                    if(eachTP[i, j] != 0){ 
                        if(isPlus)
                        {
                            str_sum += " + ";
                        }
                        isPlus = true;
                        sum += eachTP[i, j]*(j+1);
                        //TP強化の内容と数を表示
                        str_sum += $"{items[i].Name}({items[i].Values[j]}){j+1}*{eachTP[i, j]}";
                    }
                }
            }
            resultTextBox.Text = $"合計TP: {sum} = {str_sum}";
        }        

        private void UpdateDetailTextBox()
        {
            //行が最初かどうか
            bool isFirstLine;
            //eachTPのiが変わるときに改行　ただし、列0~9の要素が0のときは改行しない   
            bool isLineBreak;
            string details = "";
            for (int i = 0; i < 8; i++)
            {
                //行が最初かどうか
                isFirstLine = true;
                isLineBreak = false;
                for (int j = 0; j < 10; j++)
                {
                    if (eachTP[i, j] != 0)
                    {
                        // 強化内容を1行に列挙するときに、+を入れるかどうかを判断する
                        if(isFirstLine){
                            details += $"{items[i].Name}({items[i].Values[j]})x{eachTP[i, j]}";
                            isLineBreak = true;
                            isFirstLine = false;
                        }else{
                            details += $" + {items[i].Name}({items[i].Values[j]})x{eachTP[i, j]}";
                            isLineBreak = true;
                        }   
                        
                    }
                }
                //eachTPのiが変わるときに改行　ただし、行0~9が0のときは改行しない
                if(isLineBreak){
                    details += "\n";
                    isLineBreak = false;
                }
            }
            detailTextBox.Text = details;
        }

        private void AddCellWithBorder(Grid grid, UIElement element, int rowIndex, int columnIndex)
        {
            Border border = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Child = element
            };

            Grid.SetRow(border, rowIndex);
            Grid.SetColumn(border, columnIndex);
            grid.Children.Add(border);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in ((Grid)this.Content).Children)
            {
                if (child is Grid rowGrid)
                {
                    foreach (var rowChild in rowGrid.Children)
                    {
                        if (rowChild is Border border && border.Child is Grid innerGrid)
                        {
                            foreach (var innerChild in innerGrid.Children)
                            {
                                if (innerChild is WindowsFormsHost host && host.Child is NumericUpDown numericUpDown)
                                {
                                    numericUpDown.Value = 0;
                                }
                            }
                        }
                    }
                }
            }
            UpdateCalculation();
            UpdateDetailTextBox();
        }
    }

    public class TreasureEnhancementItem
    {
        public string Name { get; set; }
        public string[] Values { get; set; }

        public TreasureEnhancementItem(string name, string[] values)
        {
            Name = name;
            Values = values;
        }
    }
}
