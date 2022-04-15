SELECT *
FROM "{0}"."{1}"
WHERE "Status" = 'Scheduled' AND "IsSequential" = false
ORDER BY "Id" ASC
LIMIT @Limit
FOR UPDATE SKIP LOCKED