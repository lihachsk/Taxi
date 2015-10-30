-- Table: path.addrobj

-- DROP TABLE path.addrobj;
DROP TABLE path.addrobj;
CREATE TABLE path.addrobj
(
  actstatus smallint,
  aoguid character varying(36) NOT NULL,
  aoid character varying(36),
  aolevel smallint,
  areacode character varying(3),
  autocode character varying(1),
  centstatus smallint,
  citycode character varying(3),
  code character varying(17),
  currstatus smallint,
  enddate date,
  formalname character varying(120),
  ifnsfl character varying(4),
  ifnsul character varying(4),
  nextid character varying(36),
  offname character varying(120),
  okato character varying(11),
  oktmo character varying(11),
  operstatus smallint,
  parentguid character varying(36),
  placecode character varying(3),
  plaincode character varying(15),
  postalcode character varying(6),
  previd character varying(36),
  regioncode character varying(2),
  shortname character varying(10),
  startdate date,
  streetcode character varying(4),
  terrifnsfl character varying(4),
  terrifnsul character varying(4),
  updatedate date,
  ctarcode character varying(3),
  extrcode character varying(4),
  sextcode character varying(3),
  livestatus smallint,
  normdoc character varying(36),
  worksarea boolean NOT NULL DEFAULT false,
  CONSTRAINT addrobj_pkey PRIMARY KEY (aoguid)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE path.addrobj
  OWNER TO postgres;
INSERT INTO path.addrobj(
            actstatus, aoguid, aoid, aolevel, areacode, autocode, centstatus, 
            citycode, code, currstatus, enddate, formalname, ifnsfl, ifnsul, 
            nextid, offname, okato, oktmo, operstatus, parentguid, placecode, 
            plaincode, postalcode, previd, regioncode, shortname, startdate, 
            streetcode, terrifnsfl, terrifnsul, updatedate, ctarcode, extrcode, 
            sextcode, livestatus, normdoc, worksarea)
SELECT actstatus, aoguid, aoid, aolevel, areacode, autocode, centstatus, 
       citycode, code, currstatus, enddate, formalname, ifnsfl, ifnsul, 
       nextid, offname, okato, oktmo, operstatus, parentguid, placecode, 
       plaincode, postalcode, previd, regioncode, shortname, startdate, 
       streetcode, terrifnsfl, terrifnsul, updatedate, ctarcode, extrcode, 
       sextcode, livestatus, normdoc, worksarea
  FROM path.addrobj_temp;
