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

create table penalite(
    idPenalite AS ('pena' + cast(id as varchar(10))) PERSISTED primary key,
    id int identity(1, 1),
    idEtape varchar(11) references etape(idEtape),
    etape varchar(50),
    idUser varchar(11) references uuser(idUser),
    equipe varchar(50),
    tempsPlenalite time
);
alter table penalite add idEtapeCoureur varchar(12) references etapeCoureur(idEtapeCoureur);
ALTER TABLE penalite DROP CONSTRAINT FK__penalite__idEtap__2180FB33;
ALTER TABLE penalite DROP COLUMN idEtapeCoureur;


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




create view v_info_coureur_category as
select c.idUser, u.name equipe, c.idCoureur, c.nom, cc.idCategory, cate.name category, ect.temps from etapeCoureurTemps ect 
	JOIN etapeCoureur et ON et.idEtapeCoureur = ect.idEtapeCoureur
	JOIN categoryCoureur cc ON cc.idCoureur = et.idCoureur
	JOIN coureur c ON c.idCoureur = cc.idCoureur
	JOIN uuser u ON u.idUser = c.idUser
	JOIN category cate ON cate.idCategory = cc.idCategory;

create view v_info_coureur_category as
select u.idUser, u.name equipe, c.idCoureur, c.nom coureur, cc.idCategory, cat.name category from coureur c 
    join categoryCoureur cc on cc.idCoureur = c.idCoureur
    join category cat on cat.idCategory = cc.idCategory
    join uuser u on u.idUser = c.idUser

create view v_info_coureur_category_temps as
select v.idUser, v.equipe, v.idCoureur, v.coureur, v.idCategory, v.category, ect.temps from v_info_coureur_category v 
	join etapeCoureur ec on ec.idCoureur = v.idCoureur
	join etapeCoureurTemps ect on ect.idEtapeCoureur = ec.idEtapeCoureur





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




create view v_CG_category_notSure as
WITH valiny AS (
    SELECT 
        idUser, 
        equipe, 
        idCoureur, 
        coureur, 
        idCategory, 
        category, 
        temps, 
        DENSE_RANK() OVER (PARTITION BY idCategory ORDER BY temps) AS classement
    FROM 
        v_info_coureur_category_temps
),
miarakPoint AS (
    SELECT 
        valiny.*,
        ISNULL(p.points, 0) AS point
    FROM 
        valiny
    LEFT JOIN point p ON CAST(valiny.classement AS VARCHAR(10)) = p.classement
)
SELECT 
    idCategory,
    CASE category 
        WHEN 'homme' THEN 'Homme'
        WHEN 'junior' THEN 'Junior'
        WHEN 'femme' THEN 'Femme'
        WHEN 'senior' THEN 'Senior'
        ELSE category 
    END AS Category,
    DENSE_RANK() OVER (PARTITION BY category ORDER BY SUM(point) DESC) AS Rang,
    equipe AS Equipe,
    SUM(point) AS PointTotal
FROM 
    miarakPoint
GROUP BY 
    category, equipe, idCategory


ORDER BY 
    category, PointTotal DESC;













WITH RankedResults AS (
    SELECT 
        ec.idEtape,
        u.idUser,
        u.name as equipe,
        c.idCoureur,
        c.nom as coureur,
		cc.idCategory,
		cate.name category,
        ect.temps,
        DENSE_RANK() OVER (PARTITION BY ec.idEtape, cc.idCategory ORDER BY ect.temps) AS classement
    FROM 
        etapeCoureurTemps ect
    JOIN etapecoureur ec ON ec.idEtapeCoureur = ect.idEtapeCoureur
    JOIN uuser u ON u.idUser = ec.idUser
    JOIN coureur c ON c.idCoureur = ec.idCoureur
	JOIN categoryCoureur cc ON cc.idCoureur = c.idCoureur
	JOIN category cate ON cate.idCategory = cc.idCategory
	group by 
		ec.idEtape,
        u.idUser,
        u.name,
        c.idCoureur,
        c.nom,
		cc.idCategory,
		cate.name,
        ect.temps
)
SELECT 
    subquery.*
