select 'grant execute on ' + name + ' to nvowebaccess', name, create_date from sys.objects where name like 'ivo%' order by create_date desc

grant execute on ivo_nocasematch to nvowebaccess
grant execute on ivo_hasword to nvowebaccess
grant execute on ivo_string_agg to nvowebaccess
grant execute on ivo_hashlist_has to nvowebaccess


