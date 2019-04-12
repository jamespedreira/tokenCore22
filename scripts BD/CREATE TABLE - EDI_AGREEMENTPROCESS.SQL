CREATE TABLE public.edi_CCLPRC
(   clp_identi integer NOT NULL,
    clp_cbl_identi integer NOT NULL,
    clp_nar_identi integer NOT NULL,
 	clp_ccl_identi integer,
 	clp_period numeric(1) NOT NULL,
 	clp_ediprc numeric(1) NOT NULL, 	
    CONSTRAINT edi_cclprc_pkey PRIMARY KEY (clp_identi),
    CONSTRAINT edi_clp_cbl_identi_fkey FOREIGN KEY (clp_cbl_identi)
        REFERENCES public.edi_cablay (cbl_identi) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT edi_clp_ccl_identi_fkey FOREIGN KEY (clp_ccl_identi)
        REFERENCES public.edi_clicbl (ccl_identi) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
	CONSTRAINT edi_clp_nar_identi_fkey FOREIGN KEY (clp_nar_identi)
        REFERENCES public.edi_nomarq (nar_identi) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
)

-- Add comments to the columns 
comment on column EDI_CLPCCO.clp_identi
  is 'Identificador do processo';
comment on column EDI_CLPCCO.clp_ccl_identi
  is 'Identificador único do convênio';
comment on column EDI_CLPCCO.clp_cbl_identi
  is 'Identificador único do Layout';
comment on column EDI_CLPCCO.clp_nar_identi
  is 'Identificador único do Arquivo';
comment on column EDI_CLPCCO.clp_period
  is 'Periodo de execução';
comment on column EDI_CLPCCO.clp_ediprc
  is 'Tipo do Processo';
