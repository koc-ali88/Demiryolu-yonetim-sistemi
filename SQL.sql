CREATE TABLE konumlar (
    konumNo SERIAL PRIMARY KEY,
    ulkeAdi VARCHAR(50) NOT NULL,
    sehirAdi VARCHAR(50) NOT NULL,
    ilceAdi VARCHAR(50) NOT NULL
);

CREATE TABLE istasyonlar (
    istasyonNo SERIAL PRIMARY KEY,
    istasyonAdi VARCHAR(50) NOT NULL,
    konumNo INTEGER NOT NULL REFERENCES konumlar(konumNo) ON DELETE CASCADE
);

CREATE TABLE rotalar (
    rotaNo SERIAL PRIMARY KEY,
    baslangicIstasyonNo INTEGER NOT NULL REFERENCES istasyonlar(istasyonNo) ON DELETE CASCADE,
    hedefIstasyonNo INTEGER NOT NULL REFERENCES istasyonlar(istasyonNo) ON DELETE CASCADE
    CONSTRAINT baslangicVeHedefAyniOlamaz UNIQUE (baslangicIstasyonNo, hedefIstasyonNo)
);

CREATE TABLE kisiler (
    kisiNo SERIAL PRIMARY KEY,
    isim VARCHAR(50) NOT NULL,
    soyisim VARCHAR(50) NOT NULL,
    yolcuMu BOOLEAN NOT NULL,
    personelMi BOOLEAN NOT NULL
);

CREATE TABLE yolcular (
    kisiNo INTEGER PRIMARY KEY REFERENCES kisiler(kisiNo) ON UPDATE CASCADE ON DELETE CASCADE,
    telefon VARCHAR(14),
    eposta VARCHAR(50),
    kazanilanPuan INTEGER DEFAULT 0
);

CREATE TABLE personel (
    kisiNo INTEGER PRIMARY KEY REFERENCES kisiler(kisiNo) ON UPDATE CASCADE ON DELETE CASCADE,
    istasyonNo INTEGER REFERENCES istasyonlar(istasyonNo) ON DELETE SET NULL,
    pozisyon VARCHAR(50),
    maas INTEGER
);

CREATE TABLE trenler (
    trenNo SERIAL PRIMARY KEY,
    trenAdi VARCHAR(50) NOT NULL,
    trenKapasitesi INTEGER,
    trenHiziKmh INTEGER,
    trenTuru VARCHAR(50)
);

CREATE TABLE trendurumu (
    trenNo INTEGER PRIMARY KEY REFERENCES trenler(trenNo) ON UPDATE CASCADE ON DELETE CASCADE,
    gidilenToplamMesafe INTEGER,
    tasinanToplamYolcu INTEGER,
    kazanilanToplamPara INTEGER
);

CREATE TABLE trenbakimi (
    bakimNo SERIAL PRIMARY KEY,
    trenNo INTEGER NOT NULL REFERENCES trenler(trenNo) ON DELETE CASCADE,
    bakimTuru VARCHAR(50),
    bakimTarihi DATE
);

CREATE TABLE yolculuklar (
    yolculukNo SERIAL PRIMARY KEY,
    trenNo INTEGER NOT NULL REFERENCES trenler(trenNo) ON DELETE CASCADE,
    rotaNo INTEGER NOT NULL REFERENCES rotalar(rotaNo) ON DELETE CASCADE,
    kalkisTarihi TIMESTAMP,
    varisTarihi TIMESTAMP
);

CREATE TABLE gecikmeler (
    gecikmeNo SERIAL PRIMARY KEY,
    yolculukNo INTEGER NOT NULL REFERENCES yolculuklar(yolculukNo) ON DELETE CASCADE,
    gecikmeSuresi INTERVAL
);

CREATE TABLE peronlar (
    peronNo SERIAL PRIMARY KEY,
    istasyonNo INTEGER NOT NULL REFERENCES istasyonlar(istasyonNo) ON DELETE CASCADE,
    trenNo INTEGER NOT NULL REFERENCES trenler(trenNo) ON DELETE CASCADE
);

