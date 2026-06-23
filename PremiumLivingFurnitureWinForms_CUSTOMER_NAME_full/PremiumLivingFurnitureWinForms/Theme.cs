using System.Drawing;
using System.Windows.Forms;
namespace PremiumLivingFurnitureWinForms;
public static class Theme{
 public static Color AppBg=>Color.FromArgb(245,247,251);
 public static Color Side=>Color.FromArgb(15,23,42);
 public static Color Side2=>Color.FromArgb(30,41,59);
 public static Color Primary=>Color.FromArgb(37,99,235);
 public static Color PrimarySoft=>Color.FromArgb(219,234,254);
 public static Color Text=>Color.FromArgb(15,23,42);
 public static Color Muted=>Color.FromArgb(100,116,139);
 public static Color Card=>Color.White;
 public static Font DefaultFont=>new Font("Segoe UI",10);
 public static Button PrimaryButton(string text,int width=130)=>Btn(text,width,Primary,Color.White,false);
 public static Button SecondaryButton(string text,int width=120)=>Btn(text,width,Color.White,Text,true);
 static Button Btn(string text,int width,Color back,Color fore,bool border){var b=new Button{Text=text,Width=width,Height=40,BackColor=back,ForeColor=fore,FlatStyle=FlatStyle.Flat,Font=new Font("Segoe UI",9,FontStyle.Bold),Cursor=Cursors.Hand,Margin=new Padding(0,5,10,5)};b.FlatAppearance.BorderSize=border?1:0;b.FlatAppearance.BorderColor=Color.FromArgb(226,232,240);return b;}
 public static Label Chip(string text)=>new Label{Text="  "+text+"  ",AutoSize=false,Width=165,Height=30,TextAlign=ContentAlignment.MiddleCenter,BackColor=PrimarySoft,ForeColor=Primary,Font=new Font("Segoe UI",9,FontStyle.Bold)};
 public static void Grid(DataGridView g){g.Dock=DockStyle.Fill;g.ReadOnly=true;g.AllowUserToAddRows=false;g.RowHeadersVisible=false;g.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;g.SelectionMode=DataGridViewSelectionMode.FullRowSelect;g.BackgroundColor=Card;g.BorderStyle=BorderStyle.None;g.CellBorderStyle=DataGridViewCellBorderStyle.SingleHorizontal;g.GridColor=Color.FromArgb(226,232,240);g.EnableHeadersVisualStyles=false;g.ColumnHeadersHeight=40;g.RowTemplate.Height=34;g.ColumnHeadersDefaultCellStyle.BackColor=Color.FromArgb(241,245,249);g.ColumnHeadersDefaultCellStyle.ForeColor=Text;g.ColumnHeadersDefaultCellStyle.Font=new Font("Segoe UI",9,FontStyle.Bold);g.DefaultCellStyle.Font=new Font("Segoe UI",9);g.DefaultCellStyle.Padding=new Padding(6,2,6,2);g.DefaultCellStyle.SelectionBackColor=PrimarySoft;g.DefaultCellStyle.SelectionForeColor=Text;g.AlternatingRowsDefaultCellStyle.BackColor=Color.FromArgb(248,250,252);}
}
