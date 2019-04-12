CREATE TABLE public.edi_clicbl
(   ccl_identi integer NOT NULL,
    ccl_cli_identi integer NOT NULL,
    ccl_emp_identi integer NOT NULL,
 	ccl_ativo character varying(1) default 'N' NOT NULL,
	ccl_raiz character varying(1) default 'N',
    CONSTRAINT edi_clicbl_pkey PRIMARY KEY (ccl_identi),
    CONSTRAINT edi_clicbl_emp_identi_fkey FOREIGN KEY (ccl_emp_identi)
        REFERENCES public.tms_empres (emp_identi) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT edi_clicbl_cli_identi_fkey FOREIGN KEY (ccl_cli_identi)
        REFERENCES public.edi_client (cli_identi) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

-- Add comments to the columns 
comment on column EDI_CLPCCO.ccl_identi
  is 'Identificador do convênio de cliente';
comment on column EDI_CLPCCO.ccl_cli_identi
  is 'Identificador único do cliente';
comment on column EDI_CLPCCO.ccl_emp_identi
  is 'Numero da empresa';
comment on column EDI_CLPCCO.ccl_ativo
  is 'Ativo (S-Sim - N-Não)';
comment on column EDI_CLPCCO.ccl_raiz
  is 'Informa que CPF/CNPJ será RAIZ (S-Sim - N-Não)';