FROM 
    RankedResults AS subquery
	order by idCategory;






----------------resultat part category
create view v_detail_point_category as
WITH RankedResults AS (
    SELECT 
        ec.idEtape,
        u.idUser,
        u.name as equipe,
        c.idCoureur,
        c.nom as coureur,
		cc.idCategory,
		cate.name category,
        ect.temps,
        DENSE_RANK() OVER (PARTITION BY ec.idEtape, cc.idCategory ORDER BY ect.temps) AS classement
    FROM 
        etapeCoureurTemps ect
    JOIN etapecoureur ec ON ec.idEtapeCoureur = ect.idEtapeCoureur
    JOIN uuser u ON u.idUser = ec.idUser
    JOIN coureur c ON c.idCoureur = ec.idCoureur
	JOIN categoryCoureur cc ON cc.idCoureur = c.idCoureur
	JOIN category cate ON cate.idCategory = cc.idCategory
	group by 
		ec.idEtape,
        u.idUser,
        u.name,
        c.idCoureur,
        c.nom,
		cc.idCategory,
		cate.name,
        ect.temps
)
SELECT 
    subquery.*
FROM 
    RankedResults AS subquery;







create view v_CG_category_sure as
with result as (
    SELECT 
        v.*,
        ISNULL(p.points, 0) AS point
    FROM 
        v_detail_point_category v
    LEFT JOIN point p ON CAST(v.classement AS VARCHAR(10)) = p.classement
)
select idUser, equipe, idCategory, category, sum(point) pointtotal from result group by idUser, equipe, idCategory, category

order by category, pointtotal desc



create view v_result_category as
SELECT 
    *,
    DENSE_RANK() OVER (PARTITION BY category ORDER BY pointtotal DESC) AS classement
FROM 
    v_CG_category_sure 


ORDER BY category, pointtotal DESC;





















select * from uuser 

select * from etapeCoureurTemps e join etapeCoureur ec on ec.idEtapeCoureur = e.idEtapeCoureur join coureur c on c.idCoureur = ec.idCoureur where c.idUser='u3' and ec.idEtape = 'e2'

select * from coureur where idUser = 'u3'















WITH RankedResults AS (
    SELECT 
        ect.idEtapeCoureur,
        ec.idEtape,
        u.idUser,
        u.name AS equipe,
        c.idCoureur,
        c.nom AS coureur,
        c.numDossard,
        c.genre,
        c.dtn,
        ect.hDepart,
        ect.hArriver,
        ect.temps,
        SUM(DATEDIFF(SECOND, '00:00:00', ect.temps)) OVER (PARTITION BY ec.idEtape, u.idUser, u.name, c.idCoureur, c.nom, c.numDossard, c.genre, c.dtn) AS sum_seconds -- Convert time to seconds and then sum
        --DENSE_RANK() OVER (PARTITION BY ec.idEtape ORDER BY SUM(DATEDIFF(SECOND, '00:00:00', ect.temps))) AS classement -- New classement based on sum of temps
    FROM 
        etapeCoureurTemps ect
    JOIN etapecoureur ec ON ec.idEtapeCoureur = ect.idEtapeCoureur
    JOIN uuser u ON u.idUser = ec.idUser
    JOIN coureur c ON c.idCoureur = ec.idCoureur
),
RankedWithSum AS (
    SELECT 
        idEtape,
        idUser,
        equipe,
        idCoureur,
        coureur,
        numDossard,
        genre,
        dtn,
        --SUM(temps) AS total_time_seconds, -- Sum of temps per idEtape
        DENSE_RANK() OVER (PARTITION BY idEtape, idUser, equipe, idCoureur, coureur, numDossard, genre, dtn ORDER BY SUM(temps)) AS classement -- Ranking based on sum of temps
    FROM 
        RankedResults
    -- GROUP BY 
    --     idEtape,
    --     idUser,
    --     name,
    --     idCoureur,
    --     nom,
    --     numDossard,
    --     genre,
    --     dtn
)
SELECT 
    ec.idEtape,
    RankedResults.idEtapeCoureur,
    u.idUser,
    u.name AS equipe,
    c.idCoureur,
    c.nom AS coureur,
    c.numDossard,
    c.genre,
    c.dtn,
    ect.hDepart,
    ect.hArriver,
    ect.temps,
    sum_seconds,
    classement,
    --RankedWithSum.total_time_seconds,
    CASE 
        WHEN RankedWithSum.classement = 1 THEN 10
        WHEN RankedWithSum.classement = 2 THEN 6
        WHEN RankedWithSum.classement = 3 THEN 4
        WHEN RankedWithSum.classement = 4 THEN 2
        WHEN RankedWithSum.classement = 5 THEN 1
        ELSE 0
    END AS point
