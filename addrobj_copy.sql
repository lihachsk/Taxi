INSERT INTO path.addrobj_temp(
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
  FROM(select 
       ROW_NUMBER () over (partition by aoguid order by startdate desc, updatedate desc) as RwNo,
	   *
	   from path.addrobj )
where src.RwNo = 1;