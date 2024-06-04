create database race;

create table uuser(
    idUser AS ('u' + cast(id as varchar(10))) PERSISTED primary key,
    id int identity(1, 1),
    name varchar(100),
    user varchar(100),
    passWord varchar(150),
    admin int --0:no, 1:yes
);
insert into uuser(name, uuser, passWord, admin) 
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
    ('Etape2', 10, 3, 'rang2'),
    ('Etape3', 1, 1, 'rang3');
update etape set name = 'Etape 1 de Betsizaraina' where name = 'Etape1';
update etape set name = 'Etape 3 d’Ampasimbe' where name = 'Etape3';
-- update etape set nbCoureur = 3 where lkm = 100;
alter table etape add dhDepart datetime;
update etape set dhDepart = '01/06/2024 09:00:00' where idEtape = 'e1';
update etape set dhDepart = '01/06/2024 13:15:00' where idEtape = 'e2';
update etape set dhDepart = '02/06/2024 11:00:00' where idEtape = 'e3';
update etape set dhDepart = '02/06/2024 12:00:00' where idEtape = 'e4';
ALTER TABLE etape
ADD CONSTRAINT UC_rangEtape UNIQUE (rangEtape);

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
ALTER TABLE coureur
ADD CONSTRAINT UC_numDossard UNIQUE (numDossard);

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
insert into categoryCoureur (idCoureur, idCategory)
values
    ('c1', 'cat1'),
    ('c1', 'cat3'),
    ('c2', 'cat2'),
    ('c2', 'cat3'),
    ('c3', 'cat1'),
    ('c3', 'cat4'),
    ('c4', 'cat1'),
    ('c4', 'cat3'),
    ('c4', 'cat4'),
    ('c5', 'cat1'),
    ('c5', 'cat3');

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
alter table etapeCoureurTemps add dhDepart datetime;
alter table etapeCoureurTemps add dhArriver datetime;

create table point(
    idPoint AS ('point' + cast(id as varchar(10))) PERSISTED primary key,
    id int identity(1, 1),
    classement varchar(10),
    points int
);
insert into point (classement, points)
values
    ('1', 10),
    ('2', 6),
    ('3', 4),
    ('4', 2),
    ('5', 1);


create view v_detail_result as
WITH RankedResults AS (
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
        DENSE_RANK() OVER (PARTITION BY ec.idEtape ORDER BY ect.temps) AS classement
    FROM 
        etapeCoureurTemps ect
    JOIN etapecoureur ec ON ec.idEtapeCoureur = ect.idEtapeCoureur
    JOIN uuser u ON u.idUser = ec.idUser
    JOIN coureur c ON c.idCoureur = ec.idCoureur
),
RankedWithPoints AS (
    SELECT 
        RankedResults.*,
        ISNULL(p.points, 0) AS point
    FROM 
        RankedResults
    LEFT JOIN point p ON CAST(RankedResults.classement AS VARCHAR(10)) = p.classement
)
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
FROM 
    RankedWithPoints AS subquery;







WITH RankedResults AS (
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
        DENSE_RANK() OVER (PARTITION BY ec.idEtape ORDER BY ect.temps) AS classement
    FROM 
        etapeCoureurTemps ect
    JOIN etapecoureur ec ON ec.idEtapeCoureur = ect.idEtapeCoureur
    JOIN uuser u ON u.idUser = ec.idUser
    JOIN coureur c ON c.idCoureur = ec.idCoureur
),
RankedWithPoints AS (
    SELECT 
        RankedResults.*,
        ISNULL(p.points, 0) AS point
    FROM 
        RankedResults
    LEFT JOIN point p ON CAST(RankedResults.classement AS VARCHAR(10)) = p.classement
)
SELECT 
    subquery.*
FROM 
    RankedWithPoints AS subquery;







create view v_CG as
SELECT 
    CASE 
        WHEN DENSE_RANK() OVER (ORDER BY SUM(point) DESC) = 1 THEN '1er'
        WHEN DENSE_RANK() OVER (ORDER BY SUM(point) DESC) = 2 THEN '2eme'
        WHEN DENSE_RANK() OVER (ORDER BY SUM(point) DESC) = 3 THEN '3eme'
        WHEN DENSE_RANK() OVER (ORDER BY SUM(point) DESC) = 4 THEN '4eme'
        WHEN DENSE_RANK() OVER (ORDER BY SUM(point) DESC) = 5 THEN '5eme'
        ELSE 'Non classé'
    END AS rang,
    equipe, 
    SUM(point) AS point
FROM 
    v_detail_result 
GROUP BY 
    equipe;

--classement G par category
select * from v_detail_result v join categoryCoureur cc on cc.idCoureur = v.idCoureur







SET LANGUAGE French;