CREATE TABLE duraklar (
    durakNo SERIAL PRIMARY KEY,
    istasyonNo INTEGER NOT NULL REFERENCES istasyonlar(istasyonNo) ON DELETE CASCADE,
    yolculukNo INTEGER NOT NULL REFERENCES yolculuklar(yolculukNo) ON DELETE CASCADE,
    kalkisSaati TIME,
    varisSaati TIME
);

CREATE TABLE biletler (
    biletNo SERIAL PRIMARY KEY,
    yolcuNo INTEGER NOT NULL REFERENCES yolcular(kisiNo) ON DELETE CASCADE,
    yolculukNo INTEGER NOT NULL REFERENCES yolculuklar(yolculukNo) ON DELETE CASCADE,
    koltukNo INTEGER NOT NULL,
    ucret INTEGER,
    CONSTRAINT yolculukVeKoltukAyniOlamaz UNIQUE (yolculukNo, koltukNo)
);

CREATE TABLE kazanc (
    toplamKazanc INTEGER PRIMARY KEY
);

CREATE TABLE geribildirimler (
    geribildirimNo SERIAL PRIMARY KEY,
    biletNo INTEGER REFERENCES biletler(biletNo) ON DELETE CASCADE,
    verilenPuan INTEGER,
    yorum TEXT
);

CREATE OR REPLACE FUNCTION ortalamaPuanHesapla(geciciYolculukNo INTEGER)
RETURNS NUMERIC
AS
$$
DECLARE
    geciciOrtalamaPuan NUMERIC;
BEGIN
    SELECT AVG(verilenPuan) INTO geciciOrtalamaPuan FROM geribildirimler
    WHERE biletNo IN (SELECT biletNo FROM biletler WHERE yolculukNo = geciciYolculukNo);

    RETURN COALESCE(geciciOrtalamaPuan, 0);
END;
$$
LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION ortalamaGecikmeHesapla()
RETURNS INTERVAL
AS
$$
DECLARE
    ortalamaGecikme INTERVAL;
BEGIN
    SELECT AVG(gecikmeSuresi) INTO ortalamaGecikme FROM gecikmeler;
    RETURN ortalamaGecikme;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION enKazancliTren()
RETURNS TABLE (trenNo INTEGER, trenAdi VARCHAR(50))
AS
$$
BEGIN
    RETURN QUERY EXECUTE
        'SELECT trenler.trenNo, trenler.trenAdi FROM trenler
        JOIN trendurumu on trenler.trenNo = trendurumu.trenNo
        ORDER BY trendurumu.kazanilanToplamPara DESC
        LIMIT 1';
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION bosKoltukSayisi(geciciYolculukNo INTEGER)
RETURNS INTEGER
AS
$$
DECLARE
    toplamKoltuk INTEGER;
    doluKoltuk INTEGER;
    bosKoltuk INTEGER;
BEGIN
    SELECT trenKapasitesi INTO toplamKoltuk FROM trenler
    WHERE trenNo = (SELECT trenNo FROM yolculuklar WHERE yolculukNo = geciciYolculukNo);

    SELECT COUNT(*) INTO doluKoltuk FROM biletler
    WHERE yolculukNo = geciciYolculukNo;

    bosKoltuk := toplamKoltuk - doluKoltuk;
    RETURN bosKoltuk;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE yolcuyaPuanVer(geciciYolcuNo INTEGER, verilecekPuan INTEGER)
AS
$$
BEGIN
    UPDATE yolcular
    SET kazanilanPuan = kazanilanPuan + verilecekPuan
    WHERE kisiNo = geciciYolcuNo;
END;
$$
LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION kisiUygunluk()
RETURNS TRIGGER
AS
$$
BEGIN
    IF NEW.yolcuMu = false AND NEW.personelMi = false THEN
        RAISE EXCEPTION 'Kisi yolcu ya da personel, ikisinden biri olmak zorunda';
    END IF;

    RETURN NEW;
END;
$$
LANGUAGE plpgsql;

CREATE TRIGGER kisiUygunlukTrigger
BEFORE INSERT ON kisiler
FOR EACH ROW
EXECUTE FUNCTION kisiUygunluk();

