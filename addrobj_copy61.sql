INSERT INTO path.addrobj61(
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
  FROM path.addrobj_temp where offname in ('Батайск','Ростов-на-Дону','Аксай','Азов') and aolevel=4;
INSERT INTO path.addrobj61(
            actstatus, aoguid, aoid, aolevel, areacode, autocode, centstatus, 
            citycode, code, currstatus, enddate, formalname, ifnsfl, ifnsul, 
            nextid, offname, okato, oktmo, operstatus, parentguid, placecode, 
            plaincode, postalcode, previd, regioncode, shortname, startdate, 
            streetcode, terrifnsfl, terrifnsul, updatedate, ctarcode, extrcode, 
            sextcode, livestatus, normdoc, worksarea)
SELECT child.*
  FROM path.addrobj_temp as child 
  join path.addrobj_temp as parent
	on child.parentguid = parent.aoguid
  where parent.offname in ('Батайск','Ростов-на-Дону','Аксай','Азов');
  
UPDATE path.addrobj61
   SET worksarea=true;