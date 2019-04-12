CREATE TABLE public.edi_cblnoc
(
    clo_identi integer NOT NULL,
    clo_ccl_identi integer NOT NULL,
    clo_noc_identi integer,    
    CONSTRAINT edi_cblnoc_pkey PRIMARY KEY (clo_identi),
    CONSTRAINT edi_cblnoc_ccl_identi_fkey FOREIGN KEY (clo_ccl_identi)
        REFERENCES public.edi_clicbl (ccl_identi) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

-- Add comments to the columns 
comment on column EDI_CLPCCO.clo_identi
  is 'Identificador da Ocorrência';
comment on column EDI_CLPCCO.clo_ccl_identi
  is 'Identificador único do convênio';
comment on column EDI_CLPCCO.clo_noc_identi
  is 'Identificador único da natureza de ocorrência';
