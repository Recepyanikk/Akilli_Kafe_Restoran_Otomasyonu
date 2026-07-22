
CREATE TABLE Personel (
    PersonelID numeric(5, 0) IDENTITY(1,1) NOT NULL, -- Sadece bu IDENTITY olmalı
    Ad char(50) NULL,
    KullaniciAdi char(30) NULL,
    SifreHash char(50) NULL,
    Pozisyon char(20) NULL,

    -- VARLIK KISITI (Entity Constraint) - PRIMARY KEY
    CONSTRAINT PK_Personel PRIMARY KEY (PersonelID)
);



CREATE TABLE Masalar (
    MasaID numeric(5, 0) IDENTITY(1,1) NOT NULL, -- Sadece bu IDENTITY olmalı
    MasaNo numeric(5, 0) NULL,
    Durum char(10) NULL,
    HesapTutari numeric(10, 2) NULL,
    Aciklama char(100) NULL,
    MasaAdi char(50) NULL,

    -- VARLIK KISITI (Entity Constraint) - PRIMARY KEY
    CONSTRAINT PK_Masalar PRIMARY KEY (MasaID)
);


CREATE TABLE Kategoriler (
    KategoriID numeric(5, 0) IDENTITY(1,1) NOT NULL, -- Sadece bu IDENTITY olmalı
    KategoriAdi char(50) NULL,
    ResimYolu char(255) NULL,

    -- VARLIK KISITI (Entity Constraint) - PRIMARY KEY
    CONSTRAINT PK_Kategoriler PRIMARY KEY (KategoriID)
);



CREATE TABLE Urunler (
    UrunID numeric(5, 0) IDENTITY(1,1) NOT NULL, -- Sadece bu IDENTITY olmalı
    KategoriID numeric(5, 0) NULL, -- IDENTITY olmamalı (Kategoriler'den gelecek)
    UrunAdi char(50) NULL,
    Fiyat numeric(10, 2) NULL,
    ResimYolu char(255) NULL,

    -- VARLIK KISITI (Entity Constraint) - PRIMARY KEY
    CONSTRAINT PK_Urunler PRIMARY KEY (UrunID),

    -- GENEL KISIT (General Constraint) - FOREIGN KEY: Urunler -> Kategoriler
    CONSTRAINT FK_Urunler_Kategoriler FOREIGN KEY (KategoriID) 
        REFERENCES Kategoriler (KategoriID)
);


CREATE TABLE Siparis (
    SiparisID numeric(5, 0) IDENTITY(1,1) NOT NULL, -- Sadece bu IDENTITY olmalı
    MasaID numeric(5, 0) NULL, -- IDENTITY olmamalı (Masalar'dan gelecek)
    PersonelID numeric(5, 0) NULL, -- IDENTITY olmamalı (Personel'den gelecek)
    ToplamTutar numeric(10, 2) NULL,
    Tarih datetime NULL,
    Durum char(20) NOT NULL DEFAULT ('Yeni'), -- GENEL KISIT: DEFAULT değer

   
    CONSTRAINT PK_Siparis PRIMARY KEY (SiparisID),

    
    CONSTRAINT FK_Siparis_Masalar FOREIGN KEY (MasaID) 
        REFERENCES Masalar (MasaID),
    CONSTRAINT FK_Siparis_Personel FOREIGN KEY (PersonelID) 
        REFERENCES Personel (PersonelID)
);


CREATE TABLE SiparisDetay (
    DetayID numeric(5, 0) IDENTITY(1,1) NOT NULL, -- Sadece bu IDENTITY olmalı
    SiparisID numeric(5, 0) NULL, -- IDENTITY olmamalı (Siparis'ten gelecek)
    UrunID numeric(5, 0) NULL, -- IDENTITY olmamalı (Urunler'den gelecek)
    Adet numeric(5, 0) NULL,
    Fiyat numeric(10, 2) NULL,

    -- VARLIK KISITI (Entity Constraint) - PRIMARY KEY
    CONSTRAINT PK_SiparisDetay PRIMARY KEY (DetayID),

    -- GENEL KISITLAR (General Constraints) - FOREIGN KEY'ler
    CONSTRAINT FK_SiparisDetay_Siparis FOREIGN KEY (SiparisID) 
        REFERENCES Siparis (SiparisID),
    CONSTRAINT FK_SiparisDetay_Urunler FOREIGN KEY (UrunID) 
        REFERENCES Urunler (UrunID)
);