insert into uuser(name, uuser, passWord, admin) 
values
    ('Admin', 'admin', 'admin', 1),
    ('EquipeA', 'equipeA', '123', 0),
    ('EquipeB', 'equipeB', '123', 0),
    ('EquipeC', 'equipeC', '123', 0);

insert into category (name)
values
    ('homme'),
    ('femme'),
    ('junior'),
    ('senior');

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

insert into categoryCoureur (idCoureur, idCategory)
values
    ('c1', 'cat1'),
    ('c1', 'cat3'),
    ('c2', 'cat2'),
    ('c2', 'cat3'),
    ('c3', 'cat1'),
    ('c3', 'cat4'),
    ('c4', 'cat1'),
    ('c4', 'cat3'),
    ('c4', 'cat4'),
    ('c5', 'cat1'),
    ('c5', 'cat3');

insert into point (classement, points)
values
    ('1', 10),
    ('2', 6),
    ('3', 4),
    ('4', 2),
    ('5', 1);




select e.idEtapeCoureur, e.idEtape, u.name, c.nom from etapecoureur e join uuser u on u.idUser = e.iduser join coureur c on c.idCoureur = e.idCoureur order by e.id


select v.idEtapeCoureur, v.idEtape, e.name as etape, e.lkm, e.nbCoureur, e.rangEtape, e.dhDepart, v.idUser, v.equipe, v.coureur, v.numDossard, v.genre, v.dtn, v.temps, v.point, v.rang from v_detail_result v join etape e on e.idEtape = v.idEtape


create view v_chronos as
select v.idEtapeCoureur, v.idEtape, e.name as etape, e.lkm, e.nbCoureur, e.rangEtape, e.dhDepart, v.idUser, v.equipe, v.coureur, v.numDossard, v.genre, v.dtn, v.temps, v.point, v.rang from v_detail_result v join etape e on e.idEtape = v.idEtape




-- create view v_info_coureur_category as
-- select c.idUser, u.name equipe, c.idCoureur, c.nom, cc.idCategory, cate.name category, ect.temps from etapeCoureurTemps ect 
-- 	JOIN etapeCoureur et ON et.idEtapeCoureur = ect.idEtapeCoureur
-- 	JOIN categoryCoureur cc ON cc.idCoureur = et.idCoureur
-- 	JOIN coureur c ON c.idCoureur = cc.idCoureur
-- 	JOIN uuser u ON u.idUser = c.idUser
-- 	JOIN category cate ON cate.idCategory = cc.idCategory;
create view v_info_coureur_category as
select u.idUser, u.name equipe, c.idCoureur, c.nom coureur, cc.idCategory, cat.name category from coureur c 
    join categoryCoureur cc on cc.idCoureur = c.idCoureur
    join category cat on cat.idCategory = cc.idCategory
    join uuser u on u.idUser = c.idUser

create view v_info_coureur_category_temps as
select v.idUser, v.equipe, v.idCoureur, v.coureur, v.idCategory, v.category, ect.temps from v_info_coureur_category v 
	join etapeCoureur ec on ec.idCoureur = v.idCoureur
	join etapeCoureurTemps ect on ect.idEtapeCoureur = ec.idEtapeCoureur


create view v_detail_point_category as
WITH valiny AS (
	select idUser, equipe, idCoureur, coureur, idCategory, category, temps, 
		DENSE_RANK() OVER (PARTITION BY idCategory ORDER BY temps) AS classement
		from v_info_coureur_category_temps
),
miarakPoint AS (
SELECT 
        valiny.*,
        ISNULL(p.points, 0) AS point
        --p.points AS point
    FROM 
        valiny
    LEFT JOIN point p ON CAST(valiny.classement AS VARCHAR(10)) = p.classement
)
SELECT 
    tenaValiny.*
FROM 
    miarakPoint AS tenaValiny;





select idUser, equipe, idCategory, category, sum(point) point from v_detail_point_category where category = 'homme' group by idUser, equipe, idCategory, category order by point desc 



WITH CategoryTotals AS (
    SELECT 
        category,
        equipe,
        SUM(point) AS PointTotal,
        ROW_NUMBER() OVER(PARTITION BY category ORDER BY SUM(point) DESC) AS Rang
    FROM 
        v_detail_point_category
    GROUP BY 
        category, equipe
),
RankedCategoryTotals AS (
    SELECT 
        category,
        equipe,
        PointTotal,
        Rang,
        ROW_NUMBER() OVER(PARTITION BY category ORDER BY Rang) AS RangOrder
    FROM 
        CategoryTotals
)
SELECT 
    category AS Category,
    Rang AS Rang,
    equipe AS Equipe,
    PointTotal AS PointTotal
FROM 
    RankedCategoryTotals
ORDER BY 
    category, RangOrder;









