using Npgsql;
using System.Data;

namespace demiryoluSistemi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            kutulariDevreDisiBirak();
            comboBoxDoldur();
        }

        string baglantiString = "server=localHost; port=5432; Database=demiryoluSistemi; user ID=postgres; password=6579";
        NpgsqlConnection baglanti;
        NpgsqlCommand komut;

        private void baglan()
        {
            baglanti = new NpgsqlConnection(baglantiString);
            baglanti.ConnectionString = baglantiString;

            if (baglanti.State == ConnectionState.Closed)
            {
                baglanti.Open();
            }
        }

        public DataTable veriGetir(string sql)
        {
            DataTable dt = new DataTable();
            baglan();
            komut = new NpgsqlCommand();
            komut.Connection = baglanti;
            komut.CommandText = sql;

            NpgsqlDataReader dr = komut.ExecuteReader();

            dt.Load(dr);

            return dt;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void TabloGoster(string sql)
        {
            DataTable dt = veriGetir(sql);

            TabloKutusu.DataSource = dt;

            foreach (DataGridViewColumn column in TabloKutusu.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void comboBoxDoldur()
        {
            tabloSecComboBox.Items.Add("konumlar"); //0
            tabloSecComboBox.Items.Add("istasyonlar"); //1
            tabloSecComboBox.Items.Add("rotalar"); //2
            tabloSecComboBox.Items.Add("kisiler"); //3
            tabloSecComboBox.Items.Add("yolcular"); //4
            tabloSecComboBox.Items.Add("personel"); //5
            tabloSecComboBox.Items.Add("trenler"); //6
            tabloSecComboBox.Items.Add("trendurumu"); //7
            tabloSecComboBox.Items.Add("trenbakimi"); //8
            tabloSecComboBox.Items.Add("yolculuklar"); //9
            tabloSecComboBox.Items.Add("gecikmeler"); //10
            tabloSecComboBox.Items.Add("peronlar"); //11
            tabloSecComboBox.Items.Add("duraklar"); //12
            tabloSecComboBox.Items.Add("biletler"); //13
            tabloSecComboBox.Items.Add("kazanc"); //14
            tabloSecComboBox.Items.Add("geribildirimler"); //15
        }

        private void ekle()
        {
            bool islemUygunMu = false;

            string sorgu = "INSERT INTO " + tabloSecComboBox.SelectedItem + " (";

            if (tabloSecComboBox.SelectedIndex == 4 || tabloSecComboBox.SelectedIndex == 5
                || tabloSecComboBox.SelectedIndex == 7 || tabloSecComboBox.SelectedIndex == 14)
            {
                if (box1Edit.Enabled && !String.IsNullOrEmpty(box1Edit.Text)) sorgu += box1Label.Text + ", ";
            }
            if (box2Edit.Enabled && !String.IsNullOrEmpty(box2Edit.Text)) sorgu += box2Label.Text + ", ";
            if (box3Edit.Enabled && !String.IsNullOrEmpty(box3Edit.Text)) sorgu += box3Label.Text + ", ";
            if (box4Edit.Enabled && !String.IsNullOrEmpty(box4Edit.Text)) sorgu += box4Label.Text + ", ";
            if (box5Edit.Enabled && !String.IsNullOrEmpty(box5Edit.Text)) sorgu += box5Label.Text + ", ";
            if (checkBox1.Enabled) sorgu += checkBox1Label.Text + ", ";
            if (checkBox2.Enabled) sorgu += checkBox2Label.Text + ", ";
            sorgu = sorgu.Substring(0, sorgu.Length - 2);

            sorgu += ") VALUES (";
            if (tabloSecComboBox.SelectedIndex == 4 || tabloSecComboBox.SelectedIndex == 5
                || tabloSecComboBox.SelectedIndex == 7 || tabloSecComboBox.SelectedIndex == 14)
            {
                if (box1Edit.Enabled && !String.IsNullOrEmpty(box1Edit.Text))
                {
                    islemUygunMu = true;
                    if (int.TryParse(box1Edit.Text, out int sonuc)) sorgu += sonuc +", ";
                    else sorgu += "'" + box1Edit.Text + "', ";
                }
            }
            if (box2Edit.Enabled && !String.IsNullOrEmpty(box2Edit.Text))
            {
                islemUygunMu = true;
                if (int.TryParse(box2Edit.Text, out int sonuc)) sorgu += sonuc + ", ";
                else sorgu += "'" + box2Edit.Text + "', ";
            }
            if (box3Edit.Enabled && !String.IsNullOrEmpty(box3Edit.Text))
            {
                islemUygunMu = true;
                if (int.TryParse(box3Edit.Text, out int sonuc)) sorgu += sonuc + ", ";
                else sorgu += "'" + box3Edit.Text + "', ";
            }
            if (box4Edit.Enabled && !String.IsNullOrEmpty(box4Edit.Text))
            {
                islemUygunMu = true;
                if (int.TryParse(box4Edit.Text, out int sonuc)) sorgu += sonuc + ", ";
                else sorgu += "'" + box4Edit.Text + "', ";
            }
            if (box5Edit.Enabled && !String.IsNullOrEmpty(box5Edit.Text))
            {
                islemUygunMu = true;
                if (int.TryParse(box5Edit.Text, out int sonuc)) sorgu += sonuc + ", ";
                else sorgu += "'" + box5Edit.Text + "', ";
            }
            if (checkBox1.Enabled)
            {
                islemUygunMu = true;
                if (checkBox1.Checked) sorgu += "true, ";
                else sorgu += "false, ";
            }
            if (checkBox2.Enabled)
            {
                islemUygunMu = true;
                if (checkBox2.Checked) sorgu += "true, ";
                else sorgu += "false, ";
            }
            sorgu = sorgu.Substring(0, sorgu.Length - 2);
            sorgu += ")";

            if (islemUygunMu)
            {
                komut = new NpgsqlCommand(sorgu, baglanti);
                komut.ExecuteNonQuery();

                sorgu = "SELECT * FROM " + tabloSecComboBox.SelectedItem;
                TabloGoster(sorgu);

                MessageBox.Show("Ekleme basarili!");
            }
            else
            {
                MessageBox.Show("En az bir kutu dolu olmali");
            }
        }

        private void ara()
        {
            string sorgu = "SELECT * FROM " + tabloSecComboBox.SelectedItem + " WHERE ";
            if (box1Edit.Enabled && !String.IsNullOrEmpty(box1Edit.Text))
            {
                if (int.TryParse(box1Edit.Text, out int sonuc)) sorgu += box1Label.Text + " = " + sonuc + " AND ";
                else sorgu += box1Label.Text + " = '" + box1Edit.Text + "' AND ";
            }
            if (box2Edit.Enabled && !String.IsNullOrEmpty(box2Edit.Text))
            {
                if (int.TryParse(box2Edit.Text, out int sonuc)) sorgu += box2Label.Text + " = " + sonuc + " AND ";
                else sorgu += box2Label.Text + " = '" + box2Edit.Text + "' AND ";
            }
            if (box3Edit.Enabled && !String.IsNullOrEmpty(box3Edit.Text))
            {
                if (int.TryParse(box3Edit.Text, out int sonuc)) sorgu += box3Label.Text + " = " + sonuc + " AND ";
                else sorgu += box3Label.Text + " = '" + box3Edit.Text + "' AND ";
            }
            if (box4Edit.Enabled && !String.IsNullOrEmpty(box4Edit.Text))
            {
                if (int.TryParse(box4Edit.Text, out int sonuc)) sorgu += box4Label.Text + " = " + sonuc + " AND ";
                else sorgu += box4Label.Text + " = '" + box4Edit.Text + "' AND ";
            }
            if (box5Edit.Enabled && !String.IsNullOrEmpty(box5Edit.Text))
            {
                if (int.TryParse(box5Edit.Text, out int sonuc)) sorgu += box5Label.Text + " = " + sonuc + " AND ";
                else sorgu += box5Label.Text + " = '" + box5Edit.Text + "' AND ";
            }
            if (checkBox1.Enabled && checkBox1.Checked) sorgu += checkBox1Label.Text + " = true AND ";
            if (checkBox2.Enabled && checkBox2.Checked) sorgu += checkBox2Label.Text + " = true AND ";
            sorgu = sorgu.Substring(0, sorgu.Length - 5);

            TabloGoster(sorgu);
        }

        void sil()
        {
            bool islemUygunMu = false;

            string sorgu = "DELETE FROM " + tabloSecComboBox.SelectedItem + " WHERE ";
            if (box1Edit.Enabled && !String.IsNullOrEmpty(box1Edit.Text))
            {
                islemUygunMu = true;
                if (int.TryParse(box1Edit.Text, out int sonuc)) sorgu += box1Label.Text + " = " + sonuc + " AND ";
                else sorgu += box1Label.Text + " = '" + box1Edit.Text + "' AND ";
            }
            if (box2Edit.Enabled && !String.IsNullOrEmpty(box2Edit.Text))
            {
                islemUygunMu = true;
                if (int.TryParse(box2Edit.Text, out int sonuc)) sorgu += box2Label.Text + " = " + sonuc + " AND ";
                else sorgu += box2Label.Text + " = '" + box2Edit.Text + "' AND ";
            }
            if (box3Edit.Enabled && !String.IsNullOrEmpty(box3Edit.Text))
            {
                islemUygunMu = true;
                if (int.TryParse(box3Edit.Text, out int sonuc)) sorgu += box3Label.Text + " = " + sonuc + " AND ";
                else sorgu += box3Label.Text + " = '" + box3Edit.Text + "' AND ";
            }
            if (box4Edit.Enabled && !String.IsNullOrEmpty(box4Edit.Text))
            {
                islemUygunMu = true;
                if (int.TryParse(box4Edit.Text, out int sonuc)) sorgu += box4Label.Text + " = " + sonuc + " AND ";
                else sorgu += box4Label.Text + " = '" + box4Edit.Text + "' AND ";
            }
            if (box5Edit.Enabled && !String.IsNullOrEmpty(box5Edit.Text))
            {
                islemUygunMu = true;
                if (int.TryParse(box5Edit.Text, out int sonuc)) sorgu += box5Label.Text + " = " + sonuc + " AND ";
                else sorgu += box5Label.Text + " = '" + box5Edit.Text + "' AND ";
            }
            if (checkBox1.Enabled && checkBox1.Checked) 
            {
                islemUygunMu = true;
                sorgu += checkBox1Label.Text + " = true AND ";
            }
            if (checkBox2.Enabled && checkBox2.Checked) 
            {
                islemUygunMu = true;
                sorgu += checkBox2Label.Text + " = true AND ";
            }
            sorgu = sorgu.Substring(0, sorgu.Length - 5);

            if (islemUygunMu)
            {
                komut = new NpgsqlCommand(sorgu, baglanti);
                int etkilenenSatirlar = komut.ExecuteNonQuery();

                if (etkilenenSatirlar == 0) MessageBox.Show("Silinecek satir bulunamadi");
                else
                {
                    sorgu = "SELECT * FROM " + tabloSecComboBox.SelectedItem;
                    TabloGoster(sorgu);

                    MessageBox.Show("Silme basarili!");
                }
            }
            else
            {
                MessageBox.Show("En az bir kutu dolu olmali");
            }

        }
        private void guncelle()
        {
            int uygunMu = 0;
            string sorgu = "UPDATE " + tabloSecComboBox.SelectedItem + " SET ";
            if (box2Edit.Enabled && !String.IsNullOrEmpty(box2Edit.Text))
            {
                uygunMu = 1;
                if (int.TryParse(box2Edit.Text, out int sonuc)) sorgu += box2Label.Text + " = " + sonuc + ", ";
                else sorgu += box2Label.Text + " = '" + box2Edit.Text + "', ";
            }
            if (box3Edit.Enabled && !String.IsNullOrEmpty(box3Edit.Text))
            {
                uygunMu = 1;
                if (int.TryParse(box3Edit.Text, out int sonuc)) sorgu += box3Label.Text + " = " + sonuc + ", ";
                else sorgu += box3Label.Text + " = '" + box3Edit.Text + "', ";
            }
            if (box4Edit.Enabled && !String.IsNullOrEmpty(box4Edit.Text))
            {
                uygunMu = 1;
                if (int.TryParse(box4Edit.Text, out int sonuc)) sorgu += box4Label.Text + " = " + sonuc + ", ";
                else sorgu += box4Label.Text + " = '" + box4Edit.Text + "', ";
            }
            if (box5Edit.Enabled && !String.IsNullOrEmpty(box5Edit.Text))
            {
                uygunMu = 1;
                if (int.TryParse(box5Edit.Text, out int sonuc)) sorgu += box5Label.Text + " = " + sonuc + ", ";
                else sorgu += box5Label.Text + " = '" + box5Edit.Text + "', ";
            }
            if (checkBox1.Enabled && checkBox1.Checked)
            {
                uygunMu = 1;
                sorgu += checkBox1Label.Text + " = true, ";
            }
            if (checkBox2.Enabled && checkBox2.Checked)
            {
                uygunMu = 1;
                sorgu += checkBox2Label.Text + " = true, ";
            }

            sorgu = sorgu.Substring(0, sorgu.Length - 2);

            if (box1Edit.Enabled && !String.IsNullOrEmpty(box1Edit.Text))
            {
                uygunMu += 2;
                if (int.TryParse(box1Edit.Text, out int sonuc)) sorgu += " WHERE " + box1Label.Text + " = " + sonuc;
                else sorgu += " WHERE " + box1Label.Text + " = '" + box1Edit.Text + "'";
            }
            
            if (uygunMu == 3)
            {
                komut = new NpgsqlCommand(sorgu, baglanti);
                int etkilenenSatirlar = komut.ExecuteNonQuery();

                if (etkilenenSatirlar == 0) MessageBox.Show("Guncellenecek satir bulunamadi");
                else
                {
                    sorgu = "SELECT * FROM " + tabloSecComboBox.SelectedItem;
                    TabloGoster(sorgu);

                    MessageBox.Show("Guncelleme basarili!");
                }
            }
            if (uygunMu == 2)
            {
                MessageBox.Show("Degistirilecek herhangi bir veri girmediniz");
            }
            if (uygunMu == 1)
            {
                MessageBox.Show("Hangi satirin degistirilecegi bilgisini girmediniz");
            }
            if (uygunMu == 0)
            {
                MessageBox.Show("Hangi satirin degistirilecegi bilgisini ve degistirilecek herhangi bir veriyi girmediniz");
            }
        }
        private void kutulariDevreDisiBirak()
        {
            box1Label.Text = "---";
            box1Edit.Text = "";
            box1Edit.Enabled = false;
            box2Label.Text = "---";
            box2Edit.Text = "";
            box2Edit.Enabled = false;
            box3Label.Text = "---";
            box3Edit.Text = "";
            box3Edit.Enabled = false;
            box4Label.Text = "---";
            box4Edit.Text = "";
            box4Edit.Enabled = false;
            box5Label.Text = "---";
            box5Edit.Text = "";
            box5Edit.Enabled = false;
            checkBox1Label.Text = "---";
            checkBox1.Checked = false;
            checkBox1.Enabled = false;
            checkBox2Label.Text = "---";
            checkBox2.Checked = false;
            checkBox2.Enabled = false;

            araButton.Enabled = false;
            ekleButton.Enabled = false;
            silButton.Enabled = false;
            guncelleButton.Enabled = false;
        }

        private void tabloSecComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            kutulariDevreDisiBirak();
            araButton.Enabled = true;
            ekleButton.Enabled = true;
            silButton.Enabled = true;
            guncelleButton.Enabled = true;

            switch (tabloSecComboBox.SelectedIndex)
            {
                case 0: //konumlar
                    TabloGoster("SELECT * FROM konumlar");
                    box1Label.Text = "konumNo";
                    box1Edit.Enabled = true;
                    box2Label.Text = "ulkeAdi";
                    box2Edit.Enabled = true;
                    box3Label.Text = "sehirAdi";
                    box3Edit.Enabled = true;
                    box4Label.Text = "ilceAdi";
                    box4Edit.Enabled = true;
                    break;
                case 1: //istasyonlar
                    TabloGoster("SELECT * FROM istasyonlar");
                    box1Label.Text = "istasyonNo";
                    box1Edit.Enabled = true;
                    box2Label.Text = "konumNo";
                    box2Edit.Enabled = true;
                    box3Label.Text = "istasyonAdi";
                    box3Edit.Enabled = true;
                    break;
                case 2: //rotalar
                    TabloGoster("SELECT * FROM rotalar");
                    box1Label.Text = "rotaNo";
                    box1Edit.Enabled = true;
                    box2Label.Text = "baslangicIstasyonNo";
                    box2Edit.Enabled = true;
                    box3Label.Text = "hedefIstasyonNo";
                    box3Edit.Enabled = true;
                    break;
                case 3: //kisiler
                    TabloGoster("SELECT * FROM kisiler");
                    box1Label.Text = "kisiNo";
                    box1Edit.Enabled = true;
                    box2Label.Text = "isim";
                    box2Edit.Enabled = true;
                    box3Label.Text = "soyisim";
                    box3Edit.Enabled = true;
                    checkBox1Label.Text = "yolcuMu";
                    checkBox1.Enabled = true;
                    checkBox2Label.Text = "personelMi";
                    checkBox2.Enabled = true;
                    break;
                case 4: //yolcular
                    TabloGoster("SELECT * FROM yolcular");
                    box1Label.Text = "kisiNo";
                    box1Edit.Enabled = true;
                    box2Label.Text = "telefon";
                    box2Edit.Enabled = true;
                    box3Label.Text = "eposta";
                    box3Edit.Enabled = true;
                    box4Label.Text = "kazanilanPuan";
                    box4Edit.Enabled = true;
                    break;
                case 5: //personel
                    TabloGoster("SELECT * FROM personel");
                    box1Label.Text = "kisiNo";
                    box1Edit.Enabled = true;
                    box2Label.Text = "istasyonNo";
                    box2Edit.Enabled = true;
                    box3Label.Text = "pozisyon";
                    box3Edit.Enabled = true;
                    box4Label.Text = "maas";
                    box4Edit.Enabled = true;
                    break;
                case 6: //trenler
                    TabloGoster("SELECT * FROM trenler");
                    box1Label.Text = "trenNo";
                    box1Edit.Enabled = true;
                    box2Label.Text = "trenAdi";
                    box2Edit.Enabled = true;
                    box3Label.Text = "trenKapasitesi";
                    box3Edit.Enabled = true;
                    box4Label.Text = "trenHiziKmh";
                    box4Edit.Enabled = true;
                    box5Label.Text = "trenTuru";
                    box5Edit.Enabled = true;
                    break;
                case 7: //trendurumu
                    TabloGoster("SELECT * FROM trendurumu");
                    box1Label.Text = "trenNo";
                    box1Edit.Enabled = true;
                    box2Label.Text = "gidilenToplamMesafe";
                    box2Edit.Enabled = true;
                    box3Label.Text = "tasinanToplamYolcu";
                    box3Edit.Enabled = true;
                    box4Label.Text = "kazanilanToplamPara";
                    box4Edit.Enabled = true;
                    break;
                case 8: //trenbakimi
                    TabloGoster("SELECT * FROM trenbakimi");
                    box1Label.Text = "bakimNo";
                    box1Edit.Enabled = true;
                    box2Label.Text = "trenNo";
                    box2Edit.Enabled = true;
                    box3Label.Text = "bakimTuru";
                    box3Edit.Enabled = true;
                    box4Label.Text = "bakimTarihi";
                    box4Edit.Enabled = true;
                    break;
                case 9: //yolculuklar
                    TabloGoster("SELECT * FROM yolculuklar");
                    box1Label.Text = "yolculukNo";
                    box1Edit.Enabled = true;
                    box2Label.Text = "trenNo";
                    box2Edit.Enabled = true;
                    box3Label.Text = "rotaNo";
                    box3Edit.Enabled = true;
                    box4Label.Text = "kalkisTarihi";
                    box4Edit.Enabled = true;
                    box5Label.Text = "varisTarihi";
                    box5Edit.Enabled = true;
                    break;
                case 10: //gecikmeler
                    TabloGoster("SELECT * FROM gecikmeler");
                    box1Label.Text = "gecikmeNo";
                    box1Edit.Enabled = true;
                    box2Label.Text = "yolculukNo";
                    box2Edit.Enabled = true;
                    box3Label.Text = "gecikmeSebebi";
                    box3Edit.Enabled = true;
                    box4Label.Text = "gecikmeSuresi";
                    box4Edit.Enabled = true;
                    break;
                case 11: //peronlar
                    TabloGoster("SELECT * FROM peronlar");
                    box1Label.Text = "peronNo";
                    box1Edit.Enabled = true;
                    box2Label.Text = "istasyonNo";
                    box2Edit.Enabled = true;
                    box3Label.Text = "trenNo";
                    box3Edit.Enabled = true;
                    break;
                case 12: //duraklar
                    TabloGoster("SELECT * FROM duraklar");
                    box1Label.Text = "durakNo";
                    box1Edit.Enabled = true;
                    box2Label.Text = "istasyonNo";
                    box2Edit.Enabled = true;
                    box3Label.Text = "yolculukNo";
                    box3Edit.Enabled = true;
                    box4Label.Text = "kalkisSaati";
                    box4Edit.Enabled = true;
                    box5Label.Text = "varisSaati";
                    box5Edit.Enabled = true;
                    break;
                case 13: //biletler
                    TabloGoster("SELECT * FROM biletler");
                    box1Label.Text = "biletNo";
                    box1Edit.Enabled = true;
                    box2Label.Text = "yolcuNo";
                    box2Edit.Enabled = true;
                    box3Label.Text = "yolculukNo";
                    box3Edit.Enabled = true;
                    box4Label.Text = "koltukNo";
                    box4Edit.Enabled = true;
                    box5Label.Text = "ucret";
                    box5Edit.Enabled = true;
                    break;
                case 14: //kazanc
                    TabloGoster("SELECT * FROM kazanc");
                    box1Label.Text = "toplamKazanc";
                    box1Edit.Enabled = true;
                    break;
                case 15: //geribildirimler
                    TabloGoster("SELECT * FROM geribildirimler");
                    box1Label.Text = "geribildirimNo";
                    box1Edit.Enabled = true;
                    box2Label.Text = "biletNo";
                    box2Edit.Enabled = true;
                    box3Label.Text = "verilenPuan";
                    box3Edit.Enabled = true;
                    box4Label.Text = "yorum";
                    box4Edit.Enabled = true;
                    break;
            }
        }

        private void ekleButton_Click(object sender, EventArgs e)
        {
            ekle();
        }

        private void araButton_Click(object sender, EventArgs e)
        {
            ara();
        }

        private void silButton_Click(object sender, EventArgs e)
        {
            sil();
        }

        private void guncelleButton_Click(object sender, EventArgs e)
        {
            guncelle();
        }
    }
}