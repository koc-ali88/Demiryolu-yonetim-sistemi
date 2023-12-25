# Demiryolu Yönetim Sistemi
Veritabanı Yönetim Sistemleri proje ödevi

”Demiryolu Yönetim Sistemi” adlı bu uygulama modern bir demiryolu taşımacılığı yönetiminin verilerini düzenli bir şekilde tablolar halinde saklamak ve işlemek için tasarlanmıştır.

## İş kuralları:
• Yolcular ve personel, kişiler tablosundan kalıtım alır.

• Bir kişi hem yolcu hem personel olabilir, ama en az biri olmak zorundadır.

• Bir kişinin isim soyisim bilgileri mevcuttur.

• Bir konumun ülke adı, şehir adı ve ilçe adına sahip olması zorunludur.

• Bir istasyonun konumu ve istasyon adı bulunur

• Bir personel sadece bir istasyonda çalışabilir, bir istasyonda birden çok personel çalışabilir.

• Bir istasyon sadece bir konumda bulunabilir, bir konumda birden çok istasyon bulunabilir.

• Bir rotanın başlangıç istasyonu ve hedef istasyonu mevcuttur.

• Bir trenin adı, kapasitesi, hızı ve türü mevcuttur.

• Belirli bir tren için bakım yapıldığında bakım türü ve tarihi kayıt edilir.

• Bir tren için birçok kez bakım yapılabilir, bir bakım sadece bir trene aittir

• Herhangi bir tren için durum tablosu oluşturulur; gidilen toplam mesafe, taşınan toplam yolcu, kazanılan toplam para kayıt edilir.

• Bir trenin sadece bir durum tablosu olabilir, bu tablo sadece bir trene ait olabilir.

• Yolculuklar; trenin numarası, takip edilecek rota ve kalkış tarihi ile kayıt edilir.

• Bir tren birden çok yolculuk yapabilir, bir yolculuk sadece o trene aittir.

• Herhangi bir yolculuğun gecikmesi durumunda yolculuk numarası ile beraber gecikme süresi ve gecikme sebebi kayıt edilir.

• Bir yolculuk birden fazla kez gecikebilir, ama bir gecikme sadece bir yolculuğa ait olabilir.

• Peronlar belirli bir istasyonda bulunur, istasyonNo ve trenNo kayıt edilir.

• Bir tren sadece bir peronda bulunabilir, bir peronda sadece bir tren bulunabilir.

• Bir istasyonda birden çok peron bulunabilir, bir peron sadece bir istasyonda bulunur.

• Duraklar için istasyonNo, yolculukNo, kalkış saati ve varış saati bilgileri mevcuttur.

• Bir durak sadece bir istasyonda bulunabilir, bir istasyonda birden çok durak bulunabilir.

• Bir yolculukta birden çok kez durak olabilir, ama bir durak sadece bir yolculukta gerçekleşir.

• Biletler için yolcu numarası, yolculuk numarası, kolktuk numarası ve ücret bilgileri mevcuttur.

• Bir yolcu birden çok bilet alabilir, ama bir bilet sadece bir yolcuya aittir.

• Bir yolculuk için birden çok bilet bulunabilir, ama bir bilet sadece bir yolculuğa aittir.

• Biletlerin satışından elde edilen kazanç, kazanç tablosunda saklanır.

• Yolcu tarafından herhangi bir bilet için geri bildirim verilebilir; bilet numarası, verilen puan ve yorum kayıt edilir.

• Belli bir yolcuya ait bilet için sadece bir yorum yapılabilir, bir yorum sadece belli bir bilete ait olabilir.

## İlişkisel şema:
konumlar(konumNo: serial, ulkeAdi: varchar(50), sehirAdi: varchar(50), ilceAdi: varchar(50))

istasyonlar(istasyonNo: serial, istasyonAdi: varchar(50), konumNo: integer)

rotalar(rotaNo: serial, baslangicIstasyonNo: integer, hedefIstasyonNo: integer)

kisiler(kisiNo: serial, isim: varchar(50), soyisim: varchar(50), yolcuMu: bool, personelMi: bool)

yolcular(kisiNo: integer, telefon: varchar(14), eposta: varchar(50), kazanilanPuan: integer)

personel(kisiNo: integer, istasyonNo: integer, pozisyon: varchar(50), maas: integer)

trenler(trenNo: serial, trenAdi: varchar(50), trenKapasitesi: integer, trenHiziKmh: integer, trenTuru: varchar(50))

trendurumu(trenNo: integer, gidilenToplamMesafe: integer, tasinanToplamYolcu: integer, kazanilanToplamPara: integer)