FROM 
    RankedResults;









create view v_detail_result as
WITH RankedResults AS (
    SELECT 
        ec.idEtape,
        c.idCoureur,
        c.nom AS coureur,
		CONVERT(TIME, DATEADD(SECOND, SUM(DATEDIFF(SECOND, '00:00:00', ect.temps)), '00:00:00')) AS temps,
        DENSE_RANK() OVER (PARTITION BY ec.idEtape ORDER BY SUM(DATEDIFF(SECOND, '00:00:00', ect.temps))) AS classement
    FROM 
        etapeCoureurTemps ect
    JOIN etapecoureur ec ON ec.idEtapeCoureur = ect.idEtapeCoureur
    JOIN uuser u ON u.idUser = ec.idUser
    JOIN coureur c ON c.idCoureur = ec.idCoureur
    GROUP BY ec.idEtape, c.idCoureur, c.nom
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



create view v_CG as
SELECT 
    CASE 
        WHEN DENSE_RANK() OVER (ORDER BY SUM(v.point) DESC) = 1 THEN '1er'
        WHEN DENSE_RANK() OVER (ORDER BY SUM(v.point) DESC) = 2 THEN '2eme'
        WHEN DENSE_RANK() OVER (ORDER BY SUM(v.point) DESC) = 3 THEN '3eme'
        WHEN DENSE_RANK() OVER (ORDER BY SUM(v.point) DESC) = 4 THEN '4eme'
        WHEN DENSE_RANK() OVER (ORDER BY SUM(v.point) DESC) = 5 THEN '5eme'
        ELSE 'Non classé'
    END AS rang,
    c.idUser,
    u.name,
    SUM(v.point) AS point
FROM 
    v_detail_result v
	join coureur c on c.idCoureur = v.idCoureur
    join uuser u on c.idUser = c.idUser
GROUP BY 
    c.idUser, u.name;





create view v_CGPointEtape as
select v.idEtape, e.name, c.idCoureur, c.nom, sum(v.point) point 
	from v_detail_result v 
	join etape e on v.idEtape = e.idEtape 
	join coureur c on c.idCoureur = v.idCoureur 
	group by v.idEtape, e.name, c.idCoureur, c.nom 
	order by v.idEtape asc, sum(v.point) desc;



create view v_chronos as
select v.idEtape etapeID, v.idEtape, e.name as etape, e.lkm, e.nbCoureur, e.rangEtape, e.dhDepart, c.idUser, u.name equipe,  v.coureur, c.numDossard, c.genre, c.dtn, v.temps, v.point, v.rang 
	from v_detail_result v 
	join etape e on e.idEtape = v.idEtape
	join coureur c on c.idCoureur = v.idCoureur
	join uuser u on u.idUser = c.idUser











---------------------category fa sum temps

create view v_detail_point_category as
WITH RankedResults AS (
    SELECT 
        ec.idEtape,
        u.idUser,
        u.name as equipe,
        c.idCoureur,
        c.nom as coureur,
		cc.idCategory,
		cate.name category,
        -- ect.temps,
        -- DENSE_RANK() OVER (PARTITION BY ec.idEtape, cc.idCategory ORDER BY ect.temps) AS classement
		CONVERT(TIME, DATEADD(SECOND, SUM(DATEDIFF(SECOND, '00:00:00', ect.temps)), '00:00:00')) AS temps,
        DENSE_RANK() OVER (PARTITION BY ec.idEtape, cc.idCategory ORDER BY SUM(DATEDIFF(SECOND, '00:00:00', ect.temps))) AS classement
    FROM 
        etapeCoureurTemps ect
    JOIN etapecoureur ec ON ec.idEtapeCoureur = ect.idEtapeCoureur
    JOIN uuser u ON u.idUser = ec.idUser
    JOIN coureur c ON c.idCoureur = ec.idCoureur
	JOIN categoryCoureur cc ON cc.idCoureur = c.idCoureur
	JOIN category cate ON cate.idCategory = cc.idCategory
	group by 
		ec.idEtape,
        u.idUser,
        u.name,
        c.idCoureur,
        c.nom,
		cc.idCategory,
		cate.name,
        ect.temps
)
SELECT 
    subquery.*
FROM 
    RankedResults AS subquery;













create view v_CG_category_sure as
with result as (
    SELECT 
        v.*,
        ISNULL(p.points, 0) AS point
    FROM 
        v_detail_point_category v
    LEFT JOIN point p ON CAST(v.classement AS VARCHAR(10)) = p.classement
)
select idUser, equipe, idCategory, category, sum(point) pointtotal from result group by idUser, equipe, idCategory, category

order by category, pointtotal desc



create view v_result_category as
SELECT 
    *,
    DENSE_RANK() OVER (PARTITION BY category ORDER BY pointtotal DESC) AS classement
FROM 
    v_CG_category_sure 


ORDER BY category, pointtotal DESC;















create view v_aleasJ4 as
WITH RankedResults AS (
    SELECT 
        ec.idEtape,
        c.idCoureur,
        c.nom AS coureur,
        c.genre,
        CONVERT(TIME, DATEADD(SECOND, SUM(DATEDIFF(SECOND, '00:00:00', ect.temps)), '00:00:00')) AS chronos,
        pen.somTempsPen AS penalite,
        DENSE_RANK() OVER (PARTITION BY ec.idEtape ORDER BY SUM(DATEDIFF(SECOND, '00:00:00', ect.temps))) AS classement
    FROM 
        etapeCoureurTemps ect
    JOIN etapecoureur ec ON ec.idEtapeCoureur = ect.idEtapeCoureur
    JOIN uuser u ON u.idUser = ec.idUser
    JOIN coureur c ON c.idCoureur = ec.idCoureur
    LEFT JOIN (
        SELECT idUser, idEtape, SUM(DATEDIFF(SECOND, '00:00:00', tempsPlenalite)) AS somTempsPen
        FROM penalite
        GROUP BY idUser, idEtape
    ) pen ON pen.idUser = ec.idUser AND ec.idEtape = pen.idEtape
    where ect.dhArriver is not null and ect.dhDepart is not null
    GROUP BY ec.idEtape, c.idCoureur, c.nom, c.genre, pen.somTempsPen
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
    END AS rang,
    CONVERT(TIME, DATEADD(SECOND, DATEDIFF(SECOND, '00:00:00', chronos) - penalite, '00:00:00')) AS temps_apres_penalite,
    CONVERT(TIME, DATEADD(SECOND, DATEDIFF(SECOND, '00:00:00', penalite) + DATEDIFF(SECOND, '00:00:00', chronos), '00:00:00')) AS temps_final
FROM 
    RankedWithPoints AS subquery;

















-------------------------------------------------------------------------
select * from etapeCoureurTemps e join etapeCoureur ec on ec.idEtapeCoureur = e.idEtapeCoureur join coureur c on c.idCoureur = ec.idCoureur