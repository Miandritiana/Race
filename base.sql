create database race;

create table uuser(
    idUser AS ('u' + cast(id as varchar(10))) PERSISTED primary key,
    id int identity(1, 1),
    name varchar(100),
    user varchar(100),
    passWord varchar(150),
    admin int --0:no, 1:yes
);
insert into uuser(name, user, passWord, admin) 
values
    ('Admin', 'admin', 'admin', 1),
    ('EquipeA', 'equipeA', '123', 0),
    ('EquipeB', 'equipeB', '123', 0),
    ('EquipeC', 'equipeC', '123', 0);

create table etape(
    idEtape AS ('e' + cast(id as varchar(10))) PERSISTED primary key,
    id int identity(1, 1),
    name varchar(100),
    lkm float not null,
    nbCoureur int not null default 1,
    rangEtape varchar(50) not null
);
insert into etape (name, lkm, nbCoureur, rangEtape)
values
    ('Etape1', 50, 2, 'rang1'),
    ('Etape2', 100, 3, 'rang2'),
    ('Etape3', 1, 1, 'rang3');
update etape set name = 'Etape 1 de Betsizaraina' where name = 'Etape1';
update etape set name = 'Etape 3 d’Ampasimbe' where name = 'Etape3';
-- update etape set nbCoureur = 3 where lkm = 100;

create table category(
    idCategory AS ('cat' + cast(id as varchar(10))) PERSISTED primary key,
    id int identity(1, 1),
    name varchar(100) not null
);
insert into category (name)
values
    ('homme'),
    ('femme'),
    ('junior'),
    ('senior');

create table coureur(
    idCoureur AS ('c' + cast(id as varchar(10))) PERSISTED primary key,
    id int identity(1, 1),
    nom varchar(100) not null,
    numDossard varchar(100) not null,
    genre varchar(30) not null,
    dtn date not null,
    idUser varchar(11) references uuser(idUser) default null
);
insert coureur (nom, numDossard, genre, dtn, idUser)
values
    ('Lova', '1', 'masculin', '2000-01-01', 'u2'),
    ('Sabrina', '5', 'feminin', '2001-01-01', 'u2'),
    ('Victor', '11', 'masculin', '1999-01-01', 'u2'),
    --
    ('Justin', '2', 'masculin', '2000-01-01', 'u3'),
    ('Vero', '6', 'feminin', '2001-01-01', 'u3'),
    --
    ('John', '3', 'masculin', '2000-01-01', 'u4'),
    ('Jill', '7', 'feminin', '2001-01-01', 'u4');

create view v_coureur as
select c.idCoureur, c.nom, c.numDossard, c.genre, c.dtn, u.name, c.idUser from coureur c join uuser u on u.idUser = c.idUser

create table categoryCoureur(
    idCategoryCoureur AS ('catC' + cast(id as varchar(10))) PERSISTED primary key,
    id int identity(1, 1),
    idCoureur varchar(11) references coureur(idCoureur),
    idCategory varchar(13) references category(idCategory)
);
-- insert into categoryCoureur (idCoureur, idCategory)
-- values
--     ('c1', 'cat1'),
--     ('c1', 'cat3'),
--     ('c2', 'cat2'),
--     ('c2', 'cat3'),
--     ('c3', 'cat1'),
--     ('c3', 'cat4'),
--     ('c4', 'cat1'),
--     ('c4', 'cat2'),
--     ('c4', 'cat3'),
--     ('c4', 'cat4'),
--     ('c5', 'cat1'),
--     ('c5', 'cat2');

create table etapeCoureur(
    idEtapeCoureur AS ('eC' + cast(id as varchar(10))) PERSISTED primary key,
    id int identity(1, 1),
    idEtape varchar(11) references etape(idEtape),
    idUser varchar(11) references uuser(idUser),
    idCoureur varchar(11) references coureur(idCoureur)
);

create view v_infoEtapeCoureur as
select ec.idEtapeCoureur, ec.idEtape, ec.idUser, u.name equipe, ec.idCoureur, c.nom coureur, c.numDossard, c.genre, c.dtn 
from etapecoureur ec 
    join uuser u on u.idUser = ec.idUser 
    join coureur c on c.idCoureur = ec.idCoureur

create table etapeCoureurTemps(
    idECTemps AS ('eCTemps' + cast(id as varchar(10))) PERSISTED primary key,
    id int identity(1, 1),
    idEtapeCoureur varchar(12) references etapeCoureur(idEtapeCoureur) not null,
    hDepart time not null,
    hArriver time not null,
    temps time
);

create view v_detail_result as
SELECT 
    subquery.*,
    CASE 
        WHEN point = 10 THEN '1er'
        WHEN point = 6 THEN '2eme'
        WHEN point = 4 THEN '3eme'
        WHEN point = 2 THEN '4eme'
        WHEN point = 1 THEN '5eme'
        ELSE 'non classe'
    END AS rang
FROM (
    SELECT 
        ect.idECTemps,
        ect.idEtapeCoureur,
        ec.idEtape,
        u.idUser,
        u.name as equipe,
        c.idCoureur,
        c.nom as coureur,
        c.numDossard,
        c.genre,
        c.dtn,
        ect.hDepart,
        ect.hArriver,
        ect.temps,
        CASE 
            WHEN ROW_NUMBER() OVER (PARTITION BY ec.idEtape ORDER BY ect.temps) = 1 THEN 10
            WHEN ROW_NUMBER() OVER (PARTITION BY ec.idEtape ORDER BY ect.temps) = 2 THEN 6
            WHEN ROW_NUMBER() OVER (PARTITION BY ec.idEtape ORDER BY ect.temps) = 3 THEN 4
            WHEN ROW_NUMBER() OVER (PARTITION BY ec.idEtape ORDER BY ect.temps) = 4 THEN 2
            WHEN ROW_NUMBER() OVER (PARTITION BY ec.idEtape ORDER BY ect.temps) = 5 THEN 1
            ELSE 0
        END AS point
    FROM 
        etapeCoureurTemps ect
    JOIN etapecoureur ec ON ec.idEtapeCoureur = ect.idEtapeCoureur
	JOIN uuser u ON u.idUser = ec.idUser
	JOIN coureur c ON c.idCoureur = ec.idCoureur
) AS subquery;


create view v_CG as
SELECT 
    CASE 
        WHEN RANK() OVER (ORDER BY SUM(point) DESC) = 1 THEN '1er'
        WHEN RANK() OVER (ORDER BY SUM(point) DESC) = 2 THEN '2eme'
        WHEN RANK() OVER (ORDER BY SUM(point) DESC) = 3 THEN '3eme'
        WHEN RANK() OVER (ORDER BY SUM(point) DESC) = 4 THEN '4eme'
        WHEN RANK() OVER (ORDER BY SUM(point) DESC) = 5 THEN '5eme'
        ELSE 'Non classé'
    END AS rang,
    equipe, 
    SUM(point) AS point
FROM 
    v_detail_result 
GROUP BY 
    equipe;



















