 create table public.edi_client
(
  cli_identi   	  integer not null,
  cli_pes_identi  integer not null,
  cli_emp_identi integer NOT NULL,
  CONSTRAINT edi_client_pkey PRIMARY KEY (cli_identi),
  CONSTRAINT edi_client_cli_emp_identi_fkey FOREIGN KEY (cli_emp_identi)
        REFERENCES public.tms_empres (emp_identi) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION, 	
  CONSTRAINT edi_client_cli_pes_identi_fkey FOREIGN KEY (cli_pes_identi)
        REFERENCES public.edi_pessoa (pes_identi) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION 	
 );
 
 -- Add comments to the columns 
comment on column EDI_CLPCCO.cli_identi
  is 'Identificador do Cliente';
comment on column EDI_CLPCCO.cli_pes_identi
  is 'Identificador único da pessoa';
comment on column EDI_CLPCCO.cli_emp_identi
  is 'Numero da Empresa';

