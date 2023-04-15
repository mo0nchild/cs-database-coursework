-- Создание таблиц базы данных
CREATE TABLE IF NOT EXISTS public.City (
    CityId      SERIAL      PRIMARY KEY NOT NULL,
    CityName    VARCHAR(30) NOT NULL UNIQUE,
    Country     VARCHAR(30) NOT NULL
);

CREATE TABLE IF NOT EXISTS public.Location (
    LocationId  SERIAL      PRIMARY KEY NOT NULL,
    Street      VARCHAR(50) NOT NULL UNIQUE,
    "Index"     VARCHAR(10) DEFAULT NULL,
    CityId      INTEGER     NOT NULL,
    FOREIGN KEY(CityId) REFERENCES public.City(CityId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS public.GenderType (
    GenderTypeId    SERIAL      PRIMARY KEY NOT NULL,
    GenderTypeName  VARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS public.UserPicture (
    UserPictureId   INTEGER     NOT NULL PRIMARY KEY,
    FilePath        TEXT        NOT NULL,
    pictureName     VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS public.Contact (
    ContactId       SERIAL          PRIMARY KEY NOT NULL,
    Surname         VARCHAR(30)     NOT NULL,
    "Name"          VARCHAR(30)     NOT NULL,
    Patronymic      VARCHAR(30)     DEFAULT NULL,
    PhoneNumber     VARCHAR(12)     DEFAULT NULL,
    Birthday        TIMESTAMP       NOT NULL,
    FamilyStatus    VARCHAR(50)     DEFAULT NULL,
    EmailAddress    VARCHAR(100)    NOT NULL,
    UserPictureId   INTEGER         DEFAULT NULL,
    LocationId      INTEGER         DEFAULT NULL,
    GenderTypeId    INTEGER         NOT NULL,
    FOREIGN KEY (LocationId)    REFERENCES public.Location(LocationId) ON DELETE SET DEFAULT,
    FOREIGN KEY (GenderTypeId)  REFERENCES public.GenderType(GenderTypeId) ON DELETE CASCADE,
    FOREIGN KEY (UserPictureId) REFERENCES public.UserPicture(UserPictureId) ON DELETE SET DEFAULT
);

CREATE TABLE IF NOT EXISTS public.Authorization (
    AuthorizationId SERIAL      NOT NULL PRIMARY KEY,
    Login           VARCHAR(30) NOT NULL UNIQUE,
    Password        VARCHAR(30) NOT NULL,
    ContactId       INTEGER     NOT NULL UNIQUE,
    IsAdmin         BOOLEAN     NOT NULL,
    FOREIGN KEY (ContactId) REFERENCES public.Contact(ContactId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS public.Post (
    PostId      SERIAL      PRIMARY KEY NOT NULL,
    PostName    VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS public.Employee (
    EmployeeId  SERIAL      NOT NULL PRIMARY KEY,
    CompanyName VARCHAR(50) NOT NULL,
    Status      VARCHAR(30) DEFAULT NULL,
    PostId      INTEGER     DEFAULT NULL,
    ContactId   INTEGER     NOT NULL,
    FOREIGN KEY (PostId)    REFERENCES public.Post(PostId) ON DELETE SET DEFAULT,
    FOREIGN KEY (ContactId) REFERENCES public.Contact(ContactId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS public.DatingType (
    DatingTypeId    SERIAL      NOT NULL PRIMARY KEY,
    TypeOfDating    VARCHAR(30) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS public.Friends (
    FriendId        SERIAL  NOT NULL UNIQUE,
    StartTime       DATE    NOT NULL,
    DatingTypeId    INTEGER DEFAULT NULL,
    ContactId1      INTEGER NOT NULL,
    ContactId2      INTEGER NOT NULL,
    PRIMARY KEY (FriendId, ContactId1, ContactId2),
    FOREIGN KEY (DatingTypeId) REFERENCES public.DatingType(DatingTypeId) ON DELETE SET DEFAULT,
    FOREIGN KEY (ContactId1)   REFERENCES public.Contact(ContactId) ON DELETE CASCADE,
    FOREIGN KEY (ContactId2)   REFERENCES public.Contact(ContactId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS public.Message (
    MessageId   SERIAL      NOT NULL PRIMARY KEY,
    MessageBody TEXT        NOT NULL,
    SendTime    TIMESTAMP   NOT NULL,
    FriendId    INTEGER     NOT NULL,
    ContactId   INTEGER     NOT NULL,
    FOREIGN KEY (FriendId)  REFERENCES public.Friends(FriendId) ON DELETE CASCADE,
    FOREIGN KEY (ContactId) REFERENCES public.Contact(ContactId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS public.HumanQuality (
    HumanQualityId  SERIAL      NOT NULL PRIMARY KEY,
    QualityName     VARCHAR(30) NOT NULL UNIQUE,
    QualityType     VARCHAR(30) NOT NULL
);

CREATE TABLE IF NOT EXISTS public.Hobby (
    HobbyId     SERIAL      NOT NULL PRIMARY KEY,
    HobbyName   VARCHAR(30) NOT NULL UNIQUE ,
    HobbyType   VARCHAR(30) DEFAULT NULL
);

CREATE TABLE IF NOT EXISTS public.Contact_HumanQuality (
    ContactId       INTEGER NOT NULL,
    HumanQualityId  INTEGER NOT NULL,
    PRIMARY KEY (ContactId, HumanQualityId),
    FOREIGN KEY (ContactId)      REFERENCES public.Contact(ContactId) ON DELETE CASCADE,
    FOREIGN KEY (HumanQualityId) REFERENCES public.HumanQuality(HumanQualityId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS public.Contact_Hobby (
    ContactId   INTEGER NOT NULL,
    HobbyId     INTEGER NOT NULL,
    PRIMARY KEY (ContactId, HobbyId),
    FOREIGN KEY (ContactId) REFERENCES public.Contact(ContactId) ON DELETE CASCADE,
    FOREIGN KEY (HobbyId)   REFERENCES public.Hobby(HobbyId) ON DELETE CASCADE
);
-- Заполнение таблиц данными
INSERT INTO public.GenderType(GenderTypeName) VALUES ('Мужчина'), ('Женщина'), ('Неизвестно');

INSERT INTO public.City(CityName, Country) VALUES ('Воронеж', 'Россия'), ('Липецк', 'Россия'),
    ('Москва', 'Россия'), ('Санкт-Петербург', 'Россия'), ('Екатеринбург', 'Россия'),
    ('Минск', 'Белоруссия'), ('Брест', 'Белоруссия'), ('Алматы', 'Казахстан'), ('Уральск', 'Казахстан'),
    ('Самара', 'Россия'), ('Астана', 'Казахстан'), ('Полоцк', 'Белоруссия');

INSERT INTO public.Post(PostName) VALUES ('Менеджер'), ('Администратор'), ('Студент'),
    ('Практикант'), ('Кладовщик'), ('Уборщик'), ('Кассир'), ('Учитель'), ('Охраник'),
    ('Программист'), ('Диспетчер'), ('Механик'), ('Актер');

INSERT INTO public.DatingType(TypeOfDating) VALUES ('Друг'), ('Знакомый'), ('Коллега'), ('Семья');

INSERT INTO public.HumanQuality(QualityName, QualityType) VALUES ('Доброта', 'Положительный'),
    ('Честность', 'Положительный'), ('Верность', 'Положительный'), ('Отзывчивость', 'Положительный'),
    ('Щедрость', 'Положительный'), ('Юмор', 'Положительный'), ('Злость', 'Отрицательный'),
    ('Зависть', 'Отрицательный'), ('Высокомерие', 'Отрицательный'), ('Лицемерие', 'Отрицательный');

INSERT INTO public.Hobby(HobbyName, HobbyType) VALUES ('Чтение', 'Пассивный'), ('Туризм', 'Активный'),
    ('Охота', 'Активный'), ('Рыбалка', 'Активный'), ('Танцы', 'Активный'), ('Спорт', 'Активный'),
    ('Ролевые игры', 'Активный'), ('Собирательство', 'Пассивный'), ('Цветоводство', 'Пассивный'),
    ('Садоводство', 'Пассивный'), ('Фотографии', 'Пассивный'), ('Кулинария', 'Пассивный'), ('Пение', 'Пассивный');