trenbakimi(bakimNo: serial, trenNo: integer, bakimTuru: varchar(50), bakimTarihi: date)

yolculuklar(yolculukNo: serial, trenNo: integer, rotaNo: integer, kalkisTarihi: timestamp, varisTarihi: timestamp)

gecikmeler(gecikmeNo: serial, yolculukNo: integer, gecikmeSuresi: interval)

peronlar(peronNo: serial, istasyonNo: integer, trenNo: integer)

kazanc(toplamKazanc: integer)

duraklar(durakNo: serial, istasyonNo: integer, yolculukNo: integer, kalkisSaati:time, varisSaati:time)

biletler(biletNo: serial, yolcuNo: integer, yolculukNo: integer, koltukNo: integer, ucret: integer)

geribildirimler(geribildirimNo: serial, biletNo: integer, verilenPuan: integer, yorum: text)

## Varlık Bağıntı Modeli:

![VarlikBaginti](https://github.com/koc-ali88/Demiryolu-yonetim-sistemi/assets/84532261/10c1e3b7-ba86-4533-8259-133b4c927fc3)


> [!NOTE]
> SQL ifadeleri SQL.sql dosyasında bulunuyor

## Saklı Yordam – Fonksiyonlar:
• bosKoltukSayisi(geciciYolculukNo integer): Parametre olarak girilen yolculuğa ait trenin kapasitesinden dolu olan koltuk miktarını çıkararak trende kaç koltuğun boş olduğunu gösteriyor.

• enKazancliTren(): Trendurumu tablosu üzerinden en çok para kazanan treni döndürüyor.

• ortalamaGecikmeHesapla(): Gecikmeler tablosundaki gecikme sürelerinin ortalamasını döndürüyor.

• ortalamaPuanHesapla(geciciYolculukNo integer): Parametre olarak girilen yolculuk için yolcuların verdiği puanların ortalamasını döndürüyor.

• yolcuyaPuanVer(IN geciciYolcuNo integer, IN verilecekPuan integer): Parametre olarak girilen yolcunun hesabına parametre üzerinden verilen miktar kadar puan ekliyor.

## Tetikleyiciler:
• biletAlinincaTrendurumuGuncelle(): Biletler tablosuna yeni bilet eklendiğinde, ait olduğu yolculuğu yapan trenin trendurumu tablosu güncelleniyor (taşıdığı yolcu sayısı ve kazandığı para artıyor).

• biletSilininceTrendurumuGuncelle(): Biletler tablosundan bir bilet silinirse silinen biletin trendurumu tablosuna yaptığı etkisi geri çekiliyor.

• kazanciHesapla(): Trendurumu tablosunda meydana gelen herhangi bir değişiklikten sonra trenlerin toplam kazanç miktarı kazanc tablosuna kaydediliyor.

• kisiUygunluk(): Kisiler tablosuna yeni bir kişi eklenmeye çalışıldığında bu eklemenin uygunluğu kontrol ediliyor (yolcuMu ve personelMi değerleri aynı anda false olamaz).

• personelUygunluk():Personel tablosuna yeni bir kişi eklenmeye çalışıldığında bu eklemenin uygunluğu kontrol ediliyor (personelMi değeri true olmak zorunda).

• yolcuUygunluk(): Yolcular tablosuna yeni bir kişi eklenmeye çalışıldığında bu eklemenin uygunluğu kontrol ediliyor (yolcuMu değer true olmak zorunda).

• yolculukGecikinceGuncelle(): Eğer belli bir yolculuk için gecikme yaşanırsa, yolculuk tablosundaki varisTarihi değeri gecikme süresi kadar erteleniyor.

## Ekran Görüntüleri:

![arama](https://github.com/koc-ali88/Demiryolu-yonetim-sistemi/assets/84532261/a14c9534-66ac-492d-bc31-eca32557ff97)
*Arama işlemi*

![ekleme](https://github.com/koc-ali88/Demiryolu-yonetim-sistemi/assets/84532261/888b7bb4-354c-4b13-992a-3bb1d123f92a)
*Ekleme işlemi*

![silme](https://github.com/koc-ali88/Demiryolu-yonetim-sistemi/assets/84532261/de6cc230-8876-4581-bf7f-77974e449493)
*Silme işlemi*

![guncelleme](https://github.com/koc-ali88/Demiryolu-yonetim-sistemi/assets/84532261/26b246fd-efee-460f-be3e-0db0c61dec65)
*Güncelleme işlemi*
