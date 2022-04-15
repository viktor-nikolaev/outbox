SELECT *
FROM "{0}"."{1}"
WHERE "Status" = 'Scheduled' AND 
      "IsSequential" = true  AND
      "EventName" = @EventName
ORDER BY "Id" ASC 
LIMIT @Limit
FOR UPDATE NOWAIT