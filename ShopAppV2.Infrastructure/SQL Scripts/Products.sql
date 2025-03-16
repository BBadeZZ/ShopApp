-- Table: public.Products

-- DROP TABLE IF EXISTS public."Products";

CREATE TABLE IF NOT EXISTS public."Products"
(
    id
    integer
    NOT
    NULL
    DEFAULT
    nextval
(
    '"Products_id_seq"'
    :
    :
    regclass
),
    name text COLLATE pg_catalog."default",
    description text COLLATE pg_catalog."default",
    stock_quantity integer,
    category_id integer,
    created_at timestamp without time zone,
    price double precision,
    rating integer,
    is_active boolean,
    updated_at timestamp
                         without time zone,
    image_path text COLLATE pg_catalog."default",
    CONSTRAINT "Products_pkey" PRIMARY KEY
(
    id
),
    CONSTRAINT fk_category_id FOREIGN KEY
(
    category_id
)
    REFERENCES public."Categories"
(
    id
) MATCH SIMPLE
                         ON UPDATE NO ACTION
                         ON DELETE NO ACTION
    )
    TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."Products"
    OWNER to postgres;