CREATE OR REPLACE FUNCTION yolcuUygunluk()
RETURNS TRIGGER
AS
$$
BEGIN
    IF (SELECT yolcuMu FROM kisiler WHERE kisiNo = NEW.kisiNo) = false THEN
        RAISE EXCEPTION 'Eklenmeye calisilan kisi yolcu degil, sadece yolcu eklenebilir';
    END IF;

    RETURN NEW;
END;
$$
LANGUAGE plpgsql;

CREATE TRIGGER yolcuUygunlukTrigger
BEFORE INSERT ON yolcular
FOR EACH ROW
EXECUTE FUNCTION yolcuUygunluk();

CREATE OR REPLACE FUNCTION personelUygunluk()
RETURNS TRIGGER
AS
$$
BEGIN
    IF (SELECT personelMi FROM kisiler WHERE kisiNo = NEW.kisiNo) = false THEN
        RAISE EXCEPTION 'Eklenmeye calisilan kisi personel degil, sadece personel eklenebilir';
    END IF;

    RETURN NEW;
END;
$$
LANGUAGE plpgsql;

CREATE TRIGGER personelUygunlukTrigger
BEFORE INSERT ON personel
FOR EACH ROW
EXECUTE FUNCTION personelUygunluk();

CREATE OR REPLACE FUNCTION biletAlinincaTrenDurumuGuncelle()
RETURNS TRIGGER
AS
$$
BEGIN
    UPDATE trendurumu
    SET tasinanToplamYolcu = tasinanToplamYolcu + 1,
        kazanilanToplamPara = kazanilanToplamPara + NEW.ucret
    WHERE trenNo = (SELECT trenNo FROM yolculuklar WHERE yolculukNo = NEW.yolculukNo);

    RETURN NEW;
END;
$$
LANGUAGE plpgsql;

CREATE TRIGGER biletAlinincaTrenDurumuGuncelleTrigger
AFTER INSERT ON biletler
FOR EACH ROW
EXECUTE FUNCTION biletAlinincaTrenDurumuGuncelle();

CREATE OR REPLACE FUNCTION biletSilinincaTrenDurumuGuncelle()
RETURNS TRIGGER
AS
$$
BEGIN
    UPDATE trendurumu
    SET tasinanToplamYolcu = tasinanToplamYolcu - 1,
        kazanilanToplamPara = kazanilanToplamPara - OLD.ucret
    WHERE trenNo = (SELECT trenNo FROM yolculuklar WHERE yolculukNo = OLD.yolculukNo);

    RETURN OLD;
END;
$$
LANGUAGE plpgsql;

CREATE TRIGGER biletSilinincaTrenDurumuGuncelleTrigger
AFTER DELETE ON biletler
FOR EACH ROW
EXECUTE FUNCTION biletSilinincaTrenDurumuGuncelle();

CREATE OR REPLACE FUNCTION yolculukGecikinceGuncelle()
RETURNS TRIGGER
AS
$$
DECLARE
    geciciGecikmeSuresi INTERVAL;
BEGIN
    SELECT gecikmeSuresi INTO geciciGecikmeSuresi FROM gecikmeler
    WHERE yolculukNo = NEW.yolculukNo;

    UPDATE yolculuklar
    SET varisTarihi = varisTarihi + geciciGecikmeSuresi
    WHERE yolculukNo = NEW.yolculukNo;

    RETURN NEW;
END;
$$
LANGUAGE plpgsql;

CREATE TRIGGER yolculukGecikinceGuncelleTrigger
AFTER INSERT ON gecikmeler
FOR EACH ROW
EXECUTE FUNCTION yolculukGecikinceGuncelle();

CREATE OR REPLACE FUNCTION kazanciHesapla()
RETURNS TRIGGER
AS
$$
BEGIN
    UPDATE kazanc
    SET toplamKazanc = (SELECT SUM(kazanilanToplamPara) FROM trendurumu);

    RETURN NEW;
END;
$$
LANGUAGE plpgsql;

CREATE TRIGGER kazanciHesaplaTrigger
AFTER INSERT OR UPDATE OR DELETE ON trendurumu
FOR EACH ROW
EXECUTE FUNCTION kazanciHesapla();

INSERT INTO kazanc (toplamKazanc) VALUES (0);

INSERT INTO konumlar (ulkeAdi, sehirAdi, ilceAdi) VALUES
    ('Türkiye', 'Kocaeli', 'İzmit'),
    ('Türkiye', 'İstanbul', 'Kadıköy'),
    ('İngiltere', 'Londra', 'Greenwich');

INSERT INTO istasyonlar (istasyonAdi, konumNo) VALUES
    ('İzmit İstasyonu', 1),
    ('Kadıköy İstasyonu', 2),
    ('Greenwich İstasyonu', 3);

INSERT INTO rotalar (baslangicIstasyonNo, hedefIstasyonNo) VALUES
    (1, 2),
    (2, 3),
    (3, 1);

INSERT INTO kisiler (isim, soyisim, yolcuMu, personelMi) VALUES
    ('Arda', 'Demir', true, false),
    ('Elif', 'Çelik', true, false),
    ('Zeynep', 'Yılmaz', false, true),
    ('Burak', 'Aksoy', false, true),
    ('Ali', 'Koç', true, true);

INSERT INTO yolcular (kisiNo, telefon, eposta, kazanilanPuan) VALUES
    (1, '05357846524', 'ardad@example.com', 10),
    (2, '05357824158', 'c_elif@example.com', 20),
    (5, '05456884258', 'koc_ali@example.com', 30);

INSERT INTO personel (kisiNo, istasyonNo, pozisyon, maas) VALUES
    (3, 3, 'Platform görevlisi', 1500),
    (4, 2, 'Bilet satış görevlisi', 2000),
    (5, 1, 'Bakım teknisyeni', 2000);

INSERT INTO trenler (trenAdi, trenKapasitesi, trenHiziKmh, trenTuru) VALUES
    ('A-treni', 400, 150, 'Yolcu treni'),
    ('B-treni', 350, 180, 'Yolcu treni'),
    ('C-treni', 125, 400, 'Maglev');

INSERT INTO trendurumu (trenNo, gidilenToplamMesafe, tasinanToplamYolcu, kazanilanToplamPara) VALUES
    (1, 824000, 6870, 1717500),
    (2, 225000, 2315, 578750),
    (3, 374000, 1892, 473000);

INSERT INTO trenbakimi (trenNo, bakimTuru, bakimTarihi) VALUES
    (1, 'Ray inceleme ve onarım', '2024-03-05'),
    (2, 'Filtre değişimi', '2024-04-08'),
    (3, 'Fren parça değişimi', '2025-04-16'),
    (1, 'Günlük bakım', '2024-04-17'),
    (1, 'Günlük bakım', '2024-04-18'),
    (1, 'Günlük bakım', '2024-04-19');

INSERT INTO yolculuklar (trenNo, rotaNo, kalkisTarihi, varisTarihi) VALUES
    (1, 1, '2024-05-01 11:00:00', '2024-05-01 14:00:00'),
    (2, 2, '2024-05-04 12:25:00', '2024-05-04 15:25:00'),
    (2, 3, '2024-05-11 12:30:00', '2024-05-11 14:30:00'),
    (3, 3, '2024-05-15 14:00:00', '2024-05-15 17:30:00');

INSERT INTO gecikmeler (yolculukNo, gecikmeSuresi) VALUES
    (1, '2 hours'),
    (2, '5 hours'),
    (3, '3 hours'),
    (4, '6 hours');

INSERT INTO peronlar (istasyonNo, trenNo) VALUES
    (1, 3),
    (2, 2),
    (3, 1);

INSERT INTO duraklar (istasyonNo, yolculukNo, kalkisSaati, varisSaati) VALUES
    (1, 2, '17:25:00', '20:45:00'),
    (2, 3, '15:30:00', '19:20:00');

INSERT INTO biletler (yolcuNo, yolculukNo, koltukNo, ucret) VALUES
    (1, 1, 15, 250),
    (1, 2, 32, 250),
    (2, 2, 43, 250),
    (5, 4, 10, 250);

INSERT INTO geribildirimler (biletNo, verilenPuan, yorum) VALUES
    (1, 3, 'Sadece 2 saat gecikti, beklediğimden iyi bir performans'),
    (2, 1, '3 saatlik yol için 5 saat beklemek zorunda kaldım'),
    (3, 5, 'Hayatımın en iyi yolculuğuydu'),
    (4, 4, 'Gayet normal');