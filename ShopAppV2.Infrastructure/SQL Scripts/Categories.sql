-- Table: public.Categories

-- DROP TABLE IF EXISTS public."Categories";

CREATE TABLE IF NOT EXISTS public."Categories"
(
    id
    integer
    NOT
    NULL
    DEFAULT
    nextval
(
    '"Categories_id_seq"'
    :
    :
    regclass
),
    name text COLLATE pg_catalog."default",
    CONSTRAINT "Categories_pkey" PRIMARY KEY
(
    id
)
    )
    TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."Categories"
    OWNER to postgres;