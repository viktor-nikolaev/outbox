CREATE SCHEMA IF NOT EXISTS "{0}";

CREATE TABLE IF NOT EXISTS "{0}"."{1}"
(
    "Id"           BIGSERIAL                NOT NULL CONSTRAINT events_pk PRIMARY KEY,
    "EventKey"     VARCHAR(256)             NULL,
    "EventName"    VARCHAR(256)             NOT NULL,
    "IsSequential" BOOLEAN                  NOT NULL DEFAULT (FALSE),
    "Payload"      JSONB                    NOT NULL,
    "AddedAtUtc"   TIMESTAMP WITH TIME ZONE NOT NULL,
    "Headers"      JSONB                    NOT NULL,
    "Status"       VARCHAR(10)              NOT NULL